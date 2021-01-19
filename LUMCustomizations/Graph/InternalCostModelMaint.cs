using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JAMS.AM;
using JAMS.AM.Attributes;
using LumCustomizations.DAC;
using LuminaExcelLibrary;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CM;
using PX.Objects.CS;
using PX.Objects.IN;
using PX.Objects.PO;

namespace LumCustomizations.Graph
{
    public class InternalCostModelMaint : ProdDetail
    {
        #region Initialize

        public InternalCostModelMaint()
        {
            Report.AddMenuAction(InternalCostModelExcel);
        }

        #endregion

        #region Override DAC
        [AMOrderTypeField(IsKey = true, Visibility = PXUIVisibility.SelectorVisible)]
        [PXDefault("CM")]
        [AMOrderTypeSelector(typeof(OrderTypeFunction.planning))]
        [PXRestrictorAttribute(typeof(Where<AMOrderType.active, Equal<True>>), "Order Type is inactive.")]
        public virtual void _(Events.CacheAttached<AMProdItem.orderType> e) { }
        #endregion

        #region PXAction

        public PXAction<AMProdItem> Report;
        [PXUIField(DisplayName = "Reports", MapEnableRights = PXCacheRights.Select)]
        [PXButton(MenuAutoOpen = true)]
        protected void report() { }

        public PXAction<AMProdItem> InternalCostModelExcel;
        [PXButton()]
        [PXUIField(DisplayName = "Internal Cost Model Excel", MapEnableRights = PXCacheRights.Select)]
        protected IEnumerable internalCostModelExcel(PXAdapter adapter)
        {
            #region Varaible
            int rowNum = 0;
            decimal materialSum = 0;
            // Currency Rate Type of ICM
            var _ICMRateType = PXGraph.CreateInstance<InternalCostModelMaint>().Select<LifeSyncPreference>().Select(x => x.InternalCostModelRateType).FirstOrDefault();
            var _AMProdAttribute = base.ProductionAttributes.Select().FirstTableItems;
            var _AMProdItem = base.ProdItemRecords.Select().FirstTableItems.Where(x => x.ProdOrdID == ((AMProdItem)this.Caches[typeof(AMProdItem)].Current).ProdOrdID).FirstOrDefault();
            // Get Stock Item Vendor Details By LastModifierTime
            var _POvenderDetail = PXGraph.CreateInstance<InternalCostModelMaint>()
                                         .Select<POVendorInventory>()
                                         .ToList()
                                         .GroupBy(x => new { x.InventoryID})
                                         .Select(x => x.OrderByDescending(y => y.LastModifiedDateTime).FirstOrDefault());
            var _AMProdMaterail = from t in base.ProdMatlRecords.Select().FirstTableItems
                                  join i in PXGraph.CreateInstance<InternalCostModelMaint>().Select<InventoryItem>()
                                   on t.InventoryID equals i.InventoryID
                                  join v in _POvenderDetail
                                   on t.InventoryID equals v.InventoryID into result
                                  from r in result.DefaultIfEmpty()
                                  select new
                                  {
                                      t.InventoryID,
                                      t.Descr,
                                      t.UnitCost,
                                      t.UOM,
                                      t.TotalQtyRequired,
                                      t.ScrapFactor,
                                      i.InventoryCD,
                                      venderDetail = r
                                  };
            var _AMProdOper = base.ProdOperRecords.Select().FirstTableItems;

            // ReplenishmentSource From Inventory 
            var _InventoryItem = PXGraph.CreateInstance<InternalCostModelMaint>().Select<INItemSite>().Select(x => new { x.InventoryID, x.ReplenishmentSource });
            // Effect Curry Rate
            var _EffectCuryRate = GetCuryRateRecordEffData().Where(x => x.CuryRateType == _ICMRateType).ToList();
            if (_EffectCuryRate.Count == 0)
                throw new PXException("Please Select ICM Rate Type !!");

            decimal _SetUpSum = 0;
            decimal _TotalCost = 0;
            var _StandardWorkingTime = (_AMProdOper.FirstOrDefault().RunUnitTime / _AMProdOper.FirstOrDefault().RunUnits).Value;
            // AMProdAttribute 
            var _ENDC = _AMProdAttribute.Where(x => x.AttributeID.Equals("ENDC")).FirstOrDefault()?.Value;
            var _EAU = _AMProdAttribute.Where(x => x.AttributeID.Equals("EAU")).FirstOrDefault()?.Value;
            var _LBSCCost = _AMProdAttribute.Where(x => x.AttributeID == "LBSC").FirstOrDefault()?.Value ?? "0";
            var _MFSCCost = _AMProdAttribute.Where(x => x.AttributeID == "MFSC").FirstOrDefault()?.Value ?? "0";
            var _OHSCCost = _AMProdAttribute.Where(x => x.AttributeID == "OHSC").FirstOrDefault()?.Value ?? "0";
            var _SETUPSCCost = _AMProdAttribute.Where(x => x.AttributeID == "SETUPSC").FirstOrDefault()?.Value ?? "0";
            var _PRODYIELD = _AMProdAttribute.Where(x => x.AttributeID == "PRODYIELD").FirstOrDefault()?.Value ?? "0";
            var _ABADGSELL = _AMProdAttribute.Where(x => x.AttributeID == "ABADGSELL").FirstOrDefault()?.Value ?? "0";
            var _HKOHSCCost = _AMProdAttribute.Where(x => x.AttributeID == "HKOHSC").FirstOrDefault()?.Value ?? "0";
            var _ABISELLCost = _AMProdAttribute.Where(x => x.AttributeID == "ABISELL").FirstOrDefault()?.Value ?? "0";
            #endregion

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
            sheet.CreateRow(0);
            sheet.GetRow(0).CreateCell(0).SetCellValue("ABA Internal cost model");
            sheet.GetRow(0).GetCell(0).CellStyle = HeaderStyle;
            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, 10));

            sheet.CreateRow(2);
            sheet.GetRow(2).CreateCell(1).SetCellValue($"Quotation No :{_AMProdItem.ProdOrdID}");
            sheet.GetRow(2).GetCell(1).CellStyle = NormalStyle;
            sheet.GetRow(2).CreateCell(4).SetCellValue($"Revision No :01");
            sheet.GetRow(2).GetCell(4).CellStyle = NormalStyle;
            sheet.GetRow(2).CreateCell(6).SetCellValue($"Date :{_AMProdItem.ProdDate.Value.ToString("yyyy-MM-dd")}");
            sheet.GetRow(2).GetCell(6).CellStyle = NormalStyle;

