// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Wiki.WikiResult
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 504F400B-CBC4-4007-9816-31A8DED1C3FC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Wiki
{
  [DataContract]
  public class WikiResult : SearchSecuredV2Object
  {
    [DataMember(Name = "fileName")]
    public string FileName { get; set; }

    [DataMember(Name = "path")]
    public string Path { get; set; }

    [DataMember(Name = "collection")]
    public Collection Collection { get; set; }

    [DataMember(Name = "project")]
    public ProjectReference Project { get; set; }

    [DataMember(Name = "wiki")]
    public Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Wiki.Wiki Wiki { get; set; }

    [DataMember(Name = "contentId")]
    public string ContentId { get; set; }

    [DataMember(Name = "hits")]
    public IEnumerable<WikiHit> Hits { get; set; }

    public override void SetSecuredObject(Guid namespaceId, int requiredPermissions, string token)
    {
      base.SetSecuredObject(namespaceId, requiredPermissions, token);
      this.Collection?.SetSecuredObject(namespaceId, requiredPermissions, token);
      this.Project?.SetSecuredObject(namespaceId, requiredPermissions, token);
      this.Wiki?.SetSecuredObject(namespaceId, requiredPermissions, token);
      IEnumerable<WikiHit> hits = this.Hits;
      this.Hits = hits != null ? (IEnumerable<WikiHit>) hits.Select<WikiHit, WikiHit>((Func<WikiHit, WikiHit>) (i =>
      {
        i.SetSecuredObject(namespaceId, requiredPermissions, token);
        return i;
      })).ToList<WikiHit>() : (IEnumerable<WikiHit>) null;
    }

    public override string ToString() => JsonConvert.SerializeObject((object) this, Formatting.None, new JsonSerializerSettings()
    {
      NullValueHandling = NullValueHandling.Ignore
    });
  }
}
