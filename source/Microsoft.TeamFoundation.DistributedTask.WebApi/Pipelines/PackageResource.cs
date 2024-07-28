// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.PackageResource
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class PackageResource : Resource
  {
    public PackageResource()
    {
    }

    protected PackageResource(PackageResource resourceToCopy)
      : base((Resource) resourceToCopy)
    {
    }

    public PackageResource Clone() => new PackageResource(this);

    public string Type
    {
      get => this.Properties.Get<string>(PackagePropertyNames.Type);
      set => this.Properties.Set<string>(PackagePropertyNames.Type, value);
    }

    public string Name
    {
      get => this.Properties.Get<string>(PackagePropertyNames.Name);
      set => this.Properties.Set<string>(PackagePropertyNames.Name, value);
    }

    public string Version
    {
      get => this.Properties.Get<string>(PackagePropertyNames.Version);
      set => this.Properties.Set<string>(PackagePropertyNames.Version, value);
    }

    public string Tag
    {
      get => this.Properties.Get<string>(PackagePropertyNames.Tag);
      set => this.Properties.Set<string>(PackagePropertyNames.Tag, value);
    }

    public PackageResourceTrigger Trigger { get; set; }
  }
}
