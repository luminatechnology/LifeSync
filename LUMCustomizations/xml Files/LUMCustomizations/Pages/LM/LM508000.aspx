<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="LM508000.aspx.cs" Inherits="Page_LM508000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <%--<px:PXButton ID="btnTest" runat="server" CommandName="AMBomCostSettings" CommandSourceID="ds" Text="GetBOM"></px:PXButton>--%>
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="LUMCustomizations.Graph.LumCostRoll" PrimaryView="ProcBomCostRecs" BorderStyle="NotSet">
        <CallbackCommands>
            <px:PXDSCallbackCommand Name="ViewBOM" Visible="False" DependOnGrid="grid" />
            <%--<px:PXDSCallbackCommand Name="Archive" Visible="False" />--%>
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>

<asp:Content ID="cont2" ContentPlaceHolderID="phF" runat="Server">
    <px:PXFormView ID="form" runat="server" DataSourceID="ds" Style="z-index: 100" Width="100%" DataMember="rollsettings" DefaultControlID="edRollSettings"
        Caption="Parameters" NoteField="">
        <Template>
            <px:PXNumberEdit ID="edLotSize" runat="server" Enabled="True" DataField="LotSize" CommitChanges="true" />
            <px:PXLayoutRule ID="PXLayoutRule23" runat="server" StartGroup="True" GroupCaption="" StartColumn="True" LabelsWidth="S"
                ControlSize="S" Merge="True" />
            <px:PXDropDown ID="edSnglMlti" runat="server" AllowNull="False" DataField="SnglMlti" CommitChanges="true" />
        </Template>
    </px:PXFormView>
</asp:Content>

<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXGrid ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Inquire" TabIndex="2700" SyncPosition="True">
        <Levels>
            <px:PXGridLevel DataKeyNames="BOMID,RevisionID,UserID" DataMember="ProcBomCostRecs">
                <RowTemplate>
                    <px:PXCheckBox ID="edSelected" runat="server" DataField="Selected" Enabled="true" />
                    <px:PXTextEdit ID="edBOMID" runat="server" DataField="BOMID" />
                    <px:PXTextEdit ID="RevisionID" runat="server" DataField="RevisionID" />
                    <px:PXSegmentMask ID="edSiteID" runat="server" DataField="SiteID" AllowEdit="True" />
                    <px:PXSegmentMask ID="edInventoryID" runat="server" DataField="InventoryID" AllowEdit="True" />
                    <px:PXSegmentMask ID="edSubItemID" runat="server" DataField="SubItemID" />
                    <px:PXNumberEdit ID="edUnitCost" runat="server" DataField="UnitCost" />
                    <px:PXNumberEdit ID="edMatlManufacturedCost" runat="server" DataField="MatlManufacturedCost" />
                    <px:PXNumberEdit ID="edMatlNonManufacturedCost" runat="server" DataField="MatlNonManufacturedCost" />
                    <px:PXNumberEdit ID="edMatlCost" runat="server" DataField="MatlCost" />
                    <px:PXNumberEdit ID="edFLaborCost" runat="server" DataField="FLaborCost" />
                    <px:PXNumberEdit ID="edVLaborCost" runat="server" DataField="VLaborCost" />
                    <px:PXNumberEdit ID="edFOvdCost" runat="server" DataField="FOvdCost" />
                    <px:PXNumberEdit ID="edVOvdCost" runat="server" DataField="VOvdCost" />
                    <px:PXNumberEdit ID="edToolCost" runat="server" DataField="ToolCost" />
                    <px:PXNumberEdit ID="edMachCost" runat="server" DataField="MachCost" />
                    <px:PXNumberEdit ID="edSubcontractMaterialCost" runat="server" DataField="SubcontractMaterialCost" />
                    <px:PXNumberEdit ID="edReferenceMaterialCost" runat="server" DataField="ReferenceMaterialCost" />
                    <px:PXNumberEdit ID="edLotSize" runat="server" DataField="LotSize" />
                    <px:PXDropDown ID="edStatus" runat="server" DataField="AMBomItem__Status" />
                    <px:PXTextEdit ID="edBomRevDescr" runat="server" DataField="AMBomItem__Descr" />
                    <px:PXCheckBox ID="edMultiLevelProcess" runat="server" DataField="MultiLevelProcess" />
                    <px:PXNumberEdit ID="edLevel" runat="server" DataField="Level" />
                    <px:PXCheckBox ID="edIsDefaultBom" runat="server" DataField="IsDefaultBom" />
                    <px:PXMaskEdit ID="edFixedLaborTime" runat="server" DataField="FixedLaborTime" Width="108px" />
                    <px:PXMaskEdit ID="edVariableLaborTime" runat="server" DataField="VariableLaborTime" Width="108px" />
                    <px:PXMaskEdit ID="edMachineTime" runat="server" DataField="MachineTime" Width="108px" />
                    <px:PXSegmentMask CommitChanges="True" ID="edItemClassID" runat="server" DataField="ItemClassID" AllowEdit="True" />
                    <px:PXNumberEdit ID="edStdCost" runat="server" DataField="StdCost" Enabled="False" />
                    <px:PXNumberEdit ID="edPendingStdCost" runat="server" DataField="PendingStdCost" />
                </RowTemplate>
                <Columns>
                    <px:PXGridColumn DataField="Selected" Width="30px" TextAlign="Center" Type="CheckBox" AllowCheckAll="true" />
                    <px:PXGridColumn DataField="BOMID" Width="130px" LinkCommand="ViewBOM" />
                    <px:PXGridColumn DataField="RevisionID" Width="99px" />
                    <px:PXGridColumn DataField="SiteID" Width="130px" />
                    <px:PXGridColumn DataField="InventoryID" Width="130px" />
                    <px:PXGridColumn DataField="SubItemID" Width="81px" />
                    <px:PXGridColumn DataField="UnitCost" Width="108px" TextAlign="Right" />
                    <px:PXGridColumn DataField="MatlManufacturedCost" Width="108px" TextAlign="Right" />
                    <px:PXGridColumn DataField="MatlNonManufacturedCost" Width="108px" TextAlign="Right" />
                    <px:PXGridColumn DataField="MatlCost" Width="108px" TextAlign="Right" />
                    <px:PXGridColumn DataField="FLaborCost" TextAlign="Right" Width="108px" />
                    <px:PXGridColumn DataField="VLaborCost" TextAlign="Right" Width="108px" />
                    <px:PXGridColumn DataField="FOvdCost" TextAlign="Right" Width="108px" />
                    <px:PXGridColumn DataField="VOvdCost" TextAlign="Right" Width="108px" />
                    <px:PXGridColumn DataField="ToolCost" TextAlign="Right" Width="108px" />
                    <px:PXGridColumn DataField="MachCost" TextAlign="Right" Width="108px" />
                    <px:PXGridColumn DataField="SubcontractMaterialCost" TextAlign="Right" Width="120px" />
                    <px:PXGridColumn DataField="ReferenceMaterialCost" TextAlign="Right" Width="120px" />
                    <px:PXGridColumn DataField="LotSize" TextAlign="Right" Width="108px" />
                    <px:PXGridColumn DataField="AMBomItem__Status" />
                    <px:PXGridColumn DataField="AMBomItem__Descr" Width="175px" />
                    <px:PXGridColumn DataField="MultiLevelProcess" TextAlign="Center" Type="CheckBox" />
                    <px:PXGridColumn DataField="Level" Width="75px" />
                    <px:PXGridColumn DataField="IsDefaultBom" TextAlign="Center" Type="CheckBox" />
                    <px:PXGridColumn DataField="FixedLaborTime" Width="108px" TextAlign="Right" />
                    <px:PXGridColumn DataField="VariableLaborTime" Width="108px" TextAlign="Right" />
                    <px:PXGridColumn DataField="MachineTime" Width="108px" TextAlign="Right" />
                    <px:PXGridColumn DataField="ItemClassID" Width="108px" TextAlign="Right" />
                    <px:PXGridColumn DataField="StdCost" Width="108px" TextAlign="Right" />
                    <px:PXGridColumn DataField="PendingStdCost" Width="108px" TextAlign="Right" />
                </Columns>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Container="Window" Enabled="True" MinHeight="200" />
    </px:PXGrid>
</asp:Content>

