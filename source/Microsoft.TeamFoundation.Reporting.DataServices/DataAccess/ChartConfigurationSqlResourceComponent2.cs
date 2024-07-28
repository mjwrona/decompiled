// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.DataAccess.ChartConfigurationSqlResourceComponent2
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Charting.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Reporting.DataServices.DataAccess.DataModel;
using Microsoft.TeamFoundation.Reporting.DataServices.DataAccess.Traditional.Binders;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Reporting.DataServices.DataAccess
{
  internal class ChartConfigurationSqlResourceComponent2 : ChartConfigurationSqlResourceComponent
  {
    private static readonly SqlMetaData[] typ_ColorConfigurationTable = new SqlMetaData[2]
    {
      new SqlMetaData("Value", SqlDbType.NVarChar, 256L),
      new SqlMetaData("BackgroundColor", SqlDbType.Char, 7L)
    };

    public override ChartConfiguration AddChartConfiguration(
      Guid projectId,
      ChartConfigurationDataModel chartConfiguration)
    {
      this.PrepareStoredProcedure("prc_AddChartConfiguration");
      this.BindChartConfigurationTable("@chartConfigurationTable", chartConfiguration);
      this.BindProjectId(projectId);
      this.ExecuteNonQuery();
      if (chartConfiguration.UserColors != null)
        this.FlushColorPreferences(projectId, chartConfiguration.ChartId.Value, chartConfiguration.UserColors);
      return (ChartConfiguration) chartConfiguration;
    }

    public override void UpdateChartConfiguration(
      Guid projectId,
      ChartConfigurationDataModel chartConfiguration)
    {
      this.PrepareStoredProcedure("prc_UpdateChartConfiguration");
      this.BindChartConfigurationTable("@chartConfigurationTable", chartConfiguration);
      this.BindProjectId(projectId);
      this.ExecuteNonQuery();
      this.FlushColorPreferences(projectId, chartConfiguration.ChartId.Value, chartConfiguration.UserColors);
    }

    public override IEnumerable<ChartConfiguration> GetChartConfigurationsByGroup(
      Guid projectId,
      string scope,
      string groupKey)
    {
      this.PrepareStoredProcedure("prc_GetChartConfigurationsByGroup");
      this.BindString("@scope", scope, (int) byte.MaxValue, false, SqlDbType.NVarChar);
      this.BindString("@groupKey", groupKey, (int) byte.MaxValue, false, SqlDbType.NVarChar);
      this.BindProjectId(projectId);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<ChartConfiguration>((ObjectBinder<ChartConfiguration>) new ChartConfigurationBinder());
      resultCollection.AddBinder<ColorConfiguration>((ObjectBinder<ColorConfiguration>) new ColorConfigurationBinder());
      IEnumerable<ChartConfiguration> items1 = (IEnumerable<ChartConfiguration>) resultCollection.GetCurrent<ChartConfiguration>().Items;
      resultCollection.NextResult();
      IEnumerable<ColorConfiguration> items2 = (IEnumerable<ColorConfiguration>) resultCollection.GetCurrent<ColorConfiguration>().Items;
      return this.JoinColorsToCharts(items1, this.GenerateDictionary(items2));
    }

    protected SqlParameter BindColorConfigurationTable(
      string parameterName,
      IEnumerable<ColorConfiguration> rows)
    {
      rows = rows ?? Enumerable.Empty<ColorConfiguration>();
      System.Func<ColorConfiguration, SqlDataRecord> selector = (System.Func<ColorConfiguration, SqlDataRecord>) (chartColors =>
      {
        ArgumentUtility.CheckForNull<ColorConfiguration>(chartColors, "chartConfiguration");
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ChartConfigurationSqlResourceComponent2.typ_ColorConfigurationTable);
        int ordinal1 = 0;
        int num1 = ordinal1 + 1;
        sqlDataRecord.SetString(ordinal1, chartColors.Value);
        int ordinal2 = num1;
        int num2 = ordinal2 + 1;
        sqlDataRecord.SetString(ordinal2, chartColors.BackgroundColor);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_ColorConfigurationTable", rows.Select<ColorConfiguration, SqlDataRecord>(selector));
    }

    protected SqlParameter BindColorConfigurationTable(string parameterName, ColorConfiguration row) => this.BindColorConfigurationTable(parameterName, (IEnumerable<ColorConfiguration>) new List<ColorConfiguration>()
    {
      row
    });

    protected virtual void FlushColorPreferences(
      Guid projectId,
      Guid chartId,
      IEnumerable<ColorConfiguration> userColors)
    {
      this.PrepareStoredProcedure("prc_FlushColorConfiguration");
      this.BindGuid("@chartId", chartId);
      this.BindColorConfigurationTable("@colorConfigurationTable", userColors);
      this.BindProjectId(projectId);
      this.ExecuteNonQuery();
    }

    protected virtual Dictionary<Guid, List<ColorConfiguration>> GenerateDictionary(
      IEnumerable<ColorConfiguration> colorConfigs)
    {
      Dictionary<Guid, List<ColorConfiguration>> dictionary1 = new Dictionary<Guid, List<ColorConfiguration>>();
      foreach (ColorConfiguration colorConfig in colorConfigs)
      {
        Dictionary<Guid, List<ColorConfiguration>> dictionary2 = dictionary1;
        Guid? chartId = colorConfig.ChartId;
        Guid key1 = chartId.Value;
        if (!dictionary2.ContainsKey(key1))
        {
          Dictionary<Guid, List<ColorConfiguration>> dictionary3 = dictionary1;
          chartId = colorConfig.ChartId;
          Guid key2 = chartId.Value;
          List<ColorConfiguration> colorConfigurationList = new List<ColorConfiguration>();
          dictionary3[key2] = colorConfigurationList;
        }
        Dictionary<Guid, List<ColorConfiguration>> dictionary4 = dictionary1;
        chartId = colorConfig.ChartId;
        Guid key3 = chartId.Value;
        dictionary4[key3].Add(colorConfig);
      }
      return dictionary1;
    }

    protected virtual IEnumerable<ChartConfiguration> JoinColorsToCharts(
      IEnumerable<ChartConfiguration> chartConfigs,
      Dictionary<Guid, List<ColorConfiguration>> colorDict)
    {
      foreach (ChartConfiguration chartConfig in chartConfigs)
      {
        Dictionary<Guid, List<ColorConfiguration>> dictionary1 = colorDict;
        Guid? chartId = chartConfig.ChartId;
        Guid key1 = chartId.Value;
        if (dictionary1.ContainsKey(key1))
        {
          ChartConfiguration chartConfiguration = chartConfig;
          Dictionary<Guid, List<ColorConfiguration>> dictionary2 = colorDict;
          chartId = chartConfig.ChartId;
          Guid key2 = chartId.Value;
          List<ColorConfiguration> colorConfigurationList = dictionary2[key2];
          chartConfiguration.UserColors = (IEnumerable<ColorConfiguration>) colorConfigurationList;
        }
      }
      return chartConfigs;
    }
  }
}
