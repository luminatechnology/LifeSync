<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="LM300010.aspx.cs" Inherits="Page_LM300010" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource Height="" ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="AVLCustomizations.AVLCreateVendorListEntry"
        PrimaryView="AVLTableView"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView ID="form" runat="server" DataSourceID="ds" DataMember="AVLTableView" Width="100%" Height="130px" AllowAutoHide="false">
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
					<px:PXGrid ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Details" AllowAutoHide="false">
						<Levels>
							<px:PXGridLevel DataMember="AVLLineView">
							    <Columns>
								<px:PXGridColumn DataField="Avlnbr" Width="140" ></px:PXGridColumn>
								<px:PXGridColumn DataField="LineNbr" Width="50" ></px:PXGridColumn>
								<px:PXGridColumn AllowFilter="True" CommitChanges="True" DataField="VendorID" Width="220" ></px:PXGridColumn>
								<px:PXGridColumn DataField="VendorName" Width="220" ></px:PXGridColumn>
								<px:PXGridColumn CommitChanges="True" DataField="InventoryID" Width="100" ></px:PXGridColumn>
								<px:PXGridColumn DataField="InventoryDesc" Width="450" ></px:PXGridColumn>
								<px:PXGridColumn DataField="Remark" Width="280" ></px:PXGridColumn>
								<px:PXGridColumn DataField="OrigAVLNbr" Width="140" ></px:PXGridColumn></Columns>
							
								<RowTemplate>
									<px:PXSegmentMask CommitChanges="True" runat="server" ID="CstPXSegmentMask13" DataField="VendorID" FilterByAllFields="True" >
										<AutoCallBack Enabled="" ></AutoCallBack></px:PXSegmentMask>
																<px:PXSelector runat="server" ID="CstPXSelector14" DataField="InventoryID" FilterByAllFields="True" /></RowTemplate></px:PXGridLevel>
						</Levels>
						<AutoSize Container="Window" Enabled="True" MinHeight="150" ></AutoSize>
						<ActionBar >
						</ActionBar>
					</px:PXGrid>
				</Template>
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
					
		<Mode AllowAddNew="False" />
		<Mode AllowDelete="False" />
		<Mode AllowUpdate="False" /></px:PXGrid>
				</Template>
            </px:PXTabItem>
        </Items>
        <CallbackCommands>
        </CallbackCommands>
        <AutoSize Enabled="True" Container="Window" ></AutoSize>
    </px:PXTab>
</asp:Content>