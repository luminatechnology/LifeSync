<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="LM513000.aspx.cs" Inherits="Page_LM513000" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="LUMCustomizations.Graph.LUMMultiLevelBomProc" PrimaryView="Filter">
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="Filter" Width="100%" Height="75px" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXSelector runat="server" ID="CstPXSelector2" DataField="BOMID" CommitChanges="True" ></px:PXSelector>
			<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask3" DataField="InventoryID" ></px:PXSegmentMask>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule12" StartColumn="True" ></px:PXLayoutRule>
			<px:PXDateTimeEdit CommitChanges="True" runat="server" ID="CstPXDateTimeEdit1" DataField="BOMDate" ></px:PXDateTimeEdit>
			<px:PXCheckBox CommitChanges="True" runat="server" ID="CstPXCheckBox13" DataField="UsrEnblItemRoundUp" ></px:PXCheckBox></Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXGrid NoteIndicator="False" FilesIndicator="False" SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Inquire" AllowAutoHide="false">
		<Levels>
			<px:PXGridLevel DataMember="Results">
			    <Columns>
				<px:PXGridColumn DataField="CreatedDateTime" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CreatedByID" Width="220" ></px:PXGridColumn>
				<px:PXGridColumn DataField="BOMID" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="RevisionID" Width="120" ></px:PXGridColumn>
				<px:PXGridColumn DataField="InventoryID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="SiteID" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="SubItemID" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="EffStartDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="EffEndDate" Width="90" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CompInventoryID" Width="70" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CompSubItemID" Width="140" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CompUnitCost" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CompQtyReq" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CompMatlCost" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CompManufMatlCost" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CompPurchMatl" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CompVarOvdCost" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CompFixedOvdCost" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CompToolCost" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CompMachineCost" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CompExtCost" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="CompTotalExtCost" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="OperationCD" Width="120" />
				<px:PXGridColumn DataField="MatlCost" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="LotSize" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="VariableLaborCost" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="FixedLaborCost" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="VariableOvdCost" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="FixedOvdCost" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="ToolCost" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="MachineCost" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="SubcontractCost" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="UnitCost" Width="100" ></px:PXGridColumn>
				<px:PXGridColumn DataField="TotalCost" Width="100" ></px:PXGridColumn></Columns>
			
				<RowTemplate>
					<px:PXSegmentMask AllowEdit="True" runat="server" ID="CstPXSegmentMask5" DataField="CompInventoryID" ></px:PXSegmentMask>
					<px:PXSegmentMask runat="server" ID="CstPXSegmentMask6" DataField="CompSiteID" ></px:PXSegmentMask>
					<px:PXSegmentMask runat="server" ID="CstPXSegmentMask7" DataField="InventoryID" AllowEdit="True" ></px:PXSegmentMask>
					<px:PXSegmentMask AllowEdit="True" runat="server" ID="CstPXSegmentMask9" DataField="SiteID" ></px:PXSegmentMask>
					<px:PXSegmentMask AllowEdit="True" runat="server" ID="CstPXSegmentMask10" DataField="SubItemID" ></px:PXSegmentMask>
								<px:PXSegmentMask runat="server" ID="CstPXSegmentMask11" DataField="CompSubItemID" AllowEdit="True" ></px:PXSegmentMask></RowTemplate></px:PXGridLevel>
		</Levels>
		<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
		<ActionBar >
		</ActionBar>
	</px:PXGrid>
</asp:Content>