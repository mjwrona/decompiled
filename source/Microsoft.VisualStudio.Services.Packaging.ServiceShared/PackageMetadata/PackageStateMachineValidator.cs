// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata.PackageStateMachineValidator
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata
{
  public static class PackageStateMachineValidator
  {
    public static MetadataStateValidationResult ValidateRestoreToFeedTransition<TIdentity>(
      IMetadataEntry metadataEntry,
      IPackageRequest<TIdentity> packageRequest)
      where TIdentity : IPackageIdentity
    {
      PackageStateMachineValidator.ThrowIfNullInRecycleBin<TIdentity>(metadataEntry, packageRequest);
      if (!metadataEntry.IsDeleted())
        throw new Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_DeleteBeforeRestoringToFeed((object) metadataEntry.PackageIdentity.DisplayStringForMessages));
      if (metadataEntry.IsPermanentlyDeleted())
        throw new Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_CannotRestorePermanentlyDeletedPackage((object) metadataEntry.PackageIdentity.DisplayStringForMessages));
      PackageStateMachineValidator.ThrowIfUningestedUpstreamPackage<TIdentity>(metadataEntry, packageRequest);
      return MetadataStateValidationResult.Continue;
    }

    public static MetadataStateValidationResult ValidateViewPromotionTransition<TIdentity>(
      IMetadataEntry metadataEntry,
      IPackageRequest<TIdentity> packageRequest)
      where TIdentity : IPackageIdentity
    {
      if (metadataEntry == null || metadataEntry.IsDeleted() || metadataEntry.IsPermanentlyDeleted())
        throw ExceptionHelper.PackageNotFound((IPackageIdentity) packageRequest.PackageId, packageRequest.Feed);
      PackageStateMachineValidator.ThrowIfUningestedUpstreamPackage<TIdentity>(metadataEntry, packageRequest);
      return MetadataStateValidationResult.Continue;
    }

    public static MetadataStateValidationResult ValidatePermanentDeleteTransition<TIdentity>(
      IMetadataEntry metadataEntry,
      IPackageRequest<TIdentity> packageRequest)
      where TIdentity : IPackageIdentity
    {
      PackageStateMachineValidator.ThrowIfNullInRecycleBin<TIdentity>(metadataEntry, packageRequest);
      if (!metadataEntry.IsDeleted())
        throw new Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_DeleteBeforePermanentDeletion((object) metadataEntry.PackageIdentity.DisplayStringForMessages));
      if (metadataEntry.IsPermanentlyDeleted())
        return MetadataStateValidationResult.SilentlyIgnoreRequest;
      PackageStateMachineValidator.ThrowIfUningestedUpstreamPackage<TIdentity>(metadataEntry, packageRequest);
      return MetadataStateValidationResult.Continue;
    }

    public static MetadataStateValidationResult ValidateDeleteTransition<TIdentity>(
      IMetadataEntry metadataEntry,
      IPackageRequest<TIdentity> packageRequest)
      where TIdentity : IPackageIdentity
    {
      if (metadataEntry == null)
        throw ExceptionHelper.PackageNotFound((IPackageIdentity) packageRequest.PackageId, packageRequest.Feed);
      if (metadataEntry.IsDeleted() || metadataEntry.IsPermanentlyDeleted())
        return MetadataStateValidationResult.SilentlyIgnoreRequest;
      PackageStateMachineValidator.ThrowIfUningestedUpstreamPackage<TIdentity>(metadataEntry, packageRequest);
      return MetadataStateValidationResult.Continue;
    }

    private static void ThrowIfNullInRecycleBin<TIdentity>(
      IMetadataEntry metadataEntry,
      IPackageRequest<TIdentity> packageRequest)
      where TIdentity : IPackageIdentity
    {
      if (metadataEntry == null)
        throw new Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions.PackageNotFoundException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_PackageVersionNotFoundInRecycleBin((object) packageRequest.PackageId.DisplayStringForMessages));
    }

    public static void ThrowIfUningestedUpstreamPackage<TIdentity>(
      IMetadataEntry metadataEntry,
      IPackageRequest<TIdentity> packageRequest)
      where TIdentity : IPackageIdentity
    {
      if (!metadataEntry.IsLocal)
        throw new UnexpectedUningestedUpstreamPackageException(packageRequest.Feed.Id, metadataEntry.PackageIdentity);
      if (metadataEntry.IsUpstreamCached && !packageRequest.Feed.Capabilities.HasFlag((Enum) FeedCapabilities.UpstreamV2))
        throw new UnexpectedUningestedUpstreamPackageException(packageRequest.Feed.Id, metadataEntry.PackageIdentity);
    }
  }
}
