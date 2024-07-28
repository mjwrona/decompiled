// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.WebApi.QueryResultPackage
// Assembly: Microsoft.VisualStudio.Services.NuGet.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9D44F181-506D-4445-A06B-7AA7FD5D22D8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.WebApi.dll

using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.NuGet.WebApi
{
  [DataContract]
  public class QueryResultPackage : PackagingSecuredObject
  {
    [DataMember(Name = "@id")]
    public string Aid { get; set; }

    [DataMember(Name = "@type")]
    public string Atype { get; set; }

    [DataMember]
    public string Id { get; set; }

    [DataMember]
    public string Version { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public IEnumerable<QueryResultPackageVersion> Versions { get; set; }

    [DataMember]
    public List<string> Authors { get; set; }

    [DataMember]
    public string IconUrl { get; set; }

    [DataMember]
    public string LicenseUrl { get; set; }

    [DataMember]
    public string ProjectUrl { get; set; }

    [DataMember]
    public string Registration { get; set; }

    [DataMember]
    public string Summary { get; set; }

    [DataMember]
    public List<string> Tags { get; set; }

    [DataMember]
    public string Title { get; set; }

    [DataMember(Name = "totalDownloads", EmitDefaultValue = false)]
    public long? TotalDownloadsLong { get; set; }

    [Obsolete]
    public int? TotalDownloads
    {
      get => !this.TotalDownloadsLong.HasValue ? new int?() : new int?((int) Math.Min(this.TotalDownloadsLong.Value, (long) int.MaxValue));
      set
      {
        int? nullable = value;
        this.TotalDownloadsLong = nullable.HasValue ? new long?((long) nullable.GetValueOrDefault()) : new long?();
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public bool? Verified { get; set; }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      base.SetSecuredObject(securedObject);
      IEnumerable<QueryResultPackageVersion> versions = this.Versions;
      this.Versions = versions != null ? versions.ToSecuredObject<QueryResultPackageVersion>(securedObject) : (IEnumerable<QueryResultPackageVersion>) null;
    }
  }
}
