// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Controller.IterationCapacityApiController
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.Azure.Devops.Teams.Service;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Agile.Web.Controller
{
  [VersionedApiControllerCustomName(Area = "work", ResourceName = "iterationcapacities", ResourceVersion = 1)]
  [ControllerApiVersion(6.1)]
  public class IterationCapacityApiController : TfsTeamApiController
  {
    [HttpGet]
    public IterationCapacity GetTotalIterationCapacities(Guid iterationId)
    {
      this.TfsRequestContext.TraceEnter(290930, "AgileService", "AgileService", nameof (GetTotalIterationCapacities));
      try
      {
        ITeamConfigurationService service = this.TfsRequestContext.GetService<ITeamConfigurationService>();
        IDictionary<WebApiTeam, Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamCapacity> capacityForIteration = service.GetTeamCapacityForIteration(this.TfsRequestContext, iterationId);
        List<TeamCapacityTotals> source = new List<TeamCapacityTotals>();
        foreach (KeyValuePair<WebApiTeam, Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamCapacity> keyValuePair in (IEnumerable<KeyValuePair<WebApiTeam, Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamCapacity>>) capacityForIteration)
        {
          double totalCapacity = 0.0;
          int num = 0;
          ITeamWeekends weekends = service.GetTeamSettings(this.TfsRequestContext, keyValuePair.Key, false, false, true).Weekends;
          foreach (Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamMemberCapacity teamMemberCapacity in (IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamMemberCapacity>) keyValuePair.Value.TeamMemberCapacityCollection)
          {
            teamMemberCapacity.Activities.ForEach<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Activity>((Action<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Activity>) (activity => totalCapacity += (double) activity.CapacityPerDay));
            if (teamMemberCapacity.DaysOffDates != null)
            {
              List<Microsoft.TeamFoundation.Work.WebApi.DateRange> daysOff = new List<Microsoft.TeamFoundation.Work.WebApi.DateRange>();
              foreach (Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DateRange daysOffDate in (IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.DateRange>) teamMemberCapacity.DaysOffDates)
                daysOff.Add(new Microsoft.TeamFoundation.Work.WebApi.DateRange()
                {
                  Start = daysOffDate.Start,
                  End = daysOffDate.End
                });
              num += TeamSettingUtils.GetWorkingDays((IEnumerable<Microsoft.TeamFoundation.Work.WebApi.DateRange>) daysOff, weekends);
            }
          }
          source.Add(new TeamCapacityTotals()
          {
            TeamId = keyValuePair.Key.Id,
            TeamCapacityPerDay = totalCapacity,
            TeamTotalDaysOff = num
          });
        }
        return new IterationCapacity()
        {
          Teams = (IList<TeamCapacityTotals>) source,
          TotalIterationCapacityPerDay = source.Select<TeamCapacityTotals, double>((Func<TeamCapacityTotals, double>) (x => x.TeamCapacityPerDay)).Sum(),
          TotalIterationDaysOff = source.Select<TeamCapacityTotals, int>((Func<TeamCapacityTotals, int>) (x => x.TeamTotalDaysOff)).Sum()
        };
      }
      finally
      {
        this.TfsRequestContext.TraceLeave(290931, "AgileService", "AgileService", nameof (GetTotalIterationCapacities));
      }
    }
  }
}
