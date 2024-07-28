// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.CommentThread
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
  public class CommentThread : VersionControlSecuredObject
  {
    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public DateTime PublishedDate { get; set; }

    [DataMember]
    public DateTime LastUpdatedDate { get; set; }

    [DataMember]
    public IList<Comment> Comments { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public CommentThreadStatus Status { get; set; }

    [DataMember]
    public CommentThreadContext ThreadContext { get; set; }

    [DataMember]
    public PropertiesCollection Properties { get; set; }

    [DataMember]
    public Dictionary<string, IdentityRef> Identities { get; set; }

    [DataMember]
    public bool IsDeleted { get; set; }

    [DataMember(Name = "_links", EmitDefaultValue = false)]
    public ReferenceLinks Links { get; set; }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      base.SetSecuredObject(securedObject);
      IList<Comment> comments = this.Comments;
      if (comments != null)
        comments.SetSecuredObject<Comment>(securedObject);
      this.ThreadContext?.SetSecuredObject(securedObject);
    }
  }
}
