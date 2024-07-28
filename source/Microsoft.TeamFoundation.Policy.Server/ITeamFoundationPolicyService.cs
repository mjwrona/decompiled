// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.ITeamFoundationPolicyService
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Policy.Server.Framework;
using Microsoft.TeamFoundation.Policy.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Policy.Server
{
  [DefaultServiceImplementation(typeof (TeamFoundationPolicyService))]
  public interface ITeamFoundationPolicyService : IVssFrameworkService
  {
    IReadOnlyList<T> GetApplicablePolicies<T>(
      IVssRequestContext requestContext,
      ITeamFoundationPolicyTarget target,
      out List<PolicyFailures> failedPolicies,
      bool isBlockingOnly = false,
      bool includeHidden = false)
      where T : class, ITeamFoundationPolicy;

    PolicyEvaluationRecord GetPolicyEvaluationRecord(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid policyEvaluationId,
      bool throwIfNotApplicable = true,
      bool throwIfNotFound = true);

    void RequeuePolicyEvaluationRecord(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid policyEvaluationId);

    IEnumerable<PolicyEvaluationRecord> GetPolicyEvaluationRecords(
      IVssRequestContext requestContext,
      Guid projectId,
      ArtifactId artifactId,
      bool includeNotApplicable = false,
      int? top = null,
      int? skip = null,
      IActivePolicyEvaluationCache policyEvaluationCache = null);

    void NotifyPoliciesOfNewArtifact<TPolicy>(
      IVssRequestContext requestContext,
      ITeamFoundationPolicyTarget target,
      ArtifactId artifactId,
      Func<TPolicy, PolicyNotificationResult> action)
      where TPolicy : class, ITeamFoundationPolicy;

    void NotifyPolicies<TPolicy>(
      IVssRequestContext requestContext,
      ITeamFoundationPolicyTarget target,
      ArtifactId artifactId,
      Func<TPolicy, PolicyEvaluationStatus?, TeamFoundationPolicyEvaluationRecordContext, PolicyNotificationResult> action,
      ClientTraceData ctData = null)
      where TPolicy : class, ITeamFoundationPolicy;

    void NotifyPolicy<TPolicy>(
      IVssRequestContext requestContext,
      ITeamFoundationPolicyTarget target,
      ArtifactId artifactId,
      int policyConfigurationId,
      Func<TPolicy, PolicyEvaluationStatus?, TeamFoundationPolicyEvaluationRecordContext, PolicyNotificationResult> action,
      ClientTraceData ctData = null)
      where TPolicy : class, ITeamFoundationPolicy;

    PolicyEvaluationTransaction<TPolicy> CheckPolicies<TPolicy>(
      IVssRequestContext requestContext,
      ITeamFoundationPolicyTarget target,
      ArtifactId artifactId,
      out PolicyEvaluationResult result,
      Func<TPolicy, PolicyEvaluationStatus?, TeamFoundationPolicyEvaluationRecordContext, PolicyCheckResult> action,
      IActivePolicyEvaluationCache policyEvaluationCacheService = null)
      where TPolicy : class, ITeamFoundationPolicy;

    PolicyEvaluationResult CheckPolicies<TPolicy>(
      IVssRequestContext requestContext,
      ITeamFoundationPolicyTarget target,
      Func<TPolicy, PolicyCheckResult> action)
      where TPolicy : class, ITeamFoundationPolicy;

    PolicyConfigurationRecord GetPolicyConfigurationRecord(
      IVssRequestContext requestContext,
      Guid projectId,
      int configurationId,
      int? revisionId = null);

    PolicyConfigurationRecord GetLatestPolicyConfigurationRecord(
      IVssRequestContext requestContext,
      Guid projectId,
      int configurationId);

    IEnumerable<PolicyConfigurationRecord> GetPolicyConfigurationRecordRevisions(
      IVssRequestContext requestContext,
      Guid projectId,
      int configurationId,
      int? top = null,
      int? skip = null);

    IEnumerable<PolicyConfigurationRecord> GetLatestPolicyConfigurationRecords(
      IVssRequestContext requestContext,
      Guid projectId,
      int top,
      int firstConfigurationId,
      out int? nextConfigurationId,
      Guid? policyType = null);

    IEnumerable<PolicyConfigurationRecord> GetLatestPolicyConfigurationRecordsByScope(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<string> scopes,
      int top,
      int firstConfigurationId,
      out int? nextConfigurationId,
      Guid? policyType = null);

    IDictionary<string, int> GetPolicyConfigurationsCountByScope(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<string> scopes);

    ITeamFoundationPolicy GetPolicyType(IVssRequestContext requestContext, Guid typeId);

    IEnumerable<ITeamFoundationPolicy> GetPolicyTypes(IVssRequestContext requestContext);

    PolicyConfigurationRecord CreatePolicyConfiguration(
      IVssRequestContext requestContext,
      Guid typeId,
      Guid projectId,
      bool isEnabled,
      bool isBlocking,
      bool isEnterpriseManaged,
      string settings);

    PolicyConfigurationRecord UpdatePolicyConfiguration(
      IVssRequestContext requestContext,
      int configurationId,
      Guid typeId,
      Guid projectId,
      bool isEnabled,
      bool isBlocking,
      bool isEnterpriseManaged,
      string settings);

    void DeletePolicyConfiguration(
      IVssRequestContext requestContext,
      Guid projectId,
      int policyConfigurationId);

    string[] DetermineScopes(
      IVssRequestContext requestContext,
      PolicyConfigurationRecord configRecord);
  }
}
