// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.SupportedIde
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class SupportedIde : VersionControlSecuredObject
  {
    [DataMember]
    public SupportedIdeType IdeType { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string DownloadUrl { get; set; }

    [DataMember]
    public string ProtocolHandlerUrl { get; set; }

    [DataMember]
    public string[] SupportedPlatforms { get; set; }

    public SupportedIde()
      : this(SupportedIdeType.Unknown, string.Empty, string.Empty, Array.Empty<string>(), string.Empty)
    {
    }

    public SupportedIde(
      SupportedIdeType ide,
      string name,
      string url,
      string[] platforms,
      string downloadUrl)
    {
      this.IdeType = ide;
      this.Name = name;
      this.ProtocolHandlerUrl = url;
      this.SupportedPlatforms = platforms;
      this.DownloadUrl = downloadUrl;
    }
  }
}
