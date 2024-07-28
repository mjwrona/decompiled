// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenPackageFileInfo
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Maven.Server.Implementations;
using System.IO;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  public class MavenPackageFileInfo
  {
    public MavenPackageFileInfo(Stream stream, IMavenArtifactFilePath filePath)
    {
      this.Stream = stream;
      this.FilePath = filePath;
      this.IsPomFile = false;
    }

    public Stream Stream { get; }

    public IMavenArtifactFilePath FilePath { get; }

    public bool IsPomFile { get; set; }
  }
}
