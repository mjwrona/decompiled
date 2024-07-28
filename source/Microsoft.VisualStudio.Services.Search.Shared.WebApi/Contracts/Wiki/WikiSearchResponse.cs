// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Wiki.WikiSearchResponse
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
  public class WikiSearchResponse : EntitySearchResponse
  {
    [DataMember(Name = "count")]
    public int Count { get; set; }

    [DataMember(Name = "results")]
    public IEnumerable<WikiResult> Results { get; set; }

    public override void SetSecuredObject(Guid namespaceId, int requiredPermissions, string token)
    {
      base.SetSecuredObject(namespaceId, requiredPermissions, token);
      SearchSecuredV2Object.SetSecuredObject((IEnumerable<SearchSecuredV2Object>) this.Results, namespaceId, requiredPermissions, token);
    }

    public override string ToString()
    {
      int count = this.Count;
      IEnumerable<WikiResult> results = this.Results;
      IEnumerable<\u003C\u003Ef__AnonymousType3<Collection, ProjectReference, Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.Wiki.Wiki, string, string>> datas = results != null ? results.Select(v => new
      {
        collection = v.Collection,
        project = v.Project,
        wiki = v.Wiki,
        fileName = v.FileName,
        path = v.Path
      }) : null;
      return JsonConvert.SerializeObject((object) new
      {
        results = new{ count = count, results = datas },
        infoCode = this.InfoCode,
        facets = this.Facets
      }, Formatting.None, new JsonSerializerSettings()
      {
        NullValueHandling = NullValueHandling.Ignore
      });
    }
  }
}
