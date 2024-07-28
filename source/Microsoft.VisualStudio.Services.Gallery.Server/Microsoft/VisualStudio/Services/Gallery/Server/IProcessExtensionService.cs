// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.IProcessExtensionService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  [DefaultServiceImplementation(typeof (ProcessExtensionService))]
  internal interface IProcessExtensionService : IVssFrameworkService
  {
    bool ProcessExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      Guid extensionId,
      string version,
      string targetPlatform,
      Guid validationId,
      Guid vsid,
      out string resultMessage);

    bool UpdateExtensionValidationStatus(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      Guid extensionId,
      string version,
      string targetPlatform,
      Guid validationId,
      Guid vsid,
      out string resultMessage);
  }
}
