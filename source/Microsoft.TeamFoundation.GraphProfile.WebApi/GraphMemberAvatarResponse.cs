// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.GraphProfile.WebApi.GraphMemberAvatarResponse
// Assembly: Microsoft.TeamFoundation.GraphProfile.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10687127-D73A-4F03-AA93-A7EDA3B5980D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.GraphProfile.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.GraphProfile.WebApi
{
  [DataContract]
  public class GraphMemberAvatarResponse : ISecuredObjectContainer
  {
    [ClientResponseContent]
    [DataMember]
    public GraphMemberAvatar Avatar { get; set; }

    [ClientResponseHeader("Content-Type")]
    public IEnumerable<string> ContentType { get; set; }

    [ClientResponseHeader("Etag")]
    public IEnumerable<string> Etag { get; set; }

    [ClientResponseHeader("Cache-Control")]
    public IEnumerable<string> CacheControl { get; set; }
  }
}
