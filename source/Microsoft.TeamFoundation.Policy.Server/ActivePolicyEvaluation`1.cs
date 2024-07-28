// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.ActivePolicyEvaluation`1
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Policy.Server.Framework;
using Microsoft.TeamFoundation.Policy.WebApi;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Diagnostics;
using System.Text;

namespace Microsoft.TeamFoundation.Policy.Server
{
  internal class ActivePolicyEvaluation<TPolicy> where TPolicy : class, ITeamFoundationPolicy
  {
    private bool m_isStale;
    private PolicyEvaluationStatus? m_incomingStatus;
    private JObject m_rawIncomingContext;
    private IActivePolicyEvaluationCache m_cacheService;
    private Guid m_failedToLoadErrorCode;
    private bool m_isBrokenBecauseApplicabilityCheckFailed;
    private bool m_isBrokenBecauseItFailedToEvaluate;
    private bool m_isBrokenBecauseItsConfigWillNotLoad;
    private bool m_isBrokenBecauseItsTypeIsMissing;
    private bool m_isBypassable;
    private bool m_isSkippableBecauseItIsNonBlocking;
    private bool m_policyDoesntCareAboutThisCheck;
    private bool m_isThisKindOfPolicy = true;
    private bool m_isEvaluationIrrelevantBecauseItHasBeenDeleted;
    private bool m_isEvaluationIrrelevantBecauseItHasBeenDisabled;
    private bool m_isEvaluationIrrelevantBecauseItIsOutOfScope;
    private TeamFoundationPolicyEvaluationRecordContext m_incomingContext;
    private PolicyEvaluationStatus? m_outgoingStatus;
    private TeamFoundationPolicyEvaluationRecordContext m_outgoingContext;
    private const string c_layer = "PolicyService";

    internal PolicyConfigurationRecord CurrentConfiguration { get; private set; }

    internal int PolicyConfigurationId { get; private set; }

    internal TPolicy Policy { get; private set; }

    internal string FailedToLoadReason { get; private set; }

    internal bool HasOldResultsToClear
    {
      get
      {
        if (!this.m_incomingStatus.HasValue)
          return false;
        return this.m_isEvaluationIrrelevantBecauseItHasBeenDeleted || this.m_isEvaluationIrrelevantBecauseItHasBeenDisabled || this.m_isEvaluationIrrelevantBecauseItIsOutOfScope;
      }
    }

    internal bool IsBroken => this.m_isBrokenBecauseApplicabilityCheckFailed || this.m_isBrokenBecauseItFailedToEvaluate || this.m_isBrokenBecauseItsConfigWillNotLoad || this.m_isBrokenBecauseItsTypeIsMissing;

    internal bool IsSkippable => this.m_isSkippableBecauseItIsNonBlocking || this.m_policyDoesntCareAboutThisCheck;

    internal bool IsBypassable => this.m_isBypassable;

    internal bool IsApplicable => this.m_isThisKindOfPolicy && !this.m_isEvaluationIrrelevantBecauseItHasBeenDeleted && !this.m_isEvaluationIrrelevantBecauseItHasBeenDisabled && !this.m_isEvaluationIrrelevantBecauseItIsOutOfScope;

    internal string RejectionReason { get; private set; }

    internal bool HasNewResultsToSave
    {
      get
      {
        if (!this.IsApplicable || this.m_policyDoesntCareAboutThisCheck)
          return false;
        return this.m_isStale || this.m_IsChanged;
      }
    }

    private bool m_IsChanged
    {
      get
      {
        if (this.m_incomingStatus.HasValue)
        {
          PolicyEvaluationStatus? outgoingStatus = this.m_outgoingStatus;
          PolicyEvaluationStatus evaluationStatus1 = this.m_incomingStatus.Value;
          if (outgoingStatus.GetValueOrDefault() == evaluationStatus1 & outgoingStatus.HasValue)
          {
            outgoingStatus = this.m_outgoingStatus;
            PolicyEvaluationStatus evaluationStatus2 = PolicyEvaluationStatus.Broken;
            return !(outgoingStatus.GetValueOrDefault() == evaluationStatus2 & outgoingStatus.HasValue) && this.m_outgoingContext != this.m_incomingContext;
          }
        }
        return true;
      }
    }

