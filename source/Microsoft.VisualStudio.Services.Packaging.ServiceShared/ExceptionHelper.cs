// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.ExceptionHelper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public static class ExceptionHelper
  {
    public static Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException PackageNotFound(
      IPackageIdentity packageIdentity,
      FeedCore feed,
      Exception innerException = null)
    {
      return ExceptionHelper.PackageNotFound(feed, packageIdentity.Name, packageIdentity.Version, innerException);
    }

    public static Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException PackageNotFound(
      FeedCore feed,
      IPackageName packageName,
      IPackageVersion packageVersion,
      Exception innerException = null)
    {
      return new Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException(Resources.Error_PackageVersionNotFound((object) packageName.DisplayName, (object) packageVersion.DisplayVersion, (object) feed.FullyQualifiedName), innerException);
    }

    public static Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException PackageNotFound(
      Guid feedId,
      IPackageName packageName,
      IPackageVersion packageVersion,
      Exception innerException = null)
    {
      return new Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException(Resources.Error_PackageVersionNotFoundInFeedWithId((object) packageName.DisplayName, (object) packageVersion.DisplayVersion, (object) feedId), innerException);
    }

    public static Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException PackageNotFound(
      FeedCore feed,
      IPackageIdentity packageIdentity,
      string filePath,
      Exception innerException = null)
    {
      return new Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException(Resources.Error_PackageFileNotFound((object) feed.FullyQualifiedName, (object) packageIdentity, (object) filePath), innerException);
    }

    public static Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException PackageInnerFileNotFound(
      IPackageIdentity package,
      string outerFilePath,
      string innerFilePath,
      Exception innerException = null)
    {
      return new Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException(Resources.Error_PackageInnerFileNotFound((object) innerFilePath, (object) outerFilePath, (object) package.DisplayStringForMessages));
    }

    public static Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException PackageNotFound(
      string message,
      Exception innerException = null)
    {
      return new Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException(message, innerException);
    }

    public static RequiredArgumentMissingException ArgumentMissing(
      string argumentName,
      Exception innerException = null)
    {
      return new RequiredArgumentMissingException(Resources.Error_ArgumentRequired((object) argumentName), innerException);
    }

    public static HttpMethodNotAllowedException HttpMethodNotAllowed(
      HttpMethod method,
      Exception innerException = null)
    {
      return new HttpMethodNotAllowedException(method.ToString(), innerException);
    }
  }
}
