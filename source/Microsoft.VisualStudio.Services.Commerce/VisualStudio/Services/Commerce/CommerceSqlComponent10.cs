// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceSqlComponent10
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class CommerceSqlComponent10 : CommerceSqlComponent9
  {
    public override IList<OfferMeterPrice> GetOfferMeterPrice(string meterName)
    {
      try
      {
        this.TraceEnter(5108451, nameof (GetOfferMeterPrice));
        this.PrepareStoredProcedure("prc_GetOfferMeterPrice");
        this.BindString("@meterName", meterName, 200, false, SqlDbType.VarChar);
        return (IList<OfferMeterPrice>) this.GetResults<OfferMeterPrice>((ObjectBinder<OfferMeterPrice>) new OfferMeterPriceBinder(this.ComponentRequestContext)).ToList<OfferMeterPrice>();
      }
      catch (Exception ex)
      {
        this.TraceException(5108452, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(5108453, nameof (GetOfferMeterPrice));
      }
    }

    public override void AddOfferMeterPrice(
      IEnumerable<OfferMeterPrice> priceList,
      string meterName)
    {
      try
      {
        this.TraceEnter(5108454, nameof (AddOfferMeterPrice));
        this.Trace(5108457, TraceLevel.Info, string.Format("{0}: calling {1}. (meterName: {2}; count: {3})", (object) nameof (AddOfferMeterPrice), (object) "prc_AddOfferMeterPrice", (object) priceList.FirstOrDefault<OfferMeterPrice>()?.MeterName, (object) priceList.Count<OfferMeterPrice>()));
        if (priceList.Any<OfferMeterPrice>())
          this.Trace(5108460, TraceLevel.Info, "Inserting price for regions " + string.Join(", ", priceList.Select<OfferMeterPrice, string>((System.Func<OfferMeterPrice, string>) (x => x.Region))));
        this.PrepareStoredProcedure("prc_AddOfferMeterPrice");
        this.BindString("@meterName", meterName, 200, false, SqlDbType.VarChar);
        this.BindTable("@pricePlans", "Commerce.typ_OfferMeterPriceTable", new OfferMeterPriceBinder(this.ComponentRequestContext).BindOfferMeterPriceTable(priceList));
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(5108455, ex);
      }
      finally
      {
        this.Trace(5108458, TraceLevel.Info, string.Format("{0}: calling {1}. (meterName: {2} count: {3})", (object) nameof (AddOfferMeterPrice), (object) "prc_AddOfferMeterPrice", (object) priceList.FirstOrDefault<OfferMeterPrice>()?.MeterName, (object) priceList.Count<OfferMeterPrice>()));
        this.TraceLeave(5108458, nameof (AddOfferMeterPrice));
      }
    }
  }
}
