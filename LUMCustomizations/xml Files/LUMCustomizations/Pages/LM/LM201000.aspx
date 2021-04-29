<%@ Page Language="C#" MasterPageFile="~/MasterPages/FormDetail.master" AutoEventWireup="true" ValidateRequest="false" CodeFile="LM201000.aspx.cs" Inherits="Page_LM201000" Title="Untitled Page" %>

<%@ MasterType VirtualPath="~/MasterPages/FormDetail.master" %>

<asp:Content ID="cont1" ContentPlaceHolderID="phDS" runat="Server">
    <px:PXDataSource ID="ds" runat="server" Visible="True" Width="100%"
        TypeName="LumCustomizations.Graph.LumShipmentPlanMaint" PrimaryView="ShipPlan">
        <CallbackCommands>
            <%--<px:PXDSCallbackCommand Name="Report" Visible="True" CommitChanges="true" StartNewGroup="true"></px:PXDSCallbackCommand>--%>
        </CallbackCommands>
    </px:PXDataSource>
</asp:Content>
<asp:Content ID="cont3" ContentPlaceHolderID="phG" runat="Server">
    <px:PXGrid SyncPosition="True" ID="grid" runat="server" DataSourceID="ds" Width="100%" Height="150px" SkinID="Primary" AllowAutoHide="false">
        <Levels>
            <px:PXGridLevel DataMember="ShipPlan">
                <Columns>
                    <px:PXGridColumn DataField="ShipmentPlanID" Width="140"></px:PXGridColumn>
                    <px:PXGridColumn CommitChanges="True" DataField="Confirmed" Width="60" Type="CheckBox"></px:PXGridColumn>
                    <px:PXGridColumn DataField="ProdOrdID" Width="140" CommitChanges="True"></px:PXGridColumn>
                    <px:PXGridColumn DataField="SortOrder" Width="70"></px:PXGridColumn>
                    <px:PXGridColumn DataField="ProdLine" Width="280"></px:PXGridColumn>
                    <px:PXGridColumn DataField="LotSerialNbr" Width="280"></px:PXGridColumn>
                    <px:PXGridColumn CommitChanges="True" DataField="SOLineNoteID" Width="70"></px:PXGridColumn>
                    <px:PXGridColumn DataField="OrderNbr" Width="140"></px:PXGridColumn>
                    <px:PXGridColumn DataField="OrderType" Width="70"></px:PXGridColumn>
                    <px:PXGridColumn DataField="Customer" Width="180" DisplayMode="Hint"></px:PXGridColumn>
                    <px:PXGridColumn DataField="CustomerLocationID" Width="70"></px:PXGridColumn>
                    <px:PXGridColumn DataField="CustomerOrderNbr" Width="180"></px:PXGridColumn>
                    <px:PXGridColumn DataField="UOM" Width="70"></px:PXGridColumn>
                    <px:PXGridColumn DataField="OrderDate" Width="90"></px:PXGridColumn>
                    <px:PXGridColumn DataField="LineNbr" Width="70"></px:PXGridColumn>
                    <px:PXGridColumn DataField="InventoryID" Width="70"></px:PXGridColumn>
                    <px:PXGridColumn DataField="CustomerPN" Width="180"></px:PXGridColumn>
                    <px:PXGridColumn DataField="OrderQty" Width="100"></px:PXGridColumn>
                    <px:PXGridColumn DataField="OpenQty" Width="100"></px:PXGridColumn>
                    <px:PXGridColumn DataField="RequestDate" Width="90"></px:PXGridColumn>
                    <px:PXGridColumn DataField="PlannedShipDate" Width="90" CommitChanges="True"></px:PXGridColumn>
                    <px:PXGridColumn CommitChanges="True" DataField="PlannedShipQty" Width="100"></px:PXGridColumn>
                    <px:PXGridColumn CommitChanges="True" DataField="ShipVia" Width="220"></px:PXGridColumn>
                    <px:PXGridColumn DataField="ShipVia_Carrier_description" Width="220"></px:PXGridColumn>
                    <px:PXGridColumn DataField="ShipmentNbr" Width="140"></px:PXGridColumn>
                    <px:PXGridColumn DataField="CartonSize" Width="140"></px:PXGridColumn>
                    <px:PXGridColumn DataField="CartonQty" Width="100"></px:PXGridColumn>
                    <px:PXGridColumn DataField="NetWeight" Width="100"></px:PXGridColumn>
                    <px:PXGridColumn DataField="GrossWeight" Width="100"></px:PXGridColumn>
                    <px:PXGridColumn DataField="PalletWeight" Width="100"></px:PXGridColumn>
                    <px:PXGridColumn DataField="MEAS" Width="100"></px:PXGridColumn>
                    <px:PXGridColumn DataField="DimWeight" Width="100"></px:PXGridColumn>
                    <px:PXGridColumn DataField="BRNbr" Width="280"></px:PXGridColumn>
                    <px:PXGridColumn DataField="QtytoProd" Width="100"></px:PXGridColumn>
                    <px:PXGridColumn DataField="QtyComplete" Width="100"></px:PXGridColumn>
                    <px:PXGridColumn DataField="NbrOfShipment" Width="70"></px:PXGridColumn>
                    <px:PXGridColumn DataField="TotalShipNbr" Width="70"></px:PXGridColumn>
                    <px:PXGridColumn DataField="StartLabelNbr" Width="70"></px:PXGridColumn>
                    <px:PXGridColumn DataField="EndLabelNbr" Width="70"></px:PXGridColumn>
                    <px:PXGridColumn DataField="StartCartonNbr" Width="70"></px:PXGridColumn>
                    <px:PXGridColumn DataField="EndCartonNbr" Width="70"></px:PXGridColumn>
                    <px:PXGridColumn DataField="Remarks" Width="220"></px:PXGridColumn>
                </Columns>

                <RowTemplate>
                    <px:PXSegmentMask AllowEdit="True" runat="server" ID="CstPXSegmentMask6" DataField="CustomerLocationID"></px:PXSegmentMask>
                    <px:PXSegmentMask AllowEdit="True" runat="server" ID="CstPXSegmentMask7" DataField="InventoryID"></px:PXSegmentMask>
                    <px:PXSelector runat="server" ID="CstPXSelector8" DataField="OrderNbr" AllowEdit="True"></px:PXSelector>
                    <px:PXSelector AllowEdit="True" runat="server" ID="CstPXSelector9" DataField="OrderType"></px:PXSelector>
                    <px:PXSelector AllowEdit="True" runat="server" ID="CstPXSelector11" DataField="ShipVia"></px:PXSelector>
                    <px:PXSelector AutoRefresh="True" runat="server" ID="CstPXSelector12" DataField="ProdOrdID" AllowEdit="True"></px:PXSelector>
                    <px:PXSelector runat="server" ID="CstPXSelector13" DataField="ShipmentNbr" AllowEdit="True" AutoRefresh="True"></px:PXSelector>
                    <px:PXSelector runat="server" ID="edCustomer" DataField="Customer"></px:PXSelector>
                </RowTemplate>
            </px:PXGridLevel>
        </Levels>
        <AutoSize Container="Window" Enabled="True" MinHeight="150"></AutoSize>
        <ActionBar>
        </ActionBar>

        <Mode InitNewRow="True"></Mode>
    </px:PXGrid>
    <%--<script src="~/Scripts/jquery-3.1.1.min.js"></script>--%>
    <script type="text/javascript">

        window.onload = function () {
            window.setTimeout(function () {
                var filterBtn = document.querySelectorAll('div[data-cmd="FilterShow"]')[0].parentNode.parentNode;
                var ReportBtn = document.querySelectorAll('[data-drop="1"]')[2].parentNode;
                filterBtn.appendChild(ReportBtn);
            }, 100);
        };
    </script>

</asp:Content>

