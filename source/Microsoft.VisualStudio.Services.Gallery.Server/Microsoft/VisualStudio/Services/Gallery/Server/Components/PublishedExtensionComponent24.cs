// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.PublishedExtensionComponent24
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class PublishedExtensionComponent24 : PublishedExtensionComponent23
  {
    public override ExtensionSearchResult SearchExtensions(
      ExtensionSearchParams searchParams,
      ExtensionQueryFlags flags)
    {
      this.PrepareStoredProcedure("Gallery.prc_SearchExtensions");
      this.BindSearchFilterValueTable("searchFilters", (IEnumerable<SearchCriteria>) searchParams.CriteriaList);
      this.BindInt("pageNumber", searchParams.PageNumber);
      this.BindInt("pageSize", searchParams.PageSize);
      this.BindInt("sortByType", searchParams.SortBy);
      this.BindInt(nameof (flags), (int) flags);
      this.BindInt("featureflags", (int) searchParams.FeatureFlags);
      this.BindInt("sortOrderType", searchParams.SortOrder);
      this.BindInt("metadataFlags", (int) searchParams.MetadataFlags);
      this.BindString("product", searchParams.Product, 20, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      ExtensionSearchResult extensionSearchResult = new ExtensionSearchResult();
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_SearchExtensions", this.RequestContext))
      {
        resultCollection.AddBinder<PublishedExtension>((ObjectBinder<PublishedExtension>) this.GetPublishedExtensionBinder());
        Dictionary<Guid, PublishedExtension> dictionary1 = this.ProcessExtensionResult(resultCollection, flags);
        Dictionary<int, List<ExtensionFilterResultMetadata>> dictionary2 = this.ProcessResultMetadata(resultCollection);
        extensionSearchResult.Results = dictionary1.Values.ToList<PublishedExtension>();
        if (dictionary2.ContainsKey(0))
          extensionSearchResult.ResultMetadata = dictionary2[0];
        return extensionSearchResult;
      }
    }

    public override ExtensionVersionValidationStep CreateValidationStep(
      IVssRequestContext requestContext,
      Guid stepId,
      Guid parentValidationId,
      int stepType,
      int stepStatus,
      DateTime startTime)
    {
      ArgumentUtility.CheckForEmptyGuid(stepId, nameof (stepId));
      ArgumentUtility.CheckForEmptyGuid(parentValidationId, nameof (parentValidationId));
      this.PrepareStoredProcedure("Gallery.prc_CreateValidationStep");
      this.BindGuid(nameof (stepId), stepId);
      this.BindGuid("parentId", parentValidationId);
      this.BindInt(nameof (stepType), stepType);
      this.BindInt(nameof (stepStatus), stepStatus);
      this.BindDateTime2(nameof (startTime), startTime);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_CreateValidationStep", requestContext))
      {
        resultCollection.AddBinder<ExtensionVersionValidationStep>((ObjectBinder<ExtensionVersionValidationStep>) new ExtensionVersionValidationStepBinder());
        List<ExtensionVersionValidationStep> items = resultCollection.GetCurrent<ExtensionVersionValidationStep>().Items;
        return items.Count == 0 ? (ExtensionVersionValidationStep) null : items.First<ExtensionVersionValidationStep>();
      }
    }

    public override List<ExtensionVersionValidationStep> GetAllValidationSteps(
      IVssRequestContext requestContext,
      Guid parentValidationId)
    {
      this.PrepareStoredProcedure("Gallery.prc_GetValidationSteps");
      if (parentValidationId.Equals(Guid.Empty))
        this.BindNullableGuid("parentId", Guid.Empty);
      else
        this.BindGuid("parentId", parentValidationId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_GetValidationSteps", requestContext))
      {
        resultCollection.AddBinder<ExtensionVersionValidationStep>((ObjectBinder<ExtensionVersionValidationStep>) new ExtensionVersionValidationStepBinder());
        return resultCollection.GetCurrent<ExtensionVersionValidationStep>().Items;
      }
    }

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
      this.BindString("validationContext", step.ValidationContext, 1024, BindStringBehavior.EmptyStringToNull, SqlDbType.VarChar);
      this.BindInt("resultFileId", step.ResultFileId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "Gallery.prc_UpdateValidationStep", requestContext))
      {
        resultCollection.AddBinder<ExtensionVersionValidationStep>((ObjectBinder<ExtensionVersionValidationStep>) new ExtensionVersionValidationStepBinder());
        List<ExtensionVersionValidationStep> items = resultCollection.GetCurrent<ExtensionVersionValidationStep>().Items;
        return items.Count == 0 ? (ExtensionVersionValidationStep) null : items.First<ExtensionVersionValidationStep>();
      }
    }

    public override void DeleteValidationStepsByParentId(
      IVssRequestContext requestContext,
      Guid parentValidationId)
    {
      this.PrepareStoredProcedure("Gallery.prc_DeleteValidationSteps");
      this.BindGuid("parentId", parentValidationId);
      this.ExecuteNonQuery();
    }

    private void ValidateStep(ExtensionVersionValidationStep step)
    {
      ArgumentUtility.CheckForNull<ExtensionVersionValidationStep>(step, nameof (step));
      ArgumentUtility.CheckForEmptyGuid(step.StepId, "StepId");
      ArgumentUtility.CheckForEmptyGuid(step.ParentId, "ParentId");
    }
  }
}
