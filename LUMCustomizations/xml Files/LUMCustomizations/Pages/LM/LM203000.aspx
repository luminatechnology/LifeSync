<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="LM203000.aspx.cs" Inherits="Page_LM2030000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>
<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%" TypeName="LumCustomizations.Graph.LumItemsCOCMaint" PrimaryView="_viewItemsCOC">
        <CallbackCommands>
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="_viewItemsCOC" NoteIndicator="True" FilesIndicator="True" ActivityIndicator="True"
        ActivityField="NoteActivity" DefaultControlID="edInventoryCD">
        <Template>
            <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
            <px:PXSegmentMask ID="edInventoryCD" runat="server" DataField="InventoryID" DataSourceID="ds" AutoRefresh="true">
            </px:PXSegmentMask>
            <px:PXLayoutRule runat="server" ColumnSpan="2" />
            <px:PXTextEdit ID="edDescr" runat="server" DataField="InventoryID_Description" />
            <px:PXDropDown ID="cbENDC" runat="server" DataField="EndCustomer"></px:PXDropDown>
        </Template>
    </px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXTab ID="tab" runat="server" Width="100%" Height="606px" DataSourceID="ds" DataMember="_viewLine" FilesIndicator="False" NoteIndicator="False">
        <Items>
            <px:PXTabItem Text="Material">
                <Template>
                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                    <px:PXTextEdit ID="edMaterialProductDesc" TextMode="MultiLine" runat="server" DataField="MaterialProductDesc" Height="250px" Width="400px"></px:PXTextEdit>
                    <px:PXTextEdit ID="edMaterialDetailedDesc" TextMode="MultiLine" runat="server" DataField="MaterialDetailedDesc" Height="250px" Width="400px"></px:PXTextEdit>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="COC">
                <Template>
                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                    <px:PXTextEdit ID="edCOCProductDesc" TextMode="MultiLine" runat="server" DataField="COCProductDesc" Height="250px" Width="400px"></px:PXTextEdit>
                    <px:PXTextEdit ID="edCOCDetailedDesc" TextMode="MultiLine" runat="server" DataField="COCDetailedDesc" Height="250px" Width="400px"></px:PXTextEdit>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Test">
                <Template>
                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                    <px:PXTextEdit ID="edTESTProductDesc" TextMode="MultiLine" runat="server" DataField="TESTProductDesc" Height="250px" Width="400px"></px:PXTextEdit>
                    <px:PXTextEdit ID="edTESTDetailedDesc" TextMode="MultiLine" runat="server" DataField="TESTDetailedDesc" Height="250px" Width="400px"></px:PXTextEdit>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Re:ROHS">
                <Template>
                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                    <px:PXTextEdit ID="edREROHSProductDesc" TextMode="MultiLine" runat="server" DataField="REROHSProductDesc" Height="250px" Width="400px"></px:PXTextEdit>
                    <px:PXTextEdit ID="edREROHSDetailedDesc" TextMode="MultiLine" runat="server" DataField="REROHSDetailedDesc" Height="250px" Width="400px"></px:PXTextEdit>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="ROHS and REACH">
                <Template>
                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                    <px:PXTextEdit ID="edREACHProductDesc" TextMode="MultiLine" runat="server" DataField="REACHProductDesc" Height="250px" Width="400px"></px:PXTextEdit>
                    <px:PXTextEdit ID="edREACHDetailedDesc" TextMode="MultiLine" runat="server" DataField="REACHDetailedDesc" Height="250px" Width="400px"></px:PXTextEdit>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Material Compliant">
                <Template>
                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                    <px:PXTextEdit ID="edCompliantproductdesc" TextMode="MultiLine" runat="server" DataField="Compliantproductdesc" Height="250px" Width="400px"></px:PXTextEdit>
                    <px:PXTextEdit ID="edCompliantdetaileddesc" TextMode="MultiLine" runat="server" DataField="Compliantdetaileddesc" Height="250px" Width="400px"></px:PXTextEdit>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="QC Manager">
                <Template>
                    <px:PXLayoutRule runat="server" StartColumn="True" LabelsWidth="SM" ControlSize="XM" />
                    <px:PXTextEdit ID="edQCProductDesc" TextMode="MultiLine" runat="server" DataField="QCProductDesc" Height="250px" Width="400px"></px:PXTextEdit>
                    <px:PXTextEdit ID="edQCDetailedDesc" TextMode="MultiLine" runat="server" DataField="QCDetailedDesc" Height="250px" Width="400px"></px:PXTextEdit>
                </Template>
            </px:PXTabItem>
        </Items>
    </px:PXTab>
</asp:Content>
