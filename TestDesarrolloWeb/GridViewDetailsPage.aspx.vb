Imports Microsoft.VisualBasic
Imports  TestDesarrolloWeb.Model
Imports System
Imports System.Web

Partial Public Class GridViewDetailsPageModule
    Inherits System.Web.UI.Page
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        Dim recordId As Long
        If Long.TryParse(Request.QueryString("id"), recordId) Then
            Dim issue = GetCurrentIssue(recordId)
            SubjectText.InnerHtml = HttpUtility.HtmlEncode(issue.Subject)
            IdLabel.Text = recordId.ToString()
        End If
        Master.EnableBackButton = True
    End Sub

    Protected Function GetCurrentIssue(ByVal recordId As Long) As Issue
        Dim issue = DataProvider.GetIssues().Find(Function(i) i.Id = recordId)
        If issue Is Nothing Then
            Throw New Exception("The issue was not found")
        End If
        Return issue
    End Function
End Class