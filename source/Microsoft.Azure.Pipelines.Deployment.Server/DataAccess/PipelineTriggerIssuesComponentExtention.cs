// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.DataAccess.PipelineTriggerIssuesComponentExtention
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Model;
using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Deployment.DataAccess
{
  internal static class PipelineTriggerIssuesComponentExtention
  {
    private static readonly SqlMetaData[] sqlMetaData = new SqlMetaData[4]
    {
      new SqlMetaData("Alias", SqlDbType.NVarChar, 256L),
      new SqlMetaData("BuildNumber", SqlDbType.NVarChar, 256L),
      new SqlMetaData("ErrorMessage", SqlDbType.NVarChar, 256L),
      new SqlMetaData("IsError", SqlDbType.Bit)
    };

    public static void BindTriggerTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IList<PipelineTriggerIssues> triggerIssues)
    {
      ArgumentUtility.CheckForNull<TeamFoundationSqlResourceComponent>(component, nameof (component));
      component.BindTable(parameterName, "Deployment.typ_PipelineTriggerIssuesTableV1", PipelineTriggerIssuesComponentExtention.GetTriggerIssuesContent((IEnumerable<PipelineTriggerIssues>) triggerIssues));
    }

    private static IEnumerable<SqlDataRecord> GetTriggerIssuesContent(
      IEnumerable<PipelineTriggerIssues> rows)
    {
      rows = rows ?? Enumerable.Empty<PipelineTriggerIssues>();
      foreach (PipelineTriggerIssues pipelineTriggerIssues in rows.Where<PipelineTriggerIssues>((System.Func<PipelineTriggerIssues, bool>) (r => r != null)))
      {
        int ordinal = 0;
        SqlDataRecord sqlDataRecord = new SqlDataRecord(PipelineTriggerIssuesComponentExtention.sqlMetaData);
        if (!string.IsNullOrWhiteSpace(pipelineTriggerIssues.Alias))
          sqlDataRecord.SetString(ordinal, pipelineTriggerIssues.Alias);
        else
          sqlDataRecord.SetString(ordinal, string.Empty);
        int num1;
        if (!string.IsNullOrWhiteSpace(pipelineTriggerIssues.BuildNumber))
          sqlDataRecord.SetString(num1 = ordinal + 1, pipelineTriggerIssues.BuildNumber);
        else
          sqlDataRecord.SetString(num1 = ordinal + 1, string.Empty);
        int num2;
        if (!string.IsNullOrWhiteSpace(pipelineTriggerIssues.ErrorMessage))
          sqlDataRecord.SetString(num2 = num1 + 1, pipelineTriggerIssues.ErrorMessage);
        else
          sqlDataRecord.SetString(num2 = num1 + 1, string.Empty);
        int num3;
        sqlDataRecord.SetBoolean(num3 = num2 + 1, pipelineTriggerIssues.isError);
        yield return sqlDataRecord;
      }
    }
  }
}
