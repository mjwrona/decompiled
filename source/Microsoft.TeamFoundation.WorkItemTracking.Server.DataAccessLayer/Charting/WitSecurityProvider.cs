// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Charting.WitSecurityProvider
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Charting.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Reporting.DataServices;
using Microsoft.TeamFoundation.Reporting.DataServices.FeatureUtilities;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Charting
{
  internal class WitSecurityProvider : 
    IDataServicesSecurityProvider2,
    IDataServicesSecurityProvider,
    IDataServicesService
  {
    public IVssRequestContext RequestContext { get; set; }

    public void EnsureTransformPermissions(TransformOptions transformOptions)
    {
      if (WitSecurityProvider.TransformUsesAssignedToMe(transformOptions))
        return;
      this.CheckQueryPermission(DataServicesPermission.ReadChart, transformOptions.Filter);
    }

    public void EnsureChartPermissions(
      DataServicesPermission permission,
      ChartConfiguration chartConfiguration)
    {
      if (WitSecurityProvider.TransformUsesAssignedToMe(chartConfiguration.TransformOptions))
      {
        try
        {
          WitSecurityProvider.EnsureUserOwnsAssignedChart(this.RequestContext, chartConfiguration);
        }
        catch (LegacyValidationException ex)
        {
          throw new AccessCheckException(this.RequestContext.UserContext, string.Empty, 0, Guid.Empty, ex.Message);
        }
      }
      else
      {
        if (chartConfiguration.TransformOptions.Filter != chartConfiguration.GroupKey)
          throw new InvalidChartConfigurationException("QueryChartIdMismatch");
        this.CheckQueryPermission(permission, chartConfiguration.TransformOptions.Filter);
      }
    }

    public ISecuredObject GetChartConfigurationSecuredObject(
      ChartConfiguration chartConfiguration,
      Guid projectId)
    {
      return (ISecuredObject) DefaultChartSecuredObject.GenerateAndCheckProjectReadObject(this.RequestContext, projectId);
    }

    public ISecuredObject GetTransformSecuredObject(Guid projectId) => (ISecuredObject) DefaultChartSecuredObject.GenerateAndCheckProjectReadObject(this.RequestContext, projectId);

    internal virtual void CheckPermission(ServerQueryItem query, int requestedPermissions)
    {
      QueryItemMethods.PopulateSecurityInfo(this.RequestContext, query);
      QueryItemMethods.CheckPermission(this.RequestContext, query, requestedPermissions);
    }

    private void CheckQueryPermission(DataServicesPermission permission, string transformFilter)
    {
      Guid result;
      if (!Guid.TryParse(transformFilter, out result))
        throw new InvalidTransformOptionsException(string.Format("Filter: {0}", (object) transformFilter));
      int requestedPermissions = permission == DataServicesPermission.ReadChart ? 1 : 2;
      try
      {
        this.CheckPermission(new ServerQueryItem(result), requestedPermissions);
      }
      catch (LegacyValidationException ex)
      {
        throw new AccessCheckException(this.RequestContext.UserContext, string.Empty, 0, Guid.Empty, ex.Message);
      }
    }

    private static bool TransformUsesAssignedToMe(TransformOptions transformOptions) => transformOptions.Filter == "a2108d31-086c-4fb0-afda-097e4cc46df4";

    private static void EnsureUserOwnsAssignedChart(
      IVssRequestContext requestContext,
      ChartConfiguration chartConfiguration)
    {
      string groupKey = chartConfiguration.GroupKey;
      int length = groupKey.IndexOf('_');
      Guid result;
      if (length <= -1 || !Guid.TryParse(groupKey.Substring(0, length), out result))
        throw new InvalidChartConfigurationException("MissingUserForAssignedToMeGroupKey");
      if (!(result == requestContext.GetUserIdentity().Id))
        throw new LegacyValidationException("MismatchedUserForAssignedToMeGroupKey");
    }
  }
}
