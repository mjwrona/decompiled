// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.PipelineGraphNodeReference
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [DataContract]
  public class PipelineGraphNodeReference
  {
    public PipelineGraphNodeReference()
    {
    }

    public PipelineGraphNodeReference(string id, string name, int attempt = 0)
    {
      this.Id = id;
      this.Name = name;
      this.Attempt = attempt;
    }

    public PipelineGraphNodeReference(Guid id, string name, int attempt = 0)
    {
      this.Id = id.ToString("D");
      this.Name = name;
      this.Attempt = attempt;
    }

    public PipelineGraphNodeReference(int id, string name, int attempt = 0)
    {
      this.Id = id.ToString();
      this.Name = name;
      this.Attempt = attempt;
    }

    [DataMember(IsRequired = true)]
    public string Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int Attempt { get; set; }
  }
}
