// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Cleanup.IAnalyticsCleanupService
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Analytics.Cleanup
{
  [DefaultServiceImplementation(typeof (AnalyticsCleanupService))]
  public interface IAnalyticsCleanupService : IVssFrameworkService
  {
    void CleanupOnPrem(IVssRequestContext requestContext);

    void TerminateLongRunningODataQueries(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties database);

    void TerminateLongRunningODataQueries(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties database,
      ISqlConnectionInfo sqlConnectionInfo);

    bool IsLastUninstallInProgress(IVssRequestContext requestContext);
  }
}
