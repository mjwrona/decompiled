// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.DataAccess.PipelineTriggerComponentExtention2
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
  internal static class PipelineTriggerComponentExtention2
  {
    private static readonly SqlMetaData[] sqlMetaData2 = new SqlMetaData[7]
    {
      new SqlMetaData("Alias", SqlDbType.NVarChar, 256L),
      new SqlMetaData("TriggerType", SqlDbType.Int),
      new SqlMetaData("TriggerContent", SqlDbType.NVarChar, 4000L),
      new SqlMetaData("ArtifactType", SqlDbType.NVarChar, 256L),
      new SqlMetaData("UniqueResourceIdentifier", SqlDbType.NVarChar, 2048L),
      new SqlMetaData("Connection", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Properties", SqlDbType.NVarChar, 4000L)
    };

    public static void BindTriggerTable2(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IList<PipelineDefinitionTrigger> triggers)
    {
      ArgumentUtility.CheckForNull<TeamFoundationSqlResourceComponent>(component, nameof (component));
      component.BindTable(parameterName, "Deployment.typ_PipelineTriggerTableV2", PipelineTriggerComponentExtention2.GetTriggerContent((IEnumerable<PipelineDefinitionTrigger>) triggers));
    }

    private static IEnumerable<SqlDataRecord> GetTriggerContent(
      IEnumerable<PipelineDefinitionTrigger> rows)
    {
      rows = rows ?? Enumerable.Empty<PipelineDefinitionTrigger>();
      foreach (PipelineDefinitionTrigger trigger in rows.Where<PipelineDefinitionTrigger>((System.Func<PipelineDefinitionTrigger, bool>) (r => r != null)))
      {
        int ordinal = 0;
        SqlDataRecord sqlDataRecord = new SqlDataRecord(PipelineTriggerComponentExtention2.sqlMetaData2);
        sqlDataRecord.SetString(ordinal, trigger.Alias);
        int num1;
        sqlDataRecord.SetInt32(num1 = ordinal + 1, (int) trigger.TriggerType);
        int num2;
        sqlDataRecord.SetString(num2 = num1 + 1, PipelineTriggerComponentExtention2.GetTriggerContent(trigger));
        int num3;
        sqlDataRecord.SetString(num3 = num2 + 1, trigger.ArtifactDefinition.ArtifactType);
        int num4;
        sqlDataRecord.SetString(num4 = num3 + 1, trigger.ArtifactDefinition.UniqueResourceIdentifier);
        int num5;
        sqlDataRecord.SetGuid(num5 = num4 + 1, trigger.ArtifactDefinition.Connection);
        if (trigger.ArtifactDefinition.Properties != null && trigger.ArtifactDefinition.Properties.Any<KeyValuePair<string, string>>())
        {
          int num6;
          sqlDataRecord.SetString(num6 = num5 + 1, JsonConvert.SerializeObject((object) trigger.ArtifactDefinition.Properties));
        }
        yield return sqlDataRecord;
      }
    }

    private static string GetTriggerContent(PipelineDefinitionTrigger trigger)
    {
      string triggerContent = string.Empty;
      switch (trigger.TriggerType)
      {
        case PipelineTriggerType.PipelineCompletion:
          triggerContent = JsonConvert.SerializeObject((object) trigger.TriggerContent.ToPipelineResourceTrigger());
          break;
        case PipelineTriggerType.ContainerImage:
          triggerContent = JsonConvert.SerializeObject((object) trigger.TriggerContent.ToContainerResourceTrigger());
          break;
        case PipelineTriggerType.BuildResourceCompletion:
          triggerContent = JsonConvert.SerializeObject((object) trigger.TriggerContent.ToBuildResourceTrigger());
          break;
        case PipelineTriggerType.PackageUpdate:
          triggerContent = JsonConvert.SerializeObject((object) trigger.TriggerContent.ToPackageResourceTrigger());
          break;
        case PipelineTriggerType.WebhookTriggeredEvent:
          triggerContent = JsonConvert.SerializeObject((object) trigger.TriggerContent.ToWebHookResourceTrigger());
          break;
      }
      return triggerContent;
    }
  }
}
