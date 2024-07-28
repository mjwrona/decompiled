// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.WebApi.Reviewer
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
  public class Reviewer
  {
    [DataMember]
    public IdentityRef Identity { get; set; }

    [DataMember]
    public short? ReviewerStateId { get; set; }

    [DataMember]
    public ReviewerKind? Kind { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? CreatedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? ModifiedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<Reviewer> VotedForGroups { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<Guid> VotedFor { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? IterationId { get; set; }

    internal int ReviewId { get; set; }
  }
}
