// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPostUploadProcessing.Validation.IValidationPipelineStep
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.IO;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPostUploadProcessing.Validation
{
  internal interface IValidationPipelineStep
  {
    Guid ValidationId { get; }

    Guid ParentValidationId { get; }

    StepType StepType { get; set; }

    DateTime StartTime { get; }

    string ValidationContext { get; set; }

    void Initialize(Guid parentId, Guid validationId);

    void Initialize(ExtensionVersionValidationStep validationStep);

    string BeginValidation(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      Stream packageStream);

    ValidationStatus RetrieveResult(IVssRequestContext requestContext, PublishedExtension extension = null);

    string RetrieveResultMessage();

    void PostValidation(IVssRequestContext requestContext, PublishedExtension extension);

    TimeSpan GetRecheckTime(IVssRequestContext requestContext);

    string StartRepositorySigning(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      bool isRetry = false);
  }
}
