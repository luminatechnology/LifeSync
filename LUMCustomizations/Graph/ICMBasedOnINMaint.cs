using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JAMS.AM;
using JAMS.AM.Attributes;
using LumCustomizations.DAC;
using LumCustomizations.Graph;
using LUMCustomizations.DAC;
using LUMCustomizations.Library;
using LuminaExcelLibrary;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CM;
using PX.Objects.IN;
using PX.Objects.PO;
using static LumCustomizations.Graph.InternalCostModelMaint;

namespace LUMCustomizations.Graph
{
	public class ICMBasedOnINMaint : PXGraph<ICMBasedOnINMaint>
	{

		public PXSave<VisualBOMFilter> Save;
		public PXCancel<VisualBOMFilter> Cancel;

		public PXFilter<VisualBOMFilter> Filter;

		[PXFilterable]
		public PXFilteredProcessing<ICMBasedOnIN, VisualBOMFilter, Where<ICMBasedOnIN.bOMID,
			   Between<Current<VisualBOMFilter.start_InventoryID>, Current<VisualBOMFilter.end_InventoryID>>>> ICMBasedOnINList;

        public SelectFrom<AMBomItem>.
               InnerJoin<AMBomItemBomAggregate>.On<AMBomItem.bOMID.IsEqual<AMBomItemBomAggregate.bOMID>.And<AMBomItem.revisionID.IsEqual<AMBomItemBomAggregate.revisionID>>>.
               InnerJoin<InventoryItem>.On<AMBomItem.inventoryID.IsEqual<InventoryItem.inventoryID>>.View viewVisualBOM;

        public IEnumerable<AMBomItem> _amBomItems;
        public IEnumerable<AMBomMatl> _amBomMatl;
        public IEnumerable<InventoryItem> _inventoryItems;

        public ICMBasedOnINMaint()
		{
            ICMBasedOnINList.SetProcessAllVisible(false);
			ICMBasedOnINList.SetProcessVisible(false);

            Report.AddMenuAction(ICMBasedOnINExcel);
        }

        #region PXAction

        public PXAction<VisualBOMFilter> Report;
        [PXUIField(DisplayName = "Reports", MapEnableRights = PXCacheRights.Select)]
        [PXButton(MenuAutoOpen = true)]
        protected void report() { }

