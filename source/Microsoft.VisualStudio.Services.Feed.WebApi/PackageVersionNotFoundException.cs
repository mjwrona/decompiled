// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.WebApi.PackageVersionNotFoundException
// Assembly: Microsoft.VisualStudio.Services.Feed.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8DACB936-5231-4131-8ED8-082A1F46DC54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Feed.WebApi
{
  [Serializable]
  public class PackageVersionNotFoundException : VssServiceException
  {
    public PackageVersionNotFoundException(string message)
      : base(message)
    {
    }

    public PackageVersionNotFoundException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public static PackageVersionNotFoundException CreateById(
      string packageId,
      string packageVersionId,
      Guid feedId)
    {
      return new PackageVersionNotFoundException(Resources.Error_PackageVersionPackageIdAndVersionIdNotFoundMessage((object) packageId, (object) packageVersionId, (object) feedId));
    }

    public static PackageVersionNotFoundException CreateByIdRecycleBin(
      string packageId,
      string packageVersionId,
      Guid feedId)
    {
      return new PackageVersionNotFoundException(Resources.Error_PackageVersionPackageIdAndVersionIdNotFoundInRecycleBinMessage((object) packageId, (object) packageVersionId, (object) feedId));
    }

    public static PackageVersionNotFoundException CreateByName(
      string packageId,
      string normalizedPackageName,
      Guid feedId)
    {
      return new PackageVersionNotFoundException(Resources.Error_PackageVersionPackageIdAndNormalizedVersionNotFound((object) packageId, (object) normalizedPackageName, (object) feedId));
    }

    public static PackageVersionNotFoundException Create(
      string protocolType,
      string normalizedPackageName,
      string packageVersionId,
      string feedName)
    {
      return new PackageVersionNotFoundException(Resources.Error_PackageVersionFromTypeAndNameNotFoundMessage((object) protocolType, (object) normalizedPackageName, (object) packageVersionId, (object) feedName));
    }
  }
}
