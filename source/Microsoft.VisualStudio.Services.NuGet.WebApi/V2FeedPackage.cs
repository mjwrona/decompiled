// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.WebApi.V2FeedPackage
// Assembly: Microsoft.VisualStudio.Services.NuGet.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9D44F181-506D-4445-A06B-7AA7FD5D22D8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.WebApi.dll

using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Data.Services.Common;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.NuGet.WebApi
{
  [DataContract]
  [HasStream]
  [EntityPropertyMapping("Id", SyndicationItemProperty.Title, SyndicationTextContentKind.Plaintext, false)]
  [EntityPropertyMapping("Authors", SyndicationItemProperty.AuthorName, SyndicationTextContentKind.Plaintext, false)]
  [EntityPropertyMapping("LastUpdated", SyndicationItemProperty.Updated, SyndicationTextContentKind.Plaintext, false)]
  [EntityPropertyMapping("Summary", SyndicationItemProperty.Summary, SyndicationTextContentKind.Plaintext, false)]
  [DataServiceKey(new string[] {"Id", "Version"})]
  public class V2FeedPackage : PackagingSecuredObject
  {
    [DataMember]
    public string Id { get; set; }

    [DataMember]
    public string Version { get; set; }

    [DataMember]
    public string NormalizedVersion { get; set; }

    [DataMember]
    public string Authors { get; set; }

    [DataMember]
    public string Copyright { get; set; }

    [DataMember]
    public DateTime Created { get; set; }

    [DataMember]
    public string Dependencies { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public int DownloadCount { get; set; }

    [DataMember]
    public string IconUrl { get; set; }

    [DataMember]
    public bool IsLatestVersion { get; set; }

    [DataMember]
    public bool IsAbsoluteLatestVersion { get; set; }

    [DataMember]
    public bool IsPrerelease { get; set; }

    [DataMember]
    public string Language { get; set; }

    [DataMember]
    public DateTime LastUpdated { get; set; }

    [DataMember]
    public DateTime Published { get; set; }

    [DataMember]
    public long PackageSize { get; set; }

    [DataMember]
    public string ProjectUrl { get; set; }

    [DataMember]
    public string ReleaseNotes { get; set; }

    [DataMember]
    public bool RequireLicenseAcceptance { get; set; }

    [DataMember]
    public string Summary { get; set; }

    [DataMember]
    public string Tags { get; set; }

    [DataMember]
    public string Title { get; set; }

    [DataMember]
    public string MinClientVersion { get; set; }

    [DataMember]
    public DateTime? LastEdited { get; set; }

    [DataMember]
    public string LicenseUrl { get; set; }

    [DataMember]
    public bool Listed { get; set; }

    public Uri DownloadUrl { get; set; }
  }
}
