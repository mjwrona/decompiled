// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.OfferMeterPriceBinder
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class OfferMeterPriceBinder : ObjectBinder<OfferMeterPrice>
  {
    private SqlColumnBinder meterNameColumn = new SqlColumnBinder("MeterName");
    private SqlColumnBinder planNameColumn = new SqlColumnBinder("PlanName");
    private SqlColumnBinder regionColumn = new SqlColumnBinder("Region");
    private SqlColumnBinder currencyColumn = new SqlColumnBinder("CurrencyCode");
    private SqlColumnBinder quantityColumn = new SqlColumnBinder("Quantity");
    private SqlColumnBinder priceColumn = new SqlColumnBinder("Price");
    private IVssRequestContext requestContext;
    public static readonly SqlMetaData[] typ_AzureMeterPriceTable = new SqlMetaData[6]
    {
      new SqlMetaData("MeterName", SqlDbType.NVarChar, 200L),
      new SqlMetaData("PlanName", SqlDbType.NVarChar, 200L),
      new SqlMetaData("Region", SqlDbType.NVarChar, 10L),
      new SqlMetaData("CurrencyCode", SqlDbType.NVarChar, 10L),
      new SqlMetaData("Quantity", SqlDbType.Decimal, (byte) 28, (byte) 10),
      new SqlMetaData("Price", SqlDbType.Decimal, (byte) 28, (byte) 10)
    };
    private const string Area = "Commerce";
    private const string Layer = "OfferMeterPriceBinder";

    public OfferMeterPriceBinder(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(5106830, "Commerce", nameof (OfferMeterPriceBinder), ".ctor");
      this.requestContext = requestContext;
      this.requestContext.TraceLeave(5106839, "Commerce", nameof (OfferMeterPriceBinder), ".ctor");
    }

    public IEnumerable<SqlDataRecord> BindOfferMeterPriceTable(
      IEnumerable<OfferMeterPrice> pricePlans)
    {
      foreach (OfferMeterPrice pricePlan in pricePlans)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(OfferMeterPriceBinder.typ_AzureMeterPriceTable);
        sqlDataRecord.SetString(0, pricePlan.MeterName);
        sqlDataRecord.SetString(1, pricePlan.PlanName ?? "null");
        sqlDataRecord.SetString(2, pricePlan.Region);
        sqlDataRecord.SetString(3, pricePlan.CurrencyCode);
        sqlDataRecord.SetDecimal(4, Convert.ToDecimal(pricePlan.Quantity));
        sqlDataRecord.SetDecimal(5, Convert.ToDecimal(pricePlan.Price));
        yield return sqlDataRecord;
      }
    }

    [ExcludeFromCodeCoverage]
    protected override OfferMeterPrice Bind() => this.GetResourceFromReader((DbDataReader) this.Reader);

    internal OfferMeterPrice GetResourceFromReader(DbDataReader reader)
    {
      try
      {
        this.requestContext.TraceEnter(5106840, "Commerce", nameof (OfferMeterPriceBinder), nameof (GetResourceFromReader));
        return new OfferMeterPrice()
        {
          MeterName = this.meterNameColumn.GetString((IDataReader) reader, false),
          PlanName = this.planNameColumn.GetString((IDataReader) reader, false),
          Region = this.regionColumn.GetString((IDataReader) reader, false),
          CurrencyCode = this.currencyColumn.GetString((IDataReader) reader, false),
          Quantity = Convert.ToDouble(this.quantityColumn.GetObject((IDataReader) reader)),
          Price = Convert.ToDouble(this.priceColumn.GetObject((IDataReader) reader))
        };
      }
      catch (Exception ex)
      {
        this.requestContext.TraceException(5106848, "Commerce", nameof (OfferMeterPriceBinder), ex);
        throw;
      }
      finally
      {
        this.requestContext.TraceLeave(5106849, "Commerce", nameof (OfferMeterPriceBinder), nameof (GetResourceFromReader));
      }
    }
  }
}
