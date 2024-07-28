// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPostUploadProcessing.Validation.ValidationSteps.SearchIndexPopulationStep
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
  internal class SearchIndexPopulationStep : ValidationPipelineStepBase
  {
    [StaticSafe]
    private static string s_stepName = nameof (SearchIndexPopulationStep);
    [StaticSafe("Grandfathered")]
    private static StepType s_stepType = StepType.SearchIndexPopulation;
    private const string s_layer = "SearchIndexPopulationStep";

    public SearchIndexPopulationStep()
      : base(SearchIndexPopulationStep.s_stepName, SearchIndexPopulationStep.s_stepType)
    {
    }

    public override void Initialize(Guid parentId, Guid validationId)
    {
      base.Initialize(parentId, validationId);
      this.m_resultMessage = string.Empty;
    }

    public override string BeginValidation(
      IVssRequestContext requestContext,
      PublishedExtension extension,
      Stream packageStream)
    {
      requestContext.TraceEnter(12060105, "Gallery", nameof (SearchIndexPopulationStep), nameof (BeginValidation));
      base.BeginValidation(requestContext, extension, packageStream);
      this.m_Result = ValidationStatus.InProgress;
      this.ValidationContext = string.Empty;
      try
      {
        new SearchIndexer(requestContext).PopulateIndex(requestContext, new List<PublishedExtension>()
        {
          extension
        }, (string) null);
        this.m_Result = ValidationStatus.Success;
      }
      catch (Exception ex)
      {
        this.m_Result = ValidationStatus.Failure;
        requestContext.TraceException(12060103, "Gallery", nameof (SearchIndexPopulationStep), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(12060105, "Gallery", nameof (SearchIndexPopulationStep), nameof (BeginValidation));
      }
      return this.ValidationContext;
    }
  }
}
