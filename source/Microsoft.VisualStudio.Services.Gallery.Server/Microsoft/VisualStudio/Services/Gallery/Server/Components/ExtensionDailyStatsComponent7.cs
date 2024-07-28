// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.ExtensionDailyStatsComponent7
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class ExtensionDailyStatsComponent7 : ExtensionDailyStatsComponent6
  {
    public override void ExecuteDailyStatsBatch(
      List<ExtensionDailyStatsUpdateData> extensionDailyStatsCommandData)
    {
      this.PrepareStoredProcedure("Gallery.prc_ExecuteDailyStatsBatch");
      this.BindDailyStatsUpdateDataTable("dailyStatsCommandBatch", (IEnumerable<ExtensionDailyStatsUpdateData>) extensionDailyStatsCommandData);
      this.ExecuteNonQuery();
    }
  }
}
