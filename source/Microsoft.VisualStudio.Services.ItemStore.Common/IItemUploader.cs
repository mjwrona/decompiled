// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Common.IItemUploader
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 44753C0C-D541-4975-AF3F-2B606DE6FF70
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Common.dll

using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.BlobStore.WebApi;
using Microsoft.VisualStudio.Services.ItemStore.Common.Telemetry;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ItemStore.Common
{
  [CLSCompliant(false)]
  public interface IItemUploader : IDisposable
  {
    Task<ItemAssociationResult> AssociateFilesAsync(
      IEnumerable<FileBlobDescriptor> fileChunkIds,
      IItemAssociator itemAssociator,
      ItemTreeInfo specItemTreeInfo,
      bool abortIfAlreadyExists,
      IDedupUploadSession uploadSession,
      CancellationToken cancellationToken,
      object routeValues);

    Task<List<ItemUploaderRecord>> UploadFilesAsync(
      IEnumerable<FileBlobDescriptor> fileChunkIds,
      IItemAssociator itemAssociator,
      ItemTreeInfo specItemTreeInfo,
      bool abortIfAlreadyExists,
      IDedupUploadSession uploadSession,
      IDomainId domainId,
      CancellationToken cancellationToken,
      object routeValues);

    Task<List<ItemUploaderRecord>> UploadAndAssociateFilesAsync(
      AssociationsStatus firstAssociateResult,
      ItemUploaderRecord publishRecord,
      IEnumerable<FileBlobDescriptor> fileChunkIds,
      IItemAssociator itemAssociator,
      bool abortIfAlreadyExists,
      IDedupUploadSession uploadSession,
      IDomainId domainId,
      CancellationToken cancellationToken,
      object routeValues);
  }
}
