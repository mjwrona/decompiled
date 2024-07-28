// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPostUploadProcessing.Validation.PackageValidationStepBase
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.IO;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPostUploadProcessing.Validation
{
  internal abstract class PackageValidationStepBase
  {
    protected ValidationStatus m_result;
    protected Guid m_validationId;

    public PackageValidationStepBase(
      PackageValidationStepBase.ValidationStepType stepType)
    {
      this.StepType = stepType;
    }

    public string ResultMessage { get; set; }

    public PackageValidationStepBase.ValidationStepType StepType { get; set; }

    public abstract void BeginValidation(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      Stream packageStream);

    public virtual void PostValidation(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
    }

    public virtual void Initialize(Guid validationId)
    {
      this.ResultMessage = string.Empty;
      this.m_validationId = validationId;
      this.m_result = ValidationStatus.NotStarted;
    }

    internal enum ValidationStepType
    {
      ZipValidation,
      SchemaValidation,
      CertifiedPublisherValidation,
      SvgContentValidation,
      SpamValidation,
      MlSpamValidation,
    }
  }
}
