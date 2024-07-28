// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CommerceSqlComponent19
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;
using System.Data;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class CommerceSqlComponent19 : CommerceSqlComponent18
  {
    public override void UpdateOfferMeterDefinitionName(IOfferMeter meterConfig)
    {
      try
      {
        this.TraceEnter(5107291, nameof (UpdateOfferMeterDefinitionName));
        this.Trace(5107296, TraceLevel.Info, string.Format("{0}: calling {1}. (PlatformMeterId: {2}, Name: {3}, GalleryId: {4}", (object) nameof (UpdateOfferMeterDefinitionName), (object) "prc_UpdateOfferMeterDefinitionName", (object) meterConfig.PlatformMeterId, (object) meterConfig.Name, (object) meterConfig.GalleryId));
        this.PrepareStoredProcedure("prc_UpdateOfferMeterDefinitionName");
        this.BindGuid("@platformMeterId", meterConfig.PlatformMeterId);
        this.BindString("@galleryId", meterConfig.GalleryId, 256, false, SqlDbType.VarChar);
        this.BindString("@name", meterConfig.Name, 256, false, SqlDbType.VarChar);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(5107293, ex);
        throw;
      }
      finally
      {
        this.Trace(5107295, TraceLevel.Info, string.Format("{0}: calling {1}. (PlatformMeterId: {2}, Name: {3})", (object) nameof (UpdateOfferMeterDefinitionName), (object) "prc_UpdateOfferMeterDefinitionName", (object) meterConfig.PlatformMeterId, (object) meterConfig.Name));
        this.TraceLeave(5107295, nameof (UpdateOfferMeterDefinitionName));
      }
    }
  }
}
