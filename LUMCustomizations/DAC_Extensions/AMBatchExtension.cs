﻿using PX.Data;
using PX.Data.BQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JAMS.AM
{
    public class AMBatchExt : PXCacheExtension<AMBatch>
    {
        [PXDBInt]
        [PXUIField(DisplayName = "Print Count", Enabled = false)]
        [PXDefault(0)]
        public virtual int? UsrPrintCount { get; set; }
        public abstract class usrPrintCount : BqlType<IBqlInt, int>.Field<AMProdItemExt.usrPrintCount> { }
    }
}