// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.ContainerResource
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class ContainerResource : Resource
  {
    [JsonConstructor]
    public ContainerResource()
    {
    }

    private ContainerResource(ContainerResource referenceToCopy)
      : base((Resource) referenceToCopy)
    {
    }

    public IDictionary<string, string> Environment
    {
      get => this.Properties.Get<IDictionary<string, string>>("env");
      set => this.Properties.Set<IDictionary<string, string>>("env", value);
    }

    public bool MapDockerSocket
    {
      get => this.Properties.Get<bool>("mapDockerSocket");
      set => this.Properties.Set<bool>("mapDockerSocket", value);
    }

    public string Image
    {
      get => this.Properties.Get<string>("image");
      set => this.Properties.Set<string>("image", value);
    }

    public string Options
    {
      get => this.Properties.Get<string>("options");
      set => this.Properties.Set<string>("options", value);
    }

    public ContainerResourceTrigger Trigger { get; set; }

    public IList<string> Volumes
    {
      get => this.Properties.Get<IList<string>>("volumes");
      set => this.Properties.Set<IList<string>>("volumes", value);
    }

    public IList<string> Ports
    {
      get => this.Properties.Get<IList<string>>("ports");
      set => this.Properties.Set<IList<string>>("ports", value);
    }

    public IList<string> ReadOnlyMounts
    {
      get => this.Properties.Get<IList<string>>("readOnlyMounts");
      set => this.Properties.Set<IList<string>>("readOnlyMounts", value);
    }

    public ContainerResource Clone() => new ContainerResource(this);
  }
}
