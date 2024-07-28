// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Work.WebApi.Member
// Assembly: Microsoft.TeamFoundation.Work.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0C4CCFA0-0616-4E48-A4F0-952E1CB10B12
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Work.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Work.WebApi
{
  [DataContract]
  public class Member
  {
    [DataMember]
    public Guid Id { get; set; }

    [DataMember]
    public string DisplayName { get; set; }

    [DataMember]
    public string UniqueName { get; set; }

    [DataMember]
    public string Url { get; set; }

    [DataMember]
    public string ImageUrl { get; set; }
  }
}
