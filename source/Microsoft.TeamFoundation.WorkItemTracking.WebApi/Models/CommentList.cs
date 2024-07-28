// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.CommentList
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FA6C797-B300-46B2-A8C9-CFED891348F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models
{
  [DataContract]
  public class CommentList : WorkItemTrackingResource
  {
    public CommentList()
    {
    }

    public CommentList(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    [DataMember]
    public int TotalCount { get; set; }

    [DataMember]
    public int Count { get; set; }

    [DataMember]
    public IEnumerable<Comment> Comments { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Uri NextPage { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ContinuationToken { get; set; }
  }
}