        public PXAction<VisualBOMFilter> ICMBasedOnINExcel;
        [PXButton()]
        [PXUIField(DisplayName = "Internal Cost Model Based On Inventory Excel", MapEnableRights = PXCacheRights.Select)]
        protected IEnumerable iCMBasedOnINExcel(PXAdapter adapter)
        {
            #region Varaible
            InternalCostModelMaint icmGraph = CreateInstance<InternalCostModelMaint>();
            _amBomItems = SelectFrom<AMBomItem>.View.Select(this).RowCast<AMBomItem>().ToList();
            _amBomMatl = SelectFrom<AMBomMatl>.View.Select(this).RowCast<AMBomMatl>().ToList();
            _inventoryItems = SelectFrom<InventoryItem>.View.Select(this).RowCast<InventoryItem>().ToList();
            var row = this.GetCacheCurrent<VisualBOMFilter>().Current;
            // Setting Calc ojb
            icmGraph.SettingCalaObject();
            var aMBomItems = viewVisualBOM.Select().FirstTableItems.Where(x => x.BOMID?.CompareTo(row.Start_InventoryID) >= 0 && x.BOMID?.CompareTo(row.End_InventoryID) <= 0)?.ToList();
            #endregion
            
            int rowNum = 0;
            int materialNo = 1;
            decimal materialSum = 0;
            decimal _SetUpSum = 0;
            decimal _TotalCost = 0;
            
            #region Excel
            XSSFWorkbook workBook = new XSSFWorkbook();
            var excelHelper = new ExcelHelper();

            #region Excel Style
            var HeaderStyle = new ExcelStyle(workBook).HeaderStyle().ExcelStyle;
            var NormalStyle = new ExcelStyle(workBook).NormalStyle().ExcelStyle;

            var NormalStyle_Center = workBook.CreateCellStyle();
            NormalStyle_Center.CloneStyleFrom(NormalStyle);
            NormalStyle_Center.Alignment = HorizontalAlignment.Center;

            var NormalStyle_Bold_Left = new ExcelStyle(workBook).NormalStyle(12, true).ExcelStyle;

            var NormaStyle_Bold_Right = workBook.CreateCellStyle();
            NormaStyle_Bold_Right.CloneStyleFrom(NormalStyle_Bold_Left);
            NormaStyle_Bold_Right.Alignment = HorizontalAlignment.Right;

            var NormalStyle_Bold_Left_Border = new ExcelStyle(workBook).NormalStyle(12, true, true).ExcelStyle;

            var NormalStyle_Bold_Center = workBook.CreateCellStyle();
            NormalStyle_Bold_Center.CloneStyleFrom(NormalStyle);
            NormalStyle_Bold_Center.Alignment = HorizontalAlignment.Center;

            // Tabel Content Style 
            var TableHeaderStyle = new ExcelStyle(workBook).TableHeaderStyle(12, true, true).ExcelStyle;

            // Tabel Content Style Alignment is Center
            var TableContentStyle = new ExcelStyle(workBook).TableHeaderStyle(11, false, true).ExcelStyle;

            // Tabel Content Style Alignment is Left
            var TableContentStyle_Left = workBook.CreateCellStyle();
            TableContentStyle_Left.CloneStyleFrom(TableContentStyle);
            TableContentStyle_Left.Alignment = HorizontalAlignment.Left;

            // Green Cell Style
            var GreenCellStyle = new ExcelStyle(workBook).SetCellColor(IndexedColors.SeaGreen.Index);

            // Grey Cell Style 
            var GreyCellStyle = new ExcelStyle(workBook).NormalStyle().ExcelStyle;
            GreyCellStyle.Alignment = HorizontalAlignment.Center;
            new ExcelStyle(workBook).SetCellColor(IndexedColors.Grey25Percent.Index, GreyCellStyle);

            // Yellow Cell Style
            var YellowCellStyle = new ExcelStyle(workBook).NormalStyle().ExcelStyle;
            YellowCellStyle.Alignment = HorizontalAlignment.Center;
            new ExcelStyle(workBook).SetCellColor(IndexedColors.Yellow.Index, YellowCellStyle);

            // TAN Cell Style
            var TANCellStyle = new ExcelStyle(workBook).NormalStyle().ExcelStyle;
            TANCellStyle.Alignment = HorizontalAlignment.Center;
            new ExcelStyle(workBook).SetCellColor(IndexedColors.Tan.Index, TANCellStyle);

            // GOLD Cell Style
            var GOLDCellStyle = new ExcelStyle(workBook).NormalStyle().ExcelStyle;
            GOLDCellStyle.Alignment = HorizontalAlignment.Center;
            new ExcelStyle(workBook).SetCellColor(IndexedColors.Gold.Index, GOLDCellStyle);

            // ROSE Cell Style
            var ROSECellStyle = new ExcelStyle(workBook).NormalStyle().ExcelStyle;
            ROSECellStyle.Alignment = HorizontalAlignment.Center;
            new ExcelStyle(workBook).SetCellColor(IndexedColors.Rose.Index, ROSECellStyle);

            #endregion

            ISheet sheet = workBook.CreateSheet("sheet");

            #region ReSize
            // Resize
            sheet.SetColumnWidth(1, 8 * 256);
            sheet.SetColumnWidth(2, 15 * 256);
            sheet.SetColumnWidth(3, 30 * 256);
            sheet.SetColumnWidth(4, 15 * 256);
            sheet.SetColumnWidth(5, 15 * 256);
            sheet.SetColumnWidth(6, 15 * 256);
            sheet.SetColumnWidth(7, 20 * 256);
            sheet.SetColumnWidth(8, 20 * 256);
            sheet.SetColumnWidth(9, 20 * 256);
            sheet.SetColumnWidth(10, 20 * 256);
            #endregion

            #region Herder Row(0~6)
            sheet.CreateRow(rowNum);
            sheet.GetRow(rowNum).CreateCell(0).SetCellValue("ABA Internal cost model Based On Inventory");
            sheet.GetRow(rowNum).GetCell(0).CellStyle = HeaderStyle;
            sheet.AddMergedRegion(new CellRangeAddress(rowNum, rowNum, 0, 10));
            rowNum += 2;
            #endregion

            foreach (var aMBomItem in aMBomItems)
            {

                // Get All Material Data
                var materialData = from t in PXGraph.CreateInstance<ICMBasedOnINMaint>().Select<AMBomMatl>()
                                   join i in this._inventoryItems on t.InventoryID equals i.InventoryID
                                   where t.BOMID == aMBomItem.BOMID && t.RevisionID == aMBomItem.RevisionID
                                   select new { material = t, inventoryCD = i.InventoryCD };

                #region 1.Material Cost Row
                sheet.CreateRow(rowNum);
                sheet.GetRow(rowNum).CreateCell(0).SetCellValue($"{materialNo}");
                sheet.GetRow(rowNum).GetCell(0).CellStyle = NormalStyle_Bold_Center;
                sheet.GetRow(rowNum).CreateCell(1).SetCellValue($"Material cost");
                sheet.GetRow(rowNum).GetCell(1).CellStyle = NormalStyle_Bold_Left;
                rowNum++;

                #region Table Header Row
                sheet.CreateRow(rowNum);
                sheet.GetRow(rowNum).CreateCell(0).SetCellValue($"Inventory");
                sheet.GetRow(rowNum).GetCell(0).CellStyle = TableHeaderStyle;
                sheet.GetRow(rowNum).CreateCell(1).SetCellValue($"NO");
                sheet.GetRow(rowNum).GetCell(1).CellStyle = TableHeaderStyle;
                sheet.GetRow(rowNum).CreateCell(2).SetCellValue($"Part No");
                sheet.GetRow(rowNum).GetCell(2).CellStyle = TableHeaderStyle;
                sheet.GetRow(rowNum).CreateCell(3).SetCellValue($"Name");
                sheet.GetRow(rowNum).GetCell(3).CellStyle = TableHeaderStyle;
                sheet.GetRow(rowNum).CreateCell(4).SetCellValue($"U/P(RMB)");
                sheet.GetRow(rowNum).GetCell(4).CellStyle = TableHeaderStyle;
                sheet.GetRow(rowNum).CreateCell(5).SetCellValue($"U/P(HKD)");
                sheet.GetRow(rowNum).GetCell(5).CellStyle = TableHeaderStyle;
                sheet.GetRow(rowNum).CreateCell(6).SetCellValue($"U/P(USD)");
                sheet.GetRow(rowNum).GetCell(6).CellStyle = TableHeaderStyle;
                sheet.GetRow(rowNum).CreateCell(7).SetCellValue($"Unit");
                sheet.GetRow(rowNum).GetCell(7).CellStyle = TableHeaderStyle;
                sheet.GetRow(rowNum).CreateCell(8).SetCellValue($"QPA");
                sheet.GetRow(rowNum).GetCell(8).CellStyle = TableHeaderStyle;
                sheet.GetRow(rowNum).CreateCell(9).SetCellValue($"Materail cost \n(US$)");
                sheet.GetRow(rowNum).GetCell(9).CellStyle = TableHeaderStyle;
                sheet.GetRow(rowNum).CreateCell(10).SetCellValue($"Materail MOQ");
                sheet.GetRow(rowNum).GetCell(10).CellStyle = TableHeaderStyle;

                #endregion

                #region Table Content
                foreach (var amMaterial in materialData.OrderBy(x => x.inventoryCD))
                {
                    System.Collections.Stack visualBOM = new Stack();
                    visualBOM = icmGraph.GetVisualBOM(
                        visualBOM,
                        new AMProdMatl()
                        {
                            InventoryID = amMaterial.material.InventoryID,
                            UnitCost = amMaterial.material.UnitCost,
                            UOM = amMaterial.material.UOM,
                            TotalQtyRequired = amMaterial.material.QtyReq,
                            BatchSize = amMaterial.material.BatchSize,
                            QtyReq = amMaterial.material.QtyReq
                        },
                        1);
                    decimal? materailCost = 0;
                    // Material Details
                    foreach (BomNode node in visualBOM)
                    {
                        sheet.CreateRow(++rowNum);
                        // InventoryID
                        sheet.GetRow(rowNum).CreateCell(0).SetCellValue($"{aMBomItem.BOMID}");
                        sheet.GetRow(rowNum).GetCell(0).CellStyle = TableContentStyle;
                        // NO
                        sheet.GetRow(rowNum).CreateCell(1).SetCellValue($"{node.NodeLevel}");
                        sheet.GetRow(rowNum).GetCell(1).CellStyle = TableContentStyle;
                        // Part No
                        sheet.GetRow(rowNum).CreateCell(2).SetCellValue($"{node.PartNo}");
                        sheet.GetRow(rowNum).GetCell(2).CellStyle = TableContentStyle_Left;
                        // Name
                        sheet.GetRow(rowNum).CreateCell(3).SetCellValue($"{node.Name}");
                        sheet.GetRow(rowNum).GetCell(3).CellStyle = TableContentStyle_Left;
                        // U/P(RMB/HKD/USD)
                        sheet.GetRow(rowNum).CreateCell(4);
                        sheet.GetRow(rowNum).CreateCell(5);
                        sheet.GetRow(rowNum).CreateCell(6);

                        sheet.GetRow(rowNum).GetCell(4).SetCellValue($"{node.RMB}");
                        sheet.GetRow(rowNum).GetCell(5).SetCellValue($"{node.HKD}");
                        sheet.GetRow(rowNum).GetCell(6).SetCellValue($"{node.USD}");

                        sheet.GetRow(rowNum).GetCell(4).CellStyle = TableContentStyle;
                        sheet.GetRow(rowNum).GetCell(5).CellStyle = TableContentStyle;
                        sheet.GetRow(rowNum).GetCell(6).CellStyle = TableContentStyle;

                        // Unit
                        sheet.GetRow(rowNum).CreateCell(7).SetCellValue($"{node.Unit}");
                        sheet.GetRow(rowNum).GetCell(7).CellStyle = TableContentStyle;
                        // QPA
                        sheet.GetRow(rowNum).CreateCell(8).SetCellValue($"{node.QPA}");
                        sheet.GetRow(rowNum).GetCell(8).CellStyle = TableContentStyle;
                        // Materail Cost(US$)
                        sheet.GetRow(rowNum).CreateCell(9).SetCellValue(node.Cost == null ? 0 : double.Parse(node.Cost?.ToString("N5")));
                        sheet.GetRow(rowNum).GetCell(9).CellStyle = TableContentStyle;
                        // Materail MOQ
                        sheet.GetRow(rowNum).CreateCell(10).SetCellValue($"");
                        sheet.GetRow(rowNum).GetCell(10).CellStyle = TableContentStyle;
                    }
                }
                #endregion

                // Materail Sum
                rowNum += 2;
                sheet.CreateRow(rowNum);
                sheet.GetRow(rowNum).CreateCell(7).SetCellValue($"material sum");
                sheet.GetRow(rowNum).GetCell(7).CellStyle = NormalStyle;
                sheet.GetRow(rowNum).CreateCell(9).CellFormula = $"SUM(J10:J{rowNum - 1})";
                sheet.GetRow(rowNum).GetCell(9).CellStyle = GreyCellStyle;
                _SetUpSum += materialSum;

                // Green Cell
                sheet.CreateRow(++rowNum);
                excelHelper.CreateBlankCell(sheet, rowNum, 0, 10, GreenCellStyle.ExcelStyle);
                #endregion

                rowNum += 2;
                materialNo++;
            }
            #endregion

            var exceldata = new MemoryStream();
            workBook.Write(exceldata);
            string path = $"Internal Cost Model Based on Inventory.xlsx";
            var info = new PX.SM.FileInfo(path, null, exceldata.ToArray());
            throw new PXRedirectToFileException(info, true);
        }