    internal bool IsPassing
    {
      get
      {
        PolicyEvaluationStatus? outgoingStatus1 = this.m_outgoingStatus;
        PolicyEvaluationStatus evaluationStatus1 = PolicyEvaluationStatus.Approved;
        if (outgoingStatus1.GetValueOrDefault() == evaluationStatus1 & outgoingStatus1.HasValue)
          return true;
        PolicyEvaluationStatus? outgoingStatus2 = this.m_outgoingStatus;
        PolicyEvaluationStatus evaluationStatus2 = PolicyEvaluationStatus.NotApplicable;
        return outgoingStatus2.GetValueOrDefault() == evaluationStatus2 & outgoingStatus2.HasValue;
      }
    }

    internal bool AllowSyncPublishNotification
    {
      get
      {
        __Boxed<TPolicy> policy = (object) this.Policy;
        return policy != null && policy.AllowSyncPublishNotification;
      }
    }

    internal ActivePolicyEvaluation(
      PolicyEvaluationRecord existingState,
      PolicyConfigurationRecord policyConfig)
    {
      this.m_isStale = false;
      if (policyConfig != null)
      {
        this.PolicyConfigurationId = policyConfig.ConfigurationId;
        this.CurrentConfiguration = policyConfig;
      }
      if (existingState != null)
      {
        this.PolicyConfigurationId = existingState.Configuration.Id;
        this.m_rawIncomingContext = existingState.Context;
        this.m_incomingStatus = existingState.Status;
      }
      if (policyConfig != null && existingState != null)
        this.m_isStale = existingState.Configuration.Revision != policyConfig.ConfigurationRevisionId;
      this.Policy = default (TPolicy);
      this.FailedToLoadReason = (string) null;
      this.m_failedToLoadErrorCode = BrokenPolicyEvaluationRecordContext.NoError;
      this.RejectionReason = (string) null;
    }

    internal void TracePolicy(ref StringBuilder traceData)
    {
      traceData.AppendLine("Policy: ");
      if ((object) this.Policy == null)
        traceData.AppendLine("  Policy not yet fully initialized.");
      else
        traceData.AppendLine(string.Format("  {0} | {1} | {2} | {3} | ({4} | {5} | {6} | {7})", (object) this.Policy.Id, (object) this.Policy.DisplayName, (object) this.Policy.IsBypassable, (object) this.Policy.Area, (object) this.Policy.Configuration.ConfigurationId, (object) this.Policy.Configuration.ConfigurationRevisionId, (object) this.Policy.Configuration.IsBlocking, (object) this.Policy.Configuration.IsEnabled));
      traceData.AppendLine("Internal Booleans: ");
      traceData.AppendLine(string.Format("  {0}: {1}", (object) "IsEvaluationIrrelevantBecauseItHasBeenDeleted", (object) this.m_isEvaluationIrrelevantBecauseItHasBeenDeleted));
      traceData.AppendLine(string.Format("  {0}: {1}", (object) "IsEvaluationIrrelevantBecauseItHasBeenDisabled", (object) this.m_isEvaluationIrrelevantBecauseItHasBeenDisabled));
      traceData.AppendLine(string.Format("  {0}: {1}", (object) "IsEvalutionIrrelevantBecauseItIsOutOfScope", (object) this.m_isEvaluationIrrelevantBecauseItIsOutOfScope));
      traceData.AppendLine(string.Format("  {0}: {1}", (object) "IsSkippableBecauseItCanBeBypassed", (object) this.m_isBypassable));
      traceData.AppendLine(string.Format("  {0}: {1}", (object) "IsSkippableBecauseItIsNonBlocking", (object) this.m_isSkippableBecauseItIsNonBlocking));
      traceData.AppendLine(string.Format("  {0}: {1}", (object) "IsBrokenBecauseApplicabilityCheckFailed", (object) this.m_isBrokenBecauseApplicabilityCheckFailed));
      traceData.AppendLine(string.Format("  {0}: {1}", (object) "IsBrokenBecauseItFailedToEvaluate", (object) this.m_isBrokenBecauseItFailedToEvaluate));
      traceData.AppendLine(string.Format("  {0}: {1}", (object) "IsBrokenBecauseItsConfigWillNotLoad", (object) this.m_isBrokenBecauseItsConfigWillNotLoad));
      traceData.AppendLine(string.Format("  {0}: {1}", (object) "IsBrokenBecauseItsTypeIsMissing", (object) this.m_isBrokenBecauseItsTypeIsMissing));
    }

