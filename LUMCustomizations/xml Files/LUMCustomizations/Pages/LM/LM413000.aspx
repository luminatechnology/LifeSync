<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="LM413000.aspx.cs" Inherits="Page_LM413000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="LUMCustomizations.Graph.LUMMultiLevelBomInq" PrimaryView="Filter">
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="Filter" Width="100%" AllowAutoHide="false">
		<Template>
            <px:PXLayoutRule ID="PXLayoutRule15" runat="server" StartColumn="True" LabelsWidth="S" ControlSize="XM" ></px:PXLayoutRule>
            <px:PXSelector ID="edBOMID" runat="server" AutoRefresh="True" DataField="BOMID" DataSourceID="ds" CommitChanges="True" >
                <GridProperties FastFilterFields="InventoryID,InventoryItem__Descr">
                    <Layout ColumnsMenu="False" ></Layout>
                </GridProperties>
            </px:PXSelector> 
            <px:PXSelector ID="edRevisionID" runat="server" DataField="RevisionID" CommitChanges="True"  ></px:PXSelector>
            <px:PXSegmentMask ID="edInventoryID" runat="server" DataField="InventoryID" DisplayMode="Hint" CommitChanges="True" ></px:PXSegmentMask>
            <px:PXDateTimeEdit CommitChanges="True" ID="edBOMDate" runat="server" DataField="BOMDate"></px:PXDateTimeEdit>
            <px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartColumn="True" LabelsWidth="S" ControlSize="SM" ></px:PXLayoutRule>
            <px:PXCheckBox ID="chkIgnoreReplenishmentSettings" runat="server" DataField="IgnoreReplenishmentSettings" CommitChanges="True" ></px:PXCheckBox>
            <px:PXCheckBox ID="chkIncludeBomsOnHold" runat="server" DataField="IncludeBomsOnHold" CommitChanges="True" ></px:PXCheckBox>
            <px:PXCheckBox ID="chkRollCosts" runat="server" DataField="RollCosts" CommitChanges="True" ></px:PXCheckBox>
            <px:PXCheckBox ID="chkIgnoreMinMaxLotSizeValues" runat="server" DataField="IgnoreMinMaxLotSizeValues" Style="margin-left: 25px" CommitChanges="True" ></px:PXCheckBox>
            <px:PXCheckBox ID="chkUseCurrentInventoryCost" runat="server" DataField="UseCurrentInventoryCost" CommitChanges="True" ></px:PXCheckBox>
			<px:PXCheckBox runat="server" ID="CstPXCheckBox1" DataField="UsrEnblItemRoundUp" /></Template>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
	</px:PXFormView>
</asp:Content>