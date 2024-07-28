// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.WebApi.ReviewSearchCriteria
// Assembly: Microsoft.VisualStudio.Services.CodeReview.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84DE81C5-ABF4-4E22-A82B-21BA09D9141E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.WebApi.dll

using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.CodeReview.WebApi
{
  [DataContract]
  public class ReviewSearchCriteria
  {
    [DataMember(EmitDefaultValue = false)]
    public string SourceArtifactPrefix { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ReviewStatus? Status { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityDescriptor CreatorIdentity { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IdentityDescriptor ReviewerIdentity { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? MinCreatedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? MaxCreatedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? MinUpdatedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? MaxUpdatedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool? OrderAscending { get; set; }
  }
}
