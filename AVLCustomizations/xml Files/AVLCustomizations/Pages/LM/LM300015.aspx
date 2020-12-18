<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="LM300015.aspx.cs" Inherits="Page_LM300015" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource Height="" ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="AVLCustomizations.AVLCancelVendorListEntry"
        PrimaryView="AVLTableView"
        >
		<CallbackCommands></CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView SkinID="Transport" ID="form" runat="server" DataSourceID="ds" DataMember="AVLTableView" Width="100%" Height="130px" AllowAutoHide="false">
		<Template>
			<px:PXLayoutRule ID="PXLayoutRule1" runat="server" StartRow="True"></px:PXLayoutRule>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule1" StartColumn="True" ></px:PXLayoutRule>
			<px:PXSelector runat="server" ID="CstPXSelector3" DataField="Avlnbr" FilterByAllFields="True" ></px:PXSelector>
			<px:PXDropDown runat="server" ID="CstPXDropDownEdit1" DataField="AVLStatus" ></px:PXDropDown>
			<px:PXDateTimeEdit runat="server" ID="CstPXDateTimeEdit5" DataField="Avldate" ></px:PXDateTimeEdit>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule2" StartColumn="True" ></px:PXLayoutRule>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit6" DataField="CreatedByID_Creator_displayName" ></px:PXTextEdit>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit9" DataField="AVLAction" ></px:PXTextEdit>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule7" StartRow="True" ></px:PXLayoutRule>
			<px:PXTextEdit runat="server" ID="CstPXTextEdit8" DataField="Descripton" ></px:PXTextEdit></Template>
	</px:PXFormView></asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
	<px:PXTab DataMember="" ID="tab" runat="server" Height="540px" Style="z-index: 100;" Width="100%">
        <Items>
            <px:PXTabItem Text="Document Details">
                <Template>
					<px:PXGrid SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Details" AllowAutoHide="false">
						<Levels>
							<px:PXGridLevel DataMember="AVLLineView">
							    <Columns>
								<px:PXGridColumn DataField="Avlnbr" Width="140" ></px:PXGridColumn>
								<px:PXGridColumn DataField="LineNbr" Width="50" ></px:PXGridColumn>
								<px:PXGridColumn CommitChanges="True" DataField="VendorID" Width="220" ></px:PXGridColumn>
								<px:PXGridColumn DataField="VendorName" Width="220" ></px:PXGridColumn>
								<px:PXGridColumn CommitChanges="True" DataField="InventoryID" Width="100" ></px:PXGridColumn>
								<px:PXGridColumn DataField="InventoryDesc" Width="450" ></px:PXGridColumn>
								<px:PXGridColumn DataField="Remark" Width="280" ></px:PXGridColumn>
								<px:PXGridColumn DataField="OrigAVLNbr" Width="140" ></px:PXGridColumn></Columns>
							
								<RowTemplate>
									<px:PXSelector FilterByAllFields="True" runat="server" ID="CstPXSelector11" DataField="InventoryID" ></px:PXSelector>
									<px:PXSegmentMask runat="server" ID="CstPXSegmentMask12" DataField="VendorID" FilterByAllFields="True" ></px:PXSegmentMask></RowTemplate></px:PXGridLevel>
						</Levels>
						<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
						<ActionBar >
                                                
							<CustomItems>
								<px:PXToolBarButton CommandName="" CommandSourceID="" PopupPanel="CstAVLDialog" Text="Add AVL Nbr" ></px:PXToolBarButton></CustomItems></ActionBar>
					
						<Mode AllowAddNew="False" /></px:PXGrid></Template>
    		</px:PXTabItem>
    		<px:PXTabItem Text="Event History" runat="server">
                <Template>
                	<px:PXGrid ID="grid1" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Details" AllowAutoHide="false">
						<Levels>
							<px:PXGridLevel DataMember="EventHistoryView">
							    <Columns>
								<px:PXGridColumn DataField="AVLNbr" Width="140" ></px:PXGridColumn>
								<px:PXGridColumn DataField="Type" Width="50" ></px:PXGridColumn>
								<px:PXGridColumn DataField="Action" Width="220" ></px:PXGridColumn>
								<px:PXGridColumn DataField="CreatedByID_Creator_Username" Width="70" ></px:PXGridColumn>
								<px:PXGridColumn DataField="CreatedDateTime" Width="90" ></px:PXGridColumn>
								<px:PXGridColumn DataField="LastModifiedByID_Modifier_Username" Width="70" ></px:PXGridColumn>
								<px:PXGridColumn DataField="LastModifiedDateTime" Width="90" ></px:PXGridColumn></Columns>
							</px:PXGridLevel>
						</Levels>
						<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
						<ActionBar >
						</ActionBar>
					
		<Mode AllowAddNew="False" AllowUpdate="False" AllowDelete="False" ></Mode>
		<Mode AllowUpdate="False" ></Mode>
		<Mode AllowAddNew="False" ></Mode></px:PXGrid>
				</Template>
            </px:PXTabItem>
        </Items>
        <CallbackCommands>
        </CallbackCommands>
        <AutoSize Enabled="True" Container="Window" ></AutoSize>
    </px:PXTab>
	<px:PXSmartPanel CommandSourceID="ds" CommandName="Add" Key="ApprovedVendListsDialogView" CaptionVisible="True" Caption="Approved Vendor List" Height="400px" Width="70%" runat="server" ID="CstAVLDialog">
		<px:PXFormView DataMember="Filter" runat="server" ID="CstFormView3" >
			<Template>
				<px:PXSelector FilterByAllFields="" CommitChanges="True" runat="server" ID="CstPXSelector9" DataField="Avlnbr" ></px:PXSelector></Template></px:PXFormView>
		<px:PXGrid Width="" SkinID="Details" runat="server" ID="CstPXGrid4">
			<Levels>
				<px:PXGridLevel DataMember="ApprovedVendListsDialogView" >
					<Columns>
						<px:PXGridColumn Type="CheckBox" AllowCheckAll="True" TextAlign="Center" DataField="Selected" Width="60" ></px:PXGridColumn>
						<px:PXGridColumn DataField="Avlnbr" Width="80" ></px:PXGridColumn>
						<px:PXGridColumn DataField="LineNbr" Width="30" ></px:PXGridColumn>
						<px:PXGridColumn DataField="Avldate" Width="80" ></px:PXGridColumn>
						<px:PXGridColumn DataField="Vendor" Width="100" ></px:PXGridColumn>
						<px:PXGridColumn DataField="VendorName" Width="120" ></px:PXGridColumn>
						<px:PXGridColumn DataField="InventoryID" Width="70" ></px:PXGridColumn>
						<px:PXGridColumn DataField="InventoryDesc" Width="120" ></px:PXGridColumn>
						<px:PXGridColumn DataField="Remark" Width="120" ></px:PXGridColumn>
						<px:PXGridColumn DataField="LastModifiedByID_Modifier_displayName" Width="70" ></px:PXGridColumn></Columns></px:PXGridLevel></Levels>
			<AutoSize Enabled="True" MinHeight="1" ></AutoSize></px:PXGrid>
		<px:PXPanel SkinID="Buttons" Height="" runat="server" ID="CstAVLDialogPanel">
			<px:PXButton runat="server" ID="CstButton10" Text="Add" >
				<AutoCallBack Target="ds" Command="Add" ></AutoCallBack></px:PXButton>
			<px:PXButton DialogResult="OK" Text="Add &amp; Close" runat="server" ID="CstButton7" >
				<AutoCallBack Command="add" ></AutoCallBack></px:PXButton>
			<px:PXButton DialogResult="Cancel" Text="CANCEL" runat="server" ID="CstButton8" ></px:PXButton></px:PXPanel></px:PXSmartPanel></asp:Content>