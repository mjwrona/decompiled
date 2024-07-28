// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.IExtensionDraftService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [DefaultServiceImplementation(typeof (ExtensionDraftService))]
  internal interface IExtensionDraftService : IVssFrameworkService
  {
    ExtensionDraft CreateExtensionDraftForNewExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string payloadFileName,
      string productType,
      Stream payloadStream);

    ExtensionDraft CreateExtensionDraftForEditExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName);

    ExtensionDraft UpdatePayloadInDraft(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      Guid draftId,
      string payloadFileName,
      Stream payloadStream);

    IList<ExtensionDraft> GetAllDrafts(IVssRequestContext requestContext, Guid userId);

    void DeleteExtensionDraft(IVssRequestContext requestContext, Guid draftId);

    ExtensionDraft CreateExtensionFromDraft(
      IVssRequestContext requestContext,
      string publisherName,
      Guid draftId,
      UnpackagedExtensionData extensionData);

    ExtensionDraft CancelDraft(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      Guid draftId);

    ExtensionDraft UpdateExtensionFromDraft(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      Guid draftId,
      UnpackagedExtensionData extensionData);

    ExtensionDraftAsset AddAssetInDraftForNewExtension(
      IVssRequestContext requestContext,
      string publisherName,
      Guid draftId,
      string assetType,
      Stream assetStream);

    ExtensionDraftAsset GetAssetFromNewExtensionDraft(
      IVssRequestContext requestContext,
      string publisherName,
      Guid draftId,
      string assetType);

    ExtensionDraftAsset AddAssetInDraftForEditExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      Guid draftId,
      string assetType,
      Stream assetStream);

    ExtensionDraftAsset GetAssetFromEditExtensionDraft(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      Guid draftId,
      string assetType);
  }
}
