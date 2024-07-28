// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.ContributionService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  internal class ContributionService : 
    IVssFrameworkService,
    IContributionService,
    IContributionServiceWithData,
    IContributionFilterService
  {
    private const string s_contributionHierarchy = "contribution-hierarchy";
    private const string s_area = "ContributionService";
    private const string s_layer = "Contributions";
    private ContributionService.ContributedObjectCache m_contributedObjectCache;

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.m_contributedObjectCache = systemRequestContext.GetService<ContributionService.ContributedObjectCache>();

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public async Task<Stream> QueryAsset(
      IVssRequestContext requestContext,
      string contributionId,
      string assetType)
    {
      requestContext.Trace(100136276, TraceLevel.Info, nameof (ContributionService), "Contributions", "Querying for asset: contribution: {0}  assetType: {1}", (object) contributionId, (object) assetType);
      IContributionProvider contributionProvider = this.GetContributionHierarchy(requestContext).QueryContributionProvider(contributionId);
      Stream stream = (Stream) null;
      if (contributionProvider is IAssetProvider assetProvider)
      {
        requestContext.Trace(100136277, TraceLevel.Info, nameof (ContributionService), "Contributions", "QueryAsset: assetProvider found: {0}  assetType: {1}", (object) contributionId, (object) assetType);
        Task<Stream> task = assetProvider.QueryAsset(requestContext, assetType, out string _, out CompressionType _);
        if (task != null)
        {
          requestContext.Trace(100136279, TraceLevel.Info, nameof (ContributionService), "Contributions", "QueryAsset: StreamTask created: {0}  assetType: {1}", (object) contributionId, (object) assetType);
          stream = await task.ConfigureAwait(false);
        }
        else
          requestContext.Trace(100136280, TraceLevel.Error, nameof (ContributionService), "Contributions", "QueryAsset: StreamTask not created: {0}  assetType: {1}", (object) contributionId, (object) assetType);
      }
      else
        requestContext.Trace(100136278, TraceLevel.Info, nameof (ContributionService), "Contributions", "QueryAsset: assetProvider not found: {0}  assetType: {1}", (object) contributionId, (object) assetType);
      return stream;
    }

    public string QueryAssetLocation(
      IVssRequestContext requestContext,
      string contributionId,
      string assetType,
      string prefferedLocation = "Local")
    {
      requestContext.TraceEnter(10013460, nameof (ContributionService), "Contributions", nameof (QueryAssetLocation));
      try
      {
        string str = (string) null;
        Dictionary<string, string> dictionary = this.QueryAssetLocations(requestContext, contributionId, assetType);
        if (dictionary != null && !dictionary.TryGetValue(prefferedLocation, out str) && !dictionary.TryGetValue("Local", out str))
          str = (string) null;
        return str;
      }
      finally
      {
        requestContext.TraceLeave(10013460, nameof (ContributionService), "Contributions", "QueryContribution");
      }
    }

    public Dictionary<string, string> QueryAssetLocations(
      IVssRequestContext requestContext,
      string contributionId,
      string assetType)
    {
      requestContext.TraceEnter(10013460, nameof (ContributionService), "Contributions", nameof (QueryAssetLocations));
      try
      {
        IContributionProvider contributionProvider = this.GetContributionHierarchy(requestContext).QueryContributionProvider(contributionId);
        Dictionary<string, string> dictionary = (Dictionary<string, string>) null;
        requestContext.Trace(100136270, TraceLevel.Info, nameof (ContributionService), "Contributions", "Querying for asset locations for contribution: {0}  assetType: {1}", (object) contributionId, (object) assetType);
        if (contributionProvider is IAssetProvider assetProvider)
        {
          requestContext.Trace(100136271, TraceLevel.Info, nameof (ContributionService), "Contributions", "Asset provider found for contribution: {0}  assetType: {1}", (object) contributionId, (object) assetType);
          dictionary = assetProvider.QueryAssetLocations(requestContext, assetType);
        }
        else
          requestContext.Trace(100136272, TraceLevel.Info, nameof (ContributionService), "Contributions", "Asset provider not found for contribution: {0}  assetType: {1}", (object) contributionId, (object) assetType);
        return dictionary;
      }
      finally
      {
        requestContext.TraceLeave(10013460, nameof (ContributionService), "Contributions", nameof (QueryAssetLocations));
      }
    }

    public Contribution QueryContribution(IVssRequestContext requestContext, string contributionId)
    {
      requestContext.TraceEnter(10013460, nameof (ContributionService), "Contributions", nameof (QueryContribution));
      try
      {
        return this.GetContributionHierarchy(requestContext).QueryContribution(contributionId);
      }
      finally
      {
        requestContext.TraceLeave(10013460, nameof (ContributionService), "Contributions", nameof (QueryContribution));
      }
    }

    public bool QueryContribution<T>(
      IVssRequestContext requestContext,
      string contributionId,
      string associatedDataName,
      out Contribution contribution,
      out T associatedData)
    {
      bool flag = false;
      if ((contribution = this.QueryContribution(requestContext, contributionId)) != null)
      {
        string associatedDataKey = this.GetAssociatedDataKey(requestContext, associatedDataName, contribution.GetHashCode());
        flag = this.GetAssociatedData<T>(requestContext, associatedDataKey, out associatedData);
      }
      else
        associatedData = default (T);
      return flag;
    }

    public IEnumerable<Contribution> QueryContributionsForChild(
      IVssRequestContext requestContext,
      string contributionId)
    {
      requestContext.TraceEnter(10013461, nameof (ContributionService), "Contributions", nameof (QueryContributionsForChild));
      try
      {
        return this.GetContributionHierarchy(requestContext).QueryContributionsForChild(contributionId);
      }
      finally
      {
        requestContext.TraceLeave(10013461, nameof (ContributionService), "Contributions", nameof (QueryContributionsForChild));
      }
    }

    public IEnumerable<Contribution> QueryContributionsForTarget(
      IVssRequestContext requestContext,
      string contributionId)
    {
      requestContext.TraceEnter(10013461, nameof (ContributionService), "Contributions", nameof (QueryContributionsForTarget));
      try
      {
        return this.GetContributionHierarchy(requestContext).QueryContributionsForTarget(contributionId);
      }
      finally
      {
        requestContext.TraceLeave(10013461, nameof (ContributionService), "Contributions", nameof (QueryContributionsForTarget));
      }
    }

    public bool QueryContributionsForTarget<T>(
      IVssRequestContext requestContext,
      string contributionId,
      string associatedDataName,
      out IEnumerable<Contribution> contributions,
      out T associatedData)
    {
      bool flag = false;
      if ((contributions = this.QueryContributionsForTarget(requestContext, contributionId)) != null)
      {
        string associatedDataKey = this.GetAssociatedDataKey(requestContext, associatedDataName, this.GetContributionHashCode(requestContext, contributions));
        flag = this.GetAssociatedData<T>(requestContext, associatedDataKey, out associatedData);
      }
      else
        associatedData = default (T);
      return flag;
    }

    public IEnumerable<Contribution> QueryContributionsForType(
      IVssRequestContext requestContext,
      string contributionTypeId)
    {
      requestContext.TraceEnter(10013462, nameof (ContributionService), "Contributions", nameof (QueryContributionsForType));
      try
      {
        return this.GetContributionHierarchy(requestContext).QueryContributionsForType(contributionTypeId);
      }
      finally
      {
        requestContext.TraceLeave(10013462, nameof (ContributionService), "Contributions", nameof (QueryContributionsForType));
      }
    }

    public bool QueryContributionsForType<T>(
      IVssRequestContext requestContext,
      string contributionType,
      string associatedDataName,
      out IEnumerable<Contribution> contributions,
      out T associatedData)
    {
      bool flag = false;
      if ((contributions = this.QueryContributionsForType(requestContext, contributionType)) != null)
      {
        string associatedDataKey = this.GetAssociatedDataKey(requestContext, associatedDataName, this.GetContributionHashCode(requestContext, contributions));
        flag = this.GetAssociatedData<T>(requestContext, associatedDataKey, out associatedData);
      }
      else
        associatedData = default (T);
      return flag;
    }

    public IEnumerable<Contribution> QueryContributions(
      IVssRequestContext requestContext,
      IEnumerable<string> contributionIds = null,
      HashSet<string> allowedContributionTypes = null,
      ContributionQueryOptions queryOptions = ContributionQueryOptions.IncludeSelf,
      ContributionQueryCallback queryCallback = null,
      ContributionDiagnostics diagnostics = null)
    {
      requestContext.TraceEnter(10013463, nameof (ContributionService), "Contributions", nameof (QueryContributions));
      try
      {
        ContributionHierarchy contributionHierarchy = this.GetContributionHierarchy(requestContext);
        HashSet<string> userClaims = this.GetUserClaims(requestContext);
        Func<Contribution, Contribution, string, ContributionQueryOptions, ICollection<EvaluatedCondition>, ContributionQueryOptions> filterCallback = (Func<Contribution, Contribution, string, ContributionQueryOptions, ICollection<EvaluatedCondition>, ContributionQueryOptions>) ((contribution, parentContribution, relationship, contributionOptions, evaluatedConditions) =>
        {
          if (queryCallback != null)
            contributionOptions = queryCallback(requestContext, contribution, parentContribution, relationship, contributionOptions, evaluatedConditions);
          if (contributionOptions != ContributionQueryOptions.None && (queryOptions & ContributionQueryOptions.IgnoreConstraints) == ContributionQueryOptions.None)
          {
            ContributionConstraintSet contributionConstraints = contributionHierarchy.GetContributionConstraints(contribution.Id);
            if (contributionConstraints != null && !contributionConstraints.Evaluate(requestContext, contribution, relationship, evaluatedConditions, userClaims))
              contributionOptions = ContributionQueryOptions.None;
          }
          return contributionOptions;
        });
        return contributionHierarchy.QueryContributions(contributionIds, allowedContributionTypes, queryOptions, filterCallback, diagnostics);
      }
      finally
      {
        requestContext.TraceLeave(10013463, nameof (ContributionService), "Contributions", nameof (QueryContributions));
      }
    }

    public bool QueryContributions<T>(
      IVssRequestContext requestContext,
      IEnumerable<string> contributionIds,
      HashSet<string> contributionTypes,
      ContributionQueryOptions queryOptions,
      ContributionQueryCallback queryCallback,
      ContributionDiagnostics diagnostics,
      string associatedDataName,
      out IEnumerable<Contribution> contributions,
      out T associatedData)
    {
      bool flag = false;
      if ((contributions = this.QueryContributions(requestContext, contributionIds, contributionTypes, queryOptions, queryCallback, (ContributionDiagnostics) null)) != null)
      {
        string associatedDataKey = this.GetAssociatedDataKey(requestContext, associatedDataName, this.GetContributionHashCode(requestContext, contributions));
        flag = this.GetAssociatedData<T>(requestContext, associatedDataKey, out associatedData);
      }
      else
        associatedData = default (T);
      return flag;
    }

    public IDictionary<string, ContributionNode> QueryContributions(
      IVssRequestContext requestContext,
      IEnumerable<string> contributionIds,
      ContributionQueryOptions queryOptions,
      ContributionQueryCallback queryCallback,
      ContributionDiagnostics diagnostics = null)
    {
      requestContext.TraceEnter(10013463, nameof (ContributionService), "Contributions", nameof (QueryContributions));
      try
      {
        ContributionHierarchy contributionHierarchy = this.GetContributionHierarchy(requestContext);
        HashSet<string> userClaims = (queryOptions & ContributionQueryOptions.IgnoreConstraints) != ContributionQueryOptions.None ? new HashSet<string>() : this.GetUserClaims(requestContext);
        queryOptions |= ContributionQueryOptions.IncludeAll;
        Func<Contribution, Contribution, string, ContributionQueryOptions, ICollection<EvaluatedCondition>, ContributionQueryOptions> filterCallback = (Func<Contribution, Contribution, string, ContributionQueryOptions, ICollection<EvaluatedCondition>, ContributionQueryOptions>) ((contribution, parentContribution, relationship, contributionOptions, evaluatedConditions) =>
        {
          contributionOptions = queryCallback(requestContext, contribution, parentContribution, relationship, contributionOptions, evaluatedConditions);
          if (contributionOptions != ContributionQueryOptions.None && (queryOptions & ContributionQueryOptions.IgnoreConstraints) == ContributionQueryOptions.None)
          {
            ContributionConstraintSet contributionConstraints = contributionHierarchy.GetContributionConstraints(contribution.Id);
            if (contributionConstraints != null && !contributionConstraints.Evaluate(requestContext, contribution, relationship, evaluatedConditions, userClaims))
              contributionOptions = ContributionQueryOptions.None;
          }
          return contributionOptions;
        });
        return contributionHierarchy.QueryContributions(contributionIds, filterCallback, diagnostics);
      }
      finally
      {
        requestContext.TraceLeave(10013463, nameof (ContributionService), "Contributions", nameof (QueryContributions));
      }
    }

    public bool QueryContributions<T>(
      IVssRequestContext requestContext,
      IEnumerable<string> contributionIds,
      ContributionQueryOptions queryOptions,
      ContributionQueryCallback queryCallback,
      ContributionDiagnostics diagnostics,
      string associatedDataName,
      out IDictionary<string, ContributionNode> contributionNodes,
      out T associatedData)
    {
      bool flag = false;
      if ((contributionNodes = this.QueryContributions(requestContext, contributionIds, queryOptions, queryCallback, diagnostics)) != null)
      {
        string associatedDataKey = this.GetAssociatedDataKey(requestContext, associatedDataName, this.GetContributionHashCode(requestContext, (IEnumerable<ContributionNode>) contributionNodes.Values));
        flag = this.GetAssociatedData<T>(requestContext, associatedDataKey, out associatedData);
      }
      else
        associatedData = default (T);
      return flag;
    }

    public ContributionProviderDetails QueryContributionProviderDetails(
      IVssRequestContext requestContext,
      string contributionId)
    {
      ContributionProviderDetails contributionProviderDetails = (ContributionProviderDetails) null;
      IContributionProvider contributionProvider = this.GetContributionHierarchy(requestContext).QueryContributionProvider(contributionId);
      if (contributionProvider != null)
        contributionProviderDetails = contributionProvider.QueryProviderDetails(requestContext);
      return contributionProviderDetails;
    }

    public void Set(
      IVssRequestContext requestContext,
      string associatedDataName,
      IEnumerable<Contribution> contributions,
      object associatedData)
    {
      string associatedDataKey = this.GetAssociatedDataKey(requestContext, associatedDataName, this.GetContributionHashCode(requestContext, contributions));
      this.m_contributedObjectCache.Set(requestContext, associatedDataKey, associatedData);
    }

    public void Set(
      IVssRequestContext requestContext,
      string associatedDataName,
      IEnumerable<ContributionNode> contributionNodes,
      object associatedData)
    {
      string associatedDataKey = this.GetAssociatedDataKey(requestContext, associatedDataName, this.GetContributionHashCode(requestContext, contributionNodes));
      this.m_contributedObjectCache.Set(requestContext, associatedDataKey, associatedData);
    }

    public bool ApplyConstraints(
      IVssRequestContext requestContext,
      Contribution contribution,
      string phase,
      string relationship,
      ICollection<EvaluatedCondition> evaluatedConditions,
      ConstraintEvaluationOptions options = ConstraintEvaluationOptions.None,
      Func<ContributionConstraint, Contribution, bool?> evaluationCallback = null)
    {
      ContributionConstraintSet contributionConstraints = this.GetContributionHierarchy(requestContext).GetContributionConstraints(contribution.Id);
      HashSet<string> userClaims1 = (options & ConstraintEvaluationOptions.IgnoreRestrictedTo) == ConstraintEvaluationOptions.IgnoreRestrictedTo ? (HashSet<string>) null : this.GetUserClaims(requestContext);
      IVssRequestContext requestContext1 = requestContext;
      Contribution contribution1 = contribution;
      string phase1 = phase;
      string relationship1 = relationship;
      ICollection<EvaluatedCondition> evaluatedConditions1 = evaluatedConditions;
      HashSet<string> userClaims2 = userClaims1;
      Func<ContributionConstraint, Contribution, bool?> evaluationCallback1 = evaluationCallback;
      return contributionConstraints.Evaluate(requestContext1, contribution1, phase1, relationship1, evaluatedConditions1, userClaims2, evaluationCallback1);
    }

    private HashSet<string> GetUserClaims(IVssRequestContext requestContext) => requestContext.GetService<IContributionClaimService>().GetClaims(requestContext);

    private ContributionHierarchy GetContributionHierarchy(IVssRequestContext requestContext)
    {
      ContributionHierarchy contributionHierarchy;
      if (!requestContext.TryGetItem<ContributionHierarchy>("contribution-hierarchy", out contributionHierarchy))
      {
        IDisposableReadOnlyList<IContributionSource> extensions = requestContext.GetExtensions<IContributionSource>(ExtensionLifetime.Service);
        List<IContributionSource> contributionSourceList = new List<IContributionSource>();
        contributionSourceList.AddRange((IEnumerable<IContributionSource>) extensions.OrderBy<IContributionSource, int>((Func<IContributionSource, int>) (c => c.Priority)));
        Dictionary<string, IContributionProvider> dictionary = new Dictionary<string, IContributionProvider>();
        foreach (IContributionSource contributionSource in contributionSourceList)
        {
          foreach (IContributionProvider queryProvider in contributionSource.QueryProviders(requestContext))
            dictionary[queryProvider.ProviderName] = queryProvider;
        }
        List<IContributionProvider> list = dictionary.Values.OrderBy<IContributionProvider, string>((Func<IContributionProvider, string>) (p => p.ProviderName)).ToList<IContributionProvider>();
        int num = 0;
        foreach (IContributionProvider contributionProvider in list)
        {
          ContributionData contributionData = contributionProvider.QueryContributionData(requestContext);
          num ^= contributionData.GetHashCode();
          ContributionProviderDetails contributionProviderDetails = contributionProvider.QueryProviderDetails(requestContext);
          if (contributionProviderDetails != null)
            num ^= contributionProviderDetails.GetHashCode();
        }
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        ContributionService.ContributionHierarchyCache service = vssRequestContext.GetService<ContributionService.ContributionHierarchyCache>();
        if (!service.TryGetValue(vssRequestContext, num.ToString(), out contributionHierarchy))
        {
          contributionHierarchy = new ContributionHierarchy((IEnumerable<IContributionProvider>) list);
          service.TryAdd(vssRequestContext, num.ToString(), contributionHierarchy);
          CustomerIntelligenceData properties = new CustomerIntelligenceData();
          properties.Add("contributionProviders", list.Select<IContributionProvider, ContributionProviderDetails>((Func<IContributionProvider, ContributionProviderDetails>) (p => p.QueryProviderDetails(requestContext))).Where<ContributionProviderDetails>((Func<ContributionProviderDetails, bool>) (d => d != null)).Select<ContributionProviderDetails, string>((Func<ContributionProviderDetails, string>) (d => d.Name + ":" + d.Version)).ToList<string>());
          properties.Add("locale", Thread.CurrentThread.CurrentUICulture.Name);
          properties.Add("hashCode", (double) num);
          properties.Add("contributionCount", (double) contributionHierarchy.Contributions.Count<Contribution>());
          requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, nameof (ContributionService), "ContributionHierarchy.Create", properties);
        }
        requestContext.Items["contribution-hierarchy"] = (object) contributionHierarchy;
      }
      return contributionHierarchy;
    }

    private bool GetAssociatedData<T>(
      IVssRequestContext requestContext,
      string associatedDataKey,
      out T associatedData)
    {
      associatedData = default (T);
      object obj1;
      bool associatedData1 = this.m_contributedObjectCache.TryGetValue(requestContext, associatedDataKey, out obj1);
      if (associatedData1)
      {
        if (obj1 is T obj2)
          associatedData = obj2;
        else
          associatedData1 = false;
      }
      return associatedData1;
    }

    private string GetAssociatedDataKey(
      IVssRequestContext requestContext,
      string associatedDataName,
      int contributionHashCode)
    {
      return associatedDataName + ":" + contributionHashCode.ToString();
    }

    private int GetContributionHashCode(
      IVssRequestContext requestContext,
      IEnumerable<Contribution> contributions)
    {
      int contributionHashCode = 0;
      foreach (Contribution contribution in contributions)
        contributionHashCode ^= contribution.GetHashCode();
      return contributionHashCode;
    }

    private int GetContributionHashCode(
      IVssRequestContext requestContext,
      IEnumerable<ContributionNode> contributionNodes)
    {
      int contributionHashCode = 0;
      foreach (ContributionNode contributionNode in contributionNodes)
        contributionHashCode ^= contributionNode.Contribution.GetHashCode();
      return contributionHashCode;
    }

    internal class ContributedObjectCache : VssMemoryCacheService<string, object>
    {
      private static readonly TimeSpan s_inactivityInterval = new TimeSpan(0, 15, 0);
      private static readonly TimeSpan s_cacheCleanupInterval = new TimeSpan(0, 5, 0);

      public ContributedObjectCache()
        : base(ContributionService.ContributedObjectCache.s_cacheCleanupInterval)
      {
        this.InactivityInterval.Value = ContributionService.ContributedObjectCache.s_inactivityInterval;
      }

      protected override void ServiceStart(IVssRequestContext requestContext)
      {
        base.ServiceStart(requestContext);
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        vssRequestContext.GetService<CachedRegistryService>().RegisterNotification(vssRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), "/Configuration/WebAccess/CDN/...");
      }

      protected override void ServiceEnd(IVssRequestContext requestContext)
      {
        base.ServiceEnd(requestContext);
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        vssRequestContext.GetService<CachedRegistryService>().UnregisterNotification(vssRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));
      }

      private void OnRegistryChanged(
        IVssRequestContext requestContext,
        RegistryEntryCollection changedEntries)
      {
        this.Clear(requestContext);
      }
    }

    internal class ContributionHierarchyCache : VssMemoryCacheService<string, ContributionHierarchy>
    {
      private static readonly TimeSpan s_inactivityInterval = new TimeSpan(0, 15, 0);
      private static readonly TimeSpan s_cacheCleanupInterval = new TimeSpan(0, 5, 0);

      public ContributionHierarchyCache()
        : base(ContributionService.ContributionHierarchyCache.s_cacheCleanupInterval)
      {
        this.InactivityInterval.Value = ContributionService.ContributionHierarchyCache.s_inactivityInterval;
      }

      protected override void ServiceStart(IVssRequestContext requestContext)
      {
        base.ServiceStart(requestContext);
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
          throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      }

      protected override void ServiceEnd(IVssRequestContext requestContext) => base.ServiceEnd(requestContext);
    }
  }
}
