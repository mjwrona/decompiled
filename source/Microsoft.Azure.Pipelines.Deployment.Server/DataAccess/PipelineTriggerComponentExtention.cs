// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.DataAccess.PipelineTriggerComponentExtention
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Extensions;
using Microsoft.Azure.Pipelines.Deployment.Model;
using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.Azure.Pipelines.Deployment.DataAccess
{
  internal static class PipelineTriggerComponentExtention
  {
    private static readonly SqlMetaData[] sqlMetaData = new SqlMetaData[6]
    {
      new SqlMetaData("Alias", SqlDbType.NVarChar, 256L),
      new SqlMetaData("TriggerType", SqlDbType.Int),
      new SqlMetaData("TriggerContent", SqlDbType.NVarChar, 4000L),
      new SqlMetaData("ArtifactType", SqlDbType.NVarChar, 256L),
      new SqlMetaData("UniqueResourceIdentifier", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("Connection", SqlDbType.UniqueIdentifier)
    };

    public static void BindTriggerTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IList<PipelineDefinitionTrigger> triggers)
    {
      ArgumentUtility.CheckForNull<TeamFoundationSqlResourceComponent>(component, nameof (component));
      component.BindTable(parameterName, "Deployment.typ_PipelineTriggerTableV1", PipelineTriggerComponentExtention.GetTriggerContent((IEnumerable<PipelineDefinitionTrigger>) triggers));
    }

    private static IEnumerable<SqlDataRecord> GetTriggerContent(
      IEnumerable<PipelineDefinitionTrigger> rows)
    {
      rows = rows ?? Enumerable.Empty<PipelineDefinitionTrigger>();
      foreach (PipelineDefinitionTrigger trigger in rows.Where<PipelineDefinitionTrigger>((System.Func<PipelineDefinitionTrigger, bool>) (r => r != null)))
      {
        int ordinal = 0;
        SqlDataRecord sqlDataRecord = new SqlDataRecord(PipelineTriggerComponentExtention.sqlMetaData);
        sqlDataRecord.SetString(ordinal, trigger.Alias);
        int num1;
        sqlDataRecord.SetInt32(num1 = ordinal + 1, (int) trigger.TriggerType);
        int num2;
        sqlDataRecord.SetString(num2 = num1 + 1, PipelineTriggerComponentExtention.GetTriggerContent(trigger));
        int num3;
        sqlDataRecord.SetString(num3 = num2 + 1, trigger.ArtifactDefinition.ArtifactType);
        int num4;
        sqlDataRecord.SetString(num4 = num3 + 1, trigger.ArtifactDefinition.UniqueResourceIdentifier);
        int num5;
        sqlDataRecord.SetGuid(num5 = num4 + 1, trigger.ArtifactDefinition.Connection);
        yield return sqlDataRecord;
      }
    }

    private static string GetTriggerContent(PipelineDefinitionTrigger trigger)
    {
      string triggerContent = string.Empty;
      if (trigger.TriggerType == PipelineTriggerType.PipelineCompletion)
        triggerContent = JsonConvert.SerializeObject((object) trigger.TriggerContent.ToPipelineResourceTrigger());
      return triggerContent;
    }
  }
}
