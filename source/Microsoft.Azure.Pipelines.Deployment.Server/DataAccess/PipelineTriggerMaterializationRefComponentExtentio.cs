// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.DataAccess.PipelineTriggerMaterializationRefComponentExtention
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Model;
using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.Azure.Pipelines.Deployment.DataAccess
{
  internal static class PipelineTriggerMaterializationRefComponentExtention
  {
    private static readonly SqlMetaData[] sqlMetaData = new SqlMetaData[4]
    {
      new SqlMetaData("YAMLFileName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("CommitId", SqlDbType.NVarChar, 256L),
      new SqlMetaData("RepositoryUrl", SqlDbType.NVarChar, 1048L),
      new SqlMetaData("LastMaterializedDate", SqlDbType.DateTime)
    };

    public static void BindTriggerTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      PipelineTriggerMaterializationRef pipelineTriggerMaterializationRef)
    {
      ArgumentUtility.CheckForNull<TeamFoundationSqlResourceComponent>(component, nameof (component));
      component.BindTable(parameterName, "Deployment.typ_PipelineTriggerMaterializationRefTableV1", PipelineTriggerMaterializationRefComponentExtention.GetTriggerMaterializationRefContent(pipelineTriggerMaterializationRef));
    }

    private static IEnumerable<SqlDataRecord> GetTriggerMaterializationRefContent(
      PipelineTriggerMaterializationRef row)
    {
      int ordinal = 0;
      SqlDataRecord record = new SqlDataRecord(PipelineTriggerMaterializationRefComponentExtention.sqlMetaData);
      if (!string.IsNullOrWhiteSpace(row.YAMLFileName))
        record.SetString(ordinal, row.YAMLFileName);
      else
        record.SetString(ordinal, string.Empty);
      int num1;
      if (!string.IsNullOrWhiteSpace(row.CommitId))
        record.SetString(num1 = ordinal + 1, row.CommitId);
      else
        record.SetString(num1 = ordinal + 1, string.Empty);
      int num2;
      if (!string.IsNullOrWhiteSpace(row.RepositoryUrl))
        record.SetString(num2 = num1 + 1, row.RepositoryUrl);
      else
        record.SetString(num2 = num1 + 1, string.Empty);
      int num3;
      record.SetNullableDateTime(num3 = num2 + 1, row.LastMaterializedDate);
      yield return record;
    }
  }
}
