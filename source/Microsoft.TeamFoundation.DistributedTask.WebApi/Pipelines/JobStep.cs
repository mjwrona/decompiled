// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.JobStep
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class JobStep : Step
  {
    [JsonConstructor]
    public JobStep() => this.Enabled = true;

    protected JobStep(JobStep stepToClone)
      : base((Step) stepToClone)
    {
      this.Condition = stepToClone.Condition;
      this.ContinueOnError = stepToClone.ContinueOnError;
      this.TimeoutInMinutes = stepToClone.TimeoutInMinutes;
      this.RetryCountOnTaskFailure = stepToClone.RetryCountOnTaskFailure;
      this.Target = stepToClone.Target;
    }

    [DataMember(EmitDefaultValue = false)]
    public string Condition { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool ContinueOnError { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public StepTarget Target { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int TimeoutInMinutes { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int RetryCountOnTaskFailure { get; set; }
  }
}
