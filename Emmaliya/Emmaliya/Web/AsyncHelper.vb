Imports System.Reflection
Imports System.Text
Imports System.Web
Imports Emmaliya.Utilities

Namespace Web
    ''' <summary>
    ''' An internal class to generate client-side script for asynchronous calls, and to invoke methods when an asynchronous call is being made
    ''' </summary>
    Friend Class AsyncHelper

#Region "ExecuteAsyncMethod"
        Public Shared Sub ExecuteAsyncMethod(target As Object, u As IUser)
            Dim response = HttpContext.Current.Response
            Dim request = HttpContext.Current.Request
            Dim foundObject As Boolean = False
            Dim segments As String() = request.Form("__asyncmethod").Split(".")

            'Is this object the one we seek?
            If AsyncNameAttribute.GetName(target) = segments(0) Then
                foundObject = True
            ElseIf target.GetType().IsSubclassOf(GetType(System.Web.UI.Page)) Then

                'Attempt to find the requested object amongst this page's masters:
                Dim curr = CType(target, System.Web.UI.Page).Master

                While curr IsNot Nothing
                    If AsyncNameAttribute.GetName(curr) = segments(0) Then
                        target = curr
                        foundObject = True
                        Exit While
                    End If

                    curr = curr.Master
                End While
            End If

            'If we haven't found a match yet, check all RouteHandler's that have this attribute
            If Not foundObject Then
                'TODO: code this search (search all assemblies?)
            End If

            If Not foundObject Then
                response.StatusCode = 404
                response.Write("Method '" & request.Form("__asyncmethod") & "' not found")
            Else
                Dim results As New Dictionary(Of String, Object)

                results.Add("Success", True)

                Try
                    Dim mi As MethodInfo = target.GetType().GetMethod(segments(1))

                    If mi IsNot Nothing Then
                        Dim ama As AsyncMethodAttribute = mi.GetCustomAttribute(Of AsyncMethodAttribute)()

                        If ama IsNot Nothing AndAlso AccessAttribute.HasAccessTo(mi, u) Then
                            Dim invokeParameters As New List(Of Object)
                            Dim json As New Script.Serialization.JavaScriptSerializer
                            Dim jsonArguments As Dictionary(Of String, Object) = json.Deserialize(Of Dictionary(Of String, Object))(request.Form("Arguments"))

                            For Each pi As ParameterInfo In mi.GetParameters
                                Dim destType As Type = pi.ParameterType
                                Dim isNullable As Boolean = False

                                If Nullable.GetUnderlyingType(destType) IsNot Nothing Then
                                    isNullable = True
                                    destType = Nullable.GetUnderlyingType(destType)
                                End If

                                If jsonArguments.ContainsKey(pi.Name) Then
                                    Dim val As Object = jsonArguments(pi.Name)

                                    If val.GetType() Is pi.ParameterType Then
                                        invokeParameters.Add(val)
                                    ElseIf jsonArguments(pi.Name) Is Nothing OrElse (isNullable AndAlso (jsonArguments(pi.Name) Is Nothing OrElse jsonArguments(pi.Name).ToString() = "")) Then
                                        'If null or the empty string was passed into a nullable parameter, set it to nothing:
                                        invokeParameters.Add(Nothing)
                                    ElseIf pi.ParameterType Is GetType(List(Of Object)) AndAlso jsonArguments(pi.Name).GetType() Is GetType(ArrayList) Then
                                        'If the destination type is Object, or the value passed in is the same type (or an ancestor of) the destination type, don't do any conversion:
                                        invokeParameters.Add(New List(Of Object)(CType(jsonArguments(pi.Name), ArrayList).ToArray()))
                                    Else
                                        invokeParameters.Add(Convert.ChangeType(val, pi.ParameterType))
                                    End If
                                ElseIf pi.HasDefaultValue Then
                                    invokeParameters.Add(pi.DefaultValue)
                                Else
                                    invokeParameters.Add(Nothing)
                                End If
                            Next

                            Dim invokeResult = mi.Invoke(target, invokeParameters.ToArray())
                            results("Result") = invokeResult
                        Else
                            response.StatusCode = 403
                            response.Write("Access Denied")
                        End If
                    Else
                        response.StatusCode = 404
                        response.Write("Method '" & request.Form("__asyncmethod") & "' not found")
                    End If
                Catch ex As Exception
                    Dim exp As New Dictionary(Of String, Object)

                    results("Success") = False

                    exp("Error") = ex.Message
                    exp("StackTrace") = ex.StackTrace

                    results("Exception") = exp
                End Try

                response.ContentType = "application/json"

                response.Write(results.ToJSONString())
                response.End()
            End If
        End Sub
#End Region

#Region "ExtractAsyncMethods"
        ''' <summary>
        ''' Determines if target has been decorated withe AsyncName attribute, and if the current user has access, generates client-side script for invoking any methods decorated with the AsyncMethod attribute.
        ''' </summary>
        ''' <param name="target">The object to generate client-side invocation calls for</param>
        ''' <param name="u">The user to test authorization against</param>
        ''' <returns></returns>
        Public Shared Function ExtractAsyncMethods(target As Object, u As IUser) As String
            Dim aca As AsyncNameAttribute = target.GetType().GetCustomAttribute(Of AsyncNameAttribute)(True)
            Dim sb As New StringBuilder

            If aca IsNot Nothing AndAlso AccessAttribute.HasAccessTo(target, u) Then
                Dim firstMethod As Boolean = True

                sb.Append("emmaliya.async.")
                sb.Append(aca.Name)
                sb.Append(" = {")

                'Build list of async methods:
                For Each mi As MethodInfo In target.GetType().GetMethods
                    Dim ama As AsyncMethodAttribute = mi.GetCustomAttribute(Of AsyncMethodAttribute)()

                    If ama IsNot Nothing AndAlso AccessAttribute.HasAccessTo(mi, u) Then
                        If Not firstMethod Then sb.Append(", ")

                        Dim argList As New StringBuilder

                        sb.Append(mi.Name)
                        sb.Append(": function(")

                        For Each pi As ParameterInfo In mi.GetParameters
                            sb.Append(pi.Name)
                            sb.Append(", ")

                            If argList.Length > 0 Then argList.Append(",")
                            argList.Append(pi.Name)
                            argList.Append(":")
                            argList.Append(pi.Name)
                        Next

                        sb.Append("onSuccess, onFailure, onComplete) { emmaliya.fn.invokeAsync('")

                        sb.Append(HttpContext.Current.Request.Url.AbsolutePath.Replace("'", "\'"))

                        sb.Append("', '")

                        sb.Append(aca.Name)
                        sb.Append(".")
                        sb.Append(mi.Name)

                        sb.Append("',{")
                        sb.Append(argList.ToString)
                        sb.Append("}, onSuccess, onFailure, onComplete)")

                        sb.Append("} ")

                        firstMethod = False
                    End If
                Next
                sb.Append("};")
            End If

            Return sb.ToString()
        End Function
#End Region

    End Class

End Namespace