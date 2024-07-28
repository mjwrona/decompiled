// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Package.PackageResult
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 504F400B-CBC4-4007-9816-31A8DED1C3FC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Package
{
  [DataContract]
  public class PackageResult : SearchSecuredV2Object
  {
    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "id")]
    public string Id { get; set; }

    [DataMember(Name = "description")]
    public string Description { get; set; }

    [DataMember(Name = "protocolType")]
    public string ProtocolType { get; set; }

    [DataMember(Name = "feeds")]
    public IEnumerable<FeedInfo> Feeds { get; set; }

    [DataMember(Name = "hits")]
    public IEnumerable<PackageHit> Hits { get; set; }

    public PackageResult(
      string name,
      string id,
      string description,
      string protocolType,
      IEnumerable<PackageHit> hits,
      IEnumerable<FeedInfo> feeds)
    {
      this.Name = name;
      this.Id = id;
      this.Description = description;
      this.Hits = hits;
      this.ProtocolType = protocolType;
      this.Feeds = feeds;
    }

    public string ToString(int indentLevel)
    {
      StringBuilder stringBuilder = new StringBuilder();
      string indentSpacing = Extensions.GetIndentSpacing(indentLevel);
      stringBuilder.AppendFormat("{0}\\{1}\\{2}\\{3}", (object) indentSpacing, (object) this.Name, (object) this.ProtocolType, (object) this.Id);
      stringBuilder.AppendLine();
      return stringBuilder.ToString();
    }

    public override string ToString() => this.ToString(0);

    public override void SetSecuredObject(Guid namespaceId, int requiredPermissions, string token)
    {
      base.SetSecuredObject(namespaceId, requiredPermissions, token);
      IEnumerable<PackageHit> hits = this.Hits;
      this.Hits = hits != null ? (IEnumerable<PackageHit>) hits.Select<PackageHit, PackageHit>((Func<PackageHit, PackageHit>) (i =>
      {
        i.SetSecuredObject(namespaceId, requiredPermissions, token);
        return i;
      })).ToList<PackageHit>() : (IEnumerable<PackageHit>) null;
      IEnumerable<FeedInfo> feeds = this.Feeds;
      this.Feeds = feeds != null ? (IEnumerable<FeedInfo>) feeds.Select<FeedInfo, FeedInfo>((Func<FeedInfo, FeedInfo>) (i =>
      {
        i.SetSecuredObject(namespaceId, requiredPermissions, token);
        return i;
      })).ToList<FeedInfo>() : (IEnumerable<FeedInfo>) null;
    }
  }
}
