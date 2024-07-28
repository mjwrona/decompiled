// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.Model.PipelineTriggerIssues
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using System;

namespace Microsoft.Azure.Pipelines.Deployment.Model
{
  public class PipelineTriggerIssues
  {
    public int PipelineDefinitionId { get; set; }

    public string Alias { get; set; }

    public string BuildNumber { get; set; }

    public string SourceVersion { get; set; }

    public string RepositoryUrl { get; set; }

    public string ErrorMessage { get; set; }

    public DateTime CreatedOn { get; set; }

    public bool isError { get; set; }
  }
}
