// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.WorkItemTransitionsController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Common;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [ValidateModel]
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "workItemTransitions", ResourceVersion = 1)]
  public class WorkItemTransitionsController : WorkItemTrackingApiController
  {
    [HttpGet]
    [ClientExample("GET_wit_workItemTransitions.json", "Get next state on a work item", null, null)]
    public IEnumerable<WorkItemNextStateOnTransition> GetWorkItemNextStatesOnCheckinAction(
      [ClientParameterAsIEnumerable(typeof (int), ',')] string ids,
      string action = "checkin")
    {
      if (string.IsNullOrEmpty(ids))
        throw new VssPropertyValidationException(nameof (ids), ResourceStrings.NullOrEmptyParameter((object) nameof (ids)));
      if (!TFStringComparer.WorkItemActionName.Equals("checkin", action))
        throw new VssPropertyValidationException(nameof (action), ResourceStrings.WorkItemStateTransitionInvalidAction((object) action));
      ICollection<int> ids1 = ParsingHelper.ParseIds(ids);
      IEnumerable<WorkItemStateOnTransition> stateOnTransitions = this.WorkItemService.GetNextStateOnCheckinWithExceptions(this.TfsRequestContext, (IEnumerable<int>) ids1);
      if (stateOnTransitions.Count<WorkItemStateOnTransition>() < ids1.Count)
      {
        IList<int> returnedWorkItemIds = (IList<int>) new List<int>();
        stateOnTransitions.ForEach<WorkItemStateOnTransition>((Action<WorkItemStateOnTransition>) (x => returnedWorkItemIds.Add(x.WorkItemId)));
        IEnumerable<int> ints = ids1.Except<int>((IEnumerable<int>) returnedWorkItemIds);
        IList<WorkItemStateOnTransition> list = (IList<WorkItemStateOnTransition>) stateOnTransitions.ToList<WorkItemStateOnTransition>();
        foreach (int num in ints)
          list.Add(new WorkItemStateOnTransition()
          {
            WorkItemId = num,
            ErrorMessage = ResourceStrings.WorkItemNotFound((object) num.ToString())
          });
        stateOnTransitions = (IEnumerable<WorkItemStateOnTransition>) list;
      }
      IEnumerable<WorkItemNextStateOnTransition> source = stateOnTransitions.Select<WorkItemStateOnTransition, WorkItemNextStateOnTransition>((Func<WorkItemStateOnTransition, WorkItemNextStateOnTransition>) (wiNextState => WorkItemStateTransitionFactory.Create(wiNextState)));
      return source == null ? (IEnumerable<WorkItemNextStateOnTransition>) null : (IEnumerable<WorkItemNextStateOnTransition>) source.ToList<WorkItemNextStateOnTransition>();
    }
  }
}
