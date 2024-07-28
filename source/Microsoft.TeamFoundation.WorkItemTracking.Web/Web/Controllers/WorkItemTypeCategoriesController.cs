// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.WorkItemTypeCategoriesController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "workItemTypeCategories", ResourceVersion = 2)]
  public class WorkItemTypeCategoriesController : WorkItemTrackingApiController
  {
    private const int TraceRange = 5912000;

    [TraceFilter(5912000, 5912010)]
    [HttpGet]
    [ClientExample("GET__wit_workItemTypeCategories.json", null, null, null)]
    [PublicProjectRequestRestrictions("5.0")]
    public IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeCategory> GetWorkItemTypeCategories() => (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeCategory>) this.TfsRequestContext.GetService<IWorkItemTypeCategoryService>().GetWorkItemTypeCategories(this.TfsRequestContext, this.ProjectInfo.Id).Select<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTypeCategory, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeCategory>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTypeCategory, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeCategory>) (wit => WorkItemTypeCategoryFactory.Create(this.TfsRequestContext, wit))).ToList<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeCategory>();

    [TraceFilter(5912000, 5912010)]
    [HttpGet]
    [ClientExample("GET__wit_workItemTypeCategories__categoryName_.json", "Get category by name", null, null)]
    [PublicProjectRequestRestrictions("5.0")]
    public Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeCategory GetWorkItemTypeCategory(
      string category)
    {
      return WorkItemTypeCategoryFactory.Create(this.TfsRequestContext, this.TfsRequestContext.GetService<IWorkItemTypeCategoryService>().GetWorkItemTypeCategory(this.TfsRequestContext, this.ProjectInfo.Id, category));
    }

    public override string TraceArea => "workItemTypes";
  }
}
