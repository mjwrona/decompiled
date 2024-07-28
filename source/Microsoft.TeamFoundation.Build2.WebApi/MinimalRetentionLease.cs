// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.MinimalRetentionLease
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  [ClientIncludeModel]
  public class MinimalRetentionLease
  {
    [DataMember]
    public string OwnerId { get; set; }

    [DataMember]
    public int RunId { get; set; }

    [DataMember]
    public int DefinitionId { get; set; }

    public override string ToString() => JsonUtility.ToString((object) this);
  }
}
