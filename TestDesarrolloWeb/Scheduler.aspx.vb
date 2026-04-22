Imports Microsoft.VisualBasic
Imports System
Imports  TestDesarrolloWeb.Model
Imports DevExpress.Web.Internal
Imports DevExpress.XtraScheduler

Partial Public Class SchedulerModule
    Inherits System.Web.UI.Page
    Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs)
        ResourcesListBox.DataSource = ResourceDataSourceHelper.GetItems()
        ResourcesListBox.DataBind()
        If (Not IsPostBack) Then
            ResourcesListBox.SelectAll()
        End If

        ' DXCOMMENT: Setting ViewType: a compact view (Day) for mobile devices, a large view (WorkWeek) for desktops
        Scheduler.ActiveViewType = If(RenderUtils.Browser.Platform.IsMobileUI, SchedulerViewType.Day, SchedulerViewType.WorkWeek)

        If (Not IsPostBack) Then
            ' DXCOMMENT: Scroll to actual time
            Dim currentTime = New TimeSpan(DateTime.Now.Hour - 1, 0, 0)

            Scheduler.DayView.TopRowTime = currentTime
            Scheduler.WorkWeekView.TopRowTime = currentTime
            Scheduler.FullWeekView.TopRowTime = currentTime
        End If

        ' DXCOMMENT: Map labels by their ids
        Scheduler.Storage.Appointments.Labels.Clear()
        For Each label As SchedulerLabel In SchedulerLabelsHelper.GetItems()
            Scheduler.Storage.Appointments.Labels.Add(label.Id, label.Name, label.Name, label.Color)
        Next label
    End Sub

    Protected Sub Scheduler_FilterResource(ByVal sender As Object, ByVal e As PersistentObjectCancelEventArgs)
        If ResourcesListBox.SelectedValues.Count <> ResourcesListBox.Items.Count Then
            If ResourcesListBox.SelectedValues.Count = 0 Then
                e.Cancel = True
            Else
                Dim resourceId = CLng(Fix((TryCast(e.Object, Resource)).Id))
                If (Not ResourcesListBox.SelectedValues.Contains(resourceId)) Then
                    e.Cancel = True
                End If
            End If
        End If
    End Sub
End Class