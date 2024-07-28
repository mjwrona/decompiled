// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ExtensionVersionValidationService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPostUploadProcessing.Validation;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.IO;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal class ExtensionVersionValidationService : 
    IExtensionVersionValidationService,
    IPostUploadExtensionProcessorService,
    IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public bool ProcessExtension(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      Stream packageStream,
      Guid validationId,
      Guid vsid,
      out string resultMessage)
    {
      bool flag = false;
      requestContext.TraceEnter(12061094, "Gallery", "VersionValidationService", nameof (ProcessExtension));
      requestContext.GetService<IPublishedExtensionService>();
      ValidationPipeline validationPipeline = new ValidationPipeline();
      resultMessage = validationPipeline.Run(requestContext, extension, packageStream, validationId, vsid);
      if (resultMessage.IsNullOrEmpty<char>())
        flag = true;
      requestContext.TraceLeave(12061094, "Gallery", "VersionValidationService", nameof (ProcessExtension));
      return flag;
    }

    public bool UpdateValidationStatus(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      Guid validationId,
      Guid vsid,
      out string resultMessage)
    {
      bool flag = true;
      resultMessage = string.Empty;
      requestContext.TraceEnter(12061094, "Gallery", "VersionValidationService", nameof (UpdateValidationStatus));
      ValidationPipeline validationPipeline = new ValidationPipeline();
      resultMessage = validationPipeline.ProcessValidationResult(requestContext, extension, validationId, vsid);
      if (!resultMessage.IsNullOrEmpty<char>())
        flag = false;
      requestContext.TraceLeave(12061094, "Gallery", "VersionValidationService", nameof (UpdateValidationStatus));
      return flag;
    }
  }
}
