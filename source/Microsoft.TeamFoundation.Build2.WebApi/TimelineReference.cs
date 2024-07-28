// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.TimelineReference
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public class TimelineReference : BaseSecuredObject
  {
    internal TimelineReference()
    {
    }

    internal TimelineReference(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    [DataMember(Order = 1)]
    public Guid Id { get; internal set; }

    [DataMember(Order = 2)]
    public int ChangeId { get; internal set; }

    [DataMember(Order = 3)]
    public string Url { get; set; }
  }
}
