<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="LM209002.aspx.cs" Inherits="Page_LM209002" Title="Untitled Page" %>
<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" Runat="Server">
	<px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="LUMCustomizations.Graph.ICMBasedOnINMaint"
        PrimaryView="Filter"
        >
		<CallbackCommands>

		</CallbackCommands>
	</px:PXDataSource>
</asp:Content>
<asp:Content ID="cont2" ContentPlaceHolderID="phF" Runat="Server">
	<px:PXFormView SkinID="" ID="form" runat="server" DataSourceID="ds" DataMember="Filter" Width="100%" Height="50px" AllowAutoHide="false">
		<Template>
			<px:PXSelector runat="server" ID="Start_InventoryID" DataField="start_InventoryID" CommitChanges="True" Size="S" ></px:PXSelector>
			<px:PXLayoutRule runat="server" ID="CstPXLayoutRule1" StartColumn="True" ></px:PXLayoutRule>
                        <px:PXSelector runat="server" ID="End_InventoryID" DataField="end_InventoryID" CommitChanges="True" Size="S" ></px:PXSelector></Template>
	</px:PXFormView>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" Runat="Server">
    <px:PXGrid SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="PrimaryInquire" AllowAutoHide="false">
        <Levels>
            <px:PXGridLevel DataMember="ICMBasedOnINList">
                <Columns>
	<px:PXGridColumn DataField="LineNbr" Width="100" ></px:PXGridColumn>
	<px:PXGridColumn DataField="InventoryCD" Width="200" ></px:PXGridColumn>
	<px:PXGridColumn DataField="RevisionID" Width="200" ></px:PXGridColumn>
	<px:PXGridColumn DataField="BOMID" Width="200" ></px:PXGridColumn>
	<px:PXGridColumn DataField="SubItemID" Width="150" ></px:PXGridColumn>
	<px:PXGridColumn DataField="Descr" Width="400" ></px:PXGridColumn></Columns>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Container="Window" Enabled="True" MinHeight="150" />
        <ActionBar>
        </ActionBar>
    </px:PXGrid>
</asp:Content>