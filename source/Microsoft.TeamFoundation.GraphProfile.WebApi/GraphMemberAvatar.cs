// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.GraphProfile.WebApi.GraphMemberAvatar
// Assembly: Microsoft.TeamFoundation.GraphProfile.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10687127-D73A-4F03-AA93-A7EDA3B5980D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.GraphProfile.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.GraphProfile.WebApi
{
  [DataContract]
  public class GraphMemberAvatar : ISecuredObject
  {
    [DataMember]
    public byte[] ImageData { get; private set; }

    [DataMember]
    public string ImageType { get; private set; }

    [DataMember]
    public GraphMemberAvatarSize Size { get; private set; }

    [DataMember(EmitDefaultValue = false, Name = "_links")]
    public ReferenceLinks Links { get; private set; }

    public GraphMemberAvatar(
      byte[] imageData,
      string imageType,
      GraphMemberAvatarSize size,
      ReferenceLinks links)
    {
      this.ImageData = imageData;
      this.ImageType = imageType;
      this.Size = size;
      this.Links = links;
    }

    Guid ISecuredObject.NamespaceId => GraphSecurityConstants.NamespaceId;

    int ISecuredObject.RequiredPermissions => 1;

    string ISecuredObject.GetToken() => GraphSecurityConstants.RefsToken;
  }
}
