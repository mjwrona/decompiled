// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.WebApi.ArtifactStats
// Assembly: Microsoft.VisualStudio.Services.CodeReview.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84DE81C5-ABF4-4E22-A82B-21BA09D9141E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.CodeReview.WebApi
{
  [DataContract]
  public class ArtifactStats
  {
    [DataMember]
    public string ArtifactId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string DiscussionArtifactId { get; set; }

    [DataMember]
    public IdentityRef User { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? LastUpdatedDate { get; set; }

    [DataMember]
    public Dictionary<CommentThreadType, int?> CommentsCount { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Dictionary<CommentThreadType, int?> NewCommentsCount { get; set; }
  }
}
