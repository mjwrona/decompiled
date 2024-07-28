// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PackageIngestion.InvalidPackageExceptionHelper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.WebApi.Exceptions;
using System;

namespace Microsoft.VisualStudio.Services.NuGet.Server.PackageIngestion
{
  internal static class InvalidPackageExceptionHelper
  {
    public static InvalidPackageException PackageIdTooLong(
      int maxPackageIdLength,
      Exception innerException = null)
    {
      return InvalidPackageExceptionHelper.ConstructWithMessage(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_PackageIdTooLong((object) maxPackageIdLength), innerException);
    }

    public static InvalidPackageException PackageIdInvalid(Exception innerException = null) => InvalidPackageExceptionHelper.ConstructWithMessage(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_PackageIdInvalid(), innerException);

    public static InvalidPackageException DependencyVersionFormat(
      string targetFramework,
      string id,
      Exception innerException = null)
    {
      return InvalidPackageExceptionHelper.ConstructWithMessage(string.IsNullOrWhiteSpace(targetFramework) ? Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_PackageDependencyVersionFormatNoGroup((object) id) : Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_PackageDependencyVersionFormat((object) id, (object) targetFramework), innerException);
    }

    public static InvalidPackageException DependencyIdNotSpecified(Exception innerException = null) => InvalidPackageExceptionHelper.ConstructWithMessage(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_DependencyIdNotSpecified(), innerException);

    public static InvalidPackageException PackageDataIsCorrupt(
      string detail,
      Exception innerException = null)
    {
      return InvalidPackageExceptionHelper.ConstructWithMessage(detail, innerException);
    }

    public static InvalidPackageException PackageTooSmall(Exception innerException = null) => InvalidPackageExceptionHelper.ConstructWithMessage(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_PackageTooSmall(), innerException);

    public static InvalidPackageException CouldntFindNuspec(Exception innerException = null) => InvalidPackageExceptionHelper.ConstructWithMessage(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_CouldNotFindNuspec(), innerException);

    public static InvalidPackageException UnreadableNuspec(Exception innerException = null) => InvalidPackageExceptionHelper.ConstructWithMessage(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_UnreadableNuspec(), innerException);

    public static InvalidPackageException InvalidVersionFormat(Exception innerException = null) => InvalidPackageExceptionHelper.ConstructWithMessage(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_IngestionInvalidVersion(), innerException);

    public static InvalidPackageException VersionHadBuildMetadata(Exception innerException = null) => InvalidPackageExceptionHelper.ConstructWithMessage(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_InvalidPackageMetadataInVersion(), innerException);

    public static InvalidPackageException InvalidVersionLength(
      int maxPackageVersionLength,
      Exception innerException = null)
    {
      return InvalidPackageExceptionHelper.ConstructWithMessage(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_PackageVersionTooLong((object) maxPackageVersionLength), innerException);
    }

    public static InvalidPackageException MissingNuspecElement(
      string elementName,
      Exception innerException = null)
    {
      return InvalidPackageExceptionHelper.ConstructWithMessage(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_MissingNuspecElement((object) elementName), innerException);
    }

    public static InvalidPackageException NuspecTooLong(
      int nuspecSize,
      int maxNuspecSize,
      Exception innerException = null)
    {
      return InvalidPackageExceptionHelper.ConstructWithMessage(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_NuspecTooLarge((object) nuspecSize, (object) maxNuspecSize), innerException);
    }

    private static InvalidPackageException ConstructWithMessage(
      string message,
      Exception innerException)
    {
      return new InvalidPackageException(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_InvalidPackageWithArgument((object) message), innerException);
    }
  }
}