        #endregion

        public IEnumerable iCMBasedOnINList()
		{
			 var aMBomItem = SelectFrom<AMBomItem>.
               InnerJoin<AMBomItemBomAggregate>.On<AMBomItem.bOMID.IsEqual<AMBomItemBomAggregate.bOMID>.And<AMBomItem.revisionID.IsEqual<AMBomItemBomAggregate.revisionID>>>.
               InnerJoin<InventoryItem>.On<AMBomItem.inventoryID.IsEqual<InventoryItem.inventoryID>>.View.Select(this).RowCast<AMBomItem>().ToList();

            var currFilter = this.Filter.Current;
			if (!string.IsNullOrEmpty(currFilter?.Start_InventoryID) && !string.IsNullOrEmpty(currFilter?.End_InventoryID))
			{
                aMBomItem = aMBomItem.Where(x => x.BOMID?.CompareTo(currFilter.Start_InventoryID) >= 0 && x.BOMID?.CompareTo(currFilter.End_InventoryID) <= 0)?.ToList();
			}
			else
			{
                aMBomItem = new List<AMBomItem>();
			}
			
            var inventoryData = SelectFrom<InventoryItem>.View.Select(this).RowCast<InventoryItem>().ToList();
			int lineNbr = 1;
			foreach (var item in aMBomItem)
			{
                var summary = new ICMBasedOnIN();
                summary.LineNbr = lineNbr++;
				summary.BOMID = item.BOMID;
                summary.RevisionID = item.RevisionID;
				summary.InventoryCD = inventoryData.FirstOrDefault(x => x.InventoryID == item.InventoryID).InventoryCD;
                summary.Descr = item.Descr;
                summary.SubItemID = item.SubItemID;
                yield return summary;
			}

			yield return null;
		}

