// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.Contracts.WorkItem.WorkItemResult
// Assembly: Microsoft.VisualStudio.Services.Search.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9E496DAE-109A-4A16-A97B-F7DEDEC6CB20
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.WebApi.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.WebApi.Contracts.WorkItem
{
  [DataContract]
  public class WorkItemResult : SearchSecuredV2Object
  {
    [DataMember(Name = "project")]
    public Project Project { get; set; }

    [DataMember(Name = "fields")]
    public IDictionary<string, string> Fields { get; set; }

    [DataMember(Name = "hits")]
    public IEnumerable<WorkItemHit> Hits { get; set; }

    [DataMember(Name = "url")]
    public string Url { get; set; }

    public override void SetSecuredObject(Guid namespaceId, int requiredPermissions, string token)
    {
      base.SetSecuredObject(namespaceId, requiredPermissions, token);
      this.Project?.SetSecuredObject(namespaceId, requiredPermissions, token);
      IEnumerable<WorkItemHit> hits = this.Hits;
      this.Hits = hits != null ? (IEnumerable<WorkItemHit>) hits.Select<WorkItemHit, WorkItemHit>((Func<WorkItemHit, WorkItemHit>) (i =>
      {
        i.SetSecuredObject(namespaceId, requiredPermissions, token);
        return i;
      })).ToList<WorkItemHit>() : (IEnumerable<WorkItemHit>) null;
    }

    public override string ToString() => JsonConvert.SerializeObject((object) this, Formatting.None, new JsonSerializerSettings()
    {
      NullValueHandling = NullValueHandling.Ignore
    });
  }
}
