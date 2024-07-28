// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Discussion.Server.LegacyComment
// Assembly: Microsoft.TeamFoundation.Discussion.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4DCA91C2-88ED-4792-BE4A-3104961AE8D8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Discussion.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Discussion.Server
{
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Internal)]
  [ClientType("Comment")]
  [XmlType("Comment")]
  public class LegacyComment
  {
    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private, PropertyName = "Id")]
    public short CommentId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private, PropertyName = "ParentId")]
    public short ParentCommentId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public int DiscussionId { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private, PropertyName = "AuthorIdentity")]
    public Guid Author { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public byte CommentType { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string Content { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public DateTime PublishedDate { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public bool IsDeleted { get; set; }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[Comment Id={0} DiscussionId={1} ParentCommentId={2} Author={3} CommentType={4} Content={5} PublishedDate={6} IsDeleted={7}]", (object) this.CommentId, (object) this.DiscussionId, (object) this.ParentCommentId, (object) this.Author, (object) this.CommentType, (object) this.Content, (object) this.PublishedDate, (object) this.IsDeleted);

    public DiscussionComment ToDiscussionComment() => new DiscussionComment()
    {
      CommentId = this.CommentId,
      ParentCommentId = this.ParentCommentId,
      DiscussionId = this.DiscussionId,
      Author = new IdentityRef()
      {
        Id = this.Author.ToString()
      },
      CommentType = (Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi.CommentType) this.CommentType,
      Content = this.Content,
      PublishedDate = this.PublishedDate,
      IsDeleted = this.IsDeleted
    };
  }
}
