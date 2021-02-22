using JAMS.AEF.Standalone;
using JAMS.AM.Attributes;
using LUMCustomizations.Library;
using PX.Data;
using PX.Data.BQL.Fluent;
using PX.Objects.GL;
using PX.Objects.IN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAMS.AM
{
    public class AMReleaseProcess_Extension : PXGraphExtension<AMReleaseProcess>
    {
        public delegate void ReleaseDocProcDelegate(AMBatch doc);

        [PXOverride]
        public virtual void ReleaseDocProc(AMBatch doc, ReleaseDocProcDelegate baseMethod)
        {
            baseMethod(doc);

            string _descr = string.Empty;
            var BatchNbr = Base.GetCacheCurrent<AMBatch>().Current.BatNbr;
            var DocType = Base.GetCacheCurrent<AMBatch>().Current.DocType;

            if (!new LumLibrary().GetJournalEnhance)
                return;
            // DocTYpe is not eqaule Move or Material or Costing
            if (DocType != AMDocType.Move && DocType != AMDocType.Material && DocType != AMDocType.ProdCost)
                return;
            try
            {
                var prodItem = (AMProdItem)Base.ProductionItems.Cache.Current;
                _descr = DocType == AMDocType.Move ? "生产入库 " :
                                DocType == AMDocType.Material ? "Materials Issued for " :
                                DocType == AMDocType.ProdCost ? "Porduction Costing " : "";
                // AMMTran Data
                var AMTranData = from t in Base.Select<AMMTran>()
                                 where t.DocType == DocType && t.BatNbr == BatchNbr
                                 select t;
                switch (DocType)
                {
                    case AMDocType.Move:
                    case AMDocType.Material:
                        #region Move & Material
                        // INRegisterExt Data
                        var INRegisterExtRow = (from t in new PXGraph().Select<INRegisterAMExtension>()
                                                where t.AMBatNbr == BatchNbr
                                                select t).SingleOrDefault();

                        // INRegister Data
                        var INRegisterRow = (from t in new PXGraph().Select<INRegister>()
                                             where t.DocType == INRegisterExtRow.DocType && t.RefNbr == INRegisterExtRow.RefNbr
                                             select t).SingleOrDefault();

                        if (INRegisterExtRow == null)
                            throw new Exception("INRegisterExtRow is null");

                        if (INRegisterRow == null)
                            throw new Exception("INRegisterRow is null");

                        if (DocType == AMDocType.Material)
                            _descr += $"{prodItem.ProdOrdID} - {new LumLibrary().GetInventoryItemCD(prodItem.InventoryID)} ";
                        else
                        {
                            // Combine Descr
                            foreach (var _amTranItem in AMTranData)
                            {
                                if ((_descr + _amTranItem.InventoryID + _amTranItem.ProdOrdID).Length > 230)
                                    break;
                                _descr += $"{_amTranItem.ProdOrdID} - {new LumLibrary().GetInventoryItemCD(_amTranItem.InventoryID)} ";
                            }
                        }
                        // Update INRegister Description
                        PXUpdate<Set<INRegister.tranDesc, Required<INRegister.tranDesc>>,
                                INRegister,
                                Where<INRegister.docType, Equal<Required<INRegister.docType>>,
                                    And<INRegister.refNbr, Equal<Required<INRegister.refNbr>>,
                                    And<INRegister.tranDesc, Equal<Required<INRegister.tranDesc>>>>>>
                                .Update(Base, _descr, INRegisterRow.DocType, INRegisterRow.RefNbr, "Production Transaction");

                        // Update Journal Transaction Description
                        PXUpdate<Set<Batch.description, Required<Batch.description>>,
                                Batch,
                                Where<Batch.module, Equal<Required<Batch.module>>,
                                    And<Batch.batchNbr, Equal<Required<Batch.batchNbr>>,
                                    And<Batch.description, Equal<Required<Batch.description>>>>>>
                                .Update(Base, _descr, "IN", INRegisterRow.BatchNbr, "Production Transaction");
                        #endregion
                        break;
                    case AMDocType.ProdCost:
                        #region Cost
                        
                        _descr += $" {prodItem.ProdOrdID} - {new LumLibrary().GetInventoryItemCD(prodItem.InventoryID)} ";

                        // Update AMBatch
                        PXUpdate<Set<AMBatch.tranDesc, Required<AMBatch.tranDesc>>,
                                AMBatch,
                                Where<AMBatch.docType, Equal<Required<AMBatch.docType>>,
                                    And<AMBatch.batNbr, Equal<Required<AMBatch.batNbr>>>>>
                                .Update(Base, _descr, DocType, BatchNbr);
                        // Find GL BatNbr
                        var GLNbr = AMTranData.ToList().Where(x => !string.IsNullOrEmpty(x.GLBatNbr))?.FirstOrDefault()?.GLBatNbr;

                        // Update Batch
                        PXUpdate<Set<Batch.description, Required<Batch.description>>,
                               Batch,
                               Where<Batch.module, Equal<Required<Batch.module>>,
                                   And<Batch.batchNbr, Equal<Required<Batch.batchNbr>>,
                                   And<Batch.description, Equal<Required<Batch.description>>>>>>
                               .Update(Base, _descr, "GL", GLNbr, "Production Transaction"); 
                        #endregion
                        break;
                }

            }
            catch (Exception ex)
            {
                PXTrace.WriteInformation($"BatchNbr:{BatchNbr} Update Description fail.. Error: {ex.Message}");
            }
        }

    }
}
