// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi.DiscussionComment
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 443E2621-CB19-4319-96B1-AE621A0F5B5B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi
{
  [DataContract]
  public class DiscussionComment
  {
    [DataMember(Name = "Id")]
    public short CommentId { get; set; }

    [DataMember(Name = "ParentId", EmitDefaultValue = false)]
    public short ParentCommentId { get; set; }

    [DataMember(Name = "ThreadId")]
    public int DiscussionId { get; set; }

    [DataMember]
    public IdentityRef Author { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Content { get; set; }

    [DataMember]
    public DateTime PublishedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsDeleted { get; set; }

    [DataMember]
    public DateTime LastUpdatedDate { get; set; }

    [DataMember]
    public DateTime LastContentUpdatedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool CanDelete { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public CommentType CommentType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public List<IdentityRef> UsersLiked { get; set; }

    [DataMember(Name = "_links", EmitDefaultValue = false)]
    public ReferenceLinks Links { get; set; }

    public Guid GetAuthorId()
    {
      Guid result;
      if (this.Author == null || !Guid.TryParse(this.Author.Id, out result))
        throw new InvalidOperationException(string.Format(DiscussionWebAPIResources.CommentAuthorCannotbeNull((object) this.CommentId)));
      return result;
    }
  }
}
