// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Step
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  [KnownType(typeof (TaskStep))]
  [KnownType(typeof (TaskTemplateStep))]
  [KnownType(typeof (GroupStep))]
  [JsonConverter(typeof (StepConverter))]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class Step
  {
    protected Step() => this.Enabled = true;

    protected Step(Step stepToClone)
    {
      this.Enabled = stepToClone.Enabled;
      this.Id = stepToClone.Id;
      this.Name = stepToClone.Name;
      this.DisplayName = stepToClone.DisplayName;
    }

    [DataMember(EmitDefaultValue = false)]
    public abstract StepType Type { get; }

    [DataMember(EmitDefaultValue = false)]
    public Guid Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string DisplayName { get; set; }

    [DefaultValue(true)]
    [DataMember(EmitDefaultValue = false)]
    public bool Enabled { get; set; }

    public abstract Step Clone();
  }
}
