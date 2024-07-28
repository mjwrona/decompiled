// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Charting.TestReportsSecurityProvider
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Charting.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Reporting.DataServices;
using Microsoft.TeamFoundation.Reporting.DataServices.FeatureUtilities;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server.Charting
{
  internal class TestReportsSecurityProvider : 
    IDataServicesSecurityProvider2,
    IDataServicesSecurityProvider,
    IDataServicesService
  {
    public IVssRequestContext RequestContext { get; set; }

    public void EnsureTransformPermissions(TransformOptions transformOptions)
    {
    }

    public void EnsureChartPermissions(
      DataServicesPermission permission,
      ChartConfiguration chartConfiguration)
    {
      switch (permission)
      {
        case DataServicesPermission.CreateChart:
        case DataServicesPermission.UpdateChart:
        case DataServicesPermission.DeleteChart:
          if (chartConfiguration.Scope.Equals(FeatureProviderScopes.TestRunSummary, StringComparison.OrdinalIgnoreCase) || chartConfiguration == null || chartConfiguration.TransformOptions == null || string.IsNullOrEmpty(chartConfiguration.TransformOptions.Filter))
            break;
          string transformFilter = TestReportsHelper.GetTransformFilterMap(chartConfiguration.TransformOptions.Filter)["suiteId"];
          int result;
          if (string.IsNullOrEmpty(transformFilter) || !int.TryParse(transformFilter, out result))
            break;
          SuiteSecurityHelper.CheckTestSuiteUpdatePermission((TestManagementRequestContext) new TfsTestManagementRequestContext(this.RequestContext), (IEnumerable<int>) new int[1]
          {
            result
          });
          break;
      }
    }

    public ISecuredObject GetChartConfigurationSecuredObject(
      ChartConfiguration chartConfiguration,
      Guid projectId)
    {
      return (ISecuredObject) DefaultChartSecuredObject.GenerateAndCheckProjectReadObject(this.RequestContext, projectId);
    }

    public ISecuredObject GetTransformSecuredObject(Guid projectId) => (ISecuredObject) DefaultChartSecuredObject.GenerateAndCheckProjectReadObject(this.RequestContext, projectId);
  }
}
