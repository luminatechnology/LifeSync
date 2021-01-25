using System;
using PX.Objects;
using PX.Data;
using System.Collections;
using System.Collections.Generic;
using PX.Data.BQL.Fluent;

namespace JAMS.AM
{
    public class MatlWizard2_Extension : PXGraphExtension<MatlWizard2>
    {
        //public PXProcessing<AMWrkMatl, Where<AMWrkMatl.userID, Equal<Current<AccessInfo.userID>>>> ProcessMatl;
        /*
        public SelectFrom<AMWrkMatl>
            .LeftJoin<AMProdMatl>.On<AMWrkMatl.prodOrdID.IsEqual<AMProdMatl.prodOrdID>
                .And<AMWrkMatl.orderType.IsEqual<AMProdMatl.orderType>
                .And<AMWrkMatl.operationID.IsEqual<AMProdMatl.operationID>
                .And<AMWrkMatl.lineID.IsEqual<AMProdMatl.lineID>>>>>
        .Where<AMWrkMatl.userID.IsEqual<AccessInfo.userID.FromCurrent>>.View newProcessMatl;
        */

        //private Boolean isCalculate = true;
        /*
        public IEnumerable ProcessMatl()
        {
            var newProcessMatl = SelectFrom<AMWrkMatl>
                .LeftJoin<AMProdMatl>.On<AMWrkMatl.prodOrdID.IsEqual<AMProdMatl.prodOrdID>
                                        .And<AMWrkMatl.orderType.IsEqual<AMProdMatl.orderType>
                                        .And<AMWrkMatl.operationID.IsEqual<AMProdMatl.operationID>
                                        .And<AMWrkMatl.lineID.IsEqual<AMProdMatl.lineID>>>>>
            .Where<AMWrkMatl.userID.IsEqual<AccessInfo.userID.FromCurrent>>.View.Select(Base);
            
            foreach (PXResult<AMWrkMatl, AMProdMatl> row in newProcessMatl)
            {
                AMWrkMatl aMWrkMatl = row;
                AMProdMatl aMProdMatl = row;
                
                if (isCalculate && aMProdMatl.ScrapFactor > 0 && aMWrkMatl.QtyReq * (1 + aMProdMatl.ScrapFactor) != aMProdMatl.QtyRemaining)
                {
                    aMWrkMatl.QtyReq = aMWrkMatl.QtyReq / (1 + aMProdMatl.ScrapFactor);
                    //aMWrkMatl.QtyReq = aMWrkMatl.QtyReq / (1 + aMProdMatl.ScrapFactor) - AMWrkMatl.QtyActual;
                }
                //aMWrkMatl.MatlQty = aMWrkMatl.QtyReq <= aMWrkMatl.QtyAvail ? aMWrkMatl.QtyReq : AMWrkMatl.QtyAvail;
                

            }

            return newProcessMatl;
        }
        */
        /*
        protected void AMWrkMatl_QtyReq_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            //if (e.Row == null) return;

            var row = (AMWrkMatl)e.Row;
            var AMWrkMatlRow = newProcessMatl.Select(row.InventoryID);
            row.QtyReq = AMWrkMatlRow.TopFirst.QtyReq;// / (1 + AMWrkMatlRow.TopFirst.Sca;
        }
        */
    }
}