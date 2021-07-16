<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormView.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="LM202000.aspx.cs" Inherits="Page_LM202000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormView.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="LumCustomizations.Graph.LifeSyncPreferenceMaint"
        PrimaryView="MasterView">
        <CallbackCommands>
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="MasterView" Width="100%" AllowAutoHide="false">
        <Template>
            <px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
            <px:PXPanel runat="server" ID="PnlCust1" Caption="Proforma Invoice" RenderStyle="Fieldset">
                <px:PXCheckBox runat="server" ID="chkProformaInvoice" DataField="ProformaInvoicePrinting"></px:PXCheckBox>
                <px:PXCheckBox runat="server" ID="chkBubbleNumberPrinting" DataField="BubbleNumberPrinting"></px:PXCheckBox>
                <px:PXCheckBox runat="server" ID="chkCrossRateOverride" DataField="CrossRateOverride"></px:PXCheckBox>
                <px:PXCheckBox runat="server" ID="chkShowingTotalInHomeCurrency" DataField="ShowingTotalInHomeCurrency"></px:PXCheckBox>
                <px:PXCheckBox runat="server" ID="chkEnableJournalEnhance" DataField="EnableJournalEnhance"></px:PXCheckBox>
            </px:PXPanel>
            <px:PXLayoutRule LabelsWidth="M" GroupCaption="Internal Cost" runat="server" ID="CstPXLayoutRule6" StartGroup="True"></px:PXLayoutRule>
            <px:PXDropDown runat="server" ID="CstPXDropDown7" DataField="InternalCostModelRateType"></px:PXDropDown>
            <px:PXLayoutRule runat="server" ID="CstPXLayoutRule4" StartGroup="True" GroupCaption="Multi-Level Cost"></px:PXLayoutRule>
            <px:PXCheckBox runat="server" ID="CstPXCheckBox5" DataField="EnableProdCostAnlys" AlignLeft="True"></px:PXCheckBox>
            <px:PXLayoutRule LabelsWidth="M" runat="server" ID="PXLayoutRule2" StartGroup="True" GroupCaption="Production Order"></px:PXLayoutRule>
            <px:PXNumberEdit runat="server" ID="edMaxOverIssue" DataField="MaxOverIssue"></px:PXNumberEdit>
        </Template>
        <AutoSize Container="Window" Enabled="True" MinHeight="200"></AutoSize>
    </px:PXFormView>
</asp:Content>