    internal void UpdateDynamicPolicyEvaluationRecord(
      IVssRequestContext requestContext,
      ITeamFoundationPolicyTarget target,
      PolicyEvaluationRecord record)
    {
      this.m_rawIncomingContext = record.Context;
      this.m_incomingStatus = record.Status;
      this.CheckScope(requestContext, target);
      this.RunPolicy(requestContext, target.TeamProjectId, false, (Func<TPolicy, PolicyEvaluationStatus?, TeamFoundationPolicyEvaluationRecordContext, PolicyActionResult>) ((_, status, context) => (PolicyActionResult) ((IDynamicEvaluationPolicy) (object) this.Policy).OnDynamicEvaluation(requestContext, target, status, context)));
      record.Status = this.m_outgoingStatus;
      record.Context = this.ConvertContextToJObject((object) this.m_outgoingContext);
      record.Configuration = this.CurrentConfiguration.ToWebApi(requestContext, securedObject: (ISecuredObject) record);
    }

    internal void UpdateDynamicPolicyEvaluationRecordWithCaching(
      IVssRequestContext requestContext,
      ITeamFoundationPolicyTarget target,
      PolicyEvaluationRecord record)
    {
      this.m_rawIncomingContext = record.Context;
      this.m_incomingStatus = record.Status;
      this.RunPolicy(requestContext, target.TeamProjectId, false, (Func<TPolicy, PolicyEvaluationStatus?, TeamFoundationPolicyEvaluationRecordContext, PolicyActionResult>) ((_, status, context) => (PolicyActionResult) ((IDynamicEvaluationPolicy) (object) this.Policy).OnDynamicEvaluation(requestContext, target, status, context)));
      record.Status = this.m_outgoingStatus;
      record.Context = this.ConvertContextToJObject((object) this.m_outgoingContext);
      record.Configuration = this.CurrentConfiguration.ToWebApi(requestContext, securedObject: (ISecuredObject) record);
    }

    internal void Initialize(
      IVssRequestContext requestContext,
      PolicyTemplateCache allPolicies,
      Guid teamProjectId,
      IActivePolicyEvaluationCache cacheService = null)
    {
      if (this.CurrentConfiguration == null)
        this.m_isEvaluationIrrelevantBecauseItHasBeenDeleted = true;
      else if (!this.CurrentConfiguration.IsEnabled)
      {
        this.m_isEvaluationIrrelevantBecauseItHasBeenDisabled = true;
      }
      else
      {
        this.m_isSkippableBecauseItIsNonBlocking = !this.CurrentConfiguration.IsBlocking;
        ITeamFoundationPolicy template;
        allPolicies.TryGetValue(this.CurrentConfiguration.TypeId, out template);
        if (template == null)
        {
          requestContext.Trace(1390060, TraceLevel.Error, "Policy", "PolicyService", "No matching policy for policy ID {0}, with typeId {1}", (object) this.CurrentConfiguration.ConfigurationId, (object) this.CurrentConfiguration.TypeId);
          this.FailedToLoadReason = PolicyResources.Format("PolicyTypeNotFound", (object) this.CurrentConfiguration.TypeId);
          this.m_failedToLoadErrorCode = BrokenPolicyEvaluationRecordContext.MissingType;
          this.m_isBrokenBecauseItsTypeIsMissing = true;
        }
        else
        {
          this.m_isThisKindOfPolicy = template is TPolicy;
          if (!this.m_isThisKindOfPolicy)
            return;
          TPolicy instance;
          try
          {
            object settings = template.DeserializeSettings(this.CurrentConfiguration.Settings);
            instance = allPolicies.CreateInstance<TPolicy>(template);
            instance.Initialize(requestContext, this.CurrentConfiguration, settings);
          }
          catch (Exception ex)
          {
            if (!(ex is PolicyImplementationException implementationException1))
              implementationException1 = new PolicyImplementationException(template.GetType().Name, new int?(this.CurrentConfiguration.ConfigurationId), ex);
            PolicyImplementationException implementationException2 = implementationException1;
            requestContext.TraceException(1390054, "Policy", "PolicyService", (Exception) implementationException2);
            this.m_isBrokenBecauseItsConfigWillNotLoad = true;
            this.m_failedToLoadErrorCode = BrokenPolicyEvaluationRecordContext.Unknown;
            this.FailedToLoadReason = implementationException2.Message;
            return;
          }
          this.m_cacheService = cacheService;
          this.Policy = instance;
        }
      }
    }

    internal void CheckBypass(Lazy<bool> hasBypassPermissionOnTarget)
    {
      if ((object) this.Policy == null)
        return;
      this.m_isBypassable = this.Policy.IsBypassable && hasBypassPermissionOnTarget.Value;
    }

