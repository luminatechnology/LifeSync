<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="LM201001.aspx.cs" Inherits="Pages_LM201001" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="LumCustomizations.Graph.LumPIUnitPriceMaint" PrimaryView="SOTranscation">
        <CallbackCommands>
            <%--<px:PXDSCallbackCommand Name="Report" Visible="True" CommitChanges="true" StartNewGroup="true"></px:PXDSCallbackCommand>--%>
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXGrid SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="PrimaryInquire" AllowAutoHide="false">
        <Levels>
            <px:PXGridLevel DataMember="SOTranscation">
                <Columns>
                    <px:PXGridColumn DataField="SOOrder__OrderNbr" Width="200" LinkCommand="viewSOOrderDocument"></px:PXGridColumn>
                    <px:PXGridColumn DataField="SOOrder__OrderType" Width="200"></px:PXGridColumn>
                    <px:PXGridColumn DataField="SOOrder__CustomerID" Width="130" DisplayMode="Hint"></px:PXGridColumn>
                    <px:PXGridColumn DataField="SOOrder__UsrPICustomerID" Width="130" DisplayMode="Hint"></px:PXGridColumn>
                    <px:PXGridColumn DataField="SOOrder__UsrPICuryID" Width="70"></px:PXGridColumn>
                    <px:PXGridColumn DataField="SOOrder__OrderDate" Width="100"></px:PXGridColumn>
                    <px:PXGridColumn DataField="SOOrder__RequestDate" Width="100"></px:PXGridColumn>
                    <px:PXGridColumn DataField="SOLine__InventoryID" Width="70"></px:PXGridColumn>
                    <px:PXGridColumn DataField="SOLine__SiteID" Width="80"></px:PXGridColumn>
                    <px:PXGridColumn DataField="SOLine__TranDesc" Width="150"></px:PXGridColumn>
                    <px:PXGridColumn DataField="SOLine__OrderQty" Width="70"></px:PXGridColumn>
                    <px:PXGridColumn DataField="SOLine__CuryUnitPrice" Width="70"></px:PXGridColumn>
                    <px:PXGridColumn DataField="UsrPIUnitPrice" Width="70"></px:PXGridColumn>
                </Columns>
                <RowTemplate>
                    <%--<px:PXSelector runat="server" ID="edOrderNbr" DataField="SOOrder__OrderNbr" AllowEdit="true"></px:PXSelector>--%>
                    <px:PXSelector runat="server" ID="edOrderType" DataField="SOOrder__OrderType" AllowEdit="true"></px:PXSelector>
                    <px:PXSegmentMask runat="server" DataField="SOOrder__CustomerID"></px:PXSegmentMask>
                    <px:PXSelector runat="server" DataField="SOLine__InventoryID" AllowEdit="true"></px:PXSelector>
                    <px:PXSelector runat="server" DataField="SOLine__UsrPICuryID" AllowAddNew="true"></px:PXSelector>
                </RowTemplate>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Container="Window" Enabled="True" MinHeight="150"></AutoSize>
        <ActionBar>
        </ActionBar>

        <Mode InitNewRow="True"></Mode>
    </px:PXGrid>
    <%--<script src="~/Scripts/jquery-3.1.1.min.js"></script>--%>
</asp:Content>

