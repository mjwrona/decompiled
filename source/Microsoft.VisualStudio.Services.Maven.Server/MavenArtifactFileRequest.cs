// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenArtifactFileRequest
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Maven.Server.Implementations;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  public class MavenArtifactFileRequest : PackageFileRequest<MavenPackageIdentity>
  {
    public MavenArtifactFileRequest(
      IFeedRequest feedRequest,
      IMavenArtifactFilePath filePath,
      bool requireContent,
      bool streamContent)
      : base(feedRequest, new MavenPackageIdentity(filePath.PackageName, filePath.PackageVersion), filePath.FileName)
    {
      this.RequireContent = requireContent;
      this.StreamContent = streamContent;
    }

    public bool RequireContent { get; }

    public bool StreamContent { get; }
  }
}
