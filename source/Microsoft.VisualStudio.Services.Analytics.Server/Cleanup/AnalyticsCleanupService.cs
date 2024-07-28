// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Cleanup.AnalyticsCleanupService
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Analytics.Cleanup
{
  public class AnalyticsCleanupService : IAnalyticsCleanupService, IVssFrameworkService
  {
    public const string Area = "Analytics";
    public const string Layer = "Cleanup";
    private const string c_lastUninstallInProgressRegistryPath = "/Service/Analytics/State/LastUninstallInProgress";
    private static readonly RegistryQuery s_lastUninstallInProgress = new RegistryQuery("/Service/Analytics/State/LastUninstallInProgress");
    private static readonly RegistryQuery s_oDataQueryElapsedTimeThresholdInMsKey = new RegistryQuery("/Service/Analytics/CleanupService/ODataQueryElapsedTimeThresholdInMs");
    private static readonly RegistryQuery s_jobTerminateLongRunningODataQueriesBatchSizeKey = new RegistryQuery("/Service/Analytics/CleanupService/TerminateLongRunningODataQueriesBatchSize");

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void CleanupOnPrem(IVssRequestContext requestContext)
    {
      if (!requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        throw new InvalidOperationException("Cannot run cleanup in hosted environment.");
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      service.SetValue<bool>(requestContext, "/Service/Analytics/State/LastUninstallInProgress", true);
      using (AnalyticsCleanupComponent component = requestContext.CreateComponent<AnalyticsCleanupComponent>())
        component.CleanupDuringAnalyticsUninstall();
      service.DeleteEntries(requestContext, "/Service/Analytics/State/LastUninstallInProgress");
    }

    public void TerminateLongRunningODataQueries(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties database)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        throw new InvalidOperationException("Cannot run TerminateLongRunningODataQueries in onPrem environment.");
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      int elapsedTimeThreshold = service.GetValue<int>(requestContext, in AnalyticsCleanupService.s_oDataQueryElapsedTimeThresholdInMsKey, true, 600000);
      int batchSize = service.GetValue<int>(requestContext, in AnalyticsCleanupService.s_jobTerminateLongRunningODataQueriesBatchSizeKey, true, 10);
      using (AnalyticsCleanupComponent componentRaw = TeamFoundationResourceManagementService.CreateComponentRaw<AnalyticsCleanupComponent>(database, connectionType: DatabaseConnectionType.Dbo))
        componentRaw.TerminateLongRunningQueries(elapsedTimeThreshold, "ODATA-97D3CA83-0B6F-4E51-8C96-6AD31588457A", batchSize);
    }

    public void TerminateLongRunningODataQueries(
      IVssRequestContext requestContext,
      ITeamFoundationDatabaseProperties database,
      ISqlConnectionInfo sqlConnectionInfo)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        throw new InvalidOperationException("Cannot run TerminateLongRunningODataQueries in onPrem environment.");
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      int elapsedTimeThreshold = service.GetValue<int>(requestContext, in AnalyticsCleanupService.s_oDataQueryElapsedTimeThresholdInMsKey, true, 600000);
      int batchSize = service.GetValue<int>(requestContext, in AnalyticsCleanupService.s_jobTerminateLongRunningODataQueriesBatchSizeKey, true, 10);
      try
      {
        using (AnalyticsCleanupComponent componentRaw = TeamFoundationResourceManagementService.CreateComponentRaw<AnalyticsCleanupComponent>(sqlConnectionInfo, 300))
          componentRaw.TerminateLongRunningQueries(elapsedTimeThreshold, "ODATA-97D3CA83-0B6F-4E51-8C96-6AD31588457A", batchSize);
      }
      catch (Exception ex)
      {
        requestContext.TraceAlways(12019101, TraceLevel.Error, "Analytics", "Cleanup", (string[]) null, string.Format("Error while cancelling requests from {0}_{1}: ", (object) database.DatabaseName, (object) sqlConnectionInfo.ApplicationIntent) + ex.ToString());
      }
    }

    public bool IsLastUninstallInProgress(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, in AnalyticsCleanupService.s_lastUninstallInProgress, false);
  }
}
