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
using LumCustomizations.Graph;
using LUMCustomizations.DAC;
using LUMCustomizations.Library;
using LuminaExcelLibrary;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using PX.CS;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Data.EP;
using PX.Objects.AP;
using PX.Objects.IN;
using PX.Objects.PO;
using PX.Objects.TX;

namespace LUMCustomizations.Graph
{
    public class ICMSummaryMaint : PXGraph<ICMSummaryMaint>
    {
        public PXCancel<ICMFilter> Cancel;
        public PXFilter<ICMFilter> Filter;

        [PXFilterable]
        public PXFilteredProcessing<ICMSummary, ICMFilter, Where<ICMSummary.prodOrdID,
            Between<Current<ICMFilter.start_AMProdID>, Current<ICMFilter.end_AMProdID>>>> ICMList;

        public ICMSummaryMaint()
        {
            ICMList.SetProcessAllVisible(false);
            ICMList.SetProcessVisible(false);
        }

        public IEnumerable iCMList()
        {
            var amProdItem = SelectFrom<AMProdItem>
                .Where<AMProdItem.orderType.IsEqual<ICMType>>
                .View.Select(this).RowCast<AMProdItem>().ToList();
            var currFilter = this.Filter.Current;
            if (!string.IsNullOrEmpty(currFilter?.Start_AMProdID) && !string.IsNullOrEmpty(currFilter?.End_AMProdID))
                amProdItem = amProdItem.Where(
                    x => int.Parse(x.ProdOrdID.Substring(3)) >= int.Parse(currFilter.Start_AMProdID.Substring(3)) &&
                         int.Parse(x.ProdOrdID.Substring(3)) <= int.Parse(currFilter.End_AMProdID.Substring(3))).ToList();
            else
            {
                amProdItem = new List<AMProdItem>();
            }
            var inventoryData = SelectFrom<InventoryItem>.View.Select(this).RowCast<InventoryItem>().ToList();
            var initemXRef = SelectFrom<INItemXRef>.View.Select(this).RowCast<INItemXRef>().ToList();
            int lineNbr = 1;
            foreach (var item in amProdItem)
            {
                var summary = GetICMData(item);
                summary.LineNbr = lineNbr++;
                summary.ProdOrdID = item.ProdOrdID;
                summary.InventoryCD = inventoryData.FirstOrDefault(x => x.InventoryID == item.InventoryID).InventoryCD;
                summary.Description = inventoryData.FirstOrDefault(x => x.InventoryID == item.InventoryID).Descr;
                summary.CustomerPN = initemXRef.FirstOrDefault(x => x.InventoryID == item.InventoryID).AlternateID;
                yield return summary;
            }

            yield return null;
        }

