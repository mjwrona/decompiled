// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.Comments.WebApi.CommentMention
// Assembly: Microsoft.Azure.DevOps.Comments.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A55BAA93-5FAF-48BE-A9EC-2F097131C70D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.Comments.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.Azure.DevOps.Comments.WebApi
{
  [DataContract]
  public class CommentMention : CommentResourceReference
  {
    public CommentMention()
    {
    }

    public CommentMention(ISecuredObject securedObject) => this.SetSecuredObject(securedObject);

    [DataMember]
    public string ArtifactId { get; set; }

    [DataMember]
    public int? CommentId { get; set; }

    [DataMember]
    public CommentMentionType Type { get; set; }

    [DataMember]
    public string MentionedArtifact { get; set; }
  }
}
