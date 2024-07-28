// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.OSInfo
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts
{
  [DataContract]
  public class OSInfo
  {
    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "release")]
    public string Release { get; set; }

    [DataMember(Name = "version")]
    public string Version { get; set; }

    [DataMember(Name = "distroName")]
    public string DistributionName { get; set; }

    [DataMember(Name = "distroVersion")]
    public string DistributionVersion { get; set; }
  }
}
