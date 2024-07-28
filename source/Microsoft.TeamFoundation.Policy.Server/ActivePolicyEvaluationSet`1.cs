// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.ActivePolicyEvaluationSet`1
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Policy.Server.Framework;
using Microsoft.TeamFoundation.Policy.WebApi;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.TeamFoundation.Policy.Server
{
  internal class ActivePolicyEvaluationSet<TPolicy> where TPolicy : class, ITeamFoundationPolicy
  {
    private readonly Dictionary<int, ActivePolicyEvaluation<TPolicy>> m_activePolicies;
    private readonly ArtifactId m_artifactId;
    private readonly Guid m_teamProjectId;
    private const string c_layer = "PolicyService";

    internal static ActivePolicyEvaluationSet<TPolicy> LoadAllPoliciesForProject(
      IVssRequestContext requestContext,
      Func<IVssRequestContext, IPolicyComponent> componentFactory,
      Func<PolicyConfigurationRecord, string[]> determineScopes,
      Guid projectId,
      string[] scopes,
      bool includeHidden = false)
    {
      return ActivePolicyEvaluationSet<TPolicy>.LoadPolicies(requestContext, componentFactory, determineScopes, projectId, (ArtifactId) null, scopes, new int?(), includeHidden);
    }

    internal static ActivePolicyEvaluationSet<TPolicy> LoadAllPoliciesForArtifact(
      IVssRequestContext requestContext,
      Func<IVssRequestContext, IPolicyComponent> componentFactory,
      Func<PolicyConfigurationRecord, string[]> determineScopes,
      Guid projectId,
      ArtifactId artifact,
      string[] scopes,
      bool includeHidden = false)
    {
      ArgumentUtility.CheckForNull<ArtifactId>(artifact, nameof (artifact));
      return ActivePolicyEvaluationSet<TPolicy>.LoadPolicies(requestContext, componentFactory, determineScopes, projectId, artifact, scopes, new int?(), includeHidden);
    }

    internal static ActivePolicyEvaluationSet<TPolicy> LoadSpecificPolicyForArtifact(
      IVssRequestContext requestContext,
      Func<IVssRequestContext, IPolicyComponent> componentFactory,
      Guid projectId,
      ArtifactId artifact,
      int policyConfigurationId,
      bool includeHidden = false)
    {
      ArgumentUtility.CheckForNull<ArtifactId>(artifact, nameof (artifact));
      return ActivePolicyEvaluationSet<TPolicy>.LoadPolicies(requestContext, componentFactory, (Func<PolicyConfigurationRecord, string[]>) null, projectId, artifact, (string[]) null, new int?(policyConfigurationId), includeHidden);
    }

    internal IEnumerable<ActivePolicyEvaluation<TPolicy>> Entries => (IEnumerable<ActivePolicyEvaluation<TPolicy>>) this.m_activePolicies.Values;

    private void TraceLoadedPolicies(ref StringBuilder traceData)
    {
      traceData.AppendLine(string.Format("Number of loaded policies: {0}", (object) this.Entries.Count<ActivePolicyEvaluation<TPolicy>>()));
      traceData.AppendLine("ExistingEvaluationRecord? | OldResultsToClear? | Applicable? | Broken? | Skippable? | NewResultsToSave? | Passing? \\");
      traceData.AppendLine("Policy: ID | DisplayName | Bypassable? | Area | Config (ID | Revision ID | Blocking? | Enabled? | Project ID | Settings)");
      foreach (ActivePolicyEvaluation<TPolicy> entry in this.Entries)
      {
        traceData.AppendLine(string.Format("{0} | {1} | {2} | {3} | {4} | {5} \\", (object) entry.HasOldResultsToClear, (object) entry.IsApplicable, (object) entry.IsBroken, (object) entry.IsSkippable, (object) entry.HasNewResultsToSave, (object) entry.IsPassing));
        entry.TracePolicy(ref traceData);
      }
    }

    private void TraceDetailedData(
      IVssRequestContext requestContext,
      int traceDataId,
      string traceHeader,
      params string[] extraTraceData)
    {
      if (!requestContext.IsTracing(traceDataId, TraceLevel.Verbose, "Policy", "PolicyService"))
        return;
      StringBuilder traceData = new StringBuilder(traceHeader);
      traceData.AppendLine();
      if (extraTraceData != null)
      {
        foreach (string str in extraTraceData)
          traceData.AppendLine(str);
      }
      traceData.AppendLine();
      this.TraceLoadedPolicies(ref traceData);
      requestContext.Trace(traceDataId, TraceLevel.Verbose, "Policy", "PolicyService", traceData.ToString());
    }

    internal void InitializePolicies(
      IVssRequestContext requestContext,
      PolicyTemplateCache policyTypes,
      Guid teamProjectId,
      IActivePolicyEvaluationCache cacheService = null)
    {
      using (requestContext.TraceBlock(1390087, 1390088, "Policy", "PolicyService", MethodBase.GetCurrentMethod().Name))
      {
        this.TraceDetailedData(requestContext, 1390089, "Initialize Data:");
        if (cacheService != null)
        {
          IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
          int num1 = service.GetValue<int>(requestContext, (RegistryQuery) "/Service/Policy/Settings/PolicyEvaluation/MinPoliciesNumberToApplyCaching", true, 200);
          int num2 = service.GetValue<int>(requestContext, (RegistryQuery) "/Service/Policy/Settings/PolicyEvaluation/MinFilesNumberToApplyCaching", true, 20000);
          if (this.Entries.Count<ActivePolicyEvaluation<TPolicy>>() < num1 && cacheService.MinFileCountToUseCache < num2)
          {
            requestContext.Trace(1390144, TraceLevel.Info, "Policy", "PolicyService", "Cache for policy evaluation was bypassed as the PR is not big enough: actualPoliciesAmount={0} < minPoliciesThresholdForCaching={1} && TargetFilesCount={2} < minFilesThresholdForCaching={3}", (object) this.Entries.Count<ActivePolicyEvaluation<TPolicy>>(), (object) num1, (object) cacheService.MinFileCountToUseCache, (object) num2);
            if (requestContext.IsFeatureEnabled("Policy.EventBasedCacheEnabled"))
              cacheService.BypassCache(requestContext);
            else
              cacheService = (IActivePolicyEvaluationCache) null;
          }
        }
        foreach (ActivePolicyEvaluation<TPolicy> entry in this.Entries)
          entry.Initialize(requestContext, policyTypes, teamProjectId, cacheService);
      }
    }

    internal void CheckBypass(IVssRequestContext requestContext, ITeamFoundationPolicyTarget target)
    {
      using (requestContext.TraceBlock(1390129, 1390130, "Policy", "PolicyService", MethodBase.GetCurrentMethod().Name))
      {
        this.TraceDetailedData(requestContext, 1390131, "Check Bypass Data:");
        Lazy<bool> isBypassable = new Lazy<bool>((Func<bool>) (() => target.HasBypassPermissionInTarget(requestContext)), false);
        this.ForEachActiveEvaluation(requestContext, (Action<ActivePolicyEvaluation<TPolicy>>) (p => p.CheckBypass(isBypassable)), nameof (CheckBypass));
      }
    }

    internal void CheckScopes(IVssRequestContext requestContext, ITeamFoundationPolicyTarget target)
    {
      using (requestContext.TimeRegion("Policy", "PolicyService", regionName: nameof (CheckScopes)))
      {
        using (requestContext.TraceBlock(1390091, 1390092, "Policy", "PolicyService", MethodBase.GetCurrentMethod().Name))
        {
          this.TraceDetailedData(requestContext, 1390093, "Check Scope Data:");
          this.ForEachActiveEvaluation(requestContext, (Action<ActivePolicyEvaluation<TPolicy>>) (p => p.CheckScope(requestContext, target)), nameof (CheckScopes));
        }
      }
    }

    private void ForEachActiveEvaluation(
      IVssRequestContext requestContext,
      Action<ActivePolicyEvaluation<TPolicy>> action,
      [CallerMemberName] string caller = null)
    {
      Dictionary<Guid, int> dictionary1 = new Dictionary<Guid, int>();
      Dictionary<Guid, long> dictionary2 = new Dictionary<Guid, long>();
      Stopwatch stopwatch1 = Stopwatch.StartNew();
      Stopwatch stopwatch2 = new Stopwatch();
      foreach (ActivePolicyEvaluation<TPolicy> entry in this.Entries)
      {
        PolicyConfigurationRecord currentConfiguration = entry.CurrentConfiguration;
        Guid key = currentConfiguration != null ? currentConfiguration.TypeId : Guid.Empty;
        dictionary1[key] = dictionary1.GetValueOrDefault<Guid, int>(key, 0) + 1;
        stopwatch2.Restart();
        action(entry);
        dictionary2[key] = dictionary2.GetValueOrDefault<Guid, long>(key, 0L) + stopwatch2.ElapsedMilliseconds;
      }
      if (stopwatch1.ElapsedMilliseconds <= 2000L)
        return;
      try
      {
        TeamFoundationPolicyService service = requestContext.GetService<ITeamFoundationPolicyService>() as TeamFoundationPolicyService;
        Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
        dictionary3["$caller"] = (object) caller;
        foreach (Guid key1 in dictionary1.Keys)
        {
          ITeamFoundationPolicy policyType = (ITeamFoundationPolicy) null;
          service?.TryGetPolicyType(requestContext, key1, out policyType);
          string key2 = policyType?.GetType().Name ?? key1.ToString();
          dictionary3[key2] = (object) new
          {
            count = dictionary1.GetValueOrDefault<Guid, int>(key1, 0),
            elapsedMs = dictionary2.GetValueOrDefault<Guid, long>(key1, 0L)
          };
        }
        string format = JsonConvert.SerializeObject((object) dictionary3);
        requestContext.TraceAlways(1390096, TraceLevel.Warning, "Policy", "PolicyService", format, (object[]) null);
      }
      catch
      {
      }
    }

    internal void Notify(
      IVssRequestContext requestContext,
      Func<TPolicy, PolicyEvaluationStatus?, TeamFoundationPolicyEvaluationRecordContext, PolicyActionResult> action)
    {
      using (requestContext.TimeRegion("Policy", "PolicyService", regionName: nameof (Notify)))
      {
        using (requestContext.TraceBlock(1390094, 1390095, "Policy", "PolicyService", MethodBase.GetCurrentMethod().Name))
        {
          this.TraceDetailedData(requestContext, 1390099, "Notify Data:", string.Format("Action Method Name: {0}.", (object) action.Method.Name));
          foreach (ActivePolicyEvaluation<TPolicy> entry in this.Entries)
            entry.Notify(requestContext, this.m_teamProjectId, action);
        }
      }
    }

    internal PolicyEvaluationResult Check(
      IVssRequestContext requestContext,
      Func<TPolicy, PolicyEvaluationStatus?, TeamFoundationPolicyEvaluationRecordContext, PolicyCheckResult> action)
    {
      using (requestContext.TraceBlock(1390099, 1390100, "Policy", "PolicyService", MethodBase.GetCurrentMethod().Name))
      {
        this.TraceDetailedData(requestContext, 1390078, "Check Data:", string.Format("Action Method Name: {0}.", (object) action.Method.Name));
        bool isPassing = true;
        bool requiresBypass = false;
        string rejectionReason = (string) null;
        foreach (ActivePolicyEvaluation<TPolicy> entry in this.Entries)
        {
          entry.Check(requestContext, this.m_teamProjectId, action);
          if (entry.IsApplicable && !entry.IsPassing && !entry.IsSkippable)
          {
            if (entry.IsBypassable)
              requiresBypass = true;
            else
              isPassing = false;
            rejectionReason = entry.RejectionReason;
          }
        }
        if (!isPassing)
          requiresBypass = false;
        return new PolicyEvaluationResult(isPassing, rejectionReason, requiresBypass);
      }
    }

    internal void UpdateDynamicPolicyEvaluationRecord(
      IVssRequestContext requestContext,
      ITeamFoundationPolicyTarget target,
      PolicyEvaluationRecord record,
      out bool notApplicable)
    {
      notApplicable = true;
      ActivePolicyEvaluation<TPolicy> policyEvaluation;
      this.m_activePolicies.TryGetValue(record.Configuration.Id, out policyEvaluation);
      if (policyEvaluation == null || (object) policyEvaluation.Policy == null || !policyEvaluation.IsApplicable)
        return;
      notApplicable = false;
      policyEvaluation.UpdateDynamicPolicyEvaluationRecord(requestContext, target, record);
    }

    internal void UpdateDynamicPolicyEvaluationRecordWithCaching(
      IVssRequestContext requestContext,
      ITeamFoundationPolicyTarget target,
      PolicyEvaluationRecord record,
      out bool notApplicable)
    {
      notApplicable = true;
      ActivePolicyEvaluation<TPolicy> policyEvaluation;
      if (!this.m_activePolicies.TryGetValue(record.Configuration.Id, out policyEvaluation) || (object) policyEvaluation.Policy == null || !policyEvaluation.IsApplicable)
        return;
      notApplicable = false;
      policyEvaluation.UpdateDynamicPolicyEvaluationRecordWithCaching(requestContext, target, record);
    }

    internal void SavePolicyEvaluationRecords(
      IVssRequestContext requestContext,
      Func<IVssRequestContext, IPolicyComponent> componentFactory,
      ClientTraceData ctData)
    {
      using (requestContext.TimeRegion("Policy", "PolicyService", regionName: nameof (SavePolicyEvaluationRecords)))
      {
        using (requestContext.TraceBlock(1390101, 1390102, "Policy", "PolicyService", MethodBase.GetCurrentMethod().Name))
        {
          this.TraceDetailedData(requestContext, 1390079, "Save Data:", string.Format("Artifact ID: {0}.", (object) this.m_artifactId));
          List<PolicyEvaluationRecord> updatedRecords = new List<PolicyEvaluationRecord>();
          List<int> idsOfRecordsToDelete = new List<int>();
          bool allowSyncPublish = true;
          foreach (ActivePolicyEvaluation<TPolicy> entry in this.Entries)
          {
            if (entry.HasNewResultsToSave)
            {
              PolicyEvaluationRecord evaluationRecord = entry.GetNewPolicyEvaluationRecord(this.m_artifactId);
              updatedRecords.Add(evaluationRecord);
              allowSyncPublish &= entry.AllowSyncPublishNotification;
            }
            else if (entry.HasOldResultsToClear)
            {
              idsOfRecordsToDelete.Add(entry.PolicyConfigurationId);
              allowSyncPublish &= entry.AllowSyncPublishNotification;
            }
          }
          if (updatedRecords.Count > 0 || idsOfRecordsToDelete.Count > 0)
          {
            using (IPolicyComponent policyComponent = componentFactory(requestContext))
              policyComponent.UpdatePolicyEvaluationRecords(this.m_teamProjectId, this.m_artifactId, (IEnumerable<PolicyEvaluationRecord>) updatedRecords, (IEnumerable<int>) idsOfRecordsToDelete);
            PolicyEvaluationUpdateNotification notification = new PolicyEvaluationUpdateNotification(this.m_teamProjectId, LinkingUtilities.EncodeUri(this.m_artifactId));
            this.PublishNotification(requestContext, notification, allowSyncPublish, ctData);
          }
          else
          {
            \u003C\u003Ef__AnonymousType1<Guid?, int?, int?, bool?, bool, bool, bool, bool, bool, string, string>[] array = this.Entries.Select(entry => new
            {
              TypeId = entry.CurrentConfiguration?.TypeId,
              ConfigurationId = entry.CurrentConfiguration?.ConfigurationId,
              ConfigurationRevisionId = entry.CurrentConfiguration?.ConfigurationRevisionId,
              IsBlocking = entry.CurrentConfiguration?.IsBlocking,
              IsApplicable = entry.IsApplicable,
              IsBroken = entry.IsBroken,
              IsBypassable = entry.IsBypassable,
              IsPassing = entry.IsPassing,
              IsSkippable = entry.IsSkippable,
              RejectionReason = entry.RejectionReason,
              FailedToLoadReason = entry.FailedToLoadReason
            }).ToArray();
            ctData?.Add("PolicyEvaluationEntries", (object) array);
          }
        }
      }
    }

    private void PublishNotification(
      IVssRequestContext requestContext,
      PolicyEvaluationUpdateNotification notification,
      bool allowSyncPublish,
      ClientTraceData ctData)
    {
      if ((requestContext.GetType().Name == "JobRequestContext" ? 1 : (requestContext.Items.ContainsKey("IsTaskThread") ? 1 : 0)) == 0)
        allowSyncPublish = false;
      Stopwatch stopwatch = Stopwatch.StartNew();
      try
      {
        ITeamFoundationEventService service = requestContext.GetService<ITeamFoundationEventService>();
        if (allowSyncPublish)
          service.SyncPublishNotification(requestContext, (object) notification);
        else
          service.PublishNotification(requestContext, (object) notification);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1390080, "Policy", "PolicyService", ex);
      }
      finally
      {
        stopwatch.Stop();
        object obj = (object) new
        {
          allowSyncPublish = allowSyncPublish,
          ProjectId = notification.ProjectId,
          ArtifactId = notification.ArtifactId,
          ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
        };
        ctData?.Add("PolicyEvaluationUpdateNotificationPublishData", obj);
        requestContext.Trace(1390081, TraceLevel.Verbose, "Policy", "PolicyService", JsonConvert.SerializeObject(obj));
      }
    }

    private ActivePolicyEvaluationSet(
      Dictionary<int, ActivePolicyEvaluation<TPolicy>> activePolicies,
      Guid projectId,
      ArtifactId artifactId)
    {
      this.m_activePolicies = activePolicies;
      this.m_teamProjectId = projectId;
      this.m_artifactId = artifactId;
    }

    private static ActivePolicyEvaluationSet<TPolicy> LoadPolicies(
      IVssRequestContext requestContext,
      Func<IVssRequestContext, IPolicyComponent> componentFactory,
      Func<PolicyConfigurationRecord, string[]> determineScopes,
      Guid projectId,
      ArtifactId artifactId,
      string[] scopes,
      int? specificConfigurationId,
      bool includeHidden = false)
    {
      using (requestContext.TimeRegion("Policy", "PolicyService", regionName: nameof (LoadPolicies)))
      {
        requestContext.Trace(1390063, TraceLevel.Verbose, "Policy", "PolicyService", "Loading policies for project ID '{0}', artifact ID '{1}'.", (object) projectId, (object) artifactId);
        IEnumerable<PolicyEvaluationRecord> evaluationRecords;
        if (artifactId != null)
        {
          using (IPolicyComponent policyComponent = componentFactory(requestContext))
          {
            evaluationRecords = policyComponent.GetPolicyEvaluationRecords(projectId, specificConfigurationId, new int?(), artifactId, true, int.MaxValue, 0).GetCurrentAsEnumerable();
            evaluationRecords = TeamFoundationPolicyService.FilterOutAllButLatestRecords(evaluationRecords);
          }
        }
        else
          evaluationRecords = (IEnumerable<PolicyEvaluationRecord>) Array.Empty<PolicyEvaluationRecord>();
        bool flag1 = requestContext.IsFeatureEnabled("Policy.EnablePolicyByScopeSprocV2");
        IEnumerable<PolicyConfigurationRecord> configurations;
        using (IPolicyComponent policyComponent1 = componentFactory(requestContext))
        {
          if (specificConfigurationId.HasValue)
          {
            PolicyConfigurationRecord policyConfiguration = policyComponent1.GetLatestPolicyConfiguration(projectId, specificConfigurationId.Value);
            PolicyConfigurationRecord[] configurationRecordArray;
            if (policyConfiguration != null)
              configurationRecordArray = new PolicyConfigurationRecord[1]
              {
                policyConfiguration
              };
            else
              configurationRecordArray = Array.Empty<PolicyConfigurationRecord>();
            configurations = (IEnumerable<PolicyConfigurationRecord>) configurationRecordArray;
          }
          else
          {
            IPolicyComponent policyComponent2 = policyComponent1;
            Guid projectId1 = projectId;
            string[] scopes1 = scopes;
            Func<PolicyConfigurationRecord, string[]> determineScopes1 = determineScopes;
            bool flag2 = includeHidden;
            bool flag3 = flag1;
            Guid? policyType = new Guid?();
            int num1 = flag2 ? 1 : 0;
            int num2 = flag3 ? 1 : 0;
            configurations = (IEnumerable<PolicyConfigurationRecord>) policyComponent2.GetLatestPolicyConfigurationsByScope(projectId1, (IEnumerable<string>) scopes1, (Func<PolicyConfigurationRecord, IEnumerable<string>>) determineScopes1, int.MaxValue, 1, policyType, num1 != 0, num2 != 0);
          }
        }
        Dictionary<int, PolicyEvaluationRecord> dictionary = evaluationRecords.ToDictionary<PolicyEvaluationRecord, int, PolicyEvaluationRecord>((Func<PolicyEvaluationRecord, int>) (r => r.Configuration.Id), (Func<PolicyEvaluationRecord, PolicyEvaluationRecord>) (r => r));
        return new ActivePolicyEvaluationSet<TPolicy>(ActivePolicyEvaluationSet<TPolicy>.ApplyPolicyConfigurations(configurations, dictionary), projectId, artifactId);
      }
    }

    private static Dictionary<int, ActivePolicyEvaluation<TPolicy>> ApplyPolicyConfigurations(
      IEnumerable<PolicyConfigurationRecord> configurations,
      Dictionary<int, PolicyEvaluationRecord> evaluationRecords)
    {
      Dictionary<int, ActivePolicyEvaluation<TPolicy>> dictionary = evaluationRecords.Values.ToDictionary<PolicyEvaluationRecord, int, ActivePolicyEvaluation<TPolicy>>((Func<PolicyEvaluationRecord, int>) (r => r.Configuration.Id), (Func<PolicyEvaluationRecord, ActivePolicyEvaluation<TPolicy>>) (r => new ActivePolicyEvaluation<TPolicy>(r, (PolicyConfigurationRecord) null)));
      foreach (PolicyConfigurationRecord configuration in configurations)
      {
        PolicyEvaluationRecord existingState;
        if (!evaluationRecords.TryGetValue(configuration.ConfigurationId, out existingState))
          existingState = (PolicyEvaluationRecord) null;
        dictionary[configuration.ConfigurationId] = new ActivePolicyEvaluation<TPolicy>(existingState, configuration);
      }
      return dictionary;
    }
  }
}
