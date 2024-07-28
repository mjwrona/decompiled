// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.Comments.WebApi.CommentList
// Assembly: Microsoft.Azure.DevOps.Comments.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A55BAA93-5FAF-48BE-A9EC-2F097131C70D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.Comments.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Azure.DevOps.Comments.WebApi
{
  [DataContract]
  public class CommentList : CommentResourceReference
  {
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
