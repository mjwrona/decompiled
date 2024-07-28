// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DistributedTaskHostedPoolHelper
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal static class DistributedTaskHostedPoolHelper
  {
    private const int c_agentTimeoutBuffer = 1;
    private const int c_resizeJobQueueDelaySeconds = 300;
    private const int c_defaultPoolSizeOverride = 5;
    private const string c_hasRunPublicBuildsKey = "HasRunPublicBuilds";
    private const string c_layer = "DistributedTaskHostedPoolHelper";
    private const string c_isForkVariableName = "system.pullRequest.isFork";
    private const string c_QueuedByVariableName = "build.queuedBy";
    private const string c_QueuedByVariableValue = "GitHub";
    private static RegistryQuery s_currentPoolSizeQuery = (RegistryQuery) "/Service/DistributedTask/Settings/HostedPool/CurrentSize";
    private static RegistryQuery s_currentPoolSizeTimeoutStart = (RegistryQuery) "/Service/DistributedTask/Settings/HostedPool/CurrentSizeTimeoutStart";
    private static RegistryQuery s_publicRequestGrantedQuery = (RegistryQuery) "/Service/DistributedTask/Settings/HostedPool/PublicConcurrencyGranted";
    private static RegistryQuery s_poolSizeOverrideQuery = (RegistryQuery) "/Service/DistributedTask/Settings/HostedPool/PoolSizeOverride";
    private static RegistryQuery s_minOrgAgeInMonthsForTier1 = (RegistryQuery) "/Service/DistributedTask/Settings/HostedPool/MinOrgAgeInMonthsToGetTier1";

    public static int GetCurrentPoolSize(IVssRequestContext requestContext)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      return new TimeSpan(DateTime.UtcNow.Ticks - service.GetValue<DateTime>(requestContext, in DistributedTaskHostedPoolHelper.s_currentPoolSizeTimeoutStart, new DateTime(2020, 4, 1)).Ticks).TotalHours >= 24.0 ? -1 : service.GetValue<int>(requestContext, in DistributedTaskHostedPoolHelper.s_currentPoolSizeQuery, -1);
    }

    public static int GetMinimumOrgAgeInMonthsForTier1(IVssRequestContext requestContext) => Math.Abs(requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, in DistributedTaskHostedPoolHelper.s_minOrgAgeInMonthsForTier1, 6));

    public static HostedPoolSettings GetPoolSettings(
      IVssRequestContext requestContext,
      int defaultParallelism,
      Guid scopeId = default (Guid),
      Guid planId = default (Guid),
      string parallelismTag = null,
      bool allowStaleValues = true)
    {
      HostedPoolSettings poolSettings = new HostedPoolSettings()
      {
        HasPremiumAgents = true,
        MaxParallelism = defaultParallelism,
        RequestTimeout = PipelineConstants.ResourceLimits.PremiumAgentTimeout + 1,
        Tier = 2
      };
      if (requestContext.IsFeatureEnabled("DistributedTask.PipelineBillingModel2.SelfHosted.InfiniteResourceLimits"))
      {
        requestContext.TraceInfo(nameof (DistributedTaskHostedPoolHelper), "Infinite resource limits are set. Reading override pool size.");
        poolSettings.MaxParallelism = DistributedTaskHostedPoolHelper.GetPoolSizeOverride(requestContext);
        return poolSettings;
      }
      try
      {
        int val1 = 0;
        bool flag1 = false;
        bool flag2 = false;
        bool flag3 = false;
        IEnumerable<ResourceLimit> resourceLimits = DistributedTaskHostedPoolHelper.GetResourceLimits(requestContext, allowStaleValues);
        ResourceLimit resourceLimit1 = resourceLimits.FirstOrDefault<ResourceLimit>((Func<ResourceLimit, bool>) (x => string.Equals(x.ParallelismTag, "Private", StringComparison.OrdinalIgnoreCase)));
        if (resourceLimit1 != null)
        {
          val1 = resourceLimit1.TotalCount.GetValueOrDefault();
          flag1 = resourceLimit1.FailedToReachAllProviders;
          string s;
          int result;
          bool flag4 = resourceLimit1.Data.TryGetValue("PurchasedCount", out s) && int.TryParse(s, out result) && result > 0;
          flag2 = resourceLimit1.IsPremium;
          requestContext.TraceInfo(nameof (DistributedTaskHostedPoolHelper), "Private limits grantedTotalConcurrency: {0}, failedToReachAllProviders: {1}, hasPurchaseAgents: {2}, hasPrivatePremiumAgents: {3}", (object) val1, (object) flag1, (object) flag4, (object) flag2);
        }
        bool flag5 = string.Equals(parallelismTag, "Public", StringComparison.OrdinalIgnoreCase);
        bool flag6 = requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.AnonymousAccess") && requestContext.IsFeatureEnabled("VisualStudio.Services.DelayedIdentityValidation");
        requestContext.TraceInfo(nameof (DistributedTaskHostedPoolHelper), "publicProjectsOn: {0}, isPublicRequest: {1}", (object) flag6, (object) flag5);
        if (flag6)
        {
          bool flag7 = DistributedTaskHostedPoolHelper.GetPublicConcurrencyGranted(requestContext);
          if (!flag7)
          {
            if (flag5)
              flag7 = true;
            else
              flag7 = requestContext.GetService<IDistributedTaskResourceService>().GetAgentPools(requestContext, propertyFilters: (IList<string>) new string[1]
              {
                "HasRunPublicBuilds"
              }).Any<TaskAgentPool>((Func<TaskAgentPool, bool>) (x => x.IsHosted && x.Properties.ContainsKey("HasRunPublicBuilds")));
            if (flag7)
              DistributedTaskHostedPoolHelper.SetPublicConcurrencyGranted(requestContext);
          }
          if (flag7)
          {
            ResourceLimit resourceLimit2 = resourceLimits.FirstOrDefault<ResourceLimit>((Func<ResourceLimit, bool>) (x => string.Equals(x.ParallelismTag, "Public", StringComparison.OrdinalIgnoreCase)));
            if (resourceLimit2 != null)
            {
              int valueOrDefault = resourceLimit2.TotalCount.GetValueOrDefault();
              string s;
              int result;
              if (resourceLimit2.Data.TryGetValue("PrivatePurchasedCount", out s) && int.TryParse(s, out result) && result > 0)
                valueOrDefault -= result;
              val1 += valueOrDefault;
              flag1 |= resourceLimit2.FailedToReachAllProviders;
              flag3 = valueOrDefault > 0;
              requestContext.TraceInfo(nameof (DistributedTaskHostedPoolHelper), "Public limits grantedPublicConcurrency: {0}, failedToReachAllProviders(PublicLimits): {1}, hasPublicPremiumAgents: {2}", (object) valueOrDefault, (object) resourceLimit2.FailedToReachAllProviders, (object) flag3);
            }
          }
        }
        if (flag1)
        {
          poolSettings.MaxParallelism = Math.Max(val1, poolSettings.MaxParallelism);
          requestContext.TraceWarning(nameof (DistributedTaskHostedPoolHelper), "Failed to reach all providers for concurrency count so defaulting to {0}", (object) poolSettings.MaxParallelism);
        }
        else
        {
          poolSettings.HasPremiumAgents = flag5 ? flag3 : flag2;
          poolSettings.MaxParallelism = val1;
          if (!poolSettings.HasPremiumAgents)
            poolSettings.RequestTimeout = PipelineConstants.ResourceLimits.FreeAgentTimeout + 1;
          int inMonthsForTier1 = DistributedTaskHostedPoolHelper.GetMinimumOrgAgeInMonthsForTier1(requestContext);
          bool flag8 = false;
          if (requestContext.IsFeatureEnabled("DistributedTask.EnableTier3AssignmentToGitHubForkedRepos"))
            flag8 = DistributedTaskHostedPoolHelper.IsForkedGitHubPullRequest(requestContext, scopeId, planId);
          if (flag2 && !flag8)
          {
            IVssRequestContext requestContext1 = requestContext;
            DateTime threshold = DateTime.UtcNow.AddMonths(-inMonthsForTier1);
            poolSettings.Tier = !DistributedTaskHostedPoolHelper.IsOlderThan(requestContext1, threshold) ? 2 : 1;
          }
          else
          {
            IVssRequestContext requestContext2 = requestContext;
            DateTime threshold = DateTime.UtcNow.AddMonths(-6);
            poolSettings.Tier = !DistributedTaskHostedPoolHelper.IsOlderThan(requestContext2, threshold) || flag8 ? 3 : 2;
          }
          requestContext.TraceAlways(10015228, TraceLevel.Info, "DistributedTask", nameof (DistributedTaskHostedPoolHelper), string.Format("Assigned pool tier={0}", (object) poolSettings.Tier));
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10015159, nameof (DistributedTaskHostedPoolHelper), ex);
      }
      return poolSettings;
    }

    public static bool GetPublicConcurrencyGranted(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, in DistributedTaskHostedPoolHelper.s_publicRequestGrantedQuery, false);

    public static void ResizePoolsIfNecessary(
      IVssRequestContext requestContext,
      HostedPoolSettings settings,
      bool runNow = false)
    {
      int currentPoolSize = DistributedTaskHostedPoolHelper.GetCurrentPoolSize(requestContext);
      if (currentPoolSize == settings.MaxParallelism)
      {
        requestContext.TraceInfo(10015185, "DistributedTask", nameof (DistributedTaskHostedPoolHelper), (object) string.Format("No need to queue HostedPoolResizeJob as currentSize matches maxParallelism, currentSize={0}, stackTrace={1})", (object) currentPoolSize, (object) new StackTrace().ToString()));
      }
      else
      {
        try
        {
          ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
          requestContext.TraceAlways(10015183, TraceLevel.Info, "DistributedTask", nameof (DistributedTaskHostedPoolHelper), string.Format("HostedPoolResizeJob queued, jobId={0}, runNow={1}, currentSize={2}, maxParallelism={3}, stackTrace={4})", (object) TaskConstants.HostedPoolResizeJob, (object) runNow, (object) currentPoolSize, (object) settings.MaxParallelism, (object) new StackTrace().ToString()));
          if (runNow)
            service.QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
            {
              TaskConstants.HostedPoolResizeJob
            });
          else
            service.QueueDelayedJobs(requestContext, (IEnumerable<Guid>) new Guid[1]
            {
              TaskConstants.HostedPoolResizeJob
            }, 300);
        }
        catch (JobDefinitionNotFoundException ex)
        {
          requestContext.TraceException(10015184, nameof (DistributedTaskHostedPoolHelper), (Exception) ex);
          DistributedTaskCommerceHelper.ResizeHostedPools(requestContext, settings.MaxParallelism);
        }
      }
    }

    public static void SetCurrentPoolSize(IVssRequestContext requestContext, int currentSize)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      service.SetValue<int>(requestContext, DistributedTaskHostedPoolHelper.s_currentPoolSizeQuery.Path, currentSize);
      service.SetValue<DateTime>(requestContext, DistributedTaskHostedPoolHelper.s_currentPoolSizeTimeoutStart.Path, DateTime.UtcNow);
    }

    public static void SetPublicConcurrencyGranted(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().SetValue<bool>(requestContext, DistributedTaskHostedPoolHelper.s_publicRequestGrantedQuery.Path, true);

    private static IDictionary<string, VariableValue> GetPlan(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid planId)
    {
      ITaskHubPlanFetcher service = requestContext.GetService<ITaskHubPlanFetcher>();
      requestContext.Trace(10015276, TraceLevel.Info, "DistributedTask", nameof (DistributedTaskHostedPoolHelper), "GetPlan:1 Is projectId:{0}, planId:{1}", (object) projectId, (object) planId);
      if (projectId != Guid.Empty && planId != Guid.Empty)
      {
        TaskOrchestrationPlan plan = service.GetPlan(requestContext, projectId, planId);
        if (plan != null && plan.ProcessEnvironment != null && plan.ProcessEnvironment is PipelineEnvironment processEnvironment)
        {
          requestContext.Trace(10015276, TraceLevel.Info, "DistributedTask", nameof (DistributedTaskHostedPoolHelper), "GetPlan:3 Trying to check if pipeline env variables are present:{0}, for project id:{1}", (object) (processEnvironment != null), (object) projectId);
          if (processEnvironment != null && processEnvironment.SystemVariables != null)
            return processEnvironment.SystemVariables;
        }
      }
      requestContext.Trace(10015276, TraceLevel.Info, "DistributedTask", nameof (DistributedTaskHostedPoolHelper), "GetPlan:4 No variables found for project:{0}", (object) projectId);
      return (IDictionary<string, VariableValue>) new Dictionary<string, VariableValue>();
    }

    private static bool IsForkedGitHubPullRequest(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid planId)
    {
      IDictionary<string, VariableValue> plan = DistributedTaskHostedPoolHelper.GetPlan(requestContext, projectId, planId);
      return DistributedTaskHostedPoolHelper.IsForkBuild(plan) && DistributedTaskHostedPoolHelper.IsGitHubRepoBuild(plan);
    }

    private static bool IsForkBuild(IDictionary<string, VariableValue> variables)
    {
      VariableValue variableValue;
      bool result;
      return ((!variables.TryGetValue("system.pullRequest.isFork", out variableValue) ? 0 : (bool.TryParse(variableValue.Value, out result) ? 1 : 0)) & (result ? 1 : 0)) != 0;
    }

    private static bool IsGitHubRepoBuild(IDictionary<string, VariableValue> variables)
    {
      VariableValue variableValue;
      return variables.TryGetValue("build.queuedBy", out variableValue) && string.Equals(variableValue.Value, "GitHub", StringComparison.OrdinalIgnoreCase);
    }

    private static IEnumerable<ResourceLimit> GetResourceLimits(
      IVssRequestContext requestContext,
      bool allowStaleValues = true)
    {
      using (new MethodScope(requestContext, nameof (DistributedTaskHostedPoolHelper), nameof (GetResourceLimits)))
      {
        requestContext.CheckProjectCollectionRequestContext();
        Guid hostId = requestContext.ServiceHost.InstanceId;
        IVssRequestContext poolRequestContext = requestContext.ToPoolRequestContext();
        return poolRequestContext.GetService<ITaskHubLicenseService>().GetResourceLimits(poolRequestContext, allowStaleValues).Where<ResourceLimit>((Func<ResourceLimit, bool>) (x => x.HostId == hostId && x.IsHosted));
      }
    }

    private static int GetPoolSizeOverride(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, in DistributedTaskHostedPoolHelper.s_poolSizeOverrideQuery, true, 5);

    private static bool IsOlderThan(IVssRequestContext requestContext, DateTime threshold)
    {
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Application);
      Microsoft.VisualStudio.Services.Organization.Organization organization = context.GetService<IOrganizationService>().GetOrganization(context, (IEnumerable<string>) new List<string>());
      if (organization == null)
      {
        requestContext.TraceException(10015222, nameof (DistributedTaskHostedPoolHelper), new Exception("Organization lookup failed"));
        return false;
      }
      return organization.DateCreated < threshold;
    }
  }
}
