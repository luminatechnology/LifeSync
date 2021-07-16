<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="LM209001.aspx.cs" Inherits="Pages_LM209001" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="LUMCustomizations.Graph.ICMSummaryMaint"
        PrimaryView="Filter">
        <CallbackCommands>
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="Filter" Width="100%" Height="100px" AllowAutoHide="false">
        <Template>
            <px:PXSelector runat="server" ID="edStart_AMProdID" DataField="Start_AMProdID" CommitChanges="True" Size="S" />
            <px:PXSelector runat="server" ID="edEnd_AMProdID" DataField="End_AMProdID" CommitChanges="True" Size="S" />
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
    <px:PXGrid SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="PrimaryInquire" AllowAutoHide="false">
        <Levels>
            <px:PXGridLevel DataMember="ICMList">
                <Columns>
                    <px:PXGridColumn DataField="LineNbr"></px:PXGridColumn>
                    <px:PXGridColumn DataField="ProdOrdID"></px:PXGridColumn>
                    <px:PXGridColumn DataField="InventoryCD"></px:PXGridColumn>
                    <px:PXGridColumn DataField="CustomerPN"></px:PXGridColumn>
                    <px:PXGridColumn DataField="Description"></px:PXGridColumn>
                    <px:PXGridColumn DataField="StandardTime"></px:PXGridColumn>
                    <px:PXGridColumn DataField="MaterialCost"></px:PXGridColumn>
                    <px:PXGridColumn DataField="LabourCost"></px:PXGridColumn>
                    <px:PXGridColumn DataField="ManufactureCost"></px:PXGridColumn>
                    <px:PXGridColumn DataField="Overhead"></px:PXGridColumn>
                    <px:PXGridColumn DataField="Lumyield"></px:PXGridColumn>
                    <px:PXGridColumn DataField="DGPrice"></px:PXGridColumn>
                    <px:PXGridColumn DataField="HKOverhead"></px:PXGridColumn>
                    <px:PXGridColumn DataField="ABIPrice"></px:PXGridColumn>
                    <px:PXGridColumn DataField="DGtoHKPrice"></px:PXGridColumn>
                </Columns>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Container="Window" Enabled="True" MinHeight="150" />
        <ActionBar>
        </ActionBar>
    </px:PXGrid>

</asp:Content>
