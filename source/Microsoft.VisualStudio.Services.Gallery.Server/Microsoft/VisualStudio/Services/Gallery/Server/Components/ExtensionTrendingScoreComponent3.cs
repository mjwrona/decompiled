// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.ExtensionTrendingScoreComponent3
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  public class ExtensionTrendingScoreComponent3 : ExtensionTrendingScoreComponent2
  {
    public override void CalculateTrendingScore(
      int installCutoff,
      string cumulativeStatisticName,
      string trendingStatisticName,
      string auditAction,
      string resourceType,
      bool useDailyStatsForTrending)
    {
      try
      {
        this.TraceEnter(12061039, "Calculate Trending score:" + trendingStatisticName);
        ArgumentUtility.CheckStringForNullOrEmpty(resourceType, nameof (resourceType));
        ArgumentUtility.CheckStringForNullOrEmpty(cumulativeStatisticName, nameof (cumulativeStatisticName));
        ArgumentUtility.CheckStringForNullOrEmpty(trendingStatisticName, nameof (trendingStatisticName));
        ArgumentUtility.CheckStringForNullOrEmpty(auditAction, nameof (auditAction));
        if (useDailyStatsForTrending)
        {
          this.PrepareStoredProcedure("Gallery.prc_UpdateExtensionTrendingScoreExtensionStatsDaily");
        }
        else
        {
          this.PrepareStoredProcedure("Gallery.prc_UpdateExtensionTrendingScore");
          this.BindString(nameof (cumulativeStatisticName), cumulativeStatisticName, 100, BindStringBehavior.NullToEmptyString, SqlDbType.VarChar);
          this.BindString("AuditAction", auditAction, 100, BindStringBehavior.NullToEmptyString, SqlDbType.NVarChar);
          this.BindString(nameof (resourceType), resourceType, 100, BindStringBehavior.NullToEmptyString, SqlDbType.VarChar);
        }
        this.BindInt(nameof (installCutoff), installCutoff);
        this.BindString("TrendingStatisticName", trendingStatisticName, 100, BindStringBehavior.NullToEmptyString, SqlDbType.VarChar);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(12061041, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(12061040, "Leave Calculate Trending score");
      }
    }
  }
}
