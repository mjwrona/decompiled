// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPostUploadProcessing.Validation.ValidationPipelineStepFactory
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPostUploadProcessing.Validation.ValidationSteps;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPostUploadProcessing.Validation
{
  internal class ValidationPipelineStepFactory
  {
    public IList<IValidationPipelineStep> GetValidationPipelineSteps(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      List<IValidationPipelineStep> validationPipelineSteps = new List<IValidationPipelineStep>();
      validationPipelineSteps.Add((IValidationPipelineStep) new PackageValidationStep(requestContext, extension));
      if (this.ShouldAddVirusScanStep(requestContext, extension))
        validationPipelineSteps.Add((IValidationPipelineStep) new VirusScanStep());
      if (this.ShouldAddRepositorySigningStep(requestContext, extension))
        validationPipelineSteps.Add((IValidationPipelineStep) new RepositorySigningStep());
      if (this.ShouldAddSearchIndexPopulationStep(requestContext, extension))
        validationPipelineSteps.Add((IValidationPipelineStep) new SearchIndexPopulationStep());
      return (IList<IValidationPipelineStep>) validationPipelineSteps;
    }

    public IValidationPipelineStep GetValidationPipelineStep(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      ExtensionVersionValidationStep validationStep)
    {
      IValidationPipelineStep validationPipelineStep = (IValidationPipelineStep) null;
      switch (validationStep.StepType)
      {
        case 0:
          validationPipelineStep = (IValidationPipelineStep) new PackageValidationStep(requestContext, extension);
          break;
        case 1:
          validationPipelineStep = (IValidationPipelineStep) new VirusScanStep();
          break;
        case 3:
          validationPipelineStep = (IValidationPipelineStep) new RepositorySigningStep();
          break;
      }
      validationPipelineStep?.Initialize(validationStep);
      return validationPipelineStep;
    }

    private bool ShouldAddVirusScanStep(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      return extension != null && requestContext.ExecutionEnvironment.IsHostedDeployment & requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableVirusScan");
    }

    private bool ShouldAddRepositorySigningStep(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      if (extension != null)
      {
        int num1 = requestContext.ExecutionEnvironment.IsHostedDeployment ? 1 : 0;
        bool flag1 = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableRepositorySigningForVSCode");
        bool flag2 = requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.EnableVirusScan");
        int num2 = flag1 ? 1 : 0;
        if ((num1 & num2 & (flag2 ? 1 : 0)) != 0)
          return extension.IsVsCodeExtension();
      }
      return false;
    }

    private bool ShouldAddSearchIndexPopulationStep(
      IVssRequestContext requestContext,
      PublishedExtension extension)
    {
      if (extension == null)
        return false;
      return !(requestContext.ExecutionEnvironment.IsHostedDeployment & extension.IsVsCodeExtension()) || !requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.DisableSearchIndexPopulationStepForVSCode");
    }
  }
}
