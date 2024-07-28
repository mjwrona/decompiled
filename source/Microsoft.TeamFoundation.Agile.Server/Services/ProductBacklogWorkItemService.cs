// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Server.Services.ProductBacklogWorkItemService
// Assembly: Microsoft.TeamFoundation.Agile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4912F51-3FCA-4D2B-A7B5-CF15E2F3B46B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Agile.Server.Services
{
  public class ProductBacklogWorkItemService : IProductBacklogWorkItemService, IVssFrameworkService
  {
    public IEnumerable<int> GetBacklogLevelWorkItems(
      IVssRequestContext requestContext,
      IAgileSettings agileSettings,
      string backlogId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IAgileSettings>(agileSettings, nameof (agileSettings));
      ArgumentUtility.CheckStringForNullOrEmpty(backlogId, nameof (backlogId));
      BacklogLevelConfiguration levelConfiguration = agileSettings.BacklogConfiguration.GetBacklogLevelConfiguration(backlogId);
      BacklogContext backlogContext = new BacklogContext(agileSettings.Team, levelConfiguration);
      string flatBacklogQuery = new ProductBacklogQueryBuilder(requestContext, agileSettings, backlogContext, agileSettings.TeamSettings.GetBacklogIterationNode(requestContext).GetPath(requestContext)).GetFlatBacklogQuery(0);
      IWorkItemQueryService service = requestContext.GetService<IWorkItemQueryService>();
      Microsoft.TeamFoundation.WorkItemTracking.Server.QueryExpression queryExpression = service.ConvertToQueryExpression(requestContext, flatBacklogQuery, skipWiqlTextLimitValidation: true);
      return service.ExecuteQuery(requestContext, queryExpression).WorkItemIds;
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }
  }
}
