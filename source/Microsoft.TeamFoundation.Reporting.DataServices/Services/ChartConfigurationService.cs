// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.Services.ChartConfigurationService
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Microsoft.TeamFoundation.Charting.WebApi;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Dashboards.CrossProjectSettings;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Reporting.DataServices.DataAccess;
using Microsoft.TeamFoundation.Reporting.DataServices.DataAccess.DataModel;
using Microsoft.TeamFoundation.Reporting.DataServices.Model;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Reporting.DataServices.Services
{
  public class ChartConfigurationService : IChartConfigurationService, IVssFrameworkService
  {
    public ChartConfiguration SaveChartConfiguration(
      IVssRequestContext requestContext,
      Guid projectId,
      ChartConfiguration chartConfiguration)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1017100, "Reporting", nameof (ChartConfigurationService), nameof (SaveChartConfiguration));
      bool flag = true;
      try
      {
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
        IDataServicesSecurityProvider securityProvider = requestContext.GetService<IFeatureProviderRegistryService>().SelectProvider(requestContext, chartConfiguration.Scope).GetSecurityProvider(requestContext);
        this.ValidateChartConfiguration(requestContext, chartConfiguration);
        this.SanitizeConfiguration(chartConfiguration);
        if (chartConfiguration.ChartId.HasValue)
        {
          Guid? chartId = chartConfiguration.ChartId;
          Guid empty = Guid.Empty;
          if ((chartId.HasValue ? (chartId.HasValue ? (chartId.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) == 0)
            goto label_4;
        }
        chartConfiguration.ChartId = new Guid?(Guid.NewGuid());
        chartConfiguration.TransformOptions.TransformId = new Guid?(Guid.NewGuid());
        flag = false;
label_4:
        ChartConfigurationDataModel chartConfiguration1 = new ChartConfigurationDataModel(chartConfiguration)
        {
          ChangedDate = DateTime.UtcNow
        };
        chartConfiguration1.ChangedBy = requestContext.GetUserIdentity().Id;
        DataServicesPermission permission = DataServicesPermission.CreateChart;
        if (flag)
          permission = DataServicesPermission.UpdateChart;
        requestContext.TraceBlock(1017108, 1017109, 1017110, "Reporting", "ChartConfigurationSecurityProvider", "EnsurePermissionsOnChartSave", (Action) (() => securityProvider.EnsureChartPermissions(permission, chartConfiguration)));
        using (ChartConfigurationSqlResourceComponent component = requestContext.CreateComponent<ChartConfigurationSqlResourceComponent>())
        {
          if (!flag)
            return component.AddChartConfiguration(projectId, chartConfiguration1);
          component.UpdateChartConfiguration(projectId, chartConfiguration1);
          return (ChartConfiguration) chartConfiguration1;
        }
      }
      finally
      {
        requestContext.TraceLeave(1017101, "Reporting", nameof (ChartConfigurationService), nameof (SaveChartConfiguration));
        string operation = flag ? "Update" : "Create";
        TelemetryHelper.PublishConfigRequest(requestContext, chartConfiguration.Scope, operation);
      }
    }

    public void DeleteChartConfiguration(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid id)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1017102, "Reporting", nameof (ChartConfigurationService), nameof (DeleteChartConfiguration));
      try
      {
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
        if (id == Guid.Empty)
          throw new ChartConfigurationDoesNotExistException(Guid.Empty);
        ChartConfiguration configurationById;
        using (ChartConfigurationSqlResourceComponent component = requestContext.CreateComponent<ChartConfigurationSqlResourceComponent>())
          configurationById = component.GetChartConfigurationById(projectId, id);
        if (configurationById == null)
          throw new ChartConfigurationDoesNotExistException(id);
        this.VerifyIfCanDeleteChartConfiguration(requestContext, configurationById);
        using (ChartConfigurationSqlResourceComponent component = requestContext.CreateComponent<ChartConfigurationSqlResourceComponent>())
          component.DeleteChartConfiguration(projectId, id);
        TelemetryHelper.PublishConfigRequest(requestContext, configurationById.Scope, "Delete");
      }
      finally
      {
        requestContext.TraceLeave(1017103, "Reporting", nameof (ChartConfigurationService), nameof (DeleteChartConfiguration));
      }
    }

    public void ValidateChartConfiguration(
      IVssRequestContext requestContext,
      ChartConfiguration chartConfiguration)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1017104, "Reporting", nameof (ChartConfigurationService), nameof (ValidateChartConfiguration));
      try
      {
        ArgumentUtility.CheckStringForNullOrWhiteSpace(chartConfiguration.Scope, "Scope");
        ArgumentUtility.CheckStringForNullOrWhiteSpace(chartConfiguration.GroupKey, "GroupKey");
        ArgumentUtility.CheckStringForNullOrWhiteSpace(chartConfiguration.ChartType, "ChartType");
        if (!ChartType.IsSupportedChartType(requestContext, chartConfiguration.ChartType))
          throw new InvalidChartConfigurationException(ReportingResources.UnsupportedChartType((object) chartConfiguration.ChartType));
        Guid? nullable = !string.IsNullOrWhiteSpace(chartConfiguration.Title) ? chartConfiguration.ChartId : throw new InvalidChartConfigurationException(ReportingResources.TitleCannotBeEmpty());
        if (nullable.HasValue)
        {
          nullable = chartConfiguration.ChartId;
          Guid empty = Guid.Empty;
          if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) == 0)
            goto label_10;
        }
        nullable = chartConfiguration.TransformOptions.TransformId;
        if (nullable.HasValue)
        {
          nullable = chartConfiguration.TransformOptions.TransformId;
          Guid empty = Guid.Empty;
          if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != empty ? 1 : 0) : 0) : 1) != 0)
            throw new InvalidChartConfigurationException(ReportingResources.CantCreateChartWithExistingTransform());
        }
