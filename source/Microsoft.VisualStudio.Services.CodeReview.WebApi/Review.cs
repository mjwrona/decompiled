// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.WebApi.Review
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
  public class Review
  {
    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public string Title { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public IdentityRef Author { get; set; }

    [DataMember]
    public DateTime? CreatedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? UpdatedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? CompletedDate { get; set; }

    [DataMember]
    public ReviewStatus? Status { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<Reviewer> Reviewers { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<Iteration> Iterations { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<Attachment> Attachments { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<Microsoft.VisualStudio.Services.CodeReview.WebApi.Status> Statuses { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public PropertiesCollection Properties { get; set; }

    [DataMember(Name = "_links", EmitDefaultValue = false)]
    public ReferenceLinks Links { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string SourceArtifactId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsDeleted { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid ProjectId { get; set; }

    public bool CustomStorage { get; set; }

    internal DateTime? PriorReviewUpdatedTimestamp { get; set; }

    internal int? DiffFileId { get; set; }

    internal int? ExpectedDiffFileId { get; set; }
  }
}
