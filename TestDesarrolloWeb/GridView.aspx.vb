Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports  TestDesarrolloWeb.Model
Imports DevExpress.Web

Partial Public Class GridViewModule
    Inherits System.Web.UI.Page
    Protected Sub GridView_InitNewRow(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataInitNewRowEventArgs)
        e.NewValues("Kind") = 1
        e.NewValues("Priority") = 2
        e.NewValues("Status") = 1
        e.NewValues("IsDraft") = True
        e.NewValues("IsArchived") = False
    End Sub

    Protected Sub GridView_CustomCallback(ByVal sender As Object, ByVal e As ASPxGridViewCustomCallbackEventArgs)
        If e.Parameters = "delete" Then
            Dim selectedIds As List(Of Long) = GridView.GetSelectedFieldValues("Id").ConvertAll(Function(id) CLng(Fix(id)))
            DataProvider.DeleteIssues(selectedIds)
            GridView.DataBind()
        End If
    End Sub
End Class