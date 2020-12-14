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
                    <px:PXRichTextEdit ID="edMaterialProductDesc" runat="server" DataField="MaterialProductDesc" Style="border-width: 0px; border-top-width: 1px; width: 100%;"
                        AllowAttached="true" AllowSearch="true" AllowLoadTemplate="false" AllowSourceMode="true">
                        <AutoSize Enabled="True" MinHeight="216"></AutoSize>
                    </px:PXRichTextEdit>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Material2">
                <Template>
                    <px:PXRichTextEdit ID="edMaterialProductDesc2" runat="server" DataField="MaterialProductDesc2" Style="border-width: 0px; border-top-width: 1px; width: 100%;"
                        AllowAttached="true" AllowSearch="true" AllowLoadTemplate="false" AllowSourceMode="true">
                        <AutoSize Enabled="True" MinHeight="216"></AutoSize>
                    </px:PXRichTextEdit>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="COC">
                <Template>
                    <px:PXRichTextEdit ID="edCOCProductDesc" runat="server" DataField="COCProductDesc" Style="border-width: 0px; border-top-width: 1px; width: 100%;"
                        AllowAttached="true" AllowSearch="true" AllowLoadTemplate="false" AllowSourceMode="true">
                        <AutoSize Enabled="True" MinHeight="216"></AutoSize>
                    </px:PXRichTextEdit>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Test">
                <Template>
                    <px:PXRichTextEdit ID="edTestProductDesc" runat="server" DataField="TestProductDesc" Style="border-width: 0px; border-top-width: 1px; width: 100%;"
                        AllowAttached="true" AllowSearch="true" AllowLoadTemplate="false" AllowSourceMode="true">
                        <AutoSize Enabled="True" MinHeight="216"></AutoSize>
                    </px:PXRichTextEdit>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Re:ROHS">
                <Template>
                    <px:PXRichTextEdit ID="edREROHSProductDesc" runat="server" DataField="REROHSProductDesc" Style="border-width: 0px; border-top-width: 1px; width: 100%;"
                        AllowAttached="true" AllowSearch="true" AllowLoadTemplate="false" AllowSourceMode="true">
                        <AutoSize Enabled="True" MinHeight="216"></AutoSize>
                    </px:PXRichTextEdit>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="ROHS and REACH">
                <Template>
                    <px:PXRichTextEdit ID="edREACHProductDesc" runat="server" DataField="REACHProductDesc" Style="border-width: 0px; border-top-width: 1px; width: 100%;"
                        AllowAttached="true" AllowSearch="true" AllowLoadTemplate="false" AllowSourceMode="true">
                        <AutoSize Enabled="True" MinHeight="216"></AutoSize>
                    </px:PXRichTextEdit>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="ROHS and REACH2">
                <Template>
                    <px:PXRichTextEdit ID="edREACHProductDesc2" runat="server" DataField="REACHProductDesc2" Style="border-width: 0px; border-top-width: 1px; width: 100%;"
                        AllowAttached="true" AllowSearch="true" AllowLoadTemplate="false" AllowSourceMode="true">
                        <AutoSize Enabled="True" MinHeight="216"></AutoSize>
                    </px:PXRichTextEdit>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="Material Compliant">
                <Template>
                    <px:PXRichTextEdit ID="edCompliantProductDesc" runat="server" DataField="CompliantProductDesc" Style="border-width: 0px; border-top-width: 1px; width: 100%;"
                        AllowAttached="true" AllowSearch="true" AllowLoadTemplate="false" AllowSourceMode="true">
                        <AutoSize Enabled="True" MinHeight="216"></AutoSize>
                    </px:PXRichTextEdit>
                </Template>
            </px:PXTabItem>
            <px:PXTabItem Text="QC Manager">
                <Template>
                    <px:PXRichTextEdit ID="edQCProductDesc" runat="server" DataField="QCProductDesc" Style="border-width: 0px; border-top-width: 1px; width: 100%;"
                        AllowAttached="true" AllowSearch="true" AllowLoadTemplate="false" AllowSourceMode="true">
                        <AutoSize Enabled="True" MinHeight="216"></AutoSize>
                    </px:PXRichTextEdit>
                </Template>
            </px:PXTabItem>
        </Items>
    </px:PXTab>
</asp:Content>
