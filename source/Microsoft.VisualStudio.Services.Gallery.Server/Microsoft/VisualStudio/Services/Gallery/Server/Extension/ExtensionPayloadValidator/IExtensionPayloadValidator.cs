// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPayloadValidator.IExtensionPayloadValidator
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.IO;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPayloadValidator
{
  internal interface IExtensionPayloadValidator
  {
    PayloadValidationResult ValidatePayload(
      IVssRequestContext requestContext,
      Stream payloadStream,
      string fileName,
      Publisher publisher,
      PublishedExtension existingExtension);

    ExtensionPayload GetPayloadFromValidationResult(PayloadValidationResult payloadValidationResult);

    PayloadValidationResult ValidatePayloadDetails(
      IVssRequestContext requestContext,
      Stream payloadStream,
      string fileName,
      UnpackagedExtensionData extensionData,
      Publisher publisher,
      PublishedExtension existingExtension);

    PayloadValidationResult ScanOverviewContentForBlockedHosts(
      IVssRequestContext requestContext,
      int detailsAssetFileId,
      PayloadValidationResult payloadValidation,
      UnpackagedExtensionData extensionData,
      Publisher publisher);
  }
}
