// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.WebApi.Iteration
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
  public class Iteration
  {
    [DataMember]
    public int? Id { get; set; }

    [DataMember]
    public int ReviewId { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public IdentityRef Author { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<ChangeEntry> ChangeList { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IList<Status> Statuses { get; set; }

    [DataMember]
    public DateTime? CreatedDate { get; set; }

    [DataMember]
    public DateTime? UpdatedDate { get; set; }

    [DataMember]
    public bool IsUnpublished { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public PropertiesCollection Properties { get; set; }

    [DataMember(Name = "_links", EmitDefaultValue = false)]
    public ReferenceLinks Links { get; set; }
  }
}
