// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.DataAccess.ChartConfigurationSqlResourceComponent3
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Microsoft.TeamFoundation.Charting.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Reporting.DataServices.DataAccess.DataModel;
using Microsoft.TeamFoundation.Reporting.DataServices.DataAccess.Traditional.Binders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Reporting.DataServices.DataAccess
{
  internal class ChartConfigurationSqlResourceComponent3 : ChartConfigurationSqlResourceComponent2
  {
    public override ChartConfiguration GetChartConfigurationById(Guid projectId, Guid id)
    {
      this.PrepareStoredProcedure("prc_GetChartConfigurationById");
      this.BindGuid("@chartId", id);
      this.BindProjectId(projectId);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<ChartConfiguration>((ObjectBinder<ChartConfiguration>) new ChartConfigurationBinder());
      resultCollection.AddBinder<ColorConfiguration>((ObjectBinder<ColorConfiguration>) new ColorConfigurationBinder());
      IEnumerable<ChartConfiguration> items1 = (IEnumerable<ChartConfiguration>) resultCollection.GetCurrent<ChartConfiguration>().Items;
      resultCollection.NextResult();
      IEnumerable<ColorConfiguration> items2 = (IEnumerable<ColorConfiguration>) resultCollection.GetCurrent<ColorConfiguration>().Items;
      return this.JoinColorsToCharts(items1, this.GenerateDictionary(items2)).FirstOrDefault<ChartConfiguration>();
    }

    public override ChartConfiguration AddChartConfiguration(
      Guid projectId,
      ChartConfigurationDataModel chartConfiguration)
    {
      this.PrepareStoredProcedure("prc_AddChartConfiguration");
      this.BindChartConfigurationTable("@chartConfigurationTable", chartConfiguration);
      this.BindColorConfigurationTable("@colorConfigurationTable", chartConfiguration.UserColors);
      this.BindProjectId(projectId);
      this.ExecuteNonQuery();
      return (ChartConfiguration) chartConfiguration;
    }

    public override void UpdateChartConfiguration(
      Guid projectId,
      ChartConfigurationDataModel chartConfiguration)
    {
      this.PrepareStoredProcedure("prc_UpdateChartConfiguration");
      this.BindChartConfigurationTable("@chartConfigurationTable", chartConfiguration);
      this.BindColorConfigurationTable("@colorConfigurationTable", chartConfiguration.UserColors);
      this.BindProjectId(projectId);
      this.ExecuteNonQuery();
    }

    public override void DeleteChartConfigurationsByGroups(
      Guid projectId,
      string scope,
      IEnumerable<string> groupKeys)
    {
      this.PrepareStoredProcedure("prc_DeleteChartConfigurationsByGroups");
      this.BindString("@scope", scope, (int) byte.MaxValue, false, SqlDbType.NVarChar);
      this.BindStringTable("@groupKeys", groupKeys);
      this.BindProjectId(projectId);
      this.ExecuteNonQuery();
    }

    public override IEnumerable<Guid> GetChartConfigurationIdsByGroups(
      Guid projectId,
      string scope,
      IEnumerable<string> groupKeys)
    {
      this.PrepareStoredProcedure("prc_GetChartConfigurationIdsByGroups");
      this.BindString("@scope", scope, (int) byte.MaxValue, false, SqlDbType.NVarChar);
      this.BindStringTable("@groupKeys", groupKeys);
      this.BindProjectId(projectId);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new ChartIdBinder());
      return (IEnumerable<Guid>) resultCollection.GetCurrent<Guid>().Items;
    }
  }
}
