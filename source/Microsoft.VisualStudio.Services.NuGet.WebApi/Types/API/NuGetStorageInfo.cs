// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API.NuGetStorageInfo
// Assembly: Microsoft.VisualStudio.Services.NuGet.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9D44F181-506D-4445-A06B-7AA7FD5D22D8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.WebApi.dll

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API
{
  [DataContract]
  public class NuGetStorageInfo
  {
    [JsonConstructor]
    public NuGetStorageInfo(NuGetPackageContentStorageInfo package) => this.Package = package;

    [DataMember]
    [JsonConverter(typeof (NuGetPackageContentStorageInfoConverter))]
    public NuGetPackageContentStorageInfo Package { get; }
  }
}
