// Decompiled with JetBrains decompiler
// Type: VsGallery.WebServices.ReleaseQueryResult
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Galleries.Domain.Model;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace VsGallery.WebServices
{
  [DataContract]
  public class ReleaseQueryResult
  {
    [DataMember]
    public int TotalCount { get; set; }

    [DataMember]
    public IEnumerable<Release> Releases { get; set; }
  }
}
