// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.ExtensionDailyStatsComponent6
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class ExtensionDailyStatsComponent6 : ExtensionDailyStatsComponent5
  {
    public override void IncrementDownloadCountStat(
      Guid extensionId,
      string version,
      DateTime statisticDate)
    {
      this.PrepareStoredProcedure("Gallery.prc_IncrementWebDownloadCountStat");
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindString(nameof (version), version, 43, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindDate(nameof (statisticDate), statisticDate.ToUniversalTime());
      this.ExecuteNonQuery();
    }

    public override List<ExtensionDailyStat> GetExtensionDailyStats(
      Guid extensionId,
      DateTime afterDate)
    {
      string str = "Gallery.prc_GetExtensionDailyStats";
      this.PrepareStoredProcedure(str);
      this.BindGuid(nameof (extensionId), extensionId);
      this.BindDate(nameof (afterDate), afterDate.ToUniversalTime());
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), str, this.RequestContext))
      {
        resultCollection.AddBinder<ExtensionDailyStat>((ObjectBinder<ExtensionDailyStat>) new ExtensionDailyStatBinder3());
        return resultCollection.GetCurrent<ExtensionDailyStat>().Items;
      }
    }
  }
}
