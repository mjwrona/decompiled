// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.WebApi.CommentThread
// Assembly: Microsoft.VisualStudio.Services.CodeReview.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84DE81C5-ABF4-4E22-A82B-21BA09D9141E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.WebApi.dll

using Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.CodeReview.WebApi
{
  [DataContract]
  [JsonConverter(typeof (CommentThreadConverter))]
  public class CommentThread : DiscussionThread
  {
    internal int ReviewId { get; set; }

    internal Guid ProjectId { get; set; }

    public bool IsReviewLevel => this.ThreadContext == null;

    [DataMember]
    public CommentThreadContext ThreadContext { get; set; }
  }
}
