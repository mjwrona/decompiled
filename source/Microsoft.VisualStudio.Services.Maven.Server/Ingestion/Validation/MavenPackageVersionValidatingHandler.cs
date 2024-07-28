// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Ingestion.Validation.MavenPackageVersionValidatingHandler
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Maven.Server.Exceptions;
using Microsoft.VisualStudio.Services.Maven.Server.Implementations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Maven.Server.Ingestion.Validation
{
  public class MavenPackageVersionValidatingHandler : 
    IAsyncHandler<MavenPackageFileInfo>,
    IAsyncHandler<MavenPackageFileInfo, NullResult>,
    IHaveInputType<MavenPackageFileInfo>,
    IHaveOutputType<NullResult>
  {
    public Task<NullResult> Handle(MavenPackageFileInfo request)
    {
      IMavenArtifactFilePath filePath = request.FilePath;
      if (filePath.UsesSnapshotLiteralVersion)
        throw new ArtifactsWithNonUniqueSnapshotVersionNotSupportedException(Resources.Error_SnapshotLiteralNotSupported((object) filePath.FileName));
      if (filePath.PackageVersion.Parser.IsGuid)
        throw new InvalidVersionException(Resources.Error_GuidVersionsNotAllowed());
      if (filePath.PackageVersion.ExceptionComputingSortableVersion != null)
        throw filePath.PackageVersion.ExceptionComputingSortableVersion;
      return NullResult.NullTask;
    }
  }
}
