// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.Comments.WebApi.Comment
// Assembly: Microsoft.Azure.DevOps.Comments.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A55BAA93-5FAF-48BE-A9EC-2F097131C70D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.Comments.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Azure.DevOps.Comments.WebApi
{
  [DataContract]
  public class Comment : CommentResourceReference
  {
    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<CommentReaction> Reactions;
    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<CommentMention> Mentions;

    [DataMember]
    public string ArtifactId { get; set; }

    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public int Version { get; set; }

    [DataMember]
    public string Text { get; set; }

    [DataMember]
    public string RenderedText { get; set; }

    [DataMember]
    public IdentityRef CreatedBy { get; set; }

    [DataMember]
    public DateTime CreatedDate { get; set; }

    [DataMember]
    public IdentityRef ModifiedBy { get; set; }

    [DataMember]
    public DateTime ModifiedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsDeleted { get; set; }

    [DataMember]
    public CommentState State { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public CommentList Replies { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? ParentId { get; set; }
  }
}
