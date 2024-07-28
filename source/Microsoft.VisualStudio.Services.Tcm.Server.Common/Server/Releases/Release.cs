// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Releases.Release
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server.Releases
{
  internal static class Release
  {
    private static int DefaultTestRunDeletionBatchSize = 500;

    internal static void Delete(
      TestManagementRequestContext context,
      Guid projectId,
      List<int> releaseIds)
    {
      try
      {
        ProjectInfo projectFromId = Microsoft.TeamFoundation.TestManagement.Server.Validator.GetProjectFromId(context.RequestContext, projectId, false);
        context.SecurityManager.CheckDeleteTestResultsPermission(context, projectFromId.Uri);
        int testRunDeletionBatchSize;
        Release.GetReleaseRegistryData(context.RequestContext, out testRunDeletionBatchSize);
        Guid teamFoundationId = context.UserTeamFoundationId;
        bool isTcmService = context.IsTcmService;
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          managementDatabase.DeleteTestReleases(projectFromId.Id, releaseIds, context.UserTeamFoundationId, testRunDeletionBatchSize, isTcmService);
        if (!context.IsFeatureEnabled("TestManagement.Server.DisableQueueingCleanUpJob"))
          context.TestManagementHost.SignalTfsJobService(context, context.JobMappings["TestManagement.Jobs.CleanupJob"].ToString());
        CustomerIntelligenceData cid = new CustomerIntelligenceData();
        cid.Add("ProjectId", (object) projectId);
        cid.Add("HardDeletedReleasesCount", (double) releaseIds.Count);
        cid.Add("HardDeletedReleasesIds", string.Join<int>(",", (IEnumerable<int>) releaseIds));
        TelemetryLogger.Instance.PublishData(context.RequestContext, "HardDeletedReleases", cid);
      }
      catch (ProjectDoesNotExistException ex)
      {
        context.RequestContext.TraceException("Exceptions", (Exception) ex);
      }
    }

    private static void GetReleaseRegistryData(
      IVssRequestContext context,
      out int testRunDeletionBatchSize)
    {
      IVssRegistryService service = context.GetService<IVssRegistryService>();
      testRunDeletionBatchSize = service.GetValue<int>(context, (RegistryQuery) "/Service/TestManagement/Settings/TestRunDeletionBatchSize", Release.DefaultTestRunDeletionBatchSize);
    }
  }
}
