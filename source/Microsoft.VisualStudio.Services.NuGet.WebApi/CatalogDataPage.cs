// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.WebApi.CatalogDataPage
// Assembly: Microsoft.VisualStudio.Services.NuGet.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9D44F181-506D-4445-A06B-7AA7FD5D22D8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.NuGet.WebApi
{
  [DataContract]
  public class CatalogDataPage : CatalogModelBase
  {
    [DataMember(Name = "id")]
    public string NuGetId { get; set; }

    [DataMember(Name = "version")]
    public string NuGetVersion { get; set; }

    [DataMember(Name = "urn:uuid:2C6454C9-C61F-4708-95C5-7DD32698D825#VsoCommitId")]
    public string VsoCommitId { get; set; }
  }
}
