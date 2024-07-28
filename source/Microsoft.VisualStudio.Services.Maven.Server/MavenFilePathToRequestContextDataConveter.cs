// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenFilePathToRequestContextDataConveter
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Maven.Server.Exceptions;
using Microsoft.VisualStudio.Services.Maven.Server.Implementations;
using Microsoft.VisualStudio.Services.Maven.Server.Models;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  internal class MavenFilePathToRequestContextDataConveter : 
    IConverter<IMavenFilePath, object>,
    IHaveInputType<IMavenFilePath>,
    IHaveOutputType<object>
  {
    public object Convert(IMavenFilePath input)
    {
      switch (input)
      {
        case IMavenFullyQualifiedFilePath qualifiedFilePath:
          return (object) new MavenPackageIdentity(qualifiedFilePath.PackageName, qualifiedFilePath.PackageVersion);
        case IMavenArtifactIdLevelMetadataFilePath metadataFilePath1:
          return (object) metadataFilePath1.PackageName;
        case IMavenGroupIdLevelMetadataFilePath metadataFilePath2:
          return (object) new MavenPackageName(metadataFilePath2.GroupId, metadataFilePath2.GroupId);
        default:
          throw new UnrecognizedMavenFilePathException();
      }
    }

    public string GetKey(IMavenFilePath filePath, object _)
    {
      switch (filePath)
      {
        case IMavenFullyQualifiedFilePath _:
          return "Packaging.PackageIdentity";
        case IMavenArtifactIdLevelMetadataFilePath _:
          return "Packaging.PackageName";
        case IMavenGroupIdLevelMetadataFilePath _:
          return "Packaging.PackageName";
        default:
          throw new UnrecognizedMavenFilePathException();
      }
    }
  }
}
