// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Cleanup.AnalyticsCleanupComponent2
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Analytics.Cleanup
{
  internal class AnalyticsCleanupComponent2 : AnalyticsCleanupComponent
  {
    public override void TerminateLongRunningQueries(
      int elapsedTimeThreshold,
      string queryIdentifier,
      int batchSize)
    {
      this.PrepareStoredProcedure("AnalyticsInternal.prc_TerminateLongRunningODataQueries");
      this.BindInt("@elapsedTimeThreshold", elapsedTimeThreshold);
      this.BindString("@queryIdentifier", queryIdentifier, 256, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindInt("@batchSize", batchSize);
      this.ExecuteNonQuery();
    }
  }
}
