// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.TfvcBranchRef
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class TfvcBranchRef : TfvcShallowBranchRef
  {
    public TfvcBranchRef() => this.IsDeleted = false;

    [DataMember(EmitDefaultValue = false, Order = 1)]
    public string Description { get; set; }

    [DataMember(EmitDefaultValue = false, Order = 2)]
    public IdentityRef Owner { get; set; }

    [DataMember(EmitDefaultValue = false, Order = 3)]
    public DateTime CreatedDate { get; set; }

    [DataMember(EmitDefaultValue = false, Order = 4)]
    public bool IsDeleted { get; set; }

    [DataMember(EmitDefaultValue = false, Order = 5)]
    public string Url { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "_links")]
    public ReferenceLinks Links { get; set; }
  }
}
