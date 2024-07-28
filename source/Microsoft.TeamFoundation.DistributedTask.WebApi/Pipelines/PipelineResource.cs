// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.PipelineResource
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class PipelineResource : Resource
  {
    public PipelineResource()
    {
    }

    protected PipelineResource(PipelineResource resourceToCopy)
      : base((Resource) resourceToCopy)
    {
    }

    public PipelineResourceTrigger Trigger { get; set; }

    public string Source
    {
      get => this.Properties.Get<string>(PipelinePropertyNames.Source);
      set => this.Properties.Set<string>(PipelinePropertyNames.Source, value);
    }

    public string Version
    {
      get => this.Properties.Get<string>(PipelinePropertyNames.Version);
      set => this.Properties.Set<string>(PipelinePropertyNames.Version, value);
    }

    public PipelineResource Clone() => new PipelineResource(this);
  }
}
