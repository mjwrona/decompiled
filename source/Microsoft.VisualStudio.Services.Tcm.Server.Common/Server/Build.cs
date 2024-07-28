// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Build
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal static class Build
  {
    private const int c_queryBuildConfigsBatchSize = 1000;
    private static IBuildConfiguration m_buildConfigurationHelper;
    private static IBuildServiceHelper m_buildServiceHelper;

    internal static void Delete(
      TestManagementRequestContext context,
      string projectName,
      string[] buildUris,
      bool deleteOnlyAutomatedRuns = false,
      bool skipPermissionCheck = false)
    {
      using (PerfManager.Measure(context.RequestContext, "Events", "BuildDeleteEvent"))
      {
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
        if (!skipPermissionCheck)
          context.SecurityManager.CheckDeleteTestResultsPermission(context, projectFromName.String);
        Guid teamFoundationId = context.UserTeamFoundationId;
        bool isTcmService = context.IsTcmService;
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          managementDatabase.DeleteTestBuild(projectFromName.GuidId, buildUris, teamFoundationId, deleteOnlyAutomatedRuns, isTcmService);
        if (context.IsFeatureEnabled("TestManagement.Server.DisableQueueingCleanUpJob"))
          return;
        context.TestManagementHost.SignalTfsJobService(context, context.JobMappings["TestManagement.Jobs.CleanupJob"].ToString());
      }
    }

    internal static void MarkTestBuildDeleted(
      TestManagementRequestContext context,
      string projectName,
      string[] buildUris)
    {
      using (PerfManager.Measure(context.RequestContext, "Events", nameof (MarkTestBuildDeleted)))
      {
        GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          managementDatabase.MarkTestBuildDeleted(projectFromName.GuidId, buildUris);
      }
    }

    internal static void UpdateBuildDeletionStateForExistingBuilds(
      TestManagementRequestContext context)
    {
      try
      {
        context.TraceEnter("BusinessLayer", "Build.UpdateBuildDeletionStatusForExistingBuilds");
        List<GuidAndString> guidAndStringList = (List<GuidAndString>) null;
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          guidAndStringList = managementDatabase.QueryProjects(context, false);
        if (guidAndStringList.Count == 0)
        {
          context.TraceInfo("BusinessLayer", "No projects found having test data.");
        }
        else
        {
          context.TraceInfo("BusinessLayer", "Total projects found: {0}", (object) guidAndStringList.Count);
          using (List<GuidAndString>.Enumerator enumerator = guidAndStringList.GetEnumerator())
          {
label_31:
            if (!enumerator.MoveNext())
              return;
            GuidAndString current = enumerator.Current;
            int count;
            do
            {
              IList<string> buildUris = Build.BuildConfigurationHelper.Query(context.RequestContext, current.GuidId, new bool?());
              count = buildUris.Count;
              if (count != 0)
              {
                context.TraceInfo("BusinessLayer", "No. of configs fetched for project: {0} are: {1}", (object) current.GuidId, (object) count);
                HashSet<string> source1 = Build.FetchDistinctTFSBuildUris(context, buildUris);
                HashSet<string> source2;
                try
                {
                  context.TraceVerbose("BusinessLayer", "Calling build service to fetch build for {0} tfs build Uris.", (object) source1.Count);
                  source2 = new HashSet<string>(Build.BuildServiceHelper.QueryBuildsByUris(context.RequestContext, current.GuidId, source1.ToList<string>()).ToList<BuildConfiguration>().Select<BuildConfiguration, string>((Func<BuildConfiguration, string>) (b => b.BuildUri)));
                  context.TraceInfo("BusinessLayer", "Called build service with {0} buildUris and got {1} valid build uris.", (object) source1.Count, (object) source2.Count);
                }
                catch (Exception ex)
                {
                  context.TraceWarning("BusinessLayer", "Error occurred in processing {0} builds for projects: {1}. Exception: {2}", (object) source1.Count, (object) current.GuidId, (object) ex.ToString());
                  source2 = source1;
                }
                Dictionary<string, bool> buildUriToDeletionStatus = new Dictionary<string, bool>();
                foreach (string key in (IEnumerable<string>) buildUris)
                {
                  bool flag = false;
                  if (source1.Contains<string>(key, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) && !source2.Contains<string>(key, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
                    flag = true;
                  if (!buildUriToDeletionStatus.ContainsKey(key))
                    buildUriToDeletionStatus[key] = flag;
                }
                using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
                  managementDatabase.UpdateBuildDeletionState(current.GuidId, buildUriToDeletionStatus);
              }
            }
            while (count == 1000);
            goto label_31;
          }
        }
      }
      finally
      {
        context.TraceLeave("BusinessLayer", "Build.UpdateBuildDeletionStatusForExistingBuilds");
      }
    }

    internal static IBuildConfiguration BuildConfigurationHelper
    {
      get
      {
        if (Build.m_buildConfigurationHelper == null)
          Build.m_buildConfigurationHelper = (IBuildConfiguration) new BuildConfiguration();
        return Build.m_buildConfigurationHelper;
      }
      set => Build.m_buildConfigurationHelper = value;
    }

    internal static IBuildServiceHelper BuildServiceHelper
    {
      get
      {
        if (Build.m_buildServiceHelper == null)
          Build.m_buildServiceHelper = (IBuildServiceHelper) new Microsoft.TeamFoundation.TestManagement.Server.BuildServiceHelper();
        return Build.m_buildServiceHelper;
      }
      set => Build.m_buildServiceHelper = value;
    }

    private static HashSet<string> FetchDistinctTFSBuildUris(
      TestManagementRequestContext context,
      IList<string> buildUris)
    {
      HashSet<string> stringSet = new HashSet<string>();
      foreach (string buildUri in (IEnumerable<string>) buildUris)
      {
        bool flag = true;
        try
        {
          ArtifactId artifactId = LinkingUtilities.DecodeUri(buildUri);
          int result = 0;
          if (!int.TryParse(artifactId.ToolSpecificId, out result))
          {
            context.TraceWarning("BusinessLayer", "Unable to parse build Id from uri: {0}", (object) buildUri);
            flag = false;
          }
        }
        catch (Exception ex)
        {
          context.TraceWarning("BusinessLayer", "Error occurred in decoding build Uri: {0}, Exception: {1}", (object) buildUri, (object) ex.ToString());
          flag = false;
        }
        if (flag)
          stringSet.Add(buildUri);
      }
      return stringSet;
    }
  }
}
