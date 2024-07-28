// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.DataAccess.ChartConfigurationSqlResourceComponent6
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Microsoft.TeamFoundation.Charting.WebApi;
using Microsoft.TeamFoundation.Reporting.DataServices.DataAccess.DataModel;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Reporting.DataServices.DataAccess
{
  internal class ChartConfigurationSqlResourceComponent6 : ChartConfigurationSqlResourceComponent5
  {
    public override void DeleteChartConfigurationsByGroups(
      Guid projectId,
      string scope,
      IEnumerable<string> groupKeys)
    {
      base.DeleteChartConfigurationsByGroups(projectId, scope, GroupKeyHelper.updateGroupKeys(groupKeys, scope));
    }

    public override IEnumerable<Guid> GetChartConfigurationIdsByGroups(
      Guid projectId,
      string scope,
      IEnumerable<string> groupKeys)
    {
      return base.GetChartConfigurationIdsByGroups(projectId, scope, GroupKeyHelper.updateGroupKeys(groupKeys, scope));
    }

    public override IEnumerable<ChartConfiguration> GetChartConfigurationsByGroup(
      Guid projectId,
      string scope,
      string groupKey)
    {
      return base.GetChartConfigurationsByGroup(projectId, scope, GroupKeyHelper.updateGroupKey(groupKey, scope));
    }

    public override ChartConfiguration AddChartConfiguration(
      Guid projectId,
      ChartConfigurationDataModel chartConfiguration)
    {
      chartConfiguration.GroupKey = GroupKeyHelper.updateGroupKey(chartConfiguration.GroupKey, chartConfiguration.Scope);
      return base.AddChartConfiguration(projectId, chartConfiguration);
    }

    public override void UpdateChartConfiguration(
      Guid projectId,
      ChartConfigurationDataModel chartConfiguration)
    {
      chartConfiguration.GroupKey = GroupKeyHelper.updateGroupKey(chartConfiguration.GroupKey, chartConfiguration.Scope);
      base.UpdateChartConfiguration(projectId, chartConfiguration);
    }
  }
}