            sheet.CreateRow(3);
            sheet.GetRow(3).CreateCell(1).SetCellValue($"Customer :{_ENDC}");
            sheet.GetRow(3).GetCell(1).CellStyle = NormalStyle;
            sheet.GetRow(3).CreateCell(4).SetCellValue($"Project Name :");
            sheet.GetRow(3).GetCell(4).CellStyle = NormalStyle;
            sheet.GetRow(3).CreateCell(6).SetCellValue($"Porject No :{_AMProdAttribute.Where(x => x.AttributeID.Equals("PROJECTNO")).FirstOrDefault()?.Value}");
            sheet.GetRow(3).GetCell(6).CellStyle = NormalStyle;

            sheet.CreateRow(4);
            sheet.GetRow(4).CreateCell(1).SetCellValue($"Customer contact person :");
            sheet.GetRow(4).GetCell(1).CellStyle = NormalStyle;
            sheet.GetRow(4).CreateCell(6).SetCellValue($"Drawing :{_AMProdAttribute.Where(x => x.AttributeID.Equals("DRAWING")).FirstOrDefault()?.Value}");
            sheet.GetRow(4).GetCell(6).CellStyle = NormalStyle;

            sheet.CreateRow(5);
            sheet.GetRow(5).CreateCell(1).SetCellValue($"EAU :{_EAU}");
            sheet.GetRow(5).GetCell(1).CellStyle = NormalStyle;

            sheet.CreateRow(6);
            sheet.GetRow(6).CreateCell(2).SetCellValue($"1FT");
            sheet.GetRow(6).GetCell(2).CellStyle = NormalStyle_Bold_Left;
            #endregion

            #region 1.Material Cost Row(7~rowNum)
            sheet.CreateRow(7);
            sheet.GetRow(7).CreateCell(0).SetCellValue($"1");
            sheet.GetRow(7).GetCell(0).CellStyle = NormalStyle_Bold_Center;
            sheet.GetRow(7).CreateCell(1).SetCellValue($"Material cost ");
            sheet.GetRow(7).GetCell(1).CellStyle = NormalStyle_Bold_Left;
            #region Table Header Row(8)
            sheet.CreateRow(8);
            sheet.GetRow(8).CreateCell(1).SetCellValue($"NO");
            sheet.GetRow(8).GetCell(1).CellStyle = TableHeaderStyle;
            sheet.GetRow(8).CreateCell(2).SetCellValue($"Part No");
            sheet.GetRow(8).GetCell(2).CellStyle = TableHeaderStyle;
            sheet.GetRow(8).CreateCell(3).SetCellValue($"Name");
            sheet.GetRow(8).GetCell(3).CellStyle = TableHeaderStyle;
            sheet.GetRow(8).CreateCell(4).SetCellValue($"U/P(RMB)");
            sheet.GetRow(8).GetCell(4).CellStyle = TableHeaderStyle;
            sheet.GetRow(8).CreateCell(5).SetCellValue($"U/P(HKD)");
            sheet.GetRow(8).GetCell(5).CellStyle = TableHeaderStyle;
            sheet.GetRow(8).CreateCell(6).SetCellValue($"U/P(USD)");
            sheet.GetRow(8).GetCell(6).CellStyle = TableHeaderStyle;
            sheet.GetRow(8).CreateCell(7).SetCellValue($"Unit");
            sheet.GetRow(8).GetCell(7).CellStyle = TableHeaderStyle;
            sheet.GetRow(8).CreateCell(8).SetCellValue($"QPA");
            sheet.GetRow(8).GetCell(8).CellStyle = TableHeaderStyle;
            sheet.GetRow(8).CreateCell(9).SetCellValue($"Materail cost \n(US$)");
            sheet.GetRow(8).GetCell(9).CellStyle = TableHeaderStyle;
            sheet.GetRow(8).CreateCell(10).SetCellValue($"Materail MOQ");
            sheet.GetRow(8).GetCell(10).CellStyle = TableHeaderStyle;

