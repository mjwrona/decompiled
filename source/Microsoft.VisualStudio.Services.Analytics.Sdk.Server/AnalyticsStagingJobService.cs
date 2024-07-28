// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Analytics.Server.AnalyticsStagingJobService
// Assembly: Microsoft.VisualStudio.Services.Analytics.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E5A0742E-601C-4AD5-8902-781963AA7C5A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.Sdk.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Analytics.Server
{
  public class AnalyticsStagingJobService : IAnalyticsStagingJobService, IVssFrameworkService
  {
    private static readonly RegistryQuery s_liveJobsQuery = new RegistryQuery("/Service/Analytics/FeatureFlaggedJobs/Live/*");
    private const string c_stagingJobRegistryPathPattern = "/Service/Analytics/FeatureFlaggedJobs/Live/*";
    private const string c_trace = "AnalyticsStagingJobService";
    private const int c_forceStateDelaySeconds = 5;

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void UpdateFeatureFlaggedStagingJobSchedules(
      IVssRequestContext requestContext,
      Action<string> logger)
    {
      if (!requestContext.GetService<IAnalyticsFeatureService>().IsAnalyticsEnabled(requestContext, bypassCache: true))
      {
        logger("Analytics service is not enabled for this account");
      }
      else
      {
        List<Guid> list = this.GetFeatureFlaggedJobs(requestContext).ToList<Guid>();
        if (list.Count == 0)
        {
          logger("Found 0 jobs under registry: /Service/Analytics/FeatureFlaggedJobs/Live/*");
        }
        else
        {
          ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
          List<TeamFoundationJobDefinition> foundationJobDefinitionList = service.QueryJobDefinitions(requestContext, (IEnumerable<Guid>) list);
          ArgumentUtility.CheckForNull<List<TeamFoundationJobDefinition>>(foundationJobDefinitionList, "jobDefinitions");
          if (foundationJobDefinitionList.Count != list.Count)
            throw new ApplicationException(AnalyticsSdkServerResources.JOB_DEFINITIONS_JOB_IDS_MIS_MATCH((object) foundationJobDefinitionList.Count<TeamFoundationJobDefinition>(), (object) list.Count));
          List<Guid> values = new List<Guid>();
          List<TeamFoundationJobReference> jobReferences = new List<TeamFoundationJobReference>();
          for (int index = 0; index < list.Count; ++index)
          {
            TeamFoundationJobDefinition foundationJobDefinition = foundationJobDefinitionList[index];
            if (foundationJobDefinition == null)
            {
              values.Add(list[index]);
              requestContext.Trace(15220002, TraceLevel.Error, "Analytics", nameof (AnalyticsStagingJobService), string.Format("Job definition with ID {0} is null.", (object) list[index]));
            }
            else
            {
              try
              {
                FeatureFlaggedSchedule featureFlaggedSchedule;
                try
                {
                  featureFlaggedSchedule = foundationJobDefinition.Data != null ? TeamFoundationSerializationUtility.Deserialize<FeatureFlaggedSchedule>(foundationJobDefinition.Data) : (FeatureFlaggedSchedule) null;
                }
                catch (Exception ex)
                {
                  logger(string.Format("Could not deserialize job definition data {0} for {1}. Exception: {2}", (object) foundationJobDefinition.Data, (object) foundationJobDefinition.Name, (object) ex));
                  continue;
                }
                logger(string.Format("job name: {0} / feature flag name: {1} / hasSchedules: {2}", (object) foundationJobDefinition.Name, (object) featureFlaggedSchedule.FeatureFlagName, (object) featureFlaggedSchedule?.Schedule?.Count));
                if (!string.IsNullOrEmpty(featureFlaggedSchedule.FeatureFlagName))
                {
                  if (!requestContext.IsFeatureEnabled(featureFlaggedSchedule.FeatureFlagName))
                    continue;
                }
                if (!featureFlaggedSchedule.Schedule.SequenceEqual<TeamFoundationJobSchedule>((IEnumerable<TeamFoundationJobSchedule>) foundationJobDefinition.Schedule, SchedulerComparer.Default))
                {
                  jobReferences.Add(foundationJobDefinition.ToJobReference());
                  logger("Queueing " + foundationJobDefinition.Name + ".");
                }
              }
              catch (Exception ex)
              {
                logger("Encountered an exception '" + ex.Message + "' while processing job " + (foundationJobDefinition?.Name ?? "null"));
              }
            }
          }
          if (jobReferences.Count > 0)
            service.QueueDelayedJobs(requestContext, (IEnumerable<TeamFoundationJobReference>) jobReferences, 5);
          if (values.Count > 0)
            throw new ApplicationException(AnalyticsSdkServerResources.MISSING_JOB_DEFINITIONS((object) string.Join<Guid>(", ", (IEnumerable<Guid>) values)));
        }
      }
    }

    public virtual HashSet<Guid> GetFeatureFlaggedJobs(IVssRequestContext requestContext)
    {
      HashSet<Guid> featureFlaggedJobs = new HashSet<Guid>();
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      foreach (RegistryItem registryItem in vssRequestContext.GetService<IVssRegistryService>().Read(vssRequestContext, in AnalyticsStagingJobService.s_liveJobsQuery))
      {
        Guid result;
        if (Guid.TryParse(registryItem.Value, out result))
          featureFlaggedJobs.Add(result);
        else
          requestContext.Trace(15220001, TraceLevel.Error, "Analytics", nameof (AnalyticsStagingJobService), "Could not parse job id from registry path: " + registryItem.Path);
      }
      return featureFlaggedJobs;
    }
  }
}
