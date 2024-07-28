// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.DataAccess.ChartConfigurationSqlResourceComponent
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Charting.WebApi;
using Microsoft.TeamFoundation.Framework.Client;
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
  public class ChartConfigurationSqlResourceComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[7]
    {
      (IComponentCreator) new ComponentCreator<ChartConfigurationSqlResourceComponent>(1, true),
      (IComponentCreator) new ComponentCreator<ChartConfigurationSqlResourceComponent2>(2),
      (IComponentCreator) new ComponentCreator<ChartConfigurationSqlResourceComponent3>(3),
      (IComponentCreator) new ComponentCreator<ChartConfigurationSqlResourceComponent4>(4),
      (IComponentCreator) new ComponentCreator<ChartConfigurationSqlResourceComponent5>(5),
      (IComponentCreator) new ComponentCreator<ChartConfigurationSqlResourceComponent6>(6),
      (IComponentCreator) new ComponentCreator<ChartConfigurationSqlResourceComponent7>(7)
    }, "ChartConfiguration");
    private static readonly SqlMetaData[] typ_ChartConfigurationTable = new SqlMetaData[15]
    {
      new SqlMetaData("ChartId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("GroupKey", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Scope", SqlDbType.NVarChar, 50L),
      new SqlMetaData("ChartType", SqlDbType.NVarChar, 50L),
      new SqlMetaData("Title", SqlDbType.NVarChar, 256L),
      new SqlMetaData("TransformId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Filter", SqlDbType.NVarChar, 256L),
      new SqlMetaData("GroupBy", SqlDbType.NVarChar, 256L),
      new SqlMetaData("OrderPropertyName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("OrderDirection", SqlDbType.NVarChar, 50L),
      new SqlMetaData("MeasureAggregation", SqlDbType.NVarChar, 50L),
      new SqlMetaData("Series", SqlDbType.NVarChar, 256L),
      new SqlMetaData("MeasurePropertyName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("ChangedBy", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ChangedDate", SqlDbType.DateTime)
    };
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>()
    {
      {
        1400001,
        new SqlExceptionFactory(typeof (ChartConfigurationAlreadyCreatedException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new ChartConfigurationAlreadyCreatedException(Guid.Parse(sqEr.ExtractString("chartId")))))
      },
      {
        1400002,
        new SqlExceptionFactory(typeof (TransformOptionsAlreadyCreatedException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new TransformOptionsAlreadyCreatedException(Guid.Parse(sqEr.ExtractString("transformId")))))
      },
      {
        1400003,
        new SqlExceptionFactory(typeof (InvalidChartConfigurationException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new InvalidChartConfigurationException(sqEr.Message)))
      },
      {
        1400004,
        new SqlExceptionFactory(typeof (TransformOptionsDoesNotExistException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new TransformOptionsDoesNotExistException(Guid.Parse(sqEr.ExtractString("transformId")))))
      },
      {
        1400005,
        new SqlExceptionFactory(typeof (ChartConfigurationDoesNotExistException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new ChartConfigurationDoesNotExistException(Guid.Parse(sqEr.ExtractString("chartId")))))
      },
      {
        1400006,
        new SqlExceptionFactory(typeof (InvalidColorConfigurationException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new InvalidColorConfigurationException(Guid.Parse(sqEr.ExtractString("chartId")))))
      },
      {
        1400007,
        new SqlExceptionFactory(typeof (TooManyChartsPerGroupException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new TooManyChartsPerGroupException()))
      },
      {
        400017,
        new SqlExceptionFactory(typeof (InternalStoredProcedureException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new InternalStoredProcedureException(sqEr.Message)))
      }
    };

    public ChartConfigurationSqlResourceComponent()
    {
      this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;
      this.ContainerErrorCode = 50000;
    }

    public virtual ChartConfiguration AddChartConfiguration(
      Guid projectId,
      ChartConfigurationDataModel chartConfiguration)
    {
      this.PrepareStoredProcedure("prc_AddChartConfiguration");
      this.BindChartConfigurationTable("@chartConfigurationTable", chartConfiguration);
      this.BindProjectId(projectId);
      this.ExecuteNonQuery();
      return (ChartConfiguration) chartConfiguration;
    }

    public virtual void UpdateChartConfiguration(
      Guid projectId,
      ChartConfigurationDataModel chartConfiguration)
    {
      this.PrepareStoredProcedure("prc_UpdateChartConfiguration");
      this.BindChartConfigurationTable("@chartConfigurationTable", chartConfiguration);
      this.BindProjectId(projectId);
      this.ExecuteNonQuery();
    }

    public virtual void DeleteChartConfiguration(Guid projectId, Guid id)
    {
      this.PrepareStoredProcedure("prc_DeleteChartConfiguration");
      this.BindGuid("@chartId", id);
      this.BindProjectId(projectId);
      this.ExecuteNonQuery();
    }

    public virtual void DeleteChartConfigurationsByGroups(
      Guid projectId,
      string scope,
      IEnumerable<string> groupKeys)
    {
    }

    public virtual IEnumerable<Guid> GetChartConfigurationIdsByGroups(
      Guid projectId,
      string scope,
      IEnumerable<string> groupKeys)
    {
      return (IEnumerable<Guid>) new List<Guid>();
    }

    public virtual IEnumerable<ChartConfiguration> GetChartConfigurationsByGroup(
      Guid projectId,
      string scope,
      string groupKey)
    {
      this.PrepareStoredProcedure("prc_GetChartConfigurationsByGroup");
      this.BindString("@scope", scope, (int) byte.MaxValue, false, SqlDbType.NVarChar);
      this.BindString("@groupKey", groupKey, (int) byte.MaxValue, false, SqlDbType.NVarChar);
      this.BindProjectId(projectId);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<ChartConfiguration>((ObjectBinder<ChartConfiguration>) new ChartConfigurationBinder2());
      return (IEnumerable<ChartConfiguration>) resultCollection.GetCurrent<ChartConfiguration>().Items;
    }

    public virtual ChartConfiguration GetChartConfigurationById(Guid projectId, Guid id)
    {
      this.PrepareStoredProcedure("prc_GetChartConfigurationById");
      this.BindGuid("@chartId", id);
      this.BindProjectId(projectId);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<ChartConfiguration>((ObjectBinder<ChartConfiguration>) new ChartConfigurationBinder2());
      List<ChartConfiguration> items = resultCollection.GetCurrent<ChartConfiguration>().Items;
      return !items.Any<ChartConfiguration>() ? (ChartConfiguration) null : items[0];
    }

    protected virtual SqlParameter BindChartConfigurationTable(
      string parameterName,
      IEnumerable<ChartConfigurationDataModel> rows)
    {
      rows = rows ?? Enumerable.Empty<ChartConfigurationDataModel>();
      System.Func<ChartConfigurationDataModel, SqlDataRecord> selector = (System.Func<ChartConfigurationDataModel, SqlDataRecord>) (chartConfiguration =>
      {
        ArgumentUtility.CheckForNull<ChartConfigurationDataModel>(chartConfiguration, nameof (chartConfiguration));
        ArgumentUtility.CheckForNull<TransformOptionsDataModel>(chartConfiguration.TransformOptions, "chartConfiguration.TransformOptions");
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ChartConfigurationSqlResourceComponent.typ_ChartConfigurationTable);
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
        sqlDataRecord.SetString(ordinal8, chartConfiguration.TransformOptions.GroupBy);
        int ordinal9 = num8;
        int num9 = ordinal9 + 1;
        sqlDataRecord.SetString(ordinal9, chartConfiguration.TransformOptions.OrderBy.PropertyName);
        int ordinal10 = num9;
        int num10 = ordinal10 + 1;
        sqlDataRecord.SetString(ordinal10, chartConfiguration.TransformOptions.OrderBy.Direction);
        int ordinal11 = num10;
        int num11 = ordinal11 + 1;
        sqlDataRecord.SetString(ordinal11, chartConfiguration.TransformOptions.Measure.Aggregation);
        int ordinal12 = num11;
        int num12 = ordinal12 + 1;
        sqlDataRecord.SetString(ordinal12, chartConfiguration.TransformOptions.Series);
        int ordinal13 = num12;
        int num13 = ordinal13 + 1;
        sqlDataRecord.SetString(ordinal13, chartConfiguration.TransformOptions.Measure.PropertyName);
        int ordinal14 = num13;
        int num14 = ordinal14 + 1;
        sqlDataRecord.SetGuid(ordinal14, chartConfiguration.ChangedBy);
        int ordinal15 = num14;
        int num15 = ordinal15 + 1;
        sqlDataRecord.SetDateTime(ordinal15, chartConfiguration.ChangedDate);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_ChartConfigurationTable", rows.Select<ChartConfigurationDataModel, SqlDataRecord>(selector));
    }

    protected SqlParameter BindChartConfigurationTable(
      string parameterName,
      ChartConfigurationDataModel row)
    {
      return this.BindChartConfigurationTable(parameterName, (IEnumerable<ChartConfigurationDataModel>) new List<ChartConfigurationDataModel>()
      {
        row
      });
    }

    protected virtual SqlParameter BindProjectId(Guid projectId) => (SqlParameter) null;

    protected override string TraceArea => "ChartConfigurationResourceComponent";

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) ChartConfigurationSqlResourceComponent.s_sqlExceptionFactories;
  }
}