            #endregion
            rowNum = 8;
            foreach (var matl in _AMProdMaterail)
            {
                decimal? _materailCost = 0;
                var _ReplenishmentSource = _InventoryItem.Where(x => x.InventoryID == matl.InventoryID)
                    .FirstOrDefault()?.ReplenishmentSource;

                sheet.CreateRow(++rowNum);
                // NO
                sheet.GetRow(rowNum).CreateCell(1).SetCellValue($"{(_ReplenishmentSource.Equals("P") ? ".1" : _ReplenishmentSource.Equals("M") ? "..2" : "")}");
                sheet.GetRow(rowNum).GetCell(1).CellStyle = TableContentStyle;
                // Part No
                sheet.GetRow(rowNum).CreateCell(2).SetCellValue($"{matl.InventoryCD}");
                sheet.GetRow(rowNum).GetCell(2).CellStyle = TableContentStyle_Left;
                // Name
                sheet.GetRow(rowNum).CreateCell(3).SetCellValue($"{matl.Descr}");
                sheet.GetRow(rowNum).GetCell(3).CellStyle = TableContentStyle_Left;
                // U/P(RMB/HKD/USD)
                sheet.GetRow(rowNum).CreateCell(4);
                sheet.GetRow(rowNum).CreateCell(5);
                sheet.GetRow(rowNum).CreateCell(6);
                // IF Purchase Unit != Matailes UOM
                if (matl.venderDetail != null && (matl.venderDetail.LastPrice ?? 0) > 0)
                {
                    var _venderLastPrice = matl.venderDetail.LastPrice.Value;
                    if (matl.venderDetail.PurchaseUnit != matl.UOM)
                    {
                        var _INUnit = from t in PXGraph.CreateInstance<InternalCostModelMaint>().Select<INUnit>()
                                      where t.InventoryID == matl.InventoryID &&
                                            t.FromUnit == matl.venderDetail.PurchaseUnit &&
                                            t.ToUnit == matl.UOM
                                      select t;
                        _venderLastPrice = _INUnit == null ? _venderLastPrice
                                                           : _INUnit.FirstOrDefault().UnitMultDiv == "M" ? _venderLastPrice / (_INUnit.FirstOrDefault()?.UnitRate ?? 1)
                                                                                                         : _venderLastPrice * (_INUnit.FirstOrDefault()?.UnitRate ?? 1);
                    }

                    if (matl.venderDetail.CuryID == "CNY")
                    {
                        sheet.GetRow(rowNum).GetCell(4).SetCellValue($"{_venderLastPrice.ToString("N4")}");
                        _materailCost = _venderLastPrice * matl.UnitCost * matl.TotalQtyRequired * (1 + matl.ScrapFactor)
                                        * (_EffectCuryRate.Where(x => x.FromCuryID == "USD" && x.ToCuryID == "CNY").FirstOrDefault()?.RateReciprocal ?? 1);
                    }
                    else if (matl.venderDetail.CuryID == "HKD")
                    {
                        sheet.GetRow(rowNum).GetCell(5).SetCellValue($"{_venderLastPrice.ToString("N4")}");
                        _materailCost = _venderLastPrice * matl.UnitCost * matl.TotalQtyRequired * (1 + matl.ScrapFactor)
                                        * (_EffectCuryRate.Where(x => x.FromCuryID == "HKD" && x.ToCuryID == "CNY").FirstOrDefault()?.CuryRate ?? 1)
                                        * (_EffectCuryRate.Where(x => x.FromCuryID == "USD" && x.ToCuryID == "CNY").FirstOrDefault()?.RateReciprocal ?? 1);
                    }
                    else if (matl.venderDetail.CuryID == "USD")
                    {
                        sheet.GetRow(rowNum).GetCell(6).SetCellValue($"{_venderLastPrice.ToString("N4")}");
                        _materailCost = _venderLastPrice * matl.UnitCost * matl.TotalQtyRequired * (1 + matl.ScrapFactor);
                    }
                }
                else
                {
                    sheet.GetRow(rowNum).GetCell(4).SetCellValue($"{(matl.UnitCost.HasValue ? matl.UnitCost.Value.ToString("N4") : "")}");
                    _materailCost = (matl.UnitCost * matl.TotalQtyRequired * (1 + matl.ScrapFactor)).Value;
                }

                sheet.GetRow(rowNum).GetCell(4).CellStyle = TableContentStyle;
                sheet.GetRow(rowNum).GetCell(5).CellStyle = TableContentStyle;
                sheet.GetRow(rowNum).GetCell(6).CellStyle = TableContentStyle;
                // Unit
                sheet.GetRow(rowNum).CreateCell(7).SetCellValue($"{matl.UOM}");
                sheet.GetRow(rowNum).GetCell(7).CellStyle = TableContentStyle;
                // QPA
                sheet.GetRow(rowNum).CreateCell(8).SetCellValue($"{(matl.TotalQtyRequired * (1 + matl.ScrapFactor)).Value.ToString("N4")}");
                sheet.GetRow(rowNum).GetCell(8).CellStyle = TableContentStyle;
                // Materail Cost(US$)
                sheet.GetRow(rowNum).CreateCell(9).SetCellValue(double.Parse(_materailCost.Value.ToString("N5")));
                //sheet.GetRow(rowNum).CreateCell(9).SetCellType(CellType.Numeric);
                //sheet.GetRow(rowNum).CreateCell(9).CellStyle.DataFormat = workBook.CreateDataFormat().GetFormat("0.0000");
                sheet.GetRow(rowNum).GetCell(9).CellStyle = TableContentStyle;
                // Materail MOQ
                sheet.GetRow(rowNum).CreateCell(10).SetCellValue($"");
                sheet.GetRow(rowNum).GetCell(10).CellStyle = TableContentStyle;

                materialSum += (matl.UnitCost * matl.TotalQtyRequired * (1 + matl.ScrapFactor)).Value;
            }

            // Materail Sum
            rowNum += 2;
            sheet.CreateRow(rowNum);
            sheet.GetRow(rowNum).CreateCell(7).SetCellValue($"material sum");
            sheet.GetRow(rowNum).GetCell(7).CellStyle = NormalStyle;
            sheet.GetRow(rowNum).CreateCell(9).CellFormula = $"SUM(J10:J{rowNum-1})";
            //sheet.GetRow(rowNum).CreateCell(9).SetCellValue($"{materialSum.ToString("N4")}");
            sheet.GetRow(rowNum).GetCell(9).CellStyle = GreyCellStyle;
            _SetUpSum += materialSum;
            // Green Cell
            sheet.CreateRow(++rowNum);
                        excelHelper.CreateBlankCell(sheet, rowNum, 0, 10, GreenCellStyle.ExcelStyle);
            #endregion

            #region 2.Labor Cost

