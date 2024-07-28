// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.Contracts.WorkItem.WorkItemSearchResponse
// Assembly: Microsoft.VisualStudio.Services.Search.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9E496DAE-109A-4A16-A97B-F7DEDEC6CB20
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.WebApi.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.WebApi.Contracts.WorkItem
{
  [DataContract]
  public class WorkItemSearchResponse : EntitySearchResponse
  {
    [DataMember(Name = "count")]
    public int Count { get; set; }

    [DataMember(Name = "results")]
    public IEnumerable<WorkItemResult> Results { get; set; }

    public override void SetSecuredObject(Guid namespaceId, int requiredPermissions, string token)
    {
      base.SetSecuredObject(namespaceId, requiredPermissions, token);
      IEnumerable<WorkItemResult> results = this.Results;
      this.Results = results != null ? (IEnumerable<WorkItemResult>) results.Select<WorkItemResult, WorkItemResult>((Func<WorkItemResult, WorkItemResult>) (i =>
      {
        i.SetSecuredObject(namespaceId, requiredPermissions, token);
        return i;
      })).ToList<WorkItemResult>() : (IEnumerable<WorkItemResult>) null;
    }

    public override string ToString()
    {
      int count = this.Count;
      IEnumerable<WorkItemResult> results = this.Results;
      IEnumerable<\u003C\u003Ef__AnonymousType4<Project, IDictionary<string, string>, int>> datas = results != null ? results.Select(v => new
      {
        project = v.Project,
        fields = v.Fields,
        hitCount = v.Hits.Count<WorkItemHit>()
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
