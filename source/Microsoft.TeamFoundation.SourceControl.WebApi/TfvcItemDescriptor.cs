// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.TfvcItemDescriptor
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class TfvcItemDescriptor
  {
    [DataMember]
    public string Path { get; set; }

    [DataMember]
    public string Version { get; set; }

    [DataMember]
    public TfvcVersionOption VersionOption { get; set; }

    [DataMember]
    public TfvcVersionType VersionType { get; set; }

    [DataMember]
    public VersionControlRecursionType RecursionLevel { get; set; }

    public TfvcItemDescriptor()
    {
      this.RecursionLevel = VersionControlRecursionType.OneLevel;
      this.VersionOption = TfvcVersionOption.None;
      this.VersionType = TfvcVersionType.Latest;
    }
  }
}
