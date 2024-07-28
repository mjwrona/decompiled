// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.BuildResource
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class BuildResource : Resource
  {
    public BuildResource()
    {
    }

    protected BuildResource(BuildResource resourceToCopy)
      : base((Resource) resourceToCopy)
    {
    }

    public string Type
    {
      get => this.Properties.Get<string>(BuildPropertyNames.Type);
      set => this.Properties.Set<string>(BuildPropertyNames.Type, value);
    }

    public string Version
    {
      get => this.Properties.Get<string>(BuildPropertyNames.Version);
      set => this.Properties.Set<string>(BuildPropertyNames.Version, value);
    }

    public string VersionName
    {
      get => this.Properties.Get<string>(BuildPropertyNames.VersionName);
      set => this.Properties.Set<string>(BuildPropertyNames.VersionName, value);
    }

    public BuildResourceTrigger Trigger { get; set; }

    public BuildResource Clone() => new BuildResource(this);
  }
}
