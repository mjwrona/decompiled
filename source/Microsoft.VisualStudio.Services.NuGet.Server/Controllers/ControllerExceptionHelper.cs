// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Controllers.ControllerExceptionHelper
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.NuGet.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Controllers
{
  internal static class ControllerExceptionHelper
  {
    public static RequiredArgumentMissingException ArgumentRequiredPackageId(
      Exception innerException = null)
    {
      return new RequiredArgumentMissingException(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_PackageIdRequired(), innerException);
    }

    public static PackageSubresourceNotFoundException PackageSubresourceNotFound(
      string subresourceName,
      VssNuGetPackageIdentity packageIdentity,
      string expectedFile,
      Exception innerException = null)
    {
      return new PackageSubresourceNotFoundException(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_PackageSubresourceDoesntExistDidYouMean((object) subresourceName, (object) packageIdentity.ToString(), (object) expectedFile), innerException);
    }

    public static PackageSubresourceNotFoundException PackageSubresourceNotFound(
      string subresourceName,
      VssNuGetPackageIdentity packageIdentity,
      Exception innerException = null)
    {
      return new PackageSubresourceNotFoundException(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_PackageSubresourceDoesntExist((object) subresourceName, (object) packageIdentity.ToString()), innerException);
    }

    [Obsolete("Use Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException")]
    public static Microsoft.VisualStudio.Services.NuGet.WebApi.Exceptions.PackageNotFoundException PackageNotFound_LegacyNuGetSpecificType(
      IPackageIdentity packageIdentity,
      FeedCore feed,
      Exception innerException = null)
    {
      return new Microsoft.VisualStudio.Services.NuGet.WebApi.Exceptions.PackageNotFoundException(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_PackageVersionNotFound((object) packageIdentity.Name.DisplayName, (object) packageIdentity.Version.DisplayVersion, (object) feed.FullyQualifiedName), innerException);
    }

    [Obsolete("Use Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException")]
    public static Microsoft.VisualStudio.Services.NuGet.WebApi.Exceptions.PackageNotFoundException PackageNotFound_LegacyNuGetSpecificType(
      IPackageName packageName,
      FeedCore feed,
      Exception innerException = null)
    {
      return new Microsoft.VisualStudio.Services.NuGet.WebApi.Exceptions.PackageNotFoundException(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_PackageNotFound((object) packageName.DisplayName, (object) feed.FullyQualifiedName), innerException);
    }

    public static RequiredArgumentMissingException ArgumentRequiredPackageVersion(
      Exception innerException = null)
    {
      return new RequiredArgumentMissingException(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_PackageVersionRequired(), innerException);
    }

    public static ArgumentFormatException InvalidVersionFormat(Exception innerException = null) => new ArgumentFormatException(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_IngestionInvalidVersion(), innerException);

    public static ReadOnlyViewOperationException FeedIsReadOnly(
      string feed,
      Exception innerException = null)
    {
      return new ReadOnlyViewOperationException(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_CannotPerformOperationOnReadOnlyFeed((object) feed), innerException);
    }

    public static ArgumentException ValueCannotBeNull(string valueName) => new ArgumentException(Microsoft.VisualStudio.Services.NuGet.Server.Resources.Error_ValueCannotBeNull((object) valueName));
  }
}
