// Decompiled with JetBrains decompiler
// Type: Nest.XPackInfoResponse
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Runtime.Serialization;

namespace Nest
{
  public class XPackInfoResponse : ResponseBase
  {
    [DataMember(Name = "build")]
    public XPackBuildInformation Build { get; internal set; }

    [DataMember(Name = "features")]
    public XPackFeatures Features { get; internal set; }

    [DataMember(Name = "license")]
    public MinimalLicenseInformation License { get; internal set; }

    [DataMember(Name = "tagline")]
    public string Tagline { get; internal set; }
  }
}
