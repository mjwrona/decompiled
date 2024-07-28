// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.WorkItemResult
// Assembly: Microsoft.VisualStudio.Services.Search.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5D4CB2D3-3C08-46C7-B9C5-51E638F57F9E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.WebApi.Legacy.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem
{
  [DataContract]
  public class WorkItemResult : SearchSecuredObject
  {
    [DataMember(Name = "project")]
    public string Project { get; set; }

    [DataMember(Name = "projectId")]
    public string ProjectId { get; set; }

    [DataMember(Name = "fields")]
    public IEnumerable<WorkItemField> Fields { get; set; }

    [DataMember(Name = "hits")]
    public IEnumerable<WorkItemHit> Hits { get; set; }

    public override string ToString() => JsonConvert.SerializeObject((object) this, Formatting.None, new JsonSerializerSettings()
    {
      NullValueHandling = NullValueHandling.Ignore
    });

    internal override void SetSecuredObject(
      Guid namespaceId,
      int requiredPermissions,
      string token)
    {
      base.SetSecuredObject(namespaceId, requiredPermissions, token);
      IEnumerable<WorkItemHit> hits = this.Hits;
      this.Hits = hits != null ? (IEnumerable<WorkItemHit>) hits.Select<WorkItemHit, WorkItemHit>((Func<WorkItemHit, WorkItemHit>) (i =>
      {
        i.SetSecuredObject(namespaceId, requiredPermissions, token);
        return i;
      })).ToList<WorkItemHit>() : (IEnumerable<WorkItemHit>) null;
      IEnumerable<WorkItemField> fields = this.Fields;
      this.Fields = fields != null ? (IEnumerable<WorkItemField>) fields.Select<WorkItemField, WorkItemField>((Func<WorkItemField, WorkItemField>) (i =>
      {
        i.SetSecuredObject(namespaceId, requiredPermissions, token);
        return i;
      })).ToList<WorkItemField>() : (IEnumerable<WorkItemField>) null;
    }
  }
}
