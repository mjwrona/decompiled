// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.PipelineBuildResult
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Validation;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class PipelineBuildResult
  {
    private readonly ValidationResult m_validationResult;

    public PipelineBuildResult(
      PipelineEnvironment environment,
      PipelineProcess process,
      ValidationResult result)
    {
      this.Environment = environment;
      this.Process = process;
      this.m_validationResult = result;
    }

    public PipelineEnvironment Environment { get; private set; }

    public PipelineProcess Process { get; private set; }

    public IList<PipelineValidationError> Errors => this.m_validationResult.Errors;

    public PipelineResources ReferencedResources => this.m_validationResult.ReferencedResources;

    public PipelineResources UnauthorizedResources => this.m_validationResult.UnauthorizedResources;
  }
}