		// Token: 0x02000769 RID: 1897
		[PXCacheName("Visual BOM Filter")]
		[Serializable]
		public class VisualBOMFilter : IBqlTable
		{
			// Token: 0x170006E4 RID: 1764
			// (get) Token: 0x06001A41 RID: 6721 RVA: 0x0005E39C File Offset: 0x0005C59C
			// (set) Token: 0x06001A42 RID: 6722 RVA: 0x0005E3A8 File Offset: 0x0005C5A8
			[BomID]
            [PXUIField(DisplayName = "Start Inventory ID")]
			[PXSelector(typeof(Search2<AMBomItem.bOMID, InnerJoin<AMBomItemBomAggregate, On<AMBomItem.bOMID, Equal<AMBomItemBomAggregate.bOMID>, And<AMBomItem.revisionID, Equal<AMBomItemBomAggregate.revisionID>>>, InnerJoin<InventoryItem, On<AMBomItem.inventoryID, Equal<InventoryItem.inventoryID>>>>>), new Type[]
			{
                typeof(AMBomItem.inventoryID),
                typeof(AMBomItem.bOMID),
				typeof(AMBomItem.revisionID),
				typeof(AMBomItem.subItemID),
				typeof(AMBomItem.siteID),
				typeof(AMBomItem.descr),
				typeof(InventoryItem.itemClassID),
				typeof(InventoryItem.descr)
			})]
			public virtual string Start_InventoryID { get; set; }
			public abstract class start_InventoryID : PX.Data.BQL.BqlString.Field<start_InventoryID> { }

			[BomID]
            [PXUIField(DisplayName = "End Inventory ID")]
            [PXSelector(typeof(Search2<AMBomItem.bOMID, InnerJoin<AMBomItemBomAggregate, On<AMBomItem.bOMID, Equal<AMBomItemBomAggregate.bOMID>, And<AMBomItem.revisionID, Equal<AMBomItemBomAggregate.revisionID>>>, InnerJoin<InventoryItem, On<AMBomItem.inventoryID, Equal<InventoryItem.inventoryID>>>>>), new Type[]
			{
                typeof(AMBomItem.inventoryID),
                typeof(AMBomItem.bOMID),
				typeof(AMBomItem.revisionID),
				typeof(AMBomItem.subItemID),
				typeof(AMBomItem.siteID),
				typeof(AMBomItem.descr),
				typeof(InventoryItem.itemClassID),
				typeof(InventoryItem.descr)
			})]
			public virtual string End_InventoryID { get; set; }
			public abstract class end_InventoryID : PX.Data.BQL.BqlString.Field<end_InventoryID> { }
		}
		
	}
}