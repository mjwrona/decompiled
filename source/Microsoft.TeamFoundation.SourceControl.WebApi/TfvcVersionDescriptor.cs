// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.TfvcVersionDescriptor
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class TfvcVersionDescriptor
  {
    [DataMember]
    public TfvcVersionOption VersionOption { get; set; }

    [DataMember]
    public TfvcVersionType VersionType { get; set; }

    [DataMember]
    public string Version { get; set; }

    public TfvcVersionDescriptor()
    {
      this.VersionOption = TfvcVersionOption.None;
      this.VersionType = TfvcVersionType.Latest;
    }

    public TfvcVersionDescriptor(
      TfvcVersionOption? versionOption,
      TfvcVersionType? versionType,
      string version)
      : this()
    {
      this.VersionOption = versionOption.GetValueOrDefault(TfvcVersionOption.None);
      this.VersionType = versionType.GetValueOrDefault(TfvcVersionType.Latest);
      if (string.IsNullOrEmpty(version))
        return;
      this.Version = version;
    }
  }
}
