// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.DeploymentGroupMetricsBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal sealed class DeploymentGroupMetricsBinder : ObjectBinder<DeploymentGroupMetrics>
  {
    private readonly IDictionary<int, DeploymentGroup> m_deploymentGroupsMap;
    private SqlColumnBinder m_queueId = new SqlColumnBinder("QueueId");
    private SqlColumnBinder m_onlineAndLastJobSuccessfulCount = new SqlColumnBinder("OnlineAndLastJobSuccessfulCount");
    private SqlColumnBinder m_onlineAndLastJobNotSuccessfulCount = new SqlColumnBinder("OnlineAndLastJobNotSuccessfulCount");
    private SqlColumnBinder m_onlineAndNoJobStatusCount = new SqlColumnBinder("OnlineAndNoJobStatusCount");
    private SqlColumnBinder m_offlineAndLastJobSuccessfulCount = new SqlColumnBinder("OfflineAndLastJobSuccessfulCount");
    private SqlColumnBinder m_offlineAndLastJobNotSuccessfulCount = new SqlColumnBinder("OfflineAndLastJobNotSuccessfulCount");
    private SqlColumnBinder m_offlineAndNoJobStatusCount = new SqlColumnBinder("OfflineAndNoJobStatusCount");
    private static readonly Lazy<List<string>> s_onlineAndLastJobSuccessDimensionsList = new Lazy<List<string>>((Func<List<string>>) (() => new List<string>()
    {
      "Online",
      "Succeeded"
    }));
    private static readonly Lazy<List<string>> s_onlineAndLastJobNotSuccessfulDimensionsList = new Lazy<List<string>>((Func<List<string>>) (() => new List<string>()
    {
      "Online",
      "Not succeeded"
    }));
    private static readonly Lazy<List<string>> s_onlineAndNoJobStatusDimensionsList = new Lazy<List<string>>((Func<List<string>>) (() => new List<string>()
    {
      "Online",
      "Not deployed"
    }));
    private static readonly Lazy<List<string>> s_offlineAndLastJobSuccessDimensionsList = new Lazy<List<string>>((Func<List<string>>) (() => new List<string>()
    {
      "Offline",
      "Succeeded"
    }));
    private static readonly Lazy<List<string>> s_offlineAndLastJobNotSuccessfulDimensionsList = new Lazy<List<string>>((Func<List<string>>) (() => new List<string>()
    {
      "Offline",
      "Not succeeded"
    }));
    private static readonly Lazy<List<string>> s_offlineAndNoJobStatusDimensionsList = new Lazy<List<string>>((Func<List<string>>) (() => new List<string>()
    {
      "Offline",
      "Not deployed"
    }));
    private static readonly Lazy<MetricsColumnsHeader> s_metricsColumnsHeadernew = new Lazy<MetricsColumnsHeader>((Func<MetricsColumnsHeader>) (() => new MetricsColumnsHeader()
    {
      Dimensions = (IList<MetricsColumnMetaData>) new List<MetricsColumnMetaData>()
      {
        new MetricsColumnMetaData()
        {
          ColumnName = "DeploymentTargetState",
          ColumnValueType = "string"
        },
        new MetricsColumnMetaData()
        {
          ColumnName = "LastDeploymentStatus",
          ColumnValueType = "string"
        }
      },
      Metrics = (IList<MetricsColumnMetaData>) new List<MetricsColumnMetaData>()
      {
        new MetricsColumnMetaData()
        {
          ColumnName = "TotalDeploymentTargetCount",
          ColumnValueType = "number"
        }
      }
    }));

    public DeploymentGroupMetricsBinder(
      IDictionary<int, DeploymentGroup> deploymentGroupsMap)
    {
      this.m_deploymentGroupsMap = deploymentGroupsMap;
    }

    protected override DeploymentGroupMetrics Bind()
    {
      DeploymentGroup deploymentGroups = this.m_deploymentGroupsMap[this.m_queueId.GetInt32((IDataReader) this.Reader)];
      return new DeploymentGroupMetrics()
      {
        DeploymentGroup = deploymentGroups.AsReference(),
        ColumnsHeader = DeploymentGroupMetricsBinder.GetDeploymentGroupsMetricsColumnsHeader(),
        Rows = (IList<MetricsRow>) new List<MetricsRow>()
        {
          new MetricsRow()
          {
            Dimensions = (IList<string>) DeploymentGroupMetricsBinder.s_onlineAndLastJobSuccessDimensionsList.Value,
            Metrics = (IList<string>) new List<string>()
            {
              this.m_onlineAndLastJobSuccessfulCount.GetInt32((IDataReader) this.Reader).ToString()
            }
          },
          new MetricsRow()
          {
            Dimensions = (IList<string>) DeploymentGroupMetricsBinder.s_onlineAndLastJobNotSuccessfulDimensionsList.Value,
            Metrics = (IList<string>) new List<string>()
            {
              this.m_onlineAndLastJobNotSuccessfulCount.GetInt32((IDataReader) this.Reader).ToString()
            }
          },
          new MetricsRow()
          {
            Dimensions = (IList<string>) DeploymentGroupMetricsBinder.s_onlineAndNoJobStatusDimensionsList.Value,
            Metrics = (IList<string>) new List<string>()
            {
              this.m_onlineAndNoJobStatusCount.GetInt32((IDataReader) this.Reader).ToString()
            }
          },
          new MetricsRow()
          {
            Dimensions = (IList<string>) DeploymentGroupMetricsBinder.s_offlineAndLastJobSuccessDimensionsList.Value,
            Metrics = (IList<string>) new List<string>()
            {
              this.m_offlineAndLastJobSuccessfulCount.GetInt32((IDataReader) this.Reader).ToString()
            }
          },
          new MetricsRow()
          {
            Dimensions = (IList<string>) DeploymentGroupMetricsBinder.s_offlineAndLastJobNotSuccessfulDimensionsList.Value,
            Metrics = (IList<string>) new List<string>()
            {
              this.m_offlineAndLastJobNotSuccessfulCount.GetInt32((IDataReader) this.Reader).ToString()
            }
          },
          new MetricsRow()
          {
            Dimensions = (IList<string>) DeploymentGroupMetricsBinder.s_offlineAndNoJobStatusDimensionsList.Value,
            Metrics = (IList<string>) new List<string>()
            {
              this.m_offlineAndNoJobStatusCount.GetInt32((IDataReader) this.Reader).ToString()
            }
          }
        }
      };
    }

    private static MetricsColumnsHeader GetDeploymentGroupsMetricsColumnsHeader() => DeploymentGroupMetricsBinder.s_metricsColumnsHeadernew.Value;
  }
}
