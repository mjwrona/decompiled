// Decompiled with JetBrains decompiler
// Type: Galleries.Domain.Model.Release
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Galleries.Domain.Model
{
  [DataContract]
  public class Release
  {
    public Release() => this.Files = (IList<ReleaseFile>) new List<ReleaseFile>();

    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public DateTime? DateReleased { get; set; }

    [DataMember]
    public bool IsCurrentRelease { get; set; }

    [DataMember]
    public Project Project { get; set; }

    [DataMember]
    public IList<ReleaseFile> Files { get; set; }

    [DataMember]
    public bool IsPublic { get; set; }

    [DataMember]
    public double Rating { get; set; }

    [DataMember]
    public int RatingsCount { get; set; }

    [DataMember]
    public int ReviewsCount { get; set; }

    [DataMember]
    public bool IsDisplayedOnHomePage { get; set; }
  }
}
