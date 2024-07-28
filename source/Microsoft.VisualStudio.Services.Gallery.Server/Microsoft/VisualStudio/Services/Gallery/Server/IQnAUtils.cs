// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.IQnAUtils
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal interface IQnAUtils
  {
    QnAItem SanitizeItem(QnAItem qnAItem);

    Microsoft.VisualStudio.Services.Identity.Identity GetAuthenticatedIdentity(
      IVssRequestContext requestContext);

    PublishedExtension ValidateAndGetPublishedExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName);

    bool IsUserPublisherWithRequiredPermissions(
      IVssRequestContext requestContext,
      PublishedExtension extension);

    QnAItem ConvertToQnAItem(IVssRequestContext requestContext, ExtensionQnAItem extensionQnAItem);

    QnAMode GetQnAMode(IDictionary<string, string> extensionProperties);

    void PublishReCaptchaTokenCIForQnA(
      IVssRequestContext requestContext,
      IDictionary<string, object> ciData);
  }
}
