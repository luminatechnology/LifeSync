using PX.Data;
using PX.Data.BQL;
using PX.Objects.IN;
using PX.Objects.SO;
using System;

public class SOLineExt : PXCacheExtension<SOLine>
{
	[PXDBPriceCost]
	[PXDefault(TypeCode.Decimal, "0.0")]
	[PXUIField(DisplayName = "PI Unit Price")]
	public virtual decimal? UsrPIUnitPrice { get; set; }
	public abstract class usrPIUnitPrice : PX.Data.BQL.BqlDecimal.Field<usrPIUnitPrice> { }

}
