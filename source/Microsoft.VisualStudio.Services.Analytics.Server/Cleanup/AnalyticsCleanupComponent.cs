// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Cleanup.AnalyticsCleanupComponent
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Analytics.Cleanup
{
  public class AnalyticsCleanupComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<AnalyticsCleanupComponent>(1, true),
      (IComponentCreator) new ComponentCreator<AnalyticsCleanupComponent2>(2)
    }, "AnalyticsCleanup");
    private static Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    public AnalyticsCleanupComponent()
    {
      this.ContainerErrorCode = 50000;
      this.SelectedFeatures = SqlResourceComponentFeatures.None;
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) AnalyticsCleanupComponent.s_sqlExceptionFactories;

    internal void CleanupDuringAnalyticsUninstall()
    {
      this.PrepareStoredProcedure("AnalyticsInternal.prc_CleanupOnPremData");
      this.ExecuteNonQuery();
    }

    public virtual void TerminateLongRunningQueries(
      int elapsedTimeThreshold,
      string queryIdentifier,
      int batchSize)
    {
      throw new NotImplementedException();
    }
  }
}
