<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="LM202000.aspx.cs" Inherits="Page_LM202000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="LumCustomizations.Graph.LifeSyncPreferenceMaint"
        PrimaryView="MasterView">
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="MasterView" Width="100%" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"/>
			<px:PXPanel runat="server" ID="PnlCust1" Caption="Proforma Invoice" RenderStyle="Fieldset">
				<px:PXCheckBox runat="server" ID="chkProformaInvoice" DataField="ProformaInvoicePrinting"></px:PXCheckBox>
				<px:PXCheckBox runat="server" ID="chkBubbleNumberPrinting" DataField="BubbleNumberPrinting"></px:PXCheckBox>
				<px:PXDropDown runat="server" ID="ddlInternalCostModelRateType" DataField="InternalCostModelRateType" Width="100px"></px:PXDropDown>
			</px:PXPanel>
		</Template>
		<AutoSize Container="Window" Enabled="True" MinHeight="200" />
	</px:PXFormView>
</asp:Content>

