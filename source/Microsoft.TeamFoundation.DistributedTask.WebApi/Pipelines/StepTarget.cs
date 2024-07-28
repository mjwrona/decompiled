// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.StepTarget
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class StepTarget
  {
    [JsonConstructor]
    public StepTarget()
    {
    }

    protected StepTarget(StepTarget targetToClone)
    {
      this.Target = targetToClone.Target;
      this.Commands = targetToClone.Commands;
      this.SettableVariables = targetToClone.SettableVariables?.Clone();
    }

    [DataMember(EmitDefaultValue = false)]
    public string Target { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Commands { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public TaskVariableRestrictions SettableVariables { get; set; }
  }
}
