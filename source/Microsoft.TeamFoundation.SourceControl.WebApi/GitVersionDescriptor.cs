// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.GitVersionDescriptor
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class GitVersionDescriptor : VersionControlSecuredObject
  {
    [DataMember(EmitDefaultValue = false)]
    public virtual GitVersionType VersionType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public virtual string Version { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public virtual GitVersionOptions VersionOptions { get; set; }

    public override string ToString() => string.Format("<{0}: {1} {2}>", (object) this.VersionType.ToString(), (object) this.Version, this.VersionOptions == GitVersionOptions.None ? (object) string.Empty : (object) this.VersionOptions.ToString());
  }
}