            // Title
            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 1, 10, TableContentStyle);
            sheet.GetRow(rowNum).CreateCell(0).SetCellValue($"2");
            sheet.GetRow(rowNum).GetCell(0).CellStyle = NormalStyle_Bold_Center;
            sheet.GetRow(rowNum).CreateCell(1).SetCellValue($"Labor Cost");
            sheet.GetRow(rowNum).GetCell(1).CellStyle = NormalStyle_Bold_Left_Border;
            sheet.GetRow(rowNum).CreateCell(8).SetCellValue($"RMB");
            sheet.GetRow(rowNum).GetCell(8).CellStyle = TableHeaderStyle;
            sheet.GetRow(rowNum).CreateCell(9).SetCellValue($"USD");
            sheet.GetRow(rowNum).GetCell(9).CellStyle = TableHeaderStyle;

            // Standard Working Time
            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 1, 10, TableContentStyle);
            sheet.GetRow(rowNum).CreateCell(1).SetCellValue($"Standard working time");
            sheet.GetRow(rowNum).GetCell(1).CellStyle = NormalStyle_Bold_Left_Border;
            sheet.GetRow(rowNum).CreateCell(3).SetCellValue($"{_StandardWorkingTime.ToString("N4")}");
            sheet.GetRow(rowNum).GetCell(3).CellStyle = TANCellStyle;
            sheet.GetRow(rowNum).CreateCell(4).SetCellValue($"Minute");
            sheet.GetRow(rowNum).GetCell(4).CellStyle = TableHeaderStyle;

            // Standard cost
            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 1, 10, TableContentStyle);
            sheet.GetRow(rowNum).CreateCell(1).SetCellValue($"Standard cost");
            sheet.GetRow(rowNum).GetCell(1).CellStyle = NormalStyle_Bold_Left_Border;
            sheet.GetRow(rowNum).CreateCell(3).SetCellValue($"{_LBSCCost}");
            sheet.GetRow(rowNum).GetCell(3).CellStyle = YellowCellStyle;
            sheet.GetRow(rowNum).CreateCell(4).SetCellValue($"RMB/minute");
            sheet.GetRow(rowNum).GetCell(4).CellStyle = TableHeaderStyle;

            // Sum
            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 1, 10);
            sheet.GetRow(rowNum).CreateCell(4).SetCellValue($"Labor Cost");
            sheet.GetRow(rowNum).GetCell(4).CellStyle = NormaStyle_Bold_Right;
            sheet.GetRow(rowNum).CreateCell(5).SetCellValue($"sum");
            sheet.GetRow(rowNum).GetCell(5).CellStyle = TableHeaderStyle;
            sheet.GetRow(rowNum).CreateCell(8).SetCellValue($"{_StandardWorkingTime * decimal.Parse(_LBSCCost)}");
            sheet.GetRow(rowNum).GetCell(8).CellStyle = GreyCellStyle;
            sheet.GetRow(rowNum).CreateCell(9).SetCellValue($"{(_StandardWorkingTime * decimal.Parse(_LBSCCost) * _EffectCuryRate.Where(x => x.FromCuryID == "USD").FirstOrDefault().RateReciprocal).Value.ToString("N4")}");
            sheet.GetRow(rowNum).GetCell(9).CellStyle = GreyCellStyle;
            _SetUpSum += (_StandardWorkingTime * decimal.Parse(_LBSCCost) * _EffectCuryRate.Where(x => x.FromCuryID == "USD").FirstOrDefault().RateReciprocal).Value;

            // Green Cells
            sheet.CreateRow(++rowNum);
                        excelHelper.CreateBlankCell(sheet, rowNum, 0, 10, GreenCellStyle.ExcelStyle);

            #endregion

            #region 3.Manufacture Cost

            // Title
            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 1, 10, TableContentStyle);
            sheet.GetRow(rowNum).CreateCell(0).SetCellValue($"3");
            sheet.GetRow(rowNum).GetCell(0).CellStyle = NormalStyle_Bold_Center;
            sheet.GetRow(rowNum).CreateCell(1).SetCellValue($"Manufacture cost");
            sheet.GetRow(rowNum).GetCell(1).CellStyle = NormalStyle_Bold_Left_Border;
            sheet.GetRow(rowNum).CreateCell(8).SetCellValue($"RMB");
            sheet.GetRow(rowNum).GetCell(8).CellStyle = TableHeaderStyle;
            sheet.GetRow(rowNum).CreateCell(9).SetCellValue($"USD");
            sheet.GetRow(rowNum).GetCell(9).CellStyle = TableHeaderStyle;

            // Standard Working Time
            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 1, 10, TableContentStyle);
            sheet.GetRow(rowNum).CreateCell(1).SetCellValue($"Standard working time");
            sheet.GetRow(rowNum).GetCell(1).CellStyle = NormalStyle_Bold_Left_Border;
            sheet.GetRow(rowNum).CreateCell(3).SetCellValue($"{_StandardWorkingTime.ToString("N4")}");
            sheet.GetRow(rowNum).GetCell(3).CellStyle = TANCellStyle;
            sheet.GetRow(rowNum).CreateCell(4).SetCellValue($"Minute");
            sheet.GetRow(rowNum).GetCell(4).CellStyle = TableHeaderStyle;

            // Standard cost
            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 1, 10);
            sheet.GetRow(rowNum).CreateCell(1).SetCellValue($"Standard cost");
            sheet.GetRow(rowNum).GetCell(1).CellStyle = NormalStyle_Bold_Left_Border;
            sheet.GetRow(rowNum).CreateCell(3).SetCellValue($"{_MFSCCost}");
            sheet.GetRow(rowNum).GetCell(3).CellStyle = YellowCellStyle;
            sheet.GetRow(rowNum).CreateCell(4).SetCellValue($"RMB/minute");
            sheet.GetRow(rowNum).GetCell(4).CellStyle = TableHeaderStyle;

            // Sum
            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 1, 10);
            sheet.GetRow(rowNum).CreateCell(4).SetCellValue($"Manufacture cost");
            sheet.GetRow(rowNum).GetCell(4).CellStyle = NormaStyle_Bold_Right;
            sheet.GetRow(rowNum).CreateCell(5).SetCellValue($"sum");
            sheet.GetRow(rowNum).GetCell(5).CellStyle = TableHeaderStyle;
            sheet.GetRow(rowNum).CreateCell(8).SetCellValue($"{_StandardWorkingTime * decimal.Parse(_MFSCCost)}");
            sheet.GetRow(rowNum).GetCell(8).CellStyle = GreyCellStyle;
            sheet.GetRow(rowNum).CreateCell(9).SetCellValue($"{(_StandardWorkingTime * decimal.Parse(_MFSCCost) * _EffectCuryRate.Where(x => x.FromCuryID == "USD").FirstOrDefault().RateReciprocal).Value.ToString("N4")}");
            sheet.GetRow(rowNum).GetCell(9).CellStyle = GreyCellStyle;
            _SetUpSum += (_StandardWorkingTime * decimal.Parse(_MFSCCost) * _EffectCuryRate.Where(x => x.FromCuryID == "USD").FirstOrDefault().RateReciprocal).Value;

            // Green Cells
            sheet.CreateRow(++rowNum);
                        excelHelper.CreateBlankCell(sheet, rowNum, 0, 10, GreenCellStyle.ExcelStyle);

            #endregion

            #region 4.Overhead

            // Title
            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 1, 10, TableContentStyle);
            sheet.GetRow(rowNum).CreateCell(0).SetCellValue($"4");
            sheet.GetRow(rowNum).GetCell(0).CellStyle = NormalStyle_Bold_Center;
            sheet.GetRow(rowNum).CreateCell(1).SetCellValue($"Overhead");
            sheet.GetRow(rowNum).GetCell(1).CellStyle = NormalStyle_Bold_Left_Border;
            sheet.GetRow(rowNum).CreateCell(8).SetCellValue($"RMB");
            sheet.GetRow(rowNum).GetCell(8).CellStyle = TableHeaderStyle;
            sheet.GetRow(rowNum).CreateCell(9).SetCellValue($"USD");
            sheet.GetRow(rowNum).GetCell(9).CellStyle = TableHeaderStyle;

            // Standard Working Time
            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 1, 10, TableContentStyle);
            sheet.GetRow(rowNum).CreateCell(1).SetCellValue($"Standard working time");
            sheet.GetRow(rowNum).GetCell(1).CellStyle = NormalStyle_Bold_Left_Border;
            sheet.GetRow(rowNum).CreateCell(3).SetCellValue($"{_StandardWorkingTime.ToString("N4")}");
            sheet.GetRow(rowNum).GetCell(3).CellStyle = TANCellStyle;
            sheet.GetRow(rowNum).CreateCell(4).SetCellValue($"Minute");
            sheet.GetRow(rowNum).GetCell(4).CellStyle = TableHeaderStyle;

            // Standard cost
            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 1, 10, TableContentStyle);
            sheet.GetRow(rowNum).CreateCell(1).SetCellValue($"Standard cost");
            sheet.GetRow(rowNum).GetCell(1).CellStyle = NormalStyle_Bold_Left_Border;
            sheet.GetRow(rowNum).CreateCell(3).SetCellValue($"{_OHSCCost}");
            sheet.GetRow(rowNum).GetCell(3).CellStyle = YellowCellStyle;
            sheet.GetRow(rowNum).CreateCell(4).SetCellValue($"RMB/minute");
            sheet.GetRow(rowNum).GetCell(4).CellStyle = TableHeaderStyle;

            // Sum
            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 1, 10);
            sheet.GetRow(rowNum).CreateCell(4).SetCellValue($"overhead");
            sheet.GetRow(rowNum).GetCell(4).CellStyle = NormaStyle_Bold_Right;
            sheet.GetRow(rowNum).CreateCell(5).SetCellValue($"sum");
            sheet.GetRow(rowNum).GetCell(5).CellStyle = TableHeaderStyle;
            sheet.GetRow(rowNum).CreateCell(8).SetCellValue($"{_StandardWorkingTime * decimal.Parse(_OHSCCost)}");
            sheet.GetRow(rowNum).GetCell(8).CellStyle = GreyCellStyle;
            sheet.GetRow(rowNum).CreateCell(9).SetCellValue($"{(_StandardWorkingTime * decimal.Parse(_OHSCCost) * _EffectCuryRate.Where(x => x.FromCuryID == "USD").FirstOrDefault().RateReciprocal).Value.ToString("N4")}");
            sheet.GetRow(rowNum).GetCell(9).CellStyle = GreyCellStyle;
            _SetUpSum += (_StandardWorkingTime * decimal.Parse(_OHSCCost) * _EffectCuryRate.Where(x => x.FromCuryID == "USD").FirstOrDefault().RateReciprocal).Value;

            // Green Cells
            sheet.CreateRow(++rowNum);
                        excelHelper.CreateBlankCell(sheet, rowNum, 0, 10, GreenCellStyle.ExcelStyle);

            #endregion

            #region 5.Set Up Cost

            // Title
            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 1, 10, TableContentStyle);
            sheet.GetRow(rowNum).CreateCell(0).SetCellValue($"5");
            sheet.GetRow(rowNum).GetCell(0).CellStyle = NormalStyle_Bold_Center;
            sheet.GetRow(rowNum).CreateCell(1).SetCellValue($"Set up cost ( for sample or small qty only)");
            sheet.GetRow(rowNum).GetCell(1).CellStyle = NormalStyle_Bold_Left_Border;

            // Create Blank Row
            for (int i = 0; i < 5; i++)
            {
                sheet.CreateRow(++rowNum);
                excelHelper.CreateBlankCell(sheet, rowNum, 1, 10, TableContentStyle);
            }

            // set up cost
            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 1, 10);
            sheet.GetRow(rowNum).CreateCell(4).SetCellValue($"set up cost");
            sheet.GetRow(rowNum).GetCell(4).CellStyle = TableHeaderStyle;
            sheet.GetRow(rowNum).CreateCell(5).SetCellValue($"sum");
            sheet.GetRow(rowNum).GetCell(5).CellStyle = TableHeaderStyle;
            sheet.GetRow(rowNum).CreateCell(8).SetCellValue($"2-4 total * rate");
            sheet.GetRow(rowNum).GetCell(8).CellStyle = TableHeaderStyle;
            sheet.GetRow(rowNum).CreateCell(9).SetCellValue($"{(decimal.Parse(_EAU ?? "0") * decimal.Parse(_SETUPSCCost)).ToString("N4")}");
            sheet.GetRow(rowNum).GetCell(9).CellStyle = TableHeaderStyle;

            // sum 1-5
            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 1, 10);
            sheet.GetRow(rowNum).CreateCell(8).SetCellValue($"Sum 1-5");
            sheet.GetRow(rowNum).GetCell(8).CellStyle = GOLDCellStyle;

            sheet.GetRow(rowNum).CreateCell(9).SetCellValue($"{_SetUpSum.ToString("N4")}");
            sheet.GetRow(rowNum).GetCell(9).CellStyle = GOLDCellStyle;

            // Green Cells
            sheet.CreateRow(++rowNum);
            sheet.CreateRow(++rowNum);
                        excelHelper.CreateBlankCell(sheet, rowNum, 0, 10, GreenCellStyle.ExcelStyle);

            #endregion

            #region 6.Production yield

            // Title
            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 1, 10, TableContentStyle);
            sheet.GetRow(rowNum).CreateCell(0).SetCellValue($"6");
            sheet.GetRow(rowNum).GetCell(0).CellStyle = NormalStyle_Bold_Center;
            sheet.GetRow(rowNum).CreateCell(1).SetCellValue($"production yield");
            sheet.GetRow(rowNum).GetCell(1).CellStyle = NormalStyle_Bold_Left_Border;

            // Standard yield rate
            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 1, 10, TableContentStyle);
            sheet.GetRow(rowNum).CreateCell(1).SetCellValue($"Standard yield rate");
            sheet.GetRow(rowNum).GetCell(1).CellStyle = NormalStyle_Bold_Left_Border;
            sheet.GetRow(rowNum).CreateCell(3).SetCellValue($"{_PRODYIELD}%");
            sheet.GetRow(rowNum).GetCell(3).CellStyle = TableContentStyle;

            // Sum
            _TotalCost = _SetUpSum * (1 + (decimal.Parse(_PRODYIELD) / 100));
            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 1, 10);
            sheet.GetRow(rowNum).CreateCell(8).SetCellValue($"Total Cost");
            sheet.GetRow(rowNum).GetCell(8).CellStyle = TableHeaderStyle;
            sheet.GetRow(rowNum).CreateCell(9).SetCellValue($"{_TotalCost.ToString("N4")}");
            sheet.GetRow(rowNum).GetCell(9).CellStyle = GreyCellStyle;

            // Green Cells
            sheet.CreateRow(++rowNum);
                        excelHelper.CreateBlankCell(sheet, rowNum, 0, 10, GreenCellStyle.ExcelStyle);

            #endregion

            #region 7.ABA DG Sell price

            // Title
            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 1, 10, TableContentStyle);
            sheet.GetRow(rowNum).CreateCell(0).SetCellValue($"7");
            sheet.GetRow(rowNum).GetCell(0).CellStyle = NormalStyle_Bold_Center;
            sheet.GetRow(rowNum).CreateCell(1).SetCellValue($"ABA DG Sell price");
            sheet.GetRow(rowNum).GetCell(1).CellStyle = NormalStyle_Bold_Left_Border;
            sheet.GetRow(rowNum).CreateCell(3).SetCellValue($"Rate");
            sheet.GetRow(rowNum).GetCell(3).CellStyle = TableHeaderStyle;
            sheet.GetRow(rowNum).CreateCell(9).SetCellValue($"USD");
            sheet.GetRow(rowNum).GetCell(9).CellStyle = TableHeaderStyle;
            sheet.GetRow(rowNum).CreateCell(10).SetCellValue($"HKD");
            sheet.GetRow(rowNum).GetCell(10).CellStyle = TableHeaderStyle;

            // Add gross margin
            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 1, 10, TableContentStyle);
            sheet.GetRow(rowNum).CreateCell(1).SetCellValue($"Add gross margin");
            sheet.GetRow(rowNum).GetCell(1).CellStyle = NormalStyle_Bold_Left_Border;
            sheet.GetRow(rowNum).CreateCell(3).SetCellValue($"{_ABADGSELL}");
            sheet.GetRow(rowNum).GetCell(3).CellStyle = TANCellStyle;
            sheet.GetRow(rowNum).CreateCell(9).SetCellValue($"{(_TotalCost * (decimal.Parse(_ABADGSELL) / 100)).ToString("N4")}");
            sheet.GetRow(rowNum).GetCell(9).CellStyle = TANCellStyle;

            // Sum
            var _abaDGPrice = _TotalCost + (_TotalCost * (decimal.Parse(_ABADGSELL) / 100));
            var _abaDGPrice_HKD = _abaDGPrice * _EffectCuryRate.Where(x => x.FromCuryID == "USD").FirstOrDefault()?.CuryRate * _EffectCuryRate.Where(x => x.FromCuryID == "HKD").FirstOrDefault()?.RateReciprocal;
            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 1, 11);
            sheet.GetRow(rowNum).CreateCell(8).SetCellValue($"ABA DG Price");
            sheet.GetRow(rowNum).GetCell(8).CellStyle = NormaStyle_Bold_Right;
            sheet.GetRow(rowNum).CreateCell(9).SetCellValue($"{_abaDGPrice.ToString("N4")}");
            sheet.GetRow(rowNum).GetCell(9).CellStyle = ROSECellStyle;
            sheet.GetRow(rowNum).CreateCell(10).SetCellValue($"{_abaDGPrice_HKD.Value.ToString("N4")}");
            sheet.GetRow(rowNum).GetCell(10).CellStyle = ROSECellStyle;
            sheet.GetRow(rowNum).CreateCell(11).SetCellValue($"(ABA HK price to ABA DG, in HKD)");
            sheet.GetRow(rowNum).GetCell(11).CellStyle = NormalStyle_Bold_Left;

            // Green Cells
            sheet.CreateRow(++rowNum);
                        excelHelper.CreateBlankCell(sheet, rowNum, 0, 10, GreenCellStyle.ExcelStyle);

            #endregion

            #region 8.ABA HK OH

            // Title
            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 1, 10, TableContentStyle);
            sheet.GetRow(rowNum).CreateCell(0).SetCellValue($"8");
            sheet.GetRow(rowNum).GetCell(0).CellStyle = NormalStyle_Bold_Center;
            sheet.GetRow(rowNum).CreateCell(1).SetCellValue($"ABA HK OH");
            sheet.GetRow(rowNum).GetCell(1).CellStyle = NormalStyle_Bold_Left_Border;

            // Standard Working Time
            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 1, 10, TableContentStyle);
            sheet.GetRow(rowNum).CreateCell(1).SetCellValue($"Standard working time");
            sheet.GetRow(rowNum).GetCell(1).CellStyle = NormalStyle_Bold_Left_Border;
            sheet.GetRow(rowNum).CreateCell(3).SetCellValue($"{_StandardWorkingTime.ToString("N4")}");
            sheet.GetRow(rowNum).GetCell(3).CellStyle = TANCellStyle;
            sheet.GetRow(rowNum).CreateCell(4).SetCellValue($"Minute");
            sheet.GetRow(rowNum).GetCell(4).CellStyle = TableHeaderStyle;

            // Standard cost
            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 1, 10);
            sheet.GetRow(rowNum).CreateCell(1).SetCellValue($"Standard cost");
            sheet.GetRow(rowNum).GetCell(1).CellStyle = NormalStyle_Bold_Left_Border;
            sheet.GetRow(rowNum).CreateCell(3).SetCellValue($"{_HKOHSCCost}");
            sheet.GetRow(rowNum).GetCell(3).CellStyle = YellowCellStyle;
            sheet.GetRow(rowNum).CreateCell(4).SetCellValue($"USD/minute");
            sheet.GetRow(rowNum).GetCell(4).CellStyle = TableHeaderStyle;

            // Sum
            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 1, 10);
            sheet.GetRow(rowNum).CreateCell(5).SetCellValue($"ABA HK overhead");
            sheet.GetRow(rowNum).GetCell(5).CellStyle = NormaStyle_Bold_Right;
            sheet.GetRow(rowNum).CreateCell(8).SetCellValue($"sum");
            sheet.GetRow(rowNum).GetCell(8).CellStyle = TableHeaderStyle;
            sheet.GetRow(rowNum).CreateCell(9).SetCellValue($"{(_StandardWorkingTime * decimal.Parse(_HKOHSCCost)).ToString("N4")}");
            sheet.GetRow(rowNum).GetCell(9).CellStyle = GreyCellStyle;
            // Green Cells
            sheet.CreateRow(++rowNum);
                        excelHelper.CreateBlankCell(sheet, rowNum, 0, 10, GreenCellStyle.ExcelStyle);

            #endregion

            #region 9.ABA HK Sell Price

            // Title
            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 1, 10, TableContentStyle);
            sheet.GetRow(rowNum).CreateCell(0).SetCellValue($"9");
            sheet.GetRow(rowNum).GetCell(0).CellStyle = NormalStyle_Bold_Center;
            sheet.GetRow(rowNum).CreateCell(1).SetCellValue($"ABA HK Sell Price");
            sheet.GetRow(rowNum).GetCell(1).CellStyle = NormalStyle_Bold_Left_Border;
            sheet.GetRow(rowNum).CreateCell(3).SetCellValue($"Rate");
            sheet.GetRow(rowNum).GetCell(3).CellStyle = TableHeaderStyle;

            // Add gross margin
            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 1, 10, TableContentStyle);
            sheet.GetRow(rowNum).CreateCell(1).SetCellValue($"Add gross margin");
            sheet.GetRow(rowNum).GetCell(1).CellStyle = NormalStyle_Bold_Left_Border;
            sheet.GetRow(rowNum).CreateCell(3).SetCellValue($"{_HKOHSCCost}");
            sheet.GetRow(rowNum).GetCell(3).CellStyle = TANCellStyle;
            sheet.GetRow(rowNum).CreateCell(9).SetCellValue($"{(_abaDGPrice * (decimal.Parse(_HKOHSCCost) / 100)).ToString("N4")}");
            sheet.GetRow(rowNum).GetCell(9).CellStyle = TANCellStyle;

            // Sum
            var _hkPrice = (_StandardWorkingTime * decimal.Parse(_HKOHSCCost)) + (_abaDGPrice * (decimal.Parse(_HKOHSCCost) / 100)) + _abaDGPrice;
            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 1, 11);
            sheet.GetRow(rowNum).CreateCell(8).SetCellValue($"ABA HK Price");
            sheet.GetRow(rowNum).GetCell(8).CellStyle = NormaStyle_Bold_Right;
            sheet.GetRow(rowNum).CreateCell(9).SetCellValue($"{_hkPrice.ToString("N4")}");
            sheet.GetRow(rowNum).GetCell(9).CellStyle = ROSECellStyle;
            sheet.GetRow(rowNum).CreateCell(11).SetCellValue($"（ABA HK price to ABI, in USD)");
            sheet.GetRow(rowNum).GetCell(11).CellStyle = NormalStyle_Bold_Left;

            // Green Cells
            sheet.CreateRow(++rowNum);
                        excelHelper.CreateBlankCell(sheet, rowNum, 0, 10, GreenCellStyle.ExcelStyle);

            #endregion

            #region 10.ABI Sell Price

            // Title
            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 1, 10, TableContentStyle);
            sheet.GetRow(rowNum).CreateCell(0).SetCellValue($"10");
            sheet.GetRow(rowNum).GetCell(0).CellStyle = NormalStyle_Bold_Center;
            sheet.GetRow(rowNum).CreateCell(1).SetCellValue($"ABI Sell Price");
            sheet.GetRow(rowNum).GetCell(1).CellStyle = NormalStyle_Bold_Left_Border;
            sheet.GetRow(rowNum).CreateCell(3).SetCellValue($"Consolidated GM %");
            sheet.GetRow(rowNum).GetCell(3).CellStyle = TableHeaderStyle;
            sheet.GetRow(rowNum).CreateCell(9).SetCellValue($"Consolidated GM per Unit");
            sheet.GetRow(rowNum).GetCell(9).CellStyle = TableHeaderStyle;

            // Add gross margin
            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 1, 10, TableContentStyle);
            sheet.GetRow(rowNum).CreateCell(1).SetCellValue($"Add gross margin");
            sheet.GetRow(rowNum).GetCell(1).CellStyle = NormalStyle_Bold_Left_Border;
            sheet.GetRow(rowNum).CreateCell(3).SetCellValue($"{_ABISELLCost}");
            sheet.GetRow(rowNum).GetCell(3).CellStyle = TANCellStyle;
            sheet.GetRow(rowNum).CreateCell(9).SetCellValue($"{(_hkPrice * (decimal.Parse(_ABISELLCost) / 100)).ToString("N4")}");
            sheet.GetRow(rowNum).GetCell(9).CellStyle = TANCellStyle;

            // Sum
            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 1, 11);
            sheet.GetRow(rowNum).CreateCell(8).SetCellValue($"ABI Price");
            sheet.GetRow(rowNum).GetCell(8).CellStyle = NormaStyle_Bold_Right;
            sheet.GetRow(rowNum).CreateCell(9).SetCellValue($"{(_hkPrice + (_hkPrice * (decimal.Parse(_ABISELLCost) / 100))).ToString("N4")}");
            sheet.GetRow(rowNum).GetCell(9).CellStyle = ROSECellStyle;

            // Green Cells
            sheet.CreateRow(++rowNum);
            sheet.CreateRow(++rowNum);
            sheet.CreateRow(++rowNum);
                        excelHelper.CreateBlankCell(sheet, rowNum, 0, 10, GreenCellStyle.ExcelStyle);

            #endregion

            #region 11.Tool Cost

            // Title
            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 1, 10, TableContentStyle);
            sheet.GetRow(rowNum).CreateCell(0).SetCellValue($"11");
            sheet.GetRow(rowNum).GetCell(0).CellStyle = NormalStyle_Bold_Center;
            sheet.GetRow(rowNum).CreateCell(1).SetCellValue($"Tool Cost");
            sheet.GetRow(rowNum).GetCell(1).CellStyle = NormalStyle_Bold_Left_Border;
            sheet.GetRow(rowNum).CreateCell(3).SetCellValue($"RMB");
            sheet.GetRow(rowNum).GetCell(3).CellStyle = TableHeaderStyle;
            sheet.GetRow(rowNum).CreateCell(9).SetCellValue($"RMB");
            sheet.GetRow(rowNum).GetCell(9).CellStyle = TableHeaderStyle;
            sheet.GetRow(rowNum).CreateCell(9).SetCellValue($"USD");
            sheet.GetRow(rowNum).GetCell(9).CellStyle = TableHeaderStyle;

            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 1, 10, TableContentStyle);
            sheet.GetRow(rowNum).CreateCell(1).SetCellValue($"Name");
            sheet.GetRow(rowNum).GetCell(1).CellStyle = NormalStyle_Bold_Left_Border;
            sheet.AddMergedRegion(new CellRangeAddress(rowNum, rowNum, 2, 3));

            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 1, 10, TableContentStyle);
            sheet.GetRow(rowNum).CreateCell(1).SetCellValue($"Name");
            sheet.GetRow(rowNum).GetCell(1).CellStyle = NormalStyle_Bold_Left_Border;
            sheet.AddMergedRegion(new CellRangeAddress(rowNum, rowNum, 2, 3));

            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 1, 10, TableContentStyle);
            sheet.GetRow(rowNum).CreateCell(1).SetCellValue($"Name");
            sheet.GetRow(rowNum).GetCell(1).CellStyle = NormalStyle_Bold_Left_Border;
            sheet.AddMergedRegion(new CellRangeAddress(rowNum, rowNum, 2, 3));

            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 1, 10, TableContentStyle);
            sheet.GetRow(rowNum).CreateCell(1).SetCellValue($"Tooling NRE cost");
            sheet.GetRow(rowNum).GetCell(1).CellStyle = NormalStyle_Bold_Left_Border;

            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 1, 10, TableContentStyle);
            sheet.GetRow(rowNum).CreateCell(1).SetCellValue($"Add gross margin");
            sheet.GetRow(rowNum).GetCell(1).CellStyle = NormalStyle_Bold_Left_Border;

            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 0, 8);
            sheet.GetRow(rowNum).CreateCell(5).SetCellValue($"sum");
            sheet.GetRow(rowNum).GetCell(5).CellStyle = NormalStyle;
            sheet.GetRow(rowNum).GetCell(6).CellStyle = ROSECellStyle;
            sheet.GetRow(rowNum).GetCell(7).CellStyle = ROSECellStyle;
            sheet.CreateRow(++rowNum);
            #endregion

            #region Currency Rate

            var _CuryUSDToHKD =
                _EffectCuryRate.Where(x => x.FromCuryID == "USD").FirstOrDefault()?.CuryRate /
                _EffectCuryRate.Where(x => x.FromCuryID == "HKD").FirstOrDefault()?.CuryRate;
            var _CuryUSDToRMB = _EffectCuryRate.Where(x => x.FromCuryID == "USD").FirstOrDefault()?.CuryRate;

            // Title
            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 1, 10, TableContentStyle);
            sheet.GetRow(rowNum).CreateCell(1).SetCellValue($"exchange rate :");
            sheet.GetRow(rowNum).GetCell(1).CellStyle = NormalStyle_Bold_Left_Border;
            sheet.GetRow(rowNum).CreateCell(4).SetCellValue($"RMB:USD");
            sheet.GetRow(rowNum).GetCell(4).CellStyle = TableHeaderStyle;
            sheet.GetRow(rowNum).CreateCell(5).SetCellValue($"HKD:USD");
            sheet.GetRow(rowNum).GetCell(5).CellStyle = TableHeaderStyle;

            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 1, 10, TableContentStyle);
            sheet.GetRow(rowNum).CreateCell(1).SetCellValue($"1USD=");
            sheet.GetRow(rowNum).GetCell(1).CellStyle = NormalStyle_Bold_Left_Border;
            sheet.GetRow(rowNum).CreateCell(2).SetCellValue($"{_CuryUSDToHKD.Value.ToString("N4")}");
            sheet.GetRow(rowNum).GetCell(2).CellStyle = TableHeaderStyle;
            sheet.GetRow(rowNum).CreateCell(3).SetCellValue($"HKD");
            sheet.GetRow(rowNum).GetCell(3).CellStyle = TableHeaderStyle;
            sheet.GetRow(rowNum).CreateCell(4).SetCellValue($"{_CuryUSDToHKD.Value.ToString("N4")}");
            sheet.GetRow(rowNum).GetCell(4).CellStyle = TableHeaderStyle;
            sheet.GetRow(rowNum).CreateCell(5).SetCellValue($"{_CuryUSDToRMB.Value.ToString("N4")}");
            sheet.GetRow(rowNum).GetCell(5).CellStyle = TableHeaderStyle;

            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 1, 10, TableContentStyle);
            sheet.GetRow(rowNum).CreateCell(1).SetCellValue($"1USD=");
            sheet.GetRow(rowNum).GetCell(1).CellStyle = NormalStyle_Bold_Left_Border;
            sheet.GetRow(rowNum).CreateCell(2).SetCellValue($"{_CuryUSDToRMB.Value.ToString("N4")}");
            sheet.GetRow(rowNum).GetCell(2).CellStyle = TableHeaderStyle;
            sheet.GetRow(rowNum).CreateCell(3).SetCellValue($"RMB");
            sheet.GetRow(rowNum).GetCell(3).CellStyle = TableHeaderStyle;

            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 1, 10, TableContentStyle);
            sheet.GetRow(rowNum).CreateCell(1).SetCellValue($"copper price refered :");
            sheet.GetRow(rowNum).GetCell(1).CellStyle = NormalStyle_Bold_Left_Border;

            sheet.CreateRow(++rowNum);
            excelHelper.CreateBlankCell(sheet, rowNum, 1, 10, TableContentStyle);
            sheet.GetRow(rowNum).CreateCell(1).SetCellValue($"oil price refered :");
            sheet.GetRow(rowNum).GetCell(1).CellStyle = NormalStyle_Bold_Left_Border;
            #endregion

            // Update Excel Formula
            sheet.ForceFormulaRecalculation = true;

            #endregion

            var exceldata = new MemoryStream();
            workBook.Write(exceldata);
            string path = $"Internal Cost Model_{((AMProdItem)this.Caches[typeof(AMProdItem)].Current).ProdOrdID}.xlsx";
            var info = new PX.SM.FileInfo(path, null, exceldata.ToArray());
            throw new PXRedirectToFileException(info, true);
        }

        #endregion

        #region Function

        /// <summary> Get Effect Currency Rate </summary>
        protected IEnumerable<CurrencyRate2> GetCuryRateRecordEffData()
        {
            PXSelectBase<CurrencyRate2> sel = new PXSelect<CurrencyRate2,
                Where<CurrencyRate2.toCuryID, Equal<Required<CurrencyRate2.toCuryID>>,
                And<CurrencyRate2.fromCuryID, Equal<Required<CurrencyRate2.fromCuryID>>,
                And<CurrencyRate2.curyRateType, Equal<Required<CurrencyRate2.curyRateType>>,
                And<CurrencyRate2.curyEffDate, Equal<Required<CurrencyRate2.curyEffDate>>>>>>>(this);

            List<CurrencyRate2> ret = new List<CurrencyRate2>();

            foreach (CurrencyRate2 r in PXSelectGroupBy<CurrencyRate2,
                Where<CurrencyRate2.toCuryID, Equal<Current<CuryRateFilter.toCurrency>>,
                And<CurrencyRate2.curyEffDate, LessEqual<Current<CuryRateFilter.effDate>>>>,
                Aggregate<Max<CurrencyRate2.curyEffDate,
                GroupBy<CurrencyRate2.curyRateType,
                GroupBy<CurrencyRate2.fromCuryID>>>>>.Select(this))
            {
                ret.Add((CurrencyRate2)sel.Select("CNY", r.FromCuryID, r.CuryRateType, r.CuryEffDate));
            }
            return ret;
        }

        #endregion

    }
}
