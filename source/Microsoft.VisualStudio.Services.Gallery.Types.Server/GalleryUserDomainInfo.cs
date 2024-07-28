// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Types.Server.GalleryUserDomainInfo
// Assembly: Microsoft.VisualStudio.Services.Gallery.Types.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF687265-4AE2-4CD2-A134-409D61826008
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Types.Server.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Gallery.Types.Server
{
  public class GalleryUserDomainInfo
  {
    [DataMember(Name = "userDomain")]
    public string UserDomain { get; set; }

    [DataMember(Name = "userAddress")]
    public string UserAddress { get; set; }

    [DataMember(Name = "userDomainId")]
    public Guid UserDomainId { get; set; }
  }
}
