<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Navigation.ascx.cs" Inherits="DesarrollosQAS.UserControls.Navigation" %>
<nav>
        <dx:ASPxButton runat="server" ID="NavigationBreadCrumbsButton" ClientInstanceName="NavigationBreadCrumbsButton" CssClass="navigation-breadcrumbs-button"
            Text="Menú" Width="100%" AutoPostBack="false" HorizontalAlign="Left" UseSubmitBehavior="false" Height="34">
            <FocusRectBorder BorderWidth="0" />
            <FocusRectPaddings Padding="0" />
            <Image SpriteProperties-CssClass="icon"/>
            <ClientSideEvents Click="function(){ NavControl.onNavigationBreadCrumbsButtonClick(); }" />
        </dx:ASPxButton>
        <div class="nav-tree-view">
            <span id="breadCrumbsText" class="breadCrumbs"/>
            <dx:ASPxTreeView runat="server" ID="NavigationTreeView" ClientInstanceName="NavigationTreeView" DataSourceID="XmlDataSource1" Width="90%"
                ShowTreeLines="true" ShowExpandButtons="true" OnNodeDataBound="NavigationTreeView_NodeDataBound" OnPreRender="NavigationTreeView_PreRender" EnableHotTrack="true"
                TextField="Title" NavigateUrlField="Url" ImageUrlField="Image" NameField="Name" Images-NodeImage-Width="20px" Styles-Node-SelectedStyle-BackColor="Teal">
                <Nodes>
                    <dx:TreeViewNode Name="Reportes" Text="Festivo" NavigateUrl="../Pages/Home.aspx"/> 
                </Nodes>
                <Styles>
                    <Node CssClass="node" />
                    <Elbow CssClass="elbow" />
                </Styles>
            </dx:ASPxTreeView>
        </div>
    <asp:XmlDataSource ID="XmlDataSource1" runat="server" DataFile="~/App_Data/Navigation.xml" XPath="/namespace/*" />
</nav>