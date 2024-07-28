// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Controller.BoardParentApiController
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Agile.Server.Exceptions;
using Microsoft.TeamFoundation.Agile.Web.Utilities;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Agile.Web.Controller
{
  [VersionedApiControllerCustomName(Area = "work", ResourceName = "boardparents")]
  public class BoardParentApiController : BoardsApiControllerBase
  {
    private static readonly string TraceLayer = nameof (BoardParentApiController);

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<BoardParentChildWIMapReadFailureException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<BoardParentChildWIMapReadFailureException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<BoardParentChildWIMapLimitExceededException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<BoardParentChildWIMapLimitExceededException>(HttpStatusCode.BadRequest);
      exceptionMap.AddTranslation(typeof (BoardParentChildWIMapReadFailureException), typeof (BoardParentChildWIMapReadFailureException));
      exceptionMap.AddTranslation(typeof (BoardParentChildWIMapLimitExceededException), typeof (BoardParentChildWIMapLimitExceededException));
    }

    [HttpGet]
    [TraceFilter(290712, 290720)]
    public IEnumerable<Microsoft.TeamFoundation.Work.WebApi.ParentChildWIMap> GetBoardMappingParentItems(
      string childBacklogContextCategoryRefName,
      [ClientParameterAsIEnumerable(typeof (int), ',')] string workitemIds)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(childBacklogContextCategoryRefName, nameof (childBacklogContextCategoryRefName));
      ArgumentUtility.CheckStringForNullOrEmpty(workitemIds, nameof (workitemIds));
      try
      {
        IEnumerable<int> ids = (IEnumerable<int>) ParsingHelper.ParseIds(workitemIds);
        if (ids.Count<int>() <= 0)
          return (IEnumerable<Microsoft.TeamFoundation.Work.WebApi.ParentChildWIMap>) new List<Microsoft.TeamFoundation.Work.WebApi.ParentChildWIMap>();
        Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration backlogLevel;
        if (!this.AgileSettings.BacklogConfiguration.TryGetBacklogLevelConfiguration(childBacklogContextCategoryRefName, out backlogLevel))
          return (IEnumerable<Microsoft.TeamFoundation.Work.WebApi.ParentChildWIMap>) new List<Microsoft.TeamFoundation.Work.WebApi.ParentChildWIMap>();
        if (TFStringComparer.BacklogLevelId.Equals(backlogLevel.Id, this.AgileSettings.BacklogConfiguration.TaskBacklog.Id))
          throw new InvalidOperationException();
        Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogLevelConfiguration backlogParent = this.AgileSettings.BacklogConfiguration.GetBacklogParent(backlogLevel.Id);
        return (IEnumerable<Microsoft.TeamFoundation.Work.WebApi.ParentChildWIMap>) this.GetBoardParentFieldFilterHelper().GetParentChildWIMap(this.TfsRequestContext, this.AgileSettings, backlogParent, ids.ToArray<int>()).Select<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.ParentChildWIMap, Microsoft.TeamFoundation.Work.WebApi.ParentChildWIMap>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.ParentChildWIMap, Microsoft.TeamFoundation.Work.WebApi.ParentChildWIMap>) (value => new Microsoft.TeamFoundation.Work.WebApi.ParentChildWIMap()
        {
          Id = value.Id,
          Title = value.Title,
          ChildWorkItemIds = value.ChildWorkItemIds,
          WorkItemTypeName = value.WorkItemTypeName
        })).ToList<Microsoft.TeamFoundation.Work.WebApi.ParentChildWIMap>();
      }
      catch (BoardParentChildWIMapLimitExceededException ex)
      {
        this.TfsRequestContext.TraceException(290718, TraceLevel.Error, "Agile", BoardParentApiController.TraceLayer, (Exception) ex);
        throw ex;
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(290719, TraceLevel.Error, "Agile", BoardParentApiController.TraceLayer, ex);
        throw new BoardParentChildWIMapReadFailureException(ex);
      }
    }

    [NonAction]
    public virtual IBoardParentWIFilterHelper GetBoardParentFieldFilterHelper() => (IBoardParentWIFilterHelper) new BoardParentWIFilterHelper();
  }
}
