// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.ProcessFieldController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Process.Factories;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [FeatureEnabled("WebAccess.Process.Hierarchy")]
  [VersionedApiControllerCustomName(Area = "processes", ResourceName = "fields", ResourceVersion = 1)]
  public class ProcessFieldController : WorkItemTrackingApiController
  {
    private const int TraceRange = 5917000;

    [HttpGet]
    [TraceFilter(5917000, 5917010)]
    [ClientLocationId("BC0AD8DC-E3F3-46B0-B06C-5BF861793196")]
    [ClientExample("GET__wit_fields.json", "Get the list of work item type fields", null, null)]
    public IReadOnlyCollection<FieldModel> GetWorkItemTypeFields(Guid processId, string witRefName)
    {
      if (processId == Guid.Empty)
        throw new VssPropertyValidationException(nameof (processId), ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      if (string.IsNullOrEmpty(witRefName))
        throw new VssPropertyValidationException("witName", ResourceStrings.NullOrEmptyParameter((object) "witName"));
      IReadOnlyCollection<ProcessFieldResult> legacyFields = this.TfsRequestContext.GetService<IProcessWorkItemTypeService>().GetWorkItemType(this.TfsRequestContext, processId, witRefName, true).GetLegacyFields(this.TfsRequestContext);
      return legacyFields != null ? (IReadOnlyCollection<FieldModel>) legacyFields.Select<ProcessFieldResult, FieldModel>((Func<ProcessFieldResult, FieldModel>) (f => FieldModelFactory.Create(this.TfsRequestContext, processId, witRefName, f))).ToList<FieldModel>() : (IReadOnlyCollection<FieldModel>) null;
    }

    [HttpGet]
    [TraceFilter(5917010, 5917020)]
    [ClientLocationId("7A0E7A1A-0B34-4AE0-9744-0AAFFB7D0ED1")]
    [ClientExample("GET__process_fields.json", "Get the list of process fields", null, null)]
    public IReadOnlyCollection<FieldModel> GetFields(Guid processId)
    {
      IReadOnlyCollection<ProcessFieldResult> source = !(processId == Guid.Empty) ? this.TfsRequestContext.GetService<IProcessFieldService>().GetFields(this.TfsRequestContext, processId) : throw new VssPropertyValidationException(nameof (processId), ResourceStrings.NullOrEmptyParameter((object) nameof (processId)));
      return source != null ? (IReadOnlyCollection<FieldModel>) source.Select<ProcessFieldResult, FieldModel>((Func<ProcessFieldResult, FieldModel>) (f => FieldModelFactory.Create(this.TfsRequestContext, processId, (string) null, f))).ToList<FieldModel>() : (IReadOnlyCollection<FieldModel>) null;
    }
  }
}
