<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RolModulo.aspx.cs" Inherits="DesarrollosQAS.Pages.RolModulo" MasterPageFile="~/Root.master" %>

<asp:Content ID="contentRolModulos" ContentPlaceHolderID="content" runat="server">
        <script type="text/javascript">
        function treeList_CustomDataCallback(s, e) {
            document.getElementById('treeListCountCell').innerHTML = e.result;
        }
        function treeList_SelectionChanged(s, e) {
            window.setTimeout(function() { s.PerformCustomDataCallback(''); }, 0)
        }
        </script>
    
    <dx:ASPxTreeList ID="treeListModulos" runat="server" KeyFieldName="Id" ParentFieldName="ParentId"
        ClientInstanceName="treeListModulos" Width="100%">
        <Columns>
            <dx:TreeListTextColumn FieldName="Nombre" Caption="Módulo" />
        </Columns>
        <SettingsSelection Enabled="true" />
        <ClientSideEvents
            CustomDataCallback="treeList_CustomDataCallback"
            SelectionChanged="treeList_SelectionChanged" />
    </dx:ASPxTreeList>
        <dx:ASPxTreeList ID="treeList" runat="server" AutoGenerateColumns="False"
        Width="100%" KeyFieldName="ID" ParentFieldName="ParentID" OnCustomDataCallback="treeList_CustomDataCallback"
        OnDataBound="treeList_DataBound">
        <Columns>
            <dx:TreeListDataColumn FieldName="DepartmentName" Caption="Department" VisibleIndex="0" />
            <dx:TreeListDataColumn FieldName="Budget" VisibleIndex="1" DisplayFormat="{0:C}" />
            <dx:TreeListDataColumn FieldName="Location" VisibleIndex="2" />
        </Columns>
        <SettingsBehavior ExpandCollapseAction="NodeDblClick" />
        <SettingsSelection Enabled="True" />
        <ClientSideEvents SelectionChanged="treeList_SelectionChanged" CustomDataCallback="treeList_CustomDataCallback" />
    </dx:ASPxTreeList>
</asp:Content>