label_10:
        nullable = chartConfiguration.ChartId;
        if (nullable.HasValue)
        {
          nullable = chartConfiguration.ChartId;
          Guid empty1 = Guid.Empty;
          if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() != empty1 ? 1 : 0) : 0) : 1) != 0)
          {
            nullable = chartConfiguration.TransformOptions.TransformId;
            if (nullable.HasValue)
            {
              nullable = chartConfiguration.TransformOptions.TransformId;
              Guid empty2 = Guid.Empty;
              if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() == empty2 ? 1 : 0) : 1) : 0) == 0)
                goto label_15;
            }
            throw new InvalidChartConfigurationException(ReportingResources.CantUpdateChartWithoutTransform());
          }
        }
label_15:
        DataTransformService.ValidateTransformOptions(requestContext, chartConfiguration.TransformOptions);
        if (chartConfiguration.UserColors == null)
          return;
        int num = 0;
        foreach (ColorConfiguration userColor in chartConfiguration.UserColors)
        {
          ++num;
          ArgumentUtility.CheckStringForNullOrWhiteSpace(userColor.Value, "Value");
          ArgumentUtility.CheckStringForNullOrWhiteSpace(userColor.BackgroundColor, "BackgroundColor");
          if (!ChartColors.IsSupportedBackgroundColor(userColor.BackgroundColor))
            throw new InvalidChartConfigurationException(ReportingResources.UnsupportedChartBackgroundColor((object) userColor.BackgroundColor));
        }
        if (num > 20)
          throw new TooManyColorsPerChartException();
      }
      catch (Exception ex)
      {
        throw new InvalidChartConfigurationException(ex.Message, ex);
      }
      finally
      {
        requestContext.TraceLeave(1017105, "Reporting", nameof (ChartConfigurationService), nameof (ValidateChartConfiguration));
      }
    }

    public IEnumerable<ChartConfiguration> GetChartConfigurationGroup(
      IVssRequestContext requestContext,
      Guid projectId,
      string scope,
      string groupKey)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1017106, "Reporting", nameof (ChartConfigurationService), nameof (GetChartConfigurationGroup));
      try
      {
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
        try
        {
          ArgumentUtility.CheckStringForNullOrEmpty(scope, nameof (scope));
          ArgumentUtility.CheckStringForNullOrEmpty(groupKey, nameof (groupKey));
        }
        catch (ArgumentException ex)
        {
          throw new InvalidChartGroupException((Exception) ex);
        }
        List<ChartConfiguration> everyChartGroup = new List<ChartConfiguration>();
        using (ChartConfigurationSqlResourceComponent component = requestContext.CreateComponent<ChartConfigurationSqlResourceComponent>())
          everyChartGroup.AddRange(component.GetChartConfigurationsByGroup(projectId, scope, groupKey));
        return (IEnumerable<ChartConfiguration>) this.FilterChartByTypeAndPermissions(requestContext, projectId, scope, everyChartGroup);
      }
      finally
      {
        requestContext.TraceLeave(1017107, "Reporting", nameof (ChartConfigurationService), nameof (GetChartConfigurationGroup));
        TelemetryHelper.PublishConfigRequest(requestContext, scope, "ReadByGroup");
      }
    }

    public ChartConfiguration GetChartConfiguration(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid id)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1017108, "Reporting", nameof (ChartConfigurationService), nameof (GetChartConfiguration));
      string scope = (string) null;
      try
      {
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
        if (id.Equals(Guid.Empty))
          throw new ChartConfigurationDoesNotExistException(Guid.Empty);
        ChartConfiguration configurationById;
        using (ChartConfigurationSqlResourceComponent component = requestContext.CreateComponent<ChartConfigurationSqlResourceComponent>())
          configurationById = component.GetChartConfigurationById(projectId, id);
        if (configurationById == null)
          throw new ChartConfigurationDoesNotExistException(id);
        IDataServicesSecurityProvider securityProvider = requestContext.GetService<IFeatureProviderRegistryService>().SelectProvider(requestContext, configurationById.Scope).GetSecurityProvider(requestContext);
        securityProvider.EnsureChartPermissions(DataServicesPermission.ReadChart, configurationById);
        if (securityProvider is IDataServicesSecurityProvider2 securityProvider2)
        {
          ISecuredObject configurationSecuredObject = securityProvider2.GetChartConfigurationSecuredObject(configurationById, projectId);
          configurationById.SetSecuredObject(configurationSecuredObject);
        }
        if (!ChartType.IsSupportedChartType(requestContext, configurationById.ChartType))
          throw new InvalidChartConfigurationException(ReportingResources.UnsupportedChartType((object) configurationById.ChartType));
        scope = configurationById.Scope;
        AggregationMediator.CheckMeasureParameters(requestContext, configurationById.TransformOptions.Measure);
        return configurationById;
      }
      finally
      {
        requestContext.TraceLeave(1017109, "Reporting", nameof (ChartConfigurationService), nameof (GetChartConfiguration));
        TelemetryHelper.PublishConfigRequest(requestContext, scope, "Read");
      }
    }

    public void DeleteChartConfigurationsByGroups(
      IVssRequestContext requestContext,
      Guid projectId,
      string scope,
      IEnumerable<string> groupKeys)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1017111, "Reporting", nameof (ChartConfigurationService), nameof (DeleteChartConfigurationsByGroups));
      try
      {
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
        ArgumentUtility.CheckStringForNullOrEmpty(scope, nameof (scope));
        ArgumentUtility.CheckForNull<IEnumerable<string>>(groupKeys, nameof (groupKeys));
        using (ChartConfigurationSqlResourceComponent component = requestContext.CreateComponent<ChartConfigurationSqlResourceComponent>())
          component.DeleteChartConfigurationsByGroups(projectId, scope, groupKeys);
      }
      finally
      {
        requestContext.TraceLeave(1017112, "Reporting", nameof (ChartConfigurationService), nameof (DeleteChartConfigurationsByGroups));
        TelemetryHelper.PublishConfigRequest(requestContext, scope, "DeleteByGroups_Cleanup");
      }
    }

    public IEnumerable<Guid> GetChartConfigurationIdsByGroups(
      IVssRequestContext requestContext,
      Guid projectId,
      string scope,
      IEnumerable<string> groupKeys)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(1017113, "Reporting", nameof (ChartConfigurationService), nameof (GetChartConfigurationIdsByGroups));
      try
      {
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
        ArgumentUtility.CheckStringForNullOrEmpty(scope, nameof (scope));
        ArgumentUtility.CheckForNull<IEnumerable<string>>(groupKeys, nameof (groupKeys));
        using (ChartConfigurationSqlResourceComponent component = requestContext.CreateComponent<ChartConfigurationSqlResourceComponent>())
          return component.GetChartConfigurationIdsByGroups(projectId, scope, groupKeys);
      }
      finally
      {
        requestContext.TraceLeave(1017114, "Reporting", nameof (ChartConfigurationService), nameof (GetChartConfigurationIdsByGroups));
        TelemetryHelper.PublishConfigRequest(requestContext, scope, "ReadByGroups_Cleanup");
      }
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    private void SanitizeConfiguration(ChartConfiguration chartConfiguration)
    {
      chartConfiguration.Scope = StringUtil.Truncate(chartConfiguration.Scope, 50, false);
      chartConfiguration.Title = StringUtil.Truncate(chartConfiguration.Title, 256, false);
      chartConfiguration.GroupKey = StringUtil.Truncate(chartConfiguration.GroupKey, 256, false);
      chartConfiguration.TransformOptions.Series = chartConfiguration.TransformOptions.Series ?? string.Empty;
      chartConfiguration.TransformOptions.HistoryRange = chartConfiguration.TransformOptions.HistoryRange ?? string.Empty;
      chartConfiguration.TransformOptions.Measure.PropertyName = chartConfiguration.TransformOptions.Measure.PropertyName ?? string.Empty;
    }

    private void VerifyIfCanDeleteChartConfiguration(
      IVssRequestContext requestContext,
      ChartConfiguration chart)
    {
      IDataServicesSecurityProvider securityProvider = requestContext.GetService<IFeatureProviderRegistryService>().SelectProvider(requestContext, chart.Scope).GetSecurityProvider(requestContext);
      requestContext.TraceBlock(1017108, 1017109, 1017110, "Reporting", "ChartConfigurationSecurityProvider", "EnsurePermissionsOnChartDelete", (Action) (() => securityProvider.EnsureChartPermissions(DataServicesPermission.DeleteChart, chart)));
    }

    private List<ChartConfiguration> FilterChartByTypeAndPermissions(
      IVssRequestContext requestContext,
      Guid projectId,
      string scope,
      List<ChartConfiguration> everyChartGroup)
    {
      IDataServicesSecurityProvider securityProvider = requestContext.GetService<IFeatureProviderRegistryService>().SelectProvider(requestContext, scope).GetSecurityProvider(requestContext);
      IDataServicesSecurityProvider2 securityProvider2 = securityProvider as IDataServicesSecurityProvider2;
      List<ChartConfiguration> chartConfigurationList = new List<ChartConfiguration>();
      foreach (ChartConfiguration chartConfiguration in everyChartGroup)
      {
        securityProvider.EnsureChartPermissions(DataServicesPermission.ReadChart, chartConfiguration);
        if (securityProvider2 != null)
        {
          ISecuredObject configurationSecuredObject = securityProvider2.GetChartConfigurationSecuredObject(chartConfiguration, projectId);
          chartConfiguration.SetSecuredObject(configurationSecuredObject);
          if (!CrossProjectSettingsSecurityHelper.CanUserViewCrossProjectSettings(requestContext))
            chartConfiguration.UserColors = (IEnumerable<ColorConfiguration>) new List<ColorConfiguration>();
        }
        if (ChartType.IsSupportedChartType(requestContext, chartConfiguration.ChartType) && AggregationMediator.IsSupportedMeasure(requestContext, chartConfiguration.TransformOptions.Measure))
          chartConfigurationList.Add(chartConfiguration);
      }
      return chartConfigurationList;
    }
  }
}
