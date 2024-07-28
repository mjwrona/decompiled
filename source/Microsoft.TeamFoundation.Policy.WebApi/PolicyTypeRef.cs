// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.WebApi.PolicyTypeRef
// Assembly: Microsoft.TeamFoundation.Policy.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E2CB80F-05BD-43A4-BD5A-A4654EDC6268
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Policy.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Policy.WebApi
{
  [DataContract]
  public class PolicyTypeRef : BaseSecuredObject
  {
    public PolicyTypeRef()
    {
    }

    public PolicyTypeRef(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    [DataMember(IsRequired = true)]
    public Guid Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string DisplayName { get; set; }
  }
}
