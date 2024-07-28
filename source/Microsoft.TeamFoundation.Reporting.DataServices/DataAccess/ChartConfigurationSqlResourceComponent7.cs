// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.DataAccess.ChartConfigurationSqlResourceComponent7
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Reporting.DataServices.DataAccess.DataModel;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Reporting.DataServices.DataAccess
{
  internal class ChartConfigurationSqlResourceComponent7 : ChartConfigurationSqlResourceComponent6
  {
    private static readonly SqlMetaData[] typ_ChartConfigurationTable3 = new SqlMetaData[16]
    {
      new SqlMetaData("ChartId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("GroupKey", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Scope", SqlDbType.NVarChar, 50L),
      new SqlMetaData("ChartType", SqlDbType.NVarChar, 50L),
      new SqlMetaData("Title", SqlDbType.NVarChar, 256L),
      new SqlMetaData("TransformId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Filter", SqlDbType.NVarChar, 256L),
      new SqlMetaData("HistoryRange", SqlDbType.NVarChar, 50L),
      new SqlMetaData("GroupBy", SqlDbType.NVarChar, 386L),
      new SqlMetaData("OrderPropertyName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("OrderDirection", SqlDbType.NVarChar, 50L),
      new SqlMetaData("MeasureAggregation", SqlDbType.NVarChar, 50L),
      new SqlMetaData("Series", SqlDbType.NVarChar, 256L),
      new SqlMetaData("MeasurePropertyName", SqlDbType.NVarChar, 386L),
      new SqlMetaData("ChangedBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ChangedDate", SqlDbType.DateTime)
    };

    protected override SqlParameter BindChartConfigurationTable(
      string parameterName,
      IEnumerable<ChartConfigurationDataModel> rows)
    {
      rows = rows ?? Enumerable.Empty<ChartConfigurationDataModel>();
      System.Func<ChartConfigurationDataModel, SqlDataRecord> selector = (System.Func<ChartConfigurationDataModel, SqlDataRecord>) (chartConfiguration =>
      {
        ArgumentUtility.CheckForNull<ChartConfigurationDataModel>(chartConfiguration, nameof (chartConfiguration));
        ArgumentUtility.CheckForNull<TransformOptionsDataModel>(chartConfiguration.TransformOptions, "chartConfiguration.TransformOptions");
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ChartConfigurationSqlResourceComponent7.typ_ChartConfigurationTable3);
        int ordinal1 = 0;
        int num1 = ordinal1 + 1;
        sqlDataRecord.SetGuid(ordinal1, chartConfiguration.ChartId.Value);
        int ordinal2 = num1;
        int num2 = ordinal2 + 1;
        sqlDataRecord.SetString(ordinal2, chartConfiguration.GroupKey);
        int ordinal3 = num2;
        int num3 = ordinal3 + 1;
        sqlDataRecord.SetString(ordinal3, chartConfiguration.Scope);
        int ordinal4 = num3;
        int num4 = ordinal4 + 1;
        sqlDataRecord.SetString(ordinal4, chartConfiguration.ChartType);
        int ordinal5 = num4;
        int num5 = ordinal5 + 1;
        sqlDataRecord.SetString(ordinal5, chartConfiguration.Title);
        int ordinal6 = num5;
        int num6 = ordinal6 + 1;
        sqlDataRecord.SetGuid(ordinal6, chartConfiguration.TransformOptions.TransformId.Value);
        int ordinal7 = num6;
        int num7 = ordinal7 + 1;
        sqlDataRecord.SetString(ordinal7, chartConfiguration.TransformOptions.Filter);
        int ordinal8 = num7;
        int num8 = ordinal8 + 1;
        sqlDataRecord.SetString(ordinal8, chartConfiguration.TransformOptions.HistoryRange);
        int ordinal9 = num8;
        int num9 = ordinal9 + 1;
        sqlDataRecord.SetString(ordinal9, chartConfiguration.TransformOptions.GroupBy);
        int ordinal10 = num9;
        int num10 = ordinal10 + 1;
        sqlDataRecord.SetString(ordinal10, chartConfiguration.TransformOptions.OrderBy.PropertyName);
        int ordinal11 = num10;
        int num11 = ordinal11 + 1;
        sqlDataRecord.SetString(ordinal11, chartConfiguration.TransformOptions.OrderBy.Direction);
        int ordinal12 = num11;
        int num12 = ordinal12 + 1;
        sqlDataRecord.SetString(ordinal12, chartConfiguration.TransformOptions.Measure.Aggregation);
        int ordinal13 = num12;
        int num13 = ordinal13 + 1;
        sqlDataRecord.SetString(ordinal13, chartConfiguration.TransformOptions.Series);
        int ordinal14 = num13;
        int num14 = ordinal14 + 1;
        sqlDataRecord.SetString(ordinal14, chartConfiguration.TransformOptions.Measure.PropertyName);
        int ordinal15 = num14;
        int num15 = ordinal15 + 1;
        sqlDataRecord.SetGuid(ordinal15, chartConfiguration.ChangedBy);
        int ordinal16 = num15;
        int num16 = ordinal16 + 1;
        sqlDataRecord.SetDateTime(ordinal16, chartConfiguration.ChangedDate);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_ChartConfigurationTable3", rows.Select<ChartConfigurationDataModel, SqlDataRecord>(selector));
    }
  }
}
