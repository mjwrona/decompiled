// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPostUploadProcessing.Validation.ValidationSteps.PackageValidationStep
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPostUploadProcessing.Validation.ValidationSteps
{
  internal class PackageValidationStep : ValidationPipelineStepBase
  {
    [StaticSafe]
    private static string s_stepName = nameof (PackageValidationStep);
    private const StepType s_stepType = StepType.PackageValidation;
    private const string s_layer = "PackageValidationStep";
    internal List<PackageValidationStepBase> packageValidationSteps;

    public PackageValidationStep(IVssRequestContext requestContext, PublishedExtension extension)
      : base(PackageValidationStep.s_stepName, StepType.PackageValidation)
    {
      this.packageValidationSteps = new List<PackageValidationStepBase>();
      if (this.ShouldAddZipValidationStep(requestContext, extension))
        this.packageValidationSteps.Add((PackageValidationStepBase) new ZipValidationStep());
      if (!extension.IsVsExtension())
        this.packageValidationSteps.Add((PackageValidationStepBase) new SchemaValidationStep());
      this.packageValidationSteps.Add((PackageValidationStepBase) new CertifiedPublisherValidationStep());
      this.packageValidationSteps.Add((PackageValidationStepBase) new SvgContentValidationStep());
      if (!extension.IsVsCodeExtension())
        return;
      if (requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableMlSpamCheckForVsCode"))
        this.packageValidationSteps.Add((PackageValidationStepBase) new MlSpamValidationStep());
      if (this.IsNewVsCodeExtension(extension))
      {
        if (!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableSpamChecksForVSCode"))
          return;
        this.packageValidationSteps.Add((PackageValidationStepBase) new SpamValidationStep());
      }
      else
      {
        if (!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableSpamChecksForVSCodeExtensionUpdate"))
          return;
        this.packageValidationSteps.Add((PackageValidationStepBase) new SpamValidationStep());
      }
    }

    public override void Initialize(Guid parentId, Guid validationId)
    {
      base.Initialize(parentId, validationId);
      foreach (PackageValidationStepBase packageValidationStep in this.packageValidationSteps)
        packageValidationStep.Initialize(parentId);
      this.m_resultMessage = string.Empty;
    }

    public override string BeginValidation(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      Stream packageStream)
    {
      base.BeginValidation(requestContext, extension, packageStream);
      this.m_Result = ValidationStatus.InProgress;
      foreach (PackageValidationStepBase packageValidationStep in this.packageValidationSteps)
      {
        packageValidationStep.BeginValidation(requestContext, extension, packageStream);
        this.m_resultMessage += packageValidationStep.ResultMessage;
        if (packageValidationStep.StepType == PackageValidationStepBase.ValidationStepType.ZipValidation)
        {
          if (!string.IsNullOrWhiteSpace(packageValidationStep.ResultMessage))
            break;
        }
      }
      if (string.IsNullOrWhiteSpace(this.m_resultMessage))
        this.m_Result = ValidationStatus.Success;
      else
        this.m_Result = ValidationStatus.Failure;
      return this.ValidationContext;
    }

    public override void PostValidation(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      foreach (PackageValidationStepBase packageValidationStep in this.packageValidationSteps)
        packageValidationStep.PostValidation(requestContext, extension);
    }

    private bool ShouldAddZipValidationStep(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      bool flag1 = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.CheckForZipSlipInVS");
      bool flag2 = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.CheckForZipSlipInVSCode");
      if (extension.IsVsixTypeExtension())
      {
        int num1 = extension.IsVsExtension() ? 1 : (extension.IsVsForMacExtension() ? 1 : 0);
        bool flag3 = extension.IsVsCodeExtension();
        int num2 = flag1 ? 1 : 0;
        if ((num1 & num2) != 0 || flag3 & flag2)
          return true;
      }
      return false;
    }

    private bool IsNewVsCodeExtension(PublishedExtension extension)
    {
      List<ExtensionVersion> versions = extension.Versions;
      // ISSUE: explicit non-virtual call
      return versions != null && __nonvirtual (versions.Count) == 1;
    }
  }
}
