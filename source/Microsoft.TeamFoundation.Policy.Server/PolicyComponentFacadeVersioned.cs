// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.PolicyComponentFacadeVersioned
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Policy.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Policy.Server
{
  internal class PolicyComponentFacadeVersioned : IPolicyComponent, IDisposable
  {
    private IPolicyComponent m_policyComponent;
    private IPolicyConfigurationVersionedCacheService m_cacheService;
    private IVssRequestContext m_requestContext;

    public PolicyComponentFacadeVersioned(
      IVssRequestContext requestContext,
      IPolicyConfigurationVersionedCacheService cacheService)
    {
      this.m_requestContext = requestContext;
      this.m_policyComponent = (IPolicyComponent) requestContext.CreateComponent<PolicyComponent>();
      this.m_cacheService = cacheService;
    }

    internal PolicyComponentFacadeVersioned(
      IVssRequestContext requestContext,
      IPolicyComponent policyComponent,
      IPolicyConfigurationVersionedCacheService cacheService)
    {
      this.m_requestContext = requestContext;
      this.m_policyComponent = policyComponent;
      this.m_cacheService = cacheService;
    }

    public PolicyConfigurationRecord CreatePolicyConfiguration(
      PolicyConfigurationRecord newConfiguration,
      Func<PolicyConfigurationRecord, IEnumerable<string>> determineScopes)
    {
      PolicyConfigurationRecord policyConfiguration = this.m_policyComponent.CreatePolicyConfiguration(newConfiguration, determineScopes);
      this.m_cacheService.Remove(this.m_requestContext, this.m_policyComponent.GetDataspaceId(newConfiguration.ProjectId));
      return policyConfiguration;
    }

    public PolicyConfigurationRecord DeletePolicyConfiguration(
      Guid projectId,
      int policyConfigurationId,
      Guid modifiedById,
      Func<PolicyConfigurationRecord, IEnumerable<string>> determineScopes,
      IEnumerable<string> scopes,
      int expectedRevisionId)
    {
      PolicyConfigurationRecord configurationRecord = this.m_policyComponent.DeletePolicyConfiguration(projectId, policyConfigurationId, modifiedById, determineScopes, scopes, expectedRevisionId);
      this.m_cacheService.Remove(this.m_requestContext, this.m_policyComponent.GetDataspaceId(projectId));
      return configurationRecord;
    }

    public IList<PolicyConfigurationRecord> GetLatestPolicyConfigurations(
      Guid projectId,
      int top,
      int firstConfigurationId,
      Guid? policyType = null)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      int dataspaceId = this.m_policyComponent.GetDataspaceId(projectId);
      IList<PolicyConfigurationRecord> policyConfigurations;
      if (top == int.MaxValue && firstConfigurationId == 1 && !policyType.HasValue)
      {
        policyConfigurations = this.m_cacheService.Get(this.m_requestContext, dataspaceId, (Func<IList<PolicyConfigurationRecord>>) (() => this.m_policyComponent.GetLatestPolicyConfigurations(projectId, top, firstConfigurationId)));
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Policy.Server.VersionedCacheQueriesPerSec").Increment();
        VssPerformanceCounter performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Policy.Server.AverageVersionedCacheQueryTime");
        performanceCounter.IncrementMilliseconds(stopwatch.ElapsedMilliseconds);
        performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Policy.Server.AverageVersionedCacheQueryTimeBase");
        performanceCounter.Increment();
      }
      else
      {
        policyConfigurations = this.m_policyComponent.GetLatestPolicyConfigurations(projectId, top, firstConfigurationId, policyType);
        VssPerformanceCounter performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Policy.Server.PaginatedProjectQueriesPerSec");
        performanceCounter.Increment();
        performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Policy.Server.AveragePaginatedProjectQueryTime");
        performanceCounter.IncrementMilliseconds(stopwatch.ElapsedMilliseconds);
        performanceCounter = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Policy.Server.AveragePaginatedProjectQueryTimeBase");
        performanceCounter.Increment();
      }
      return policyConfigurations;
    }

    public PolicyConfigurationRecord GetPolicyConfiguration(
      Guid projectId,
      int configurationId,
      int? revisionId)
    {
      return revisionId.HasValue ? this.m_policyComponent.GetPolicyConfiguration(projectId, configurationId, revisionId) : this.GetLatestPolicyConfiguration(projectId, configurationId);
    }

    public PolicyConfigurationRecord GetLatestPolicyConfiguration(
      Guid projectId,
      int configurationId)
    {
      return this.GetLatestPolicyConfigurations(projectId, int.MaxValue, 1, new Guid?()).FirstOrDefault<PolicyConfigurationRecord>((Func<PolicyConfigurationRecord, bool>) (r => r.ConfigurationId == configurationId)) ?? this.m_policyComponent.GetPolicyConfiguration(projectId, configurationId, new int?());
    }

    public PolicyConfigurationRecord UpdatePolicyConfiguration(
      PolicyConfigurationRecord updatedConfiguration,
      Func<PolicyConfigurationRecord, IEnumerable<string>> determineScopes,
      IEnumerable<string> scopes,
      int expectedRevisionId)
    {
      PolicyConfigurationRecord configurationRecord = this.m_policyComponent.UpdatePolicyConfiguration(updatedConfiguration, determineScopes, scopes, expectedRevisionId);
      this.m_cacheService.Remove(this.m_requestContext, this.m_policyComponent.GetDataspaceId(updatedConfiguration.ProjectId));
      return configurationRecord;
    }

    public PolicyEvaluationRecord GetPolicyEvaluationRecord(Guid projectId, Guid evaluationRecordId) => this.m_policyComponent.GetPolicyEvaluationRecord(projectId, evaluationRecordId);

    public VirtualResultCollection<PolicyEvaluationRecord> GetPolicyEvaluationRecords(
      Guid projectId,
      int? policyConfigurationId,
      int? policyConfigurationRevision,
      ArtifactId artifactId,
      bool includeNotApplicable,
      int top,
      int skip)
    {
      return this.m_policyComponent.GetPolicyEvaluationRecords(projectId, policyConfigurationId, policyConfigurationRevision, artifactId, includeNotApplicable, top, skip);
    }

    public void UpdatePolicyEvaluationRecords(
      Guid projectId,
      ArtifactId artifact,
      IEnumerable<PolicyEvaluationRecord> updatedRecords,
      IEnumerable<int> idsOfRecordsToDelete)
    {
      this.m_policyComponent.UpdatePolicyEvaluationRecords(projectId, artifact, updatedRecords, idsOfRecordsToDelete);
    }

    public VirtualResultCollection<PolicyConfigurationRecord> GetPolicyConfigurationRevisions(
      Guid projectId,
      int configurationId,
      int top,
      int skip)
    {
      return this.m_policyComponent.GetPolicyConfigurationRevisions(projectId, configurationId, top, skip);
    }

    public IList<PolicyConfigurationRecord> GetLatestPolicyConfigurationsByScope(
      Guid projectId,
      IEnumerable<string> scopes,
      Func<PolicyConfigurationRecord, IEnumerable<string>> determineScopes,
      int top,
      int firstConfigurationId,
      Guid? policyType = null,
      bool includeHidden = false,
      bool useVersion2 = false)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      IList<PolicyConfigurationRecord> collection = (IList<PolicyConfigurationRecord>) new List<PolicyConfigurationRecord>();
      if (includeHidden)
        collection.Add(this.GetGitCommitHardLimitsPolicy(projectId));
      collection.AddRange<PolicyConfigurationRecord, IList<PolicyConfigurationRecord>>((IEnumerable<PolicyConfigurationRecord>) this.m_policyComponent.GetLatestPolicyConfigurationsByScope(projectId, scopes, determineScopes, top, firstConfigurationId, policyType, useVersion2: useVersion2));
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Policy.Server.ScopedQueriesPerSec").Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Policy.Server.AverageScopedQueryTime").IncrementMilliseconds(stopwatch.ElapsedMilliseconds);
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Policy.Server.AverageScopedQueryTimeBase").Increment();
      return collection;
    }

    public IDictionary<string, int> GetPolicyConfigurationsCountByScope(
      Guid projectId,
      IEnumerable<string> scopes)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      IDictionary<string, int> configurationsCountByScope = this.m_policyComponent.GetPolicyConfigurationsCountByScope(projectId, scopes);
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Policy.Server.QueryCountsByScopePerSec").Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Policy.Server.AverageQueryCountsByScopeTime").IncrementMilliseconds(stopwatch.ElapsedMilliseconds);
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Policy.Server.AverageQueryCountsByScopeTimeBase").Increment();
      return configurationsCountByScope;
    }

    public int GetDataspaceId(Guid dataspaceIdentifier) => this.m_policyComponent.GetDataspaceId(dataspaceIdentifier);

    private PolicyConfigurationRecord GetGitCommitHardLimitsPolicy(Guid projectId) => new PolicyConfigurationRecord(new Guid("E2A4CFBF-0619-4DFB-B0E1-B1134CF3A128"), projectId, true, true, false, "{\"scope\":[{\"repositoryId\":null}]}", Guid.Empty);

    public void Dispose() => this.m_policyComponent.Dispose();
  }
}