    internal void CheckScope(IVssRequestContext requestContext, ITeamFoundationPolicyTarget target)
    {
      if ((object) this.Policy == null)
        return;
      try
      {
        this.m_isEvaluationIrrelevantBecauseItIsOutOfScope = !this.GetValueWithCaching<bool>(requestContext, (Func<bool>) (() => this.Policy.IsApplicableTo(requestContext, target)), (Func<ActivePolicyEvaluationCacheItem, bool>) (cachedState => cachedState.IsApplicable), (Func<bool, ActivePolicyEvaluationCacheItem>) (newValue => new ActivePolicyEvaluationCacheItem(newValue)));
      }
      catch (Exception ex)
      {
        if (!(ex is PolicyImplementationException implementationException1))
          implementationException1 = new PolicyImplementationException(this.Policy.DisplayName, new int?(this.CurrentConfiguration.ConfigurationId), ex);
        PolicyImplementationException implementationException2 = implementationException1;
        if (!(ex is RequestCanceledException))
          requestContext.TraceException(1390051, "Policy", "PolicyService", (Exception) implementationException2);
        this.m_isBrokenBecauseApplicabilityCheckFailed = true;
        this.m_failedToLoadErrorCode = BrokenPolicyEvaluationRecordContext.Unknown;
        this.FailedToLoadReason = implementationException2.Message;
      }
    }

    internal void Notify(
      IVssRequestContext requestContext,
      Guid teamProjectId,
      Func<TPolicy, PolicyEvaluationStatus?, TeamFoundationPolicyEvaluationRecordContext, PolicyActionResult> action)
    {
      this.RunPolicy(requestContext, teamProjectId, false, action);
    }

    internal void Check(
      IVssRequestContext requestContext,
      Guid teamProjectId,
      Func<TPolicy, PolicyEvaluationStatus?, TeamFoundationPolicyEvaluationRecordContext, PolicyCheckResult> action)
    {
      this.RunPolicy(requestContext, teamProjectId, true, (Func<TPolicy, PolicyEvaluationStatus?, TeamFoundationPolicyEvaluationRecordContext, PolicyActionResult>) action);
    }

    private void RunPolicy(
      IVssRequestContext requestContext,
      Guid teamProjectId,
      bool isCheck,
      Func<TPolicy, PolicyEvaluationStatus?, TeamFoundationPolicyEvaluationRecordContext, PolicyActionResult> action)
    {
      using (requestContext.TimeRegion("Policy", "PolicyService", regionName: nameof (RunPolicy)))
      {
        if (this.IsBroken)
        {
          this.m_outgoingStatus = new PolicyEvaluationStatus?(PolicyEvaluationStatus.Broken);
          this.m_outgoingContext = (TeamFoundationPolicyEvaluationRecordContext) new BrokenPolicyEvaluationRecordContext(this.m_failedToLoadErrorCode);
          this.RejectionReason = this.FailedToLoadReason;
        }
        else if (!this.IsApplicable)
        {
          if (this.m_incomingStatus.HasValue)
          {
            PolicyEvaluationStatus? incomingStatus = this.m_incomingStatus;
            PolicyEvaluationStatus evaluationStatus = PolicyEvaluationStatus.NotApplicable;
            if (!(incomingStatus.GetValueOrDefault() == evaluationStatus & incomingStatus.HasValue))
              this.m_outgoingStatus = new PolicyEvaluationStatus?(PolicyEvaluationStatus.NotApplicable);
          }
          this.m_outgoingContext = (TeamFoundationPolicyEvaluationRecordContext) null;
        }
        else
        {
          if (this.m_rawIncomingContext != null)
          {
            PolicyEvaluationStatus? incomingStatus = this.m_incomingStatus;
            PolicyEvaluationStatus evaluationStatus = PolicyEvaluationStatus.Broken;
            if (!(incomingStatus.GetValueOrDefault() == evaluationStatus & incomingStatus.HasValue))
              this.m_incomingContext = ActivePolicyEvaluation<TPolicy>.SafeDeserializeContext(requestContext, teamProjectId, (ITeamFoundationPolicy) this.Policy, this.m_rawIncomingContext);
          }
          PolicyActionResult policyActionResult;
          try
          {
            bool hasValue = this.m_outgoingStatus.HasValue;
            policyActionResult = action(this.Policy, hasValue ? this.m_outgoingStatus : this.m_incomingStatus, hasValue ? this.m_outgoingContext : this.m_incomingContext);
          }
          catch (CustomPolicyException ex)
          {
            requestContext.Trace(1390107, TraceLevel.Warning, "Policy", "PolicyService", "The policy with ID '{0}' threw a custom exception '{1}'.", (object) this.Policy.Id, (object) ex.CustomErrorCode);
            policyActionResult = (PolicyActionResult) PolicyCheckResult.Broken("PolicyCustomErrorException", ex.CustomErrorCode);
            this.m_isBrokenBecauseItFailedToEvaluate = true;
          }
          catch (Exception ex)
          {
            PolicyImplementationException implementationException = new PolicyImplementationException(this.Policy.DisplayName, new int?(this.Policy.Configuration.ConfigurationId), ex);
            if (!(ex is RequestCanceledException))
              requestContext.TraceException(1390062, "Policy", "PolicyService", (Exception) implementationException);
            policyActionResult = (PolicyActionResult) PolicyCheckResult.Broken(implementationException.Message, BrokenPolicyEvaluationRecordContext.Unknown);
            this.m_isBrokenBecauseItFailedToEvaluate = true;
            this.m_failedToLoadErrorCode = BrokenPolicyEvaluationRecordContext.Unknown;
          }
          if (isCheck && policyActionResult == null)
            this.m_policyDoesntCareAboutThisCheck = true;
          if (policyActionResult == null)
          {
            if (!this.m_incomingStatus.HasValue)
            {
              this.m_outgoingStatus = new PolicyEvaluationStatus?(PolicyEvaluationStatus.Queued);
              this.m_outgoingContext = (TeamFoundationPolicyEvaluationRecordContext) null;
            }
            else
            {
              this.m_outgoingStatus = this.m_incomingStatus;
              this.m_outgoingContext = this.m_incomingContext;
            }
            this.RejectionReason = (string) null;
          }
          else
          {
            this.m_outgoingStatus = new PolicyEvaluationStatus?(policyActionResult.Status);
            this.m_outgoingContext = policyActionResult.Context;
            if (isCheck)
              this.RejectionReason = ((PolicyCheckResult) policyActionResult).RejectionReason;
            else
              this.RejectionReason = (string) null;
          }
        }
      }
    }

