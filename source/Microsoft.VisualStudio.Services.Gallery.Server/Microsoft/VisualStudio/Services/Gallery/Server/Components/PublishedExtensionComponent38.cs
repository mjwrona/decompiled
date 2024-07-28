// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.PublishedExtensionComponent38
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class PublishedExtensionComponent38 : PublishedExtensionComponent37
  {
    public override ExtensionVersionValidationStep UpdateValidationStep(
      IVssRequestContext requestContext,
      ExtensionVersionValidationStep step)
    {
      this.ValidateStep(step);
      this.PrepareStoredProcedure("Gallery.prc_UpdateValidationStep");
      this.BindGuid("stepId", step.StepId);
      this.BindGuid("parentId", step.ParentId);
      this.BindInt("stepStatus", step.StepStatus);
      this.BindDateTime2("startTime", step.StartTime);
      this.BindDateTime2("lastUpdated", step.LastUpdated);
      this.BindString("validationContext", step.ValidationContext, 4096, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindInt("resultFileId", step.ResultFileId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_UpdateValidationStep", requestContext))
      {
        resultCollection.AddBinder<ExtensionVersionValidationStep>((ObjectBinder<ExtensionVersionValidationStep>) new ExtensionVersionValidationStepBinder());
        List<ExtensionVersionValidationStep> items = resultCollection.GetCurrent<ExtensionVersionValidationStep>().Items;
        return items.Count == 0 ? (ExtensionVersionValidationStep) null : items.First<ExtensionVersionValidationStep>();
      }
    }

    private void ValidateStep(ExtensionVersionValidationStep step)
    {
      ArgumentUtility.CheckForNull<ExtensionVersionValidationStep>(step, nameof (step));
      ArgumentUtility.CheckForEmptyGuid(step.StepId, "StepId");
      ArgumentUtility.CheckForEmptyGuid(step.ParentId, "ParentId");
    }
  }
}
