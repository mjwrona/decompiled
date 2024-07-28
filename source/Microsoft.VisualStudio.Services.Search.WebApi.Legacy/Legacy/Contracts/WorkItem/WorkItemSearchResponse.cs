// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.WorkItemSearchResponse
// Assembly: Microsoft.VisualStudio.Services.Search.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5D4CB2D3-3C08-46C7-B9C5-51E638F57F9E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.WebApi.Legacy.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.Contracts;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem
{
  [DataContract]
  public class WorkItemSearchResponse : EntitySearchResponse
  {
    [DataMember(Name = "query")]
    public WorkItemSearchRequest Query { get; set; }

    [DataMember(Name = "results")]
    public WorkItemResults Results { get; set; }

    public override string ToString() => JsonConvert.SerializeObject((object) new
    {
      results = new
      {
        count = this.Results.Count,
        values = this.Results.Values.Select(v => new
        {
          project = v.Project,
          projectId = v.ProjectId,
          fields = v.Fields,
          hitCount = v.Hits.Count<WorkItemHit>()
        })
      },
      errors = this.Errors,
      filterCategories = this.FilterCategories
    }, Formatting.None, new JsonSerializerSettings()
    {
      NullValueHandling = NullValueHandling.Ignore
    });

    internal override void SetSecuredObject(
      Guid namespaceId,
      int requiredPermissions,
      string token)
    {
      base.SetSecuredObject(namespaceId, requiredPermissions, token);
      this.Query.SetSecuredObject(namespaceId, requiredPermissions, token);
      this.Results.SetSecuredObject(namespaceId, requiredPermissions, token);
    }
  }
}
