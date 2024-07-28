// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Core.WebApi.TeamProjectCollectionReference
// Assembly: Microsoft.TeamFoundation.Core.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3A82A796-05AB-42F0-97D0-CB8516E08665
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Core.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Core.WebApi
{
  [DataContract]
  public class TeamProjectCollectionReference
  {
    [DataMember(Order = 0)]
    public Guid Id { get; set; }

    [DataMember(Order = 1, EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(Order = 2, EmitDefaultValue = false)]
    public string Url { get; set; }

    [DataMember(Order = 20)]
    public string AvatarUrl { get; set; }
  }
}
