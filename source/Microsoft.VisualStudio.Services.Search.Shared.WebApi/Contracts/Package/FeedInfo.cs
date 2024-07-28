// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Package.FeedInfo
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 504F400B-CBC4-4007-9816-31A8DED1C3FC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Package
{
  [DataContract]
  public class FeedInfo : SearchSecuredV2Object
  {
    [DataMember(Name = "collectionId")]
    public string CollectionId { get; set; }

    [DataMember(Name = "collectionName")]
    public string CollectionName { get; set; }

    [DataMember(Name = "feedId")]
    public string FeedId { get; set; }

    [DataMember(Name = "feedName")]
    public string FeedName { get; set; }

    [DataMember(Name = "packageUrl")]
    public string PackageUrl { get; set; }

    [DataMember(Name = "latestVersion")]
    public string LatestVersion { get; set; }

    [DataMember(Name = "latestMatchedVersion")]
    public string LatestMatchedVersion { get; set; }

    [DataMember(Name = "views")]
    public IEnumerable<string> Views { get; set; }

    public FeedInfo(
      string collectionId,
      string collectionName,
      string feedId,
      string feedName,
      string packageUrl,
      string latestVersion,
      string latestMatchedVersion,
      IEnumerable<string> views)
    {
      this.CollectionId = collectionId;
      this.CollectionName = collectionName;
      this.FeedId = feedId;
      this.FeedName = feedName;
      this.PackageUrl = packageUrl;
      this.LatestVersion = latestVersion;
      this.LatestMatchedVersion = latestMatchedVersion;
      this.Views = views;
    }
  }
}
