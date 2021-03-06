﻿Imports System.IO
Imports System.Reflection
Imports System.Text
Imports System.Web
Imports System.Web.Script.Serialization

Namespace Web
    ''' <summary>
    ''' The base class for all pages in the Emmaliya framework
    ''' </summary>
    Public MustInherit Class Page(Of U As IUser)
        Inherits System.Web.UI.Page

#Region "Private Members"
        Private ReadOnly ScriptsToAdd As New List(Of ClientResource)
        Private ReadOnly StylesToAdd As New List(Of ClientResource)
#End Region

#Region "Properties"
        Public Overridable ReadOnly Property AutoDetectScript As Boolean = True

        Public Overridable ReadOnly Property AutoDetectStylesheet As Boolean = True

        Public ReadOnly Property CurrentApplication As Application(Of U)
            Get
                Return Emmaliya.Web.Application(Of U).Current
            End Get
        End Property

        Public ReadOnly Property CurrentUser As U
            Get
                Return CurrentApplication.CurrentUser
            End Get
        End Property
#End Region

#Region "AddEmmaliyaClientObjectInit"
        Private Sub AddEmmaliyaClientObjectInit(clientName As String, o As Object)
            Dim script As New StringBuilder
            Dim js As New JavaScriptSerializer

            script.Append(clientName)
            script.Append(" = ")
            script.Append(clientName)
            script.Append(" || {}; ")

            For Each pi As PropertyInfo In o.GetType().GetProperties
                Dim cs As ClientSideAttribute = pi.GetCustomAttribute(Of ClientSideAttribute)()

                If cs IsNot Nothing Then
                    script.Append(clientName)
                    script.Append(".")
                    If pi.Name.Length = 1 Then
                        script.Append(pi.Name.ToLower())
                    Else
                        script.Append(pi.Name.Substring(0, 1).ToLower() & pi.Name.Substring(1))
                    End If
                    script.Append(" = jQuery.parseJSON('")

                    script.Append(js.Serialize(pi.GetValue(o)))

                    script.Append("');")
                End If
            Next

            AddScript(ClientResource.FromContents(script.ToString()))
        End Sub
#End Region

#Region "AddEmmaliyaScriptTag"
        Private Sub AddEmmaliyaScriptTag()
            ' Go through all loaded assemblies and get the latest modified date:
            Dim lastWriteTime As Long = 0

            For Each asm In AppDomain.CurrentDomain.GetAssemblies()
                If Not asm.IsDynamic Then
                    Dim fi As New FileInfo(asm.Location)

                    lastWriteTime = Math.Max(lastWriteTime, fi.LastWriteTime.Ticks)
                End If
            Next

            AddScript(ClientResource.FromUrl(CurrentApplication.EmmaliyaScriptUrl & "?v=" & lastWriteTime))
        End Sub
#End Region

#Region "AddScript"
        Public Sub AddScript(cr As ClientResource)
            cr.AddedIndex = ScriptsToAdd.Count
            ScriptsToAdd.Add(cr)
        End Sub
#End Region

#Region "AddStyle"
        Public Sub AddStyle(cr As ClientResource)
            cr.AddedIndex = StylesToAdd.Count
            StylesToAdd.Add(cr)
        End Sub
#End Region

#Region "GetScriptTags"
        ''' <summary>
        ''' Returns a string consisting of all script tags added by calls to AddScript
        ''' </summary>
        ''' <returns>A string of HTML &lt;script&gt; tags</returns>
        Public Function GetScriptTags() As String
            Dim sb As New StringBuilder

            For Each cr As ClientResource In (From s In ScriptsToAdd Where s.Value.Trim() <> "" Order By s.Priority, s.AddedIndex)
                sb.Append("<script")

                If cr.ValueIsUrl Then
                    sb.Append(" src=""")
                    sb.Append(cr.Value)

                    'Add cache-buster timestamp of last modified date:
                    '(this forces browsers to always have the most recent version of a script/stylesheet without having to refresh the page)
                    If cr.Value.StartsWith("~/") AndAlso IO.File.Exists(Server.MapPath(cr.Value)) Then
                        Dim fi As New IO.FileInfo(Server.MapPath(cr.Value))
                        sb.Append(IIf(cr.Value.Contains("?"), "&", "?"))
                        sb.Append("v=" & fi.LastWriteTime.Ticks.ToString())
                    End If

                    sb.Append(""">")
                Else
                    sb.Append(">")
                    sb.Append(cr.Value)
                End If

                sb.Append("</script>")
            Next

            Return sb.ToString()
        End Function
#End Region

#Region "GetStyleTags"
        ''' <summary>
        ''' Returns a string consisting of all style and link tags added by calls to AddStyle
        ''' </summary>
        ''' <returns>A string of HTML &lt;style&gt; or &lt;link&gt; tags</returns>
        Public Function GetStyleTags() As String
            Dim sb As New StringBuilder

            For Each cr As ClientResource In (From s In StylesToAdd Where s.Value.Trim() <> "" Order By s.Priority, s.AddedIndex)
                If cr.ValueIsUrl Then
                    sb.Append("<link rel=""stylesheet"" href=""")
                    sb.Append(cr.Value)

                    'Add cache-buster timestamp of last modified date:
                    '(this forces browsers to always have the most recent version of a script/stylesheet without having to refresh the page)
                    If cr.Value.StartsWith("~/") AndAlso IO.File.Exists(Server.MapPath(cr.Value)) Then
                        Dim fi As New IO.FileInfo(Server.MapPath(cr.Value))
                        sb.Append(IIf(cr.Value.Contains("?"), "&", "?"))
                        sb.Append("v=" & fi.LastWriteTime.Ticks.ToString())
                    End If

                    sb.Append(""" />")
                Else
                    sb.Append("<style>")
                    sb.Append(cr.Value)
                    sb.Append("</style>")
                End If
            Next

            Return sb.ToString()
        End Function
#End Region

#Region "OnLoad"
        Protected Overrides Sub OnLoad(e As EventArgs)

            If Request.Form("__asyncmethod") <> "" Then
                'Attempt to execute the asynchronous method
                AsyncHelper.ExecuteAsyncMethod(Me, CurrentUser)
            Else
                Dim currentPage As String = IO.Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath.Replace("~/", "").Replace("/", "_"))

                'Add Emmaliya.js tag:
                AddEmmaliyaScriptTag()

                'Add the Emmaliya object initializations:
                AddEmmaliyaClientObjectInit("emmaliya.currentPage", Me)
                AddEmmaliyaClientObjectInit("emmaliya.currentUser", CurrentUser)
                AddEmmaliyaClientObjectInit("emmaliya.currentApplication", CurrentApplication)

                'Go through the plugins and let them do whatever they need to do:
                For Each p In CurrentApplication.Plugins
                    p.OnPageLoad(Me)
                Next

                For Each sa As AddScriptAttribute In Me.GetType().GetCustomAttributes(Of AddScriptAttribute)()
                    AddScript(ClientResource.FromUrl(sa.ScriptPath))
                Next

                For Each sa As AddStylesheetAttribute In Me.GetType().GetCustomAttributes(Of AddStylesheetAttribute)()
                    AddStyle(ClientResource.FromUrl(sa.StylesheetPath))
                Next

                'Check and see if my master page hierarchy has asyncmethods or AddScript/AddStylesheet attributes:
                Dim curr As MasterPage(Of U) = Master

                While curr IsNot Nothing
                    For Each sa As AddScriptAttribute In curr.GetType().GetCustomAttributes(Of AddScriptAttribute)()
                        AddScript(ClientResource.FromUrl(sa.ScriptPath))
                    Next

                    For Each sa As AddStylesheetAttribute In curr.GetType().GetCustomAttributes(Of AddStylesheetAttribute)()
                        AddStyle(ClientResource.FromUrl(sa.StylesheetPath))
                    Next

                    If AsyncNameAttribute.IsAsync(curr) Then
                        AddScript(ClientResource.FromContents(AsyncHelper.ExtractAsyncMethods(curr, CurrentUser)))
                    End If

                    curr = curr.Master
                End While

                'Auto-detect a script with the same name as this page, if enabled:
                If CurrentApplication.AutoDetectScripts AndAlso AutoDetectScript Then
                    For Each path As String In CurrentApplication.ScriptPaths
                        If IO.File.Exists(Server.MapPath(path & "/" & currentPage & ".js")) Then
                            AddScript(ClientResource.FromUrl(VirtualPathUtility.ToAbsolute(path & "/" & currentPage & ".js")))
                        End If
                    Next
                End If

                'Auto-detect a stylesheet with the same name as this page, if enabled:
                If CurrentApplication.AutoDetectStylesheets AndAlso AutoDetectStylesheet Then
                    For Each path As String In CurrentApplication.StylePaths
                        If IO.File.Exists(Server.MapPath(path & "/" & currentPage & ".css")) Then
                            AddScript(ClientResource.FromUrl(VirtualPathUtility.ToAbsolute(path & "/" & currentPage & ".css")))
                        End If
                    Next
                End If

                'Add my own extracted asynchronous methods:
                AddScript(ClientResource.FromContents(AsyncHelper.ExtractAsyncMethods(Me, CurrentUser)))

                MyBase.OnLoad(e)
            End If
        End Sub
#End Region

    End Class
End Namespace