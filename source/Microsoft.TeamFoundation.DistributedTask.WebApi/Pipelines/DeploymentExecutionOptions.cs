// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.DeploymentExecutionOptions
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Validation;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  internal class DeploymentExecutionOptions
  {
    public DeploymentExecutionOptions()
    {
    }

    private DeploymentExecutionOptions(DeploymentExecutionOptions optionsToCopy)
    {
      this.RollingOption = optionsToCopy.RollingOption;
      this.RollingValue = optionsToCopy.RollingValue;
    }

    [DataMember]
    public DeploymentRollingOption RollingOption { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public uint RollingValue { get; set; }

    public DeploymentExecutionOptions Clone() => new DeploymentExecutionOptions(this);

    public void Validate(IPipelineContext context, ValidationResult result)
    {
      switch (this.RollingOption)
      {
        case DeploymentRollingOption.Absolute:
          if (this.RollingValue != 0U)
            break;
          result.Errors.Add(new PipelineValidationError(PipelineStrings.InvalidAbsoluteRollingValue()));
          break;
        case DeploymentRollingOption.Percentage:
          if (this.RollingValue != 0U && this.RollingValue <= 100U)
            break;
          result.Errors.Add(new PipelineValidationError(PipelineStrings.InvalidPercentageRollingValue()));
          break;
        default:
          result.Errors.Add(new PipelineValidationError(PipelineStrings.InvalidRollingOption((object) this.RollingOption)));
          break;
      }
    }
  }
}
