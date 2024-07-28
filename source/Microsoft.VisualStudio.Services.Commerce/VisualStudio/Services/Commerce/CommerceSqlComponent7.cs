// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceSqlComponent7
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class CommerceSqlComponent7 : CommerceSqlComponent6
  {
    public override IList<OfferMeter> GetOfferMeterConfiguration(int? meterId)
    {
      try
      {
        this.TraceEnter(5108426, nameof (GetOfferMeterConfiguration));
        this.PrepareStoredProcedure("prc_GetOfferMeters");
        if (meterId.HasValue)
          this.BindInt("@meterId", meterId.Value);
        return (IList<OfferMeter>) this.GetResults<OfferMeter>((ObjectBinder<OfferMeter>) new OfferMeterBinder(this.ComponentRequestContext)).ToList<OfferMeter>();
      }
      catch (Exception ex)
      {
        this.TraceException(5108427, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(5108428, nameof (GetOfferMeterConfiguration));
      }
    }

    public override OfferMeter GetOfferMeterConfigurationByName(string meterName)
    {
      try
      {
        this.TraceEnter(5108426, nameof (GetOfferMeterConfigurationByName));
        this.PrepareStoredProcedure("prc_GetOfferMeterByName");
        this.BindString("@meterName", meterName, 100, false, SqlDbType.VarChar);
        return this.GetResults<OfferMeter>((ObjectBinder<OfferMeter>) new OfferMeterBinder(this.ComponentRequestContext)).FirstOrDefault<OfferMeter>();
      }
      catch (Exception ex)
      {
        this.TraceException(5108427, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(5108428, nameof (GetOfferMeterConfigurationByName));
      }
    }
  }
}
