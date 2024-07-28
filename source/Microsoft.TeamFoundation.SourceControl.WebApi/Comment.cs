// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.Comment
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class Comment : VersionControlSecuredObject
  {
    [DataMember]
    public short Id { get; set; }

    [DataMember]
    public short ParentCommentId { get; set; }

    internal int ThreadId { get; set; }

    [DataMember]
    public IdentityRef Author { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Content { get; set; }

    [DataMember]
    public DateTime PublishedDate { get; set; }

    [DataMember]
    public DateTime LastUpdatedDate { get; set; }

    [DataMember]
    public DateTime LastContentUpdatedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsDeleted { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public CommentType CommentType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<IdentityRef> UsersLiked { get; set; }

    [DataMember(Name = "_links", EmitDefaultValue = false)]
    public ReferenceLinks Links { get; set; }
  }
}
