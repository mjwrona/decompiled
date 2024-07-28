// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions.ContributionHierarchyControllerBase
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Performance;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions
{
  public abstract class ContributionHierarchyControllerBase : TfsApiController
  {
    private static string[] s_wellKnownProviders = new string[2]
    {
      "ms.vss-web.component-data",
      "ms.vss-web.shared-data"
    };
    private const string c_dataProviderScopeDataKey = "DataProviderQuery.Scope";

    public override string TraceArea => "Contribution";

    public override string ActivityLogArea => "Framework";

    protected ContributionHierarchyControllerBase.ContributedHierarchy GetContributedHierarchy(
      IEnumerable<string> contributionIds,
      DataProviderContext dataProviderContext,
      string scopeName = null,
      string scopeValue = null)
    {
      if (contributionIds != null)
        this.TfsRequestContext.TraceConditionally(10013590, TraceLevel.Info, this.TraceArea, WebPlatformTraceLayers.Controller, (Func<string>) (() => "Loading hierarchies for following contributions: '" + string.Join(",", contributionIds) + "'."));
      IExtensionDataProviderService service1 = this.TfsRequestContext.GetService<IExtensionDataProviderService>();
      service1.SetRequestDataProviderContext(this.TfsRequestContext, (IDictionary<string, object>) dataProviderContext?.Properties);
      ContributionHierarchyControllerBase.ContributedHierarchy metadata = new ContributionHierarchyControllerBase.ContributedHierarchy();
      IWebDiagnosticsService service2 = this.TfsRequestContext.GetService<IWebDiagnosticsService>();
      using (PerformanceTimer.StartMeasure(this.TfsRequestContext, "ContributionHierarchyController"))
      {
        IContributionManagementService service3 = this.TfsRequestContext.GetService<IContributionManagementService>();
        using (PerformanceTimer.StartMeasure(this.TfsRequestContext, "ComputeRequestContributions"))
          service3.ComputeRequestContributions(this.TfsRequestContext, contributionIds, ContributionQueryOptions.IncludeAll, (ContributionQueryCallback) ((requestContext, contribution, parentContribution, relationship, contributionOptions, evaluatedConditions) => contributionOptions));
        using (PerformanceTimer.StartMeasure(this.TfsRequestContext, "ProcessContent"))
        {
          IContentService service4 = this.TfsRequestContext.GetService<IContentService>();
          string preferredLocation = service2.IsCdnEnabled(this.TfsRequestContext) ? "Cdn" : "Local";
          string requestContentType = service4.GetRequestContentType(this.TfsRequestContext);
          string requestScriptType = service4.GetRequestScriptType(this.TfsRequestContext);
          string requestStyleType = service4.GetRequestStyleType(this.TfsRequestContext);
          IEnumerable<ContributedContent> contributedContents = service4.QueryContent(this.TfsRequestContext);
          List<ContentSource> contentSourceList = new List<ContentSource>();
          foreach (ContributedContent contributedContent in contributedContents)
          {
            contentSourceList.AddRange(contributedContent.GetContentSources(this.TfsRequestContext, requestStyleType + requestContentType, "text/css", preferredLocation));
            contentSourceList.AddRange(contributedContent.GetContentSources(this.TfsRequestContext, requestScriptType + requestContentType, "text/javascript", preferredLocation));
          }
          if (contentSourceList.Count > 0)
            metadata.Content = (IList<ContentSource>) contentSourceList;
        }
        using (PerformanceTimer.StartMeasure(this.TfsRequestContext, "ProcessDataProviders"))
        {
          IContributionService service5 = this.TfsRequestContext.GetService<IContributionService>();
          List<Contribution> dataProviderContributions = new List<Contribution>();
          foreach (string wellKnownProvider in ContributionHierarchyControllerBase.s_wellKnownProviders)
          {
            Contribution contribution = service5.QueryContribution(this.TfsRequestContext, wellKnownProvider);
            if (contribution != null)
              dataProviderContributions.Add(contribution);
          }
          IEnumerable<ContributionNode> contributionsByType = service3.GetContributionsByType(this.TfsRequestContext, "ms.vss-web.data-provider");
          if (contributionsByType != null)
            dataProviderContributions.AddRange(contributionsByType.Select<ContributionNode, Contribution>((Func<ContributionNode, Contribution>) (providerNode => providerNode.Contribution)));
          IDataProviderScope scope = (IDataProviderScope) null;
          if (!string.IsNullOrEmpty(scopeName) && !string.IsNullOrEmpty(scopeValue))
          {
            scope = ExtensionDataProviderScopeExtensions.GetScope(this.TfsRequestContext, scopeName, scopeValue);
            this.TfsRequestContext.RootContext.Items["DataProviderQuery.Scope"] = (object) scope;
          }
          DataProviderResult dataProviderData = service1.GetDataProviderData(this.TfsRequestContext, new DataProviderContext()
          {
            Properties = dataProviderContext == null || dataProviderContext.Properties == null ? new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) : new Dictionary<string, object>((IDictionary<string, object>) dataProviderContext.Properties, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
          }, (IEnumerable<Contribution>) dataProviderContributions, scope, true);
          metadata.DataProviderExceptions = (IDictionary<string, DataProviderExceptionDetails>) dataProviderData.Exceptions;
          metadata.DataProviderSharedData = (IDictionary<string, object>) dataProviderData.SharedData;
          metadata.DataProviders = (IDictionary<string, object>) dataProviderData.Data;
        }
      }
      metadata.CheckPermission(this.TfsRequestContext);
      if (service2.IsTracePointCollectionEnabled(this.TfsRequestContext))
        metadata.Performance = PerformanceTimer.GetAllTimings(this.TfsRequestContext, (ISecuredObject) metadata);
      PerformanceTimer.SendCustomerIntelligenceData(this.TfsRequestContext);
      return metadata;
    }

    [DataContract]
    public class ContributedHierarchy : WebSdkMetadata
    {
      [DataMember(Name = "content", EmitDefaultValue = false)]
      public IList<ContentSource> Content;
      [DataMember(Name = "data", EmitDefaultValue = false)]
      public DataProviderResult Data;
      [DataMember(Name = "performance", EmitDefaultValue = false)]
      public IDictionary<string, TimingGroup> Performance;
      [DataMember(Name = "dataProviderExceptions", EmitDefaultValue = false)]
      public IDictionary<string, DataProviderExceptionDetails> DataProviderExceptions;
      [DataMember(Name = "dataProviderSharedData", EmitDefaultValue = false)]
      public IDictionary<string, object> DataProviderSharedData;
      [DataMember(Name = "dataProviders", EmitDefaultValue = false)]
      public IDictionary<string, object> DataProviders;
    }

    [DataContract]
    public class ContributedHierarchyQuery
    {
      [DataMember(Name = "dataProviderContext", EmitDefaultValue = false)]
      public DataProviderContext DataProviderContext;

      [DataMember]
      public List<string> ContributionIds { get; set; }
    }
  }
}
