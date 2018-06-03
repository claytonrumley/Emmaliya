Imports System.Runtime.CompilerServices

Namespace Utilities
    Public Module Extensions

        ''' <summary>
        ''' Determines if a class is a subclass of a raw generic
        ''' </summary>
        ''' <param name="toCheck"></param>
        ''' <param name="generic"></param>
        ''' <returns></returns>
        ''' <remarks>Inspired by https://stackoverflow.com/questions/457676/check-if-a-class-is-derived-from-a-generic-class#457708 </remarks>
        <Extension()>
        Public Function IsSubclassOfRawGeneric(toCheck As Type, generic As Type) As Boolean
            While toCheck IsNot Nothing AndAlso toCheck IsNot GetType(Object)
                Dim curr As Type = toCheck

                If toCheck.IsGenericType Then
                    curr = toCheck.GetGenericTypeDefinition()
                End If

                If generic Is curr OrElse curr Is generic.GetGenericTypeDefinition() Then
                    Return True
                End If

                toCheck = toCheck.BaseType
            End While

            Return False
        End Function

        <Extension()>
        Public Function ToJSONString(o As Object) As String
            Dim js As New System.Web.Script.Serialization.JavaScriptSerializer

            Return js.Serialize(o)
        End Function

    End Module
End Namespace