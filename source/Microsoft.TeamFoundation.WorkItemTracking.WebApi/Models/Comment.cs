// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.Comment
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
  public class Comment : WorkItemTrackingResource
  {
    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<CommentReaction> Reactions;
    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<CommentMention> Mentions;

    public Comment()
    {
    }

    public Comment(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    [DataMember]
    public int WorkItemId { get; set; }

    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public int Version { get; set; }

    [DataMember]
    public string Text { get; set; }

    [DataMember]
    public IdentityRef CreatedBy { get; set; }

    [DataMember]
    public DateTime CreatedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityRef CreatedOnBehalfOf { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? CreatedOnBehalfDate { get; set; }

    [DataMember]
    public IdentityRef ModifiedBy { get; set; }

    [DataMember]
    public DateTime ModifiedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsDeleted { get; set; }

    [DataMember]
    public CommentFormat Format { get; set; }

    [DataMember]
    public string RenderedText { get; set; }
  }
}