        public ICMSummary GetICMData(AMProdItem row)
        {
            #region Varaible
            int rowNum = 0;
            decimal materialSum = 0;

            ICMSummary summaryResult = new ICMSummary();
            // Currency Rate Type of ICM
            var graphProdDetail = PXGraph.CreateInstance<ProdDetail>();
            var iCMRateType = PXGraph.CreateInstance<InternalCostModelMaint>().Select<LifeSyncPreference>().Select(x => x.InternalCostModelRateType).FirstOrDefault();
            var aMProdAttribute = SelectFrom<AMProdAttribute>.Where<AMProdAttribute.orderType.IsEqual<P.AsString>
                .And<AMProdAttribute.prodOrdID.IsEqual<P.AsString>>>.View.Select(this, row.OrderType, row.ProdOrdID).RowCast<AMProdAttribute>().ToList();
            // Get Stock Item Vendor Details By LastModifierTime
            var pOvenderDetail = PXGraph.CreateInstance<InternalCostModelMaint>()
                                         .Select<POVendorInventory>()
                                         .ToList()
                                         .Where(x => x.IsDefault ?? false)
                                         .GroupBy(x => new { x.InventoryID })
                                         .Select(x => x.OrderByDescending(y => y.LastModifiedDateTime).FirstOrDefault());
            var taxInfo = (from v in new PXGraph().Select<Vendor>()
                           join t in new PXGraph().Select<PX.Objects.CR.Location>()
                             on v.BAccountID equals t.BAccountID
                           join z in new PXGraph().Select<TaxZoneDet>()
                             on t.VTaxZoneID equals z.TaxZoneID
                           join r in new PXGraph().Select<TaxRev>()
                             on z.TaxID equals r.TaxID
                           where r.TaxType == "P"
                           select new
                           {
                               vendorID = v.BAccountID,
                               taxRate = r.TaxRate
                           }).ToList().GroupBy(x => x.vendorID).Select(x => x.First());
            // Get All Material Data
            var materialData = from t in PXGraph.CreateInstance<InternalCostModelMaint>().Select<AMProdMatl>()
                               where t.OrderType == row.OrderType && t.ProdOrdID == row.ProdOrdID
                               select t;
            var aMProdMaterail = from t in materialData.ToList()
                                 join i in PXGraph.CreateInstance<InternalCostModelMaint>().Select<InventoryItem>()
                                  on t.InventoryID equals i.InventoryID
                                 join v in pOvenderDetail
                                  on t.InventoryID equals v.InventoryID into result
                                 from r in result.DefaultIfEmpty()
                                 join x in taxInfo
                                   on r?.VendorID ?? -1 equals x.vendorID into taxResult
                                 from _tax in taxResult.DefaultIfEmpty()
                                 orderby i.InventoryCD
                                 select new
                                 {
                                     t.InventoryID,
                                     t.Descr,
                                     t.UnitCost,
                                     t.UOM,
                                     t.TotalQtyRequired,
                                     t.ScrapFactor,
                                     t.QtyReq,
                                     i.InventoryCD,
                                     t.BatchSize,
                                     venderDetail = r,
                                     taxInfo = _tax
                                 };
            var _AMProdOper = SelectFrom<AMProdOper>.Where<AMProdOper.orderType.IsEqual<P.AsString>
                .And<AMProdOper.prodOrdID.IsEqual<P.AsString>>>.View.Select(this, row.OrderType, row.ProdOrdID).RowCast<AMProdOper>().ToList();

            // ReplenishmentSource From Inventory 
            var _InventoryItem = PXGraph.CreateInstance<InternalCostModelMaint>().Select<INItemSite>().Select(x => new { x.InventoryID, x.ReplenishmentSource });
            // Effect Curry Rate
            var _EffectCuryRate = new LumLibrary().GetCuryRateRecordEffData(this).Where(x => x.CuryRateType == iCMRateType).ToList();
            if (_EffectCuryRate.Count == 0)
                throw new PXException("Please Select ICM Rate Type !!");

            decimal _SetUpSum = 0;
            decimal _TotalCost = 0;
            var _StandardWorkingTime = (_AMProdOper.FirstOrDefault().RunUnitTime / _AMProdOper.FirstOrDefault().RunUnits).Value;
            // AMProdAttribute 
            var _ENDC = aMProdAttribute.Where(x => x.AttributeID.Equals("ENDC")).FirstOrDefault()?.Value;
            var _EAU = aMProdAttribute.Where(x => x.AttributeID.Equals("EAU")).FirstOrDefault()?.Value;
            var _LBSCCost = aMProdAttribute.Where(x => x.AttributeID == "LBSC").FirstOrDefault()?.Value ?? "0";
            var _MFSCCost = aMProdAttribute.Where(x => x.AttributeID == "MFSC").FirstOrDefault()?.Value ?? "0";
            var _OHSCCost = aMProdAttribute.Where(x => x.AttributeID == "OHSC").FirstOrDefault()?.Value ?? "0";
            var _SETUPSCCost = aMProdAttribute.Where(x => x.AttributeID == "SETUPSC").FirstOrDefault()?.Value ?? "0";
            var _PRODYIELD = aMProdAttribute.Where(x => x.AttributeID == "PRODYIELD").FirstOrDefault()?.Value ?? "0";
            var _ABADGSELL = aMProdAttribute.Where(x => x.AttributeID == "ABADGSELL").FirstOrDefault()?.Value ?? "0";
            var _HKOHSCCost = aMProdAttribute.Where(x => x.AttributeID == "HKOHSC").FirstOrDefault()?.Value ?? "0";
            var _ABISELLCost = aMProdAttribute.Where(x => x.AttributeID == "ABISELL").FirstOrDefault()?.Value ?? "0";
            #endregion

            #region 1.Material Cost Row(7~rowNum)
            foreach (var matl in aMProdMaterail)
            {
                var QPA = (matl?.QtyReq / matl?.BatchSize) ?? 1;
                decimal? _materailCost = 0;
                var _ReplenishmentSource = _InventoryItem.Where(x => x.InventoryID == matl.InventoryID)
                    .FirstOrDefault()?.ReplenishmentSource ?? "";

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
                        // 不含Tax
                        _venderLastPrice = (_venderLastPrice / (1 + (matl?.taxInfo?.taxRate ?? 0) / 100));
                        _materailCost = _venderLastPrice * Math.Round(QPA, 4)
                                        * (_EffectCuryRate.Where(x => x.FromCuryID == "USD" && x.ToCuryID == "CNY").FirstOrDefault()?.RateReciprocal ?? 1);
                    }
                    else if (matl.venderDetail.CuryID == "HKD")
                    {
                        _materailCost = _venderLastPrice * Math.Round(QPA, 4)
                                        * (_EffectCuryRate.Where(x => x.FromCuryID == "HKD" && x.ToCuryID == "CNY").FirstOrDefault()?.CuryRate ?? 1)
                                        * (_EffectCuryRate.Where(x => x.FromCuryID == "USD" && x.ToCuryID == "CNY").FirstOrDefault()?.RateReciprocal ?? 1);
                    }
                    else if (matl.venderDetail.CuryID == "USD")
                    {
                        _materailCost = _venderLastPrice * Math.Round(QPA, 4);
                    }
                }
                else
                {
                    _materailCost = (matl.UnitCost.HasValue ? matl.UnitCost.Value : 0) * Math.Round(QPA, 4)
                                                     * (_EffectCuryRate.Where(x => x.FromCuryID == "USD" && x.ToCuryID == "CNY").FirstOrDefault()?.RateReciprocal ?? 1);
                }
                materialSum += _materailCost ?? 0;
            }

