// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Model.PipelineDefinitionTrigger
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Extensions;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using System;

namespace Microsoft.Azure.Pipelines.Deployment.Model
{
  public class PipelineDefinitionTrigger : IEquatable<PipelineDefinitionTrigger>
  {
    public string Alias { get; set; }

    public ProjectInfo Project { get; set; }

    public ArtifactDefinitionReference ArtifactDefinition { get; set; }

    public int PipelineDefinitionId { get; set; }

    public PipelineTrigger TriggerContent { get; set; }

    public PipelineTriggerType TriggerType { get; set; }

    public bool Equals(PipelineDefinitionTrigger other)
    {
      if (other == null || !string.Equals(this.Alias, other.Alias, StringComparison.OrdinalIgnoreCase) || this.PipelineDefinitionId != other.PipelineDefinitionId || this.TriggerType != other.TriggerType || this.ArtifactDefinition == null && other.ArtifactDefinition != null || this.ArtifactDefinition != null && other.ArtifactDefinition == null || this.ArtifactDefinition != null && other.ArtifactDefinition != null && !this.ArtifactDefinition.Equals(other.ArtifactDefinition) || this.TriggerContent == null && other.TriggerContent != null || this.TriggerContent != null && other.TriggerContent == null)
        return false;
      if (this.TriggerContent == null || other.TriggerContent == null)
        return true;
      if (this.TriggerType == PipelineTriggerType.PipelineCompletion)
        return this.TriggerContent.ToPipelineResourceTrigger().Equals(other.TriggerContent.ToPipelineResourceTrigger());
      if (this.TriggerType == PipelineTriggerType.ContainerImage)
        return this.TriggerContent.ToContainerResourceTrigger().Equals(other.TriggerContent.ToContainerResourceTrigger());
      if (this.TriggerType == PipelineTriggerType.BuildResourceCompletion)
        return this.TriggerContent.ToBuildResourceTrigger().Equals(other.TriggerContent.ToBuildResourceTrigger());
      if (this.TriggerType == PipelineTriggerType.PackageUpdate)
        return this.TriggerContent.ToPackageResourceTrigger().Equals(other.TriggerContent.ToPackageResourceTrigger());
      return this.TriggerType == PipelineTriggerType.WebhookTriggeredEvent && this.TriggerContent.ToWebHookResourceTrigger().Equals(other.TriggerContent.ToWebHookResourceTrigger());
    }

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      if (this == obj)
        return true;
      return !(obj.GetType() != this.GetType()) && this.Equals((PipelineDefinitionTrigger) obj);
    }

    public override int GetHashCode() => (int) ((PipelineTriggerType) (((((this.Alias != null ? this.Alias.GetHashCode() : 0) * 397 ^ (this.ArtifactDefinition != null ? this.ArtifactDefinition.GetHashCode() : 0)) * 397 ^ this.PipelineDefinitionId) * 397 ^ (this.TriggerContent != null ? this.TriggerContent.GetHashCode() : 0)) * 397) ^ this.TriggerType);
  }
}
