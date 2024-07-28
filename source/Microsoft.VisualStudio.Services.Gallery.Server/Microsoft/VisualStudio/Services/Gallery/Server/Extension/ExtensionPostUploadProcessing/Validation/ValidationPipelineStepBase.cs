// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPostUploadProcessing.Validation.ValidationPipelineStepBase
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.IO;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPostUploadProcessing.Validation
{
  internal abstract class ValidationPipelineStepBase : IValidationPipelineStep
  {
    protected ValidationStatus m_Result;
    protected string m_resultMessage;

    public Guid ValidationId { get; set; }

    public Guid ParentValidationId { get; set; }

    public string Name { get; set; }

    public StepType StepType { get; set; }

    public DateTime StartTime { get; set; }

    public string LogFileName => this.Name + ".txt";

    public string ValidationContext { get; set; }

    public ValidationPipelineStepBase(string name, StepType stepType)
    {
      this.Name = name;
      this.StepType = stepType;
    }

    public virtual void Initialize(Guid parentId, Guid validationId)
    {
      this.ParentValidationId = parentId;
      this.ValidationId = validationId;
      this.ValidationContext = string.Empty;
      this.m_Result = ValidationStatus.NotStarted;
    }

    public virtual void Initialize(ExtensionVersionValidationStep validationStep)
    {
      this.ParentValidationId = validationStep.ParentId;
      this.ValidationId = validationStep.StepId;
      this.ValidationContext = validationStep.ValidationContext;
      this.StartTime = validationStep.StartTime;
      this.m_Result = (ValidationStatus) validationStep.StepStatus;
    }

    public virtual void PostValidation(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
    }

    public virtual string BeginValidation(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      Stream packageStream)
    {
      this.StartTime = DateTime.UtcNow;
      return this.ValidationContext;
    }

    public virtual TimeSpan GetRecheckTime(IVssRequestContext requestContext) => TimeSpan.MaxValue;

    public virtual ValidationStatus RetrieveResult(
      IVssRequestContext requestContext,
      PublishedExtension extension = null)
    {
      return this.m_Result;
    }

    public virtual string RetrieveResultMessage() => this.m_resultMessage;

    public virtual string StartRepositorySigning(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      bool isRetry = false)
    {
      return string.Empty;
    }
  }
}
