// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.DataAccess.PipelineDefinitionTriggerBinder2
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Model;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.Azure.Pipelines.Deployment.DataAccess
{
  public class PipelineDefinitionTriggerBinder2 : ObjectBinder<PipelineDefinitionTrigger>
  {
    private SqlColumnBinder m_alias = new SqlColumnBinder("Alias");
    private SqlColumnBinder m_pipelineDefinitionId = new SqlColumnBinder("PipelineDefinitionId");
    private SqlColumnBinder m_triggerContent = new SqlColumnBinder("TriggerContent");
    private SqlColumnBinder m_triggerType = new SqlColumnBinder("TriggerType");
    private SqlColumnBinder m_UniqueResourceIdentifier = new SqlColumnBinder("UniqueResourceIdentifier");
    private SqlColumnBinder m_ConnectionId = new SqlColumnBinder("ConnectionId");
    private SqlColumnBinder m_artifactType = new SqlColumnBinder("ArtifactType");
    private SqlColumnBinder m_properties = new SqlColumnBinder("Properties");

    protected override PipelineDefinitionTrigger Bind()
    {
      PipelineDefinitionTrigger definitionTrigger = new PipelineDefinitionTrigger();
      definitionTrigger.PipelineDefinitionId = this.m_pipelineDefinitionId.GetInt32((IDataReader) this.Reader);
      definitionTrigger.TriggerType = (PipelineTriggerType) this.m_triggerType.GetInt32((IDataReader) this.Reader);
      definitionTrigger.Alias = this.m_alias.GetString((IDataReader) this.Reader, false);
      string json = this.m_triggerContent.GetString((IDataReader) this.Reader, true);
      switch (definitionTrigger.TriggerType)
      {
        case PipelineTriggerType.PipelineCompletion:
          definitionTrigger.TriggerContent = string.IsNullOrEmpty(json) ? (PipelineTrigger) null : (PipelineTrigger) JsonUtilities.Deserialize<PipelineResourceTrigger>(json);
          break;
        case PipelineTriggerType.ContainerImage:
          definitionTrigger.TriggerContent = string.IsNullOrEmpty(json) ? (PipelineTrigger) null : (PipelineTrigger) JsonUtilities.Deserialize<ContainerResourceTrigger>(json);
          break;
        case PipelineTriggerType.BuildResourceCompletion:
          definitionTrigger.TriggerContent = string.IsNullOrEmpty(json) ? (PipelineTrigger) null : (PipelineTrigger) JsonUtilities.Deserialize<BuildResourceTrigger>(json);
          break;
        case PipelineTriggerType.PackageUpdate:
          definitionTrigger.TriggerContent = string.IsNullOrEmpty(json) ? (PipelineTrigger) null : (PipelineTrigger) JsonUtilities.Deserialize<PackageResourceTrigger>(json);
          break;
        case PipelineTriggerType.WebhookTriggeredEvent:
          definitionTrigger.TriggerContent = string.IsNullOrEmpty(json) ? (PipelineTrigger) null : (PipelineTrigger) JsonUtilities.Deserialize<WebhookResourceTrigger>(json);
          break;
      }
      string str1 = this.m_artifactType.GetString((IDataReader) this.Reader, false);
      string str2 = this.m_UniqueResourceIdentifier.GetString((IDataReader) this.Reader, false);
      Guid guid = this.m_ConnectionId.GetGuid((IDataReader) this.Reader, true);
      IDictionary<string, string> dictionary = JsonConvert.DeserializeObject<IDictionary<string, string>>(this.m_properties.GetString((IDataReader) this.Reader, true) ?? string.Empty) ?? (IDictionary<string, string>) new Dictionary<string, string>();
      definitionTrigger.ArtifactDefinition = new ArtifactDefinitionReference()
      {
        ArtifactType = str1,
        UniqueResourceIdentifier = str2,
        Connection = guid,
        Properties = (IDictionary<string, string>) new Dictionary<string, string>(dictionary, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
      };
      return definitionTrigger;
    }
  }
}