    internal PolicyEvaluationRecord GetNewPolicyEvaluationRecord(ArtifactId artifactId)
    {
      PolicyEvaluationRecord evaluationRecord = new PolicyEvaluationRecord();
      PolicyConfiguration policyConfiguration = new PolicyConfiguration();
      policyConfiguration.Id = this.CurrentConfiguration.ConfigurationId;
      policyConfiguration.Revision = this.CurrentConfiguration.ConfigurationRevisionId;
      evaluationRecord.Configuration = policyConfiguration;
      evaluationRecord.ArtifactId = LinkingUtilities.EncodeUri(artifactId);
      evaluationRecord.Status = this.m_outgoingStatus;
      evaluationRecord.Context = this.ConvertContextToJObject((object) this.m_outgoingContext);
      return evaluationRecord;
    }

    private static TeamFoundationPolicyEvaluationRecordContext SafeDeserializeContext(
      IVssRequestContext requestContext,
      Guid projectId,
      ITeamFoundationPolicy policy,
      JObject rawContext)
    {
      try
      {
        return policy.ParseContext(requestContext, projectId, rawContext);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1390047, "Policy", "PolicyService", (Exception) new PolicyImplementationException(policy.GetType().Name, new int?(policy.Configuration.ConfigurationId), ex));
      }
      return (TeamFoundationPolicyEvaluationRecordContext) null;
    }

    private JObject ConvertContextToJObject(object context)
    {
      JsonSerializer jsonSerializer = new JsonSerializer()
      {
        ContractResolver = (IContractResolver) new CamelCasePropertyNamesContractResolver()
      };
      return context != null ? JObject.FromObject(context, jsonSerializer) : (JObject) null;
    }

    private T GetValueWithCaching<T>(
      IVssRequestContext requestContext,
      Func<T> getValue,
      Func<ActivePolicyEvaluationCacheItem, T> mapFromCachedItem,
      Func<T, ActivePolicyEvaluationCacheItem> mapToCachedItem)
    {
      if (this.m_cacheService == null || this.m_cacheService.IsBypassed)
        return getValue();
      ActivePolicyEvaluationCacheItem evaluationCacheItem;
      if (this.m_cacheService.TryGet(requestContext, this.Policy.Configuration.ConfigurationId, out evaluationCacheItem))
        return mapFromCachedItem(evaluationCacheItem);
      T valueWithCaching = getValue();
      this.m_cacheService.Set(requestContext, this.Policy.Configuration.ConfigurationId, mapToCachedItem(valueWithCaching));
      return valueWithCaching;
    }
  }
}
