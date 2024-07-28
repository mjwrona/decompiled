// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExperimentService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class QueryExperimentService : IQueryExperimentService, IVssFrameworkService
  {
    private IExperimentService m_experimentService;
    private QueryExperiment m_currentQueryExperiment;
    private string m_registryPath;
    private readonly IReadOnlyDictionary<QueryExperiment, string> experimentIdLookup = (IReadOnlyDictionary<QueryExperiment, string>) new Dictionary<QueryExperiment, string>()
    {
      {
        QueryExperiment.AllowNonClusteredColumnstoreIndex,
        "ms.vss-work-web.columnstore-index-experiment"
      },
      {
        QueryExperiment.EnableQueryIdentityConstIdOptimization,
        "ms.vss-work-web.identity-constid-experiment"
      }
    };
    private const string c_currentQueryExperimentRegistryKeySuffix = "/CurrentQueryExperiment";
    private const string c_inExperiment = "inExperiment";

    public void ServiceEnd(IVssRequestContext systemRequestContext) => this.UnregisterNotification(systemRequestContext.To(TeamFoundationHostType.Deployment));

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.m_experimentService = systemRequestContext.GetService<IExperimentService>();
      this.m_registryPath = "/Service/WorkItemTracking/Settings/QueryExperiment";
      this.m_currentQueryExperiment = this.GetCurrentExperimentFromRegistry(systemRequestContext);
      this.RegisterNotification(systemRequestContext.To(TeamFoundationHostType.Deployment));
    }

    public T RunPerformanceExperiment<T>(
      IVssRequestContext requestContext,
      Func<T> controlAlgorithm,
      Func<T> experimentalAlgorithm,
      bool runBothAlgorithms = false)
    {
      string contributionId;
      if (this.experimentIdLookup.TryGetValue(this.m_currentQueryExperiment, out contributionId))
        return this.m_experimentService.RunPerformanceExperiment<T>(requestContext, contributionId, (Func<T>) (() =>
        {
          this.SetExperimentState(requestContext, false);
          return controlAlgorithm();
        }), (Func<T>) (() =>
        {
          this.SetExperimentState(requestContext, true);
          return experimentalAlgorithm();
        }), runBothAlgorithms);
      this.SetExperimentState(requestContext, false);
      return controlAlgorithm();
    }

    public T GetValueForTargetExperiment<T>(
      IVssRequestContext requestContext,
      QueryExperiment targetExperiment,
      T inExperimentValue,
      T inControlValue)
    {
      return !this.IsInExperiment(requestContext) || !this.m_currentQueryExperiment.HasFlag((Enum) targetExperiment) ? inControlValue : inExperimentValue;
    }

    public QueryExperiment GetCurrentExperiment() => this.m_currentQueryExperiment;

    public QueryExperiment GetCurrentExperimentState(IVssRequestContext requestContext) => !this.IsInExperiment(requestContext) ? QueryExperiment.None : this.m_currentQueryExperiment;

    private bool IsInExperiment(IVssRequestContext requestContext) => (bool) requestContext.Items.GetValueOrDefault<string, object>("inExperiment", (object) false);

    private void SetExperimentState(IVssRequestContext requestContext, bool inExperiment) => requestContext.Items[nameof (inExperiment)] = (object) inExperiment;

    protected virtual QueryExperiment GetCurrentExperimentFromRegistry(
      IVssRequestContext systemRequestContext)
    {
      return systemRequestContext.GetService<CachedRegistryService>().ReadEntriesFallThru(systemRequestContext, (RegistryQuery) (this.m_registryPath + "/*")).GetValueFromPath<QueryExperiment>(this.m_registryPath + "/CurrentQueryExperiment", QueryExperiment.None);
    }

    protected virtual void RegisterNotification(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<CachedRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnExperimentChanged), false, this.m_registryPath + "/*");

    protected virtual void OnExperimentChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.m_currentQueryExperiment = this.GetCurrentExperimentFromRegistry(requestContext);
    }

    protected virtual void UnregisterNotification(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<CachedRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnExperimentChanged));
  }
}
