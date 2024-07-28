// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.ExtensionTrendingScoreComponent
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  public class ExtensionTrendingScoreComponent : TeamFoundationSqlResourceComponent
  {
    [StaticSafe]
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories;
    private const string s_area = "GalleryTrendingScoreComponent";
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<ExtensionTrendingScoreComponent>(1),
      (IComponentCreator) new ComponentCreator<ExtensionTrendingScoreComponent2>(2),
      (IComponentCreator) new ComponentCreator<ExtensionTrendingScoreComponent3>(3)
    }, "TrendingScore");

    static ExtensionTrendingScoreComponent()
    {
      ExtensionTrendingScoreComponent.s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();
      ExtensionTrendingScoreComponent.s_sqlExceptionFactories.Add(270011, new SqlExceptionFactory(typeof (ArgumentException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((requestContext, errorNumber, ex, sqlError) => (Exception) new ArgumentException(ex.Message))));
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) ExtensionTrendingScoreComponent.s_sqlExceptionFactories;

    protected override string TraceArea => "GalleryTrendingScoreComponent";

    public virtual void CalculateTrendingScore(
      int installCutoff,
      string cumulativeStatisticName,
      string trendingStatisticName,
      string auditAction,
      string resourceType)
    {
      throw new NotImplementedException();
    }

    public virtual void CalculateTrendingScore(
      int installCutoff,
      string cumulativeStatisticName,
      string trendingStatisticName,
      string auditAction,
      string resourceType,
      bool useDailyStatsForTrending)
    {
      throw new NotImplementedException();
    }
  }
}
