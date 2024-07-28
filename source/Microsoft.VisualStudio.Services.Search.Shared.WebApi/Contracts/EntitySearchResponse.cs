// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.EntitySearchResponse
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 504F400B-CBC4-4007-9816-31A8DED1C3FC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts
{
  [DataContract]
  public abstract class EntitySearchResponse : SearchSecuredV2Object
  {
    [DataMember(Name = "infoCode")]
    public int InfoCode { get; set; }

    [DataMember(Name = "facets")]
    public IDictionary<string, IEnumerable<Filter>> Facets { get; set; }

    public override void SetSecuredObject(Guid namespaceId, int requiredPermissions, string token)
    {
      base.SetSecuredObject(namespaceId, requiredPermissions, token);
      if (this.Facets == null)
        return;
      IDictionary<string, IEnumerable<Filter>> dictionary1 = (IDictionary<string, IEnumerable<Filter>>) new Dictionary<string, IEnumerable<Filter>>();
      foreach (KeyValuePair<string, IEnumerable<Filter>> facet in (IEnumerable<KeyValuePair<string, IEnumerable<Filter>>>) this.Facets)
      {
        IDictionary<string, IEnumerable<Filter>> dictionary2 = dictionary1;
        string key = facet.Key;
        IEnumerable<Filter> source = facet.Value;
        List<Filter> list = source != null ? source.Select<Filter, Filter>((Func<Filter, Filter>) (i =>
        {
          i.SetSecuredObject(namespaceId, requiredPermissions, token);
          return i;
        })).ToList<Filter>() : (List<Filter>) null;
        dictionary2[key] = (IEnumerable<Filter>) list;
      }
      this.Facets = dictionary1;
    }
  }
}
