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

                if (aMProdMatl.ScrapFactor > 0 && aMProdMatl.BaseTotalQtyRequired / (1 + aMProdMatl.ScrapFactor) != aMWrkMatl.QtyReq)
                    aMWrkMatl.QtyReq = aMWrkMatl.QtyReq / (1 + aMProdMatl.ScrapFactor);

                aMWrkMatl.MatlQty = aMWrkMatl.QtyReq <= aMWrkMatl.QtyAvail ? aMWrkMatl.QtyReq : aMWrkMatl.QtyAvail;
            }

            return newProcessMatl;
        }
    }
}