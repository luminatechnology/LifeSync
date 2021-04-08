using LUMCustomizations.DAC;
using PX.Data;
using PX.Data.BQL;
using PX.Data.BQL.Fluent;
using PX.Objects.CS;
using PX.Objects.GL;
using PX.Objects.IN;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUMCustomizations.Graph
{
    public class LumDeadStockEnq : PXGraph<LumDeadStockEnq>
    {
		public PXCancel<LumDeadStockEnqFilter> Cancel;

		public PXFilter<LumDeadStockEnqFilter> Filter;

		[PXFilterable]
		public PXSelect<LumDeadStockEnqResult> Result;

		public PXSetup<Company> Company;

		public LumDeadStockEnq()
		{
			Result.AllowDelete = false;
			Result.AllowInsert = false;
			Result.AllowUpdate = false;
		}

		protected virtual IEnumerable result()
		{
			var filter = Filter.Current;

			if (!ValidateFilter(filter))
				return new LumDeadStockEnqResult[0];

			GetStartDates(filter, out DateTime? inStockSince, out DateTime? noSalesSince);
			PXSelectBase<INSiteStatus> command = CreateCommand();
			var parameters = AddFilters(filter, command, inStockSince, noSalesSince);

			var singleRow = GetRowByPrimaryKeys(command, filter, inStockSince, noSalesSince);
			if (singleRow != null)
				return new LumDeadStockEnqResult[] { singleRow };

			bool userSortsFilters = ValidateViewSortsFilters();

			var result = new PXDelegateResult();
			result.IsResultFiltered = !userSortsFilters;
			result.IsResultSorted = !userSortsFilters;
			int resultCounter = 0;

			foreach (PXResult<INSiteStatus> row in command.Select(parameters.ToArray()))
			{
				LumDeadStockEnqResult newResult = MakeResult(row, inStockSince, noSalesSince);
				if (newResult == null)
					continue;

				result.Add(new PXResult<LumDeadStockEnqResult, InventoryItem>(newResult, row.GetItem<InventoryItem>()));
				resultCounter++;

				if (!userSortsFilters && (PXView.StartRow + PXView.MaximumRows) <= resultCounter)
					break;
			}

			return result;
		}

		protected virtual bool ValidateFilter(LumDeadStockEnqFilter filter)
		{
			if (filter == null || filter.SiteID == null || filter.SelectBy == null)
				return false;

			if (filter.SelectBy == LumDeadStockEnqFilter.selectBy.Days &&
				filter.InStockDays == null &&
				filter.NoSalesDays == null)
				return false;

			if (filter.SelectBy == LumDeadStockEnqFilter.selectBy.Date &&
				filter.InStockSince == null &&
				filter.NoSalesSince == null)
				return false;

			return true;
		}

		protected virtual void GetStartDates(LumDeadStockEnqFilter filter, out DateTime? inStockSince, out DateTime? noSalesSince)
		{
			switch (filter.SelectBy)
			{
				case LumDeadStockEnqFilter.selectBy.Days:
					inStockSince = filter.InStockDays == null ? (DateTime?)null :
						GetCurrentDate().AddDays(-1 * (int)filter.InStockDays);

					noSalesSince = filter.NoSalesDays == null ? (DateTime?)null :
						GetCurrentDate().AddDays(-1 * (int)filter.NoSalesDays);
					break;
				case LumDeadStockEnqFilter.selectBy.Date:
					inStockSince = filter.InStockSince;
					noSalesSince = filter.NoSalesSince;
					break;
				default:
					throw new NotImplementedException();
			}
		}

		protected virtual DateTime GetCurrentDate()
			=> Accessinfo.BusinessDate.Value.Date;

		protected virtual bool ValidateViewSortsFilters()
		{
			if ((PXView.Filters?.Length ?? 0) != 0)
				return true;

			if ((PXView.SortColumns?.Length ?? 0) != 0 &&
					(PXView.SortColumns.Length != Result.Cache.Keys.Count ||
						!PXView.SortColumns.SequenceEqual(Result.Cache.Keys, StringComparer.OrdinalIgnoreCase) ||
						PXView.Descendings?.Any(d => d != false) == true))
				return true;

			if (PXView.ReverseOrder)
				return true;

			if (PXView.Searches?.Any(v => v != null) == true)
				return true;

			return false;
		}

		protected virtual PXSelectBase<INSiteStatus> CreateCommand()
		{
			return new SelectFrom<INSiteStatus>
				.InnerJoin<InventoryItem>.On<INSiteStatus.FK.InventoryItem>
				.OrderBy<InventoryItem.inventoryCD.Asc>.View.ReadOnly(this);
		}

		protected virtual List<object> AddFilters(LumDeadStockEnqFilter filter, PXSelectBase<INSiteStatus> command,
			DateTime? inStockSince, DateTime? noSalesSince)
		{
			var parameters = new List<object>();

			AddQtyOnHandFilter(command);
			AddSiteFilter(command, filter);
			AddInventoryFilter(command, filter);
			AddItemClassFilter(command, filter);
			AddNoSalesSinceFilter(command, parameters, noSalesSince);

			return parameters;
		}

		protected virtual void AddQtyOnHandFilter(PXSelectBase<INSiteStatus> command)
		{
			command.WhereAnd<Where<INSiteStatus.qtyOnHand.IsGreater<decimal0>>>();

			var fields = GetNegativePlanFields();

			// QtySOBackOrdered + QtyPOPrepared + QtySOBooked + ... < qtyOnHand
			var lastField = fields.Last();
			var whereTypes = new List<Type>() { typeof(Where<,>) };
			whereTypes.AddRange(
				fields.Where(field => field != lastField)
				.SelectMany(field => new[] { typeof(Add<,>), field }));
			whereTypes.Add(lastField);
			whereTypes.Add(typeof(Less<INSiteStatus.qtyOnHand>));

			var whereNegativePlansLessOnHand = BqlCommand.Compose(whereTypes.ToArray());
			command.WhereAnd(whereNegativePlansLessOnHand);
		}

		protected virtual void AddSiteFilter(PXSelectBase<INSiteStatus> command, LumDeadStockEnqFilter filter)
		{
			if (filter.SiteID != null)
			{
				command.WhereAnd<Where<INSiteStatus.siteID.IsEqual<LumDeadStockEnqFilter.siteID.FromCurrent>>>();
			}
		}

		protected virtual void AddInventoryFilter(PXSelectBase<INSiteStatus> command, LumDeadStockEnqFilter filter)
		{
			if (filter.InventoryID != null)
			{
				command.WhereAnd<Where<INSiteStatus.inventoryID
					.IsEqual<LumDeadStockEnqFilter.inventoryID.FromCurrent>>>();
			}

			command.WhereAnd<Where<InventoryItem.itemStatus.IsNotIn<
				InventoryItemStatus.markedForDeletion, InventoryItemStatus.inactive>>>();
		}

		protected virtual void AddItemClassFilter(PXSelectBase<INSiteStatus> command, LumDeadStockEnqFilter filter)
		{
			if (filter.ItemClassID != null)
			{
				command.WhereAnd<Where<InventoryItem.itemClassID.
					IsEqual<LumDeadStockEnqFilter.itemClassID.FromCurrent>>>();
			}
		}

		protected virtual void AddNoSalesSinceFilter(PXSelectBase<INSiteStatus> command,
			List<object> parameters, DateTime? noSalesSince)
		{
			if (noSalesSince != null)
			{
				command.WhereAnd<Where<NotExists<SelectFrom<INItemSiteHistDay>
					.Where<INItemSiteHistDay.siteID.IsEqual<INSiteStatus.siteID>
						.And<INItemSiteHistDay.inventoryID.IsEqual<INSiteStatus.inventoryID>>
						.And<INItemSiteHistDay.subItemID.IsEqual<INSiteStatus.subItemID>>
						.And<INItemSiteHistDay.sDate.IsGreaterEqual<@P.AsDateTime>>
						.And<INItemSiteHistDay.qtySales.IsGreater<decimal0>>>>>>();

				parameters.Add(noSalesSince);
			}
		}

		protected virtual LumDeadStockEnqResult GetRowByPrimaryKeys(PXSelectBase<INSiteStatus> command,
			LumDeadStockEnqFilter filter, DateTime? inStockSince, DateTime? noSalesSince)
		{
			if (PXView.MaximumRows == 1 && PXView.StartRow == 0 &&
				PXView.Searches?.Length == Result.Cache.Keys.Count &&
				PXView.SearchColumns.Select(sc => sc.Column)
					.SequenceEqual(Result.Cache.Keys, StringComparer.OrdinalIgnoreCase) &&
				PXView.Searches.All(k => k != null))
			{
				int startRow = 0;
				int totalRows = 0;
				var rows = command.View.Select(new object[] { filter },
					PXView.Parameters, PXView.Searches, PXView.SortColumns, PXView.Descendings,
					PXView.Filters, ref startRow, PXView.MaximumRows, ref totalRows);

				foreach (var row in rows)
				{
					if (row is PXResult)
						return MakeResult(row as PXResult<INSiteStatus>, inStockSince, noSalesSince);

					return MakeResult(new PXResult<INSiteStatus>(row as INSiteStatus), inStockSince, noSalesSince);
				}
			}

			return null;
		}

		protected virtual LumDeadStockEnqResult MakeResult(PXResult<INSiteStatus> selectResult,
			DateTime? inStockSince, DateTime? noSalesSince)
		{
			INSiteStatus siteStatus = selectResult;
			INItemSiteHistDay currentRow = GetCurrentINItemSiteHistD(siteStatus, inStockSince, noSalesSince);

			decimal deadStockQty = currentRow?.EndQty ?? 0m;
			if (deadStockQty <= 0m)
				return null;

			decimal? negativeQty = GetNegativeQty(siteStatus, inStockSince, noSalesSince);
			deadStockQty -= negativeQty ?? 0m;
			if (deadStockQty <= 0m)
				return null;

			var result = new LumDeadStockEnqResult()
			{
				BaseCuryID = Company.Current.BaseCuryID,
				DeadStockQty = deadStockQty,
				InStockQty = siteStatus.QtyOnHand,
				SiteID = siteStatus.SiteID,
				LastCost = GetLastCost(siteStatus),
				LastSaleDate = GetLastSaleDate(siteStatus),
				InventoryID = siteStatus.InventoryID,
				SubItemID = siteStatus.SubItemID
			};

			CalculateDeadStockValues(result, siteStatus, currentRow, deadStockQty);

			return result;
		}

		protected virtual INItemSiteHistDay GetCurrentINItemSiteHistD(INSiteStatus siteStatus,
			DateTime? inStockSince, DateTime? noSalesSince)
		{
			return SelectFrom<INItemSiteHistDay>
				.Where<INItemSiteHistDay.siteID.IsEqual<INSiteStatus.siteID.FromCurrent>
					.And<INItemSiteHistDay.inventoryID.IsEqual<INSiteStatus.inventoryID.FromCurrent>>
					.And<INItemSiteHistDay.subItemID.IsEqual<INSiteStatus.subItemID.FromCurrent>>
					.And<INItemSiteHistDay.sDate.IsLessEqual<@P.AsDateTime>>>
				.OrderBy<INItemSiteHistDay.sDate.Desc>
				.View.ReadOnly.SelectSingleBound(this, new object[] { siteStatus }, inStockSince ?? noSalesSince);
		}

		protected virtual decimal? GetNegativeQty(INSiteStatus siteStatus, DateTime? inStockSince, DateTime? noSalesSince)
		{
			var siteStatusCache = Caches[typeof(INSiteStatus)];

			decimal negativeQty =
				GetNegativePlanFields()
				.Sum(field =>
					(decimal?)siteStatusCache.GetValue(siteStatus, field.Name)) ?? 0m;

			INItemSiteHistDay aggregatedLastRows = SelectFrom<INItemSiteHistDay>
				.Where<INItemSiteHistDay.siteID.IsEqual<INSiteStatus.siteID.FromCurrent>
					.And<INItemSiteHistDay.inventoryID.IsEqual<INSiteStatus.inventoryID.FromCurrent>>
					.And<INItemSiteHistDay.subItemID.IsEqual<INSiteStatus.subItemID.FromCurrent>>
					.And<INItemSiteHistDay.sDate.IsGreater<@P.AsDateTime>>>
				.AggregateTo<Sum<INItemSiteHistDay.qtyCredit>>
				.View.ReadOnly.SelectSingleBound(this, new object[] { siteStatus }, inStockSince ?? noSalesSince);

			negativeQty += aggregatedLastRows?.QtyCredit ?? 0m;

			return negativeQty;
		}

		protected virtual Type[] GetNegativePlanFields()
		{
			return new Type[]
			{
				typeof(INSiteStatus.qtySOBackOrdered),
				typeof(INSiteStatus.qtySOPrepared),
				typeof(INSiteStatus.qtySOBooked),
				typeof(INSiteStatus.qtySOShipping),
				typeof(INSiteStatus.qtySOShipped),
				typeof(INSiteStatus.qtyINIssues),
				typeof(INSiteStatus.qtyFSSrvOrdPrepared),
				typeof(INSiteStatus.qtyFSSrvOrdBooked),
				typeof(INSiteStatus.qtyFSSrvOrdAllocated),
				typeof(INSiteStatus.qtyINAssemblyDemand),
				typeof(INSiteStatus.qtyProductionDemand)
			};
		}

		protected virtual decimal? GetLastCost(INSiteStatus siteStatus)
		{
			INItemStats itemStats = SelectFrom<INItemStats>
				.Where<INItemStats.inventoryID.IsEqual<INSiteStatus.inventoryID.FromCurrent>
					.And<INItemStats.siteID.IsEqual<INSiteStatus.siteID.FromCurrent>>>
				.OrderBy<INItemStats.lastCostDate.Desc>
				.View.ReadOnly.SelectSingleBound(this, new object[] { siteStatus });

			return itemStats?.LastCost;
		}

		protected virtual DateTime? GetLastSaleDate(INSiteStatus siteStatus)
		{
			INItemSiteHistDay lastSaleRow = SelectFrom<INItemSiteHistDay>
				.Where<INItemSiteHistDay.siteID.IsEqual<INSiteStatus.siteID.FromCurrent>
					.And<INItemSiteHistDay.inventoryID.IsEqual<INSiteStatus.inventoryID.FromCurrent>>
					.And<INItemSiteHistDay.subItemID.IsEqual<INSiteStatus.subItemID.FromCurrent>>
					.And<INItemSiteHistDay.qtySales.IsGreater<decimal0>>>
				.AggregateTo<Max<INItemSiteHistDay.sDate>>
				.View.ReadOnly.SelectSingleBound(this, new object[] { siteStatus });

			return lastSaleRow?.SDate;
		}

		protected virtual void CalculateDeadStockValues(LumDeadStockEnqResult result,
			INSiteStatus siteStatus, INItemSiteHistDay currentRow, decimal deadStockQty)
		{
			decimal deadStockQtyCounter = deadStockQty;
			result.InDeadStockDays = 0m;
			result.TotalDeadStockCost = 0m;

			IEnumerable<INItemSiteHistDay> lastRows = GetLastRows(siteStatus, deadStockQty, currentRow);
			foreach (INItemSiteHistDay lastRow in lastRows)
			{
				if ((lastRow.QtyDebit ?? 0m) == 0m)
					continue;

				if (CalculateDeadStockValues(ref deadStockQtyCounter, result, lastRow))
					return;
			}

			OnNotEnoughINItemSiteHistDRecords(siteStatus, currentRow, deadStockQty, deadStockQtyCounter);
		}

		protected virtual IEnumerable<INItemSiteHistDay> GetLastRows(INSiteStatus siteStatus, decimal deadStockQty, INItemSiteHistDay currentRow)
		{
			const int MaxRows = 1000;

			var getRows = new SelectFrom<INItemSiteHistDay>
				.Where<INItemSiteHistDay.siteID.IsEqual<@P.AsInt>
					.And<INItemSiteHistDay.inventoryID.IsEqual<@P.AsInt>>
					.And<INItemSiteHistDay.subItemID.IsEqual<@P.AsInt>>
					.And<INItemSiteHistDay.sDate.IsLess<@P.AsDateTime>>
					.And<INItemSiteHistDay.qtyDebit.IsGreater<decimal0>>>
				.OrderBy<INItemSiteHistDay.sDate.Desc>.View.ReadOnly(this);

			DateTime? lastDate = currentRow.SDate;
			decimal deadStockQtyCounter = deadStockQty;

			yield return currentRow;

			while (lastDate != null && deadStockQtyCounter > 0m)
			{
				// Acuminator disable once PX1015 IncorrectNumberOfSelectParameters It's acuminator issue: see jira ATR-600
				PXResultset<INItemSiteHistDay> rows = getRows.SelectWindowed(0, MaxRows,
					siteStatus.SiteID, siteStatus.InventoryID, siteStatus.SubItemID, lastDate);

				lastDate = null;

				foreach (var row in rows)
				{
					INItemSiteHistDay newRow = row;
					yield return newRow;

					lastDate = newRow.SDate;

					deadStockQtyCounter -= newRow.QtyDebit ?? 0m;
					if (deadStockQtyCounter <= 0m)
						break;
				}
			}
		}

		protected virtual bool CalculateDeadStockValues(ref decimal deadStockQtyCounter,
			LumDeadStockEnqResult result, INItemSiteHistDay lastRow)
		{
			decimal qtyDebit = (decimal)lastRow.QtyDebit;
			decimal mult = (deadStockQtyCounter >= qtyDebit) ? 1m : (deadStockQtyCounter / qtyDebit);

			//result.TotalDeadStockCost += (lastRow.CostDebit ?? 0m) * mult;
			result.TotalDeadStockCost += result.DeadStockQty * result.LastCost;

			decimal days = (decimal)GetCurrentDate().Subtract(lastRow.SDate.Value.Date).TotalDays;
			result.InDeadStockDays += days * qtyDebit * mult;

			deadStockQtyCounter -= qtyDebit;

			if (deadStockQtyCounter <= 0m)
			{
				result.AverageItemCost = result.TotalDeadStockCost / result.DeadStockQty;
				result.InDeadStockDays /= result.DeadStockQty;
				return true;
			}

			return false;
		}

		protected virtual void OnNotEnoughINItemSiteHistDRecords(INSiteStatus siteStatus, INItemSiteHistDay currentRow, decimal deadStockQty, decimal deadStockQtyCounter)
		{
			//PXTrace.WriteError(
			//	new Common.Exceptions.RowNotFoundException(Caches[typeof(INItemSiteHist)],
			//		siteStatus.SiteID,
			//		siteStatus.InventoryID,
			//		siteStatus.SubItemID,
			//		currentRow.SDate,
			//		deadStockQty,
			//		deadStockQtyCounter));
		}
	}
}
