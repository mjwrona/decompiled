// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Models.MavenPackageFile
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Maven.Server.Models
{
  public class MavenPackageFile : PackageFile
  {
    public MavenPackageFile()
    {
      this.DateAdded = DateTime.UtcNow;
      this.Size = 0L;
    }

    [IgnoreDataMember]
    public string StorageId
    {
      get => this.Metadata.StorageId;
      set => this.Metadata.StorageId = value;
    }

    [IgnoreDataMember]
    public long Size
    {
      get => this.Metadata.Size;
      set => this.Metadata.Size = value;
    }

    [IgnoreDataMember]
    public DateTime DateAdded
    {
      get => this.Metadata.DateAdded;
      set => this.Metadata.DateAdded = value;
    }

    [IgnoreDataMember]
    public string Content
    {
      get => this.Metadata.Content;
      set => this.Metadata.Content = value;
    }

    [IgnoreDataMember]
    private MavenFileInfo Metadata => this.Data<MavenFileInfo>();

    public override string ToString() => this.Name;
  }
}
