// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.FeatureProviderRegistryService
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.TeamFoundation.Reporting.DataServices
{
  public class FeatureProviderRegistryService : IFeatureProviderRegistryService, IVssFrameworkService
  {
    private Dictionary<string, IProvideChartingData> m_featureProviders;

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.m_featureProviders = FeatureProviderRegistryService.RegisterProviders(systemRequestContext);

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IProvideChartingData SelectProvider(IVssRequestContext requestContext, string scope)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(scope, nameof (scope));
      string lowerInvariant = FeatureProviderScopes.ShimLegacyLookup(scope).ToLowerInvariant();
      return this.m_featureProviders.ContainsKey(lowerInvariant) ? this.m_featureProviders[lowerInvariant] : throw new ChartScopeProviderNotFoundException(scope);
    }

    private static Dictionary<string, IProvideChartingData> RegisterProviders(
      IVssRequestContext requestContext)
    {
      Dictionary<string, IProvideChartingData> dictionary = new Dictionary<string, IProvideChartingData>();
      foreach (FeatureProviderInfo featureProviderInfo in new List<FeatureProviderInfo>()
      {
        new FeatureProviderInfo(FeatureProviderScopes.WorkItemQueries, FeatureProviderAssemblies.WorkItemDataAccessLayer, FeatureProviderTypes.WITChartsProvider),
        new FeatureProviderInfo(FeatureProviderScopes.TestReports, FeatureProviderAssemblies.TestManagementServer, FeatureProviderTypes.TestReportsChartsProvider),
        new FeatureProviderInfo(FeatureProviderScopes.TestRunSummary, FeatureProviderAssemblies.TestManagementServer, FeatureProviderTypes.TestRunChartsProvider),
        new FeatureProviderInfo(FeatureProviderScopes.TestAuthoringMetadata, FeatureProviderAssemblies.TestManagementServer, FeatureProviderTypes.TestAuthoringMetadataChartsProvider)
      })
      {
        IProvideChartingData provideChartingData = FactoryHelper.Instantiate<IProvideChartingData>(FeatureProviderRegistryService.LookupType(featureProviderInfo.AssemblyLongName, featureProviderInfo.TypeName));
        dictionary.Add(featureProviderInfo.Scope.ToLowerInvariant(), provideChartingData);
      }
      return dictionary;
    }

    private static Type LookupType(string assemblyName, string typeName) => Assembly.Load(assemblyName).GetType(typeName, true, true);
  }
}