            // Materail Sum
            summaryResult.MaterialCost = materialSum;
            _SetUpSum += materialSum;
            #endregion

            #region 2.Labor Cost

            summaryResult.StandardTime = _StandardWorkingTime;
            summaryResult.LabourCost = (_StandardWorkingTime * decimal.Parse(_LBSCCost) *
                                        _EffectCuryRate.Where(x => x.FromCuryID == "USD").FirstOrDefault()
                                            .RateReciprocal).Value; ;
            // Sum
            _SetUpSum += (_StandardWorkingTime * decimal.Parse(_LBSCCost) * _EffectCuryRate.Where(x => x.FromCuryID == "USD").FirstOrDefault().RateReciprocal).Value;

            #endregion

            #region 3.Manufacture Cost
            summaryResult.ManufactureCost = (_StandardWorkingTime * decimal.Parse(_MFSCCost) / _EffectCuryRate.FirstOrDefault(x => x.FromCuryID == "USD").CuryRate.Value);
            _SetUpSum += (_StandardWorkingTime * decimal.Parse(_MFSCCost) * _EffectCuryRate.FirstOrDefault(x => x.FromCuryID == "USD").RateReciprocal).Value;
            #endregion

            #region 4.Overhead
            // Standard cost
            summaryResult.Overhead = (_StandardWorkingTime * decimal.Parse(_OHSCCost) /
                                     _EffectCuryRate.FirstOrDefault(x => x.FromCuryID == "USD").CuryRate.Value);
            _SetUpSum += (_StandardWorkingTime * decimal.Parse(_OHSCCost) / _EffectCuryRate.FirstOrDefault(x => x.FromCuryID == "USD").RateReciprocal).Value;
            #endregion

            #region 6.Production yield
            // Sum
            _TotalCost = (summaryResult.MaterialCost + summaryResult.LabourCost +
                          summaryResult.ManufactureCost + summaryResult.Overhead).Value /
                         (1 - decimal.Parse(_PRODYIELD) / 100);
            summaryResult.DGPrice = _TotalCost;
            summaryResult.Lumyield = _TotalCost - (summaryResult.MaterialCost + summaryResult.LabourCost +
                          summaryResult.ManufactureCost + summaryResult.Overhead).Value;
            #endregion

            #region 7.ABA DG Sell price
            // Sum
            var _abaDGPrice = _TotalCost + (_TotalCost * (decimal.Parse(_ABADGSELL) / 100));
            var _abaDGPrice_HKD = _abaDGPrice * _EffectCuryRate.Where(x => x.FromCuryID == "USD").FirstOrDefault()?.CuryRate * _EffectCuryRate.Where(x => x.FromCuryID == "HKD").FirstOrDefault()?.RateReciprocal;
            summaryResult.DGtoHKPrice = (decimal.Parse(_ABADGSELL) + summaryResult.DGPrice);
            #endregion

            #region 9.ABA HK Sell Price
            // Sum
            var _hkPrice = summaryResult.DGPrice + (decimal.Parse(_HKOHSCCost) * _StandardWorkingTime) + (decimal.Parse(_HKOHSCCost) * (decimal.Parse(_HKOHSCCost) * _StandardWorkingTime));
            summaryResult.HKPrice = _hkPrice.Value;

            #endregion

            #region 10.ABI Sell Price
            // Sum
            summaryResult.ABIPrice = (summaryResult.Lumyield + (decimal.Parse(_HKOHSCCost) * _StandardWorkingTime)) /
                                     (1 - decimal.Parse(_ABISELLCost) / 100);
            #endregion

            return summaryResult;
        }

    }

    public class ICMType : PX.Data.BQL.BqlString.Constant<ICMType>
    {
        public ICMType() : base("CM") { }
    }

    [Serializable]
    public class ICMFilter : IBqlTable
    {
        [PXDefault]
        [PXSelector(typeof(SearchFor<AMProdItem.prodOrdID>.Where<AMProdItem.orderType.IsEqual<ICMType>>))]
        [PXUIField(DisplayName = "Start Production Nbr.")]
        public virtual string Start_AMProdID { get; set; }
        public abstract class start_AMProdID : PX.Data.BQL.BqlString.Field<start_AMProdID> { }

        [PXDefault]
        [PXSelector(typeof(SearchFor<AMProdItem.prodOrdID>.Where<AMProdItem.orderType.IsEqual<ICMType>>))]
        [PXUIField(DisplayName = "End Production Nbr.")]
        public virtual string End_AMProdID { get; set; }
        public abstract class end_AMProdID : PX.Data.BQL.BqlString.Field<end_AMProdID> { }
    }
}
