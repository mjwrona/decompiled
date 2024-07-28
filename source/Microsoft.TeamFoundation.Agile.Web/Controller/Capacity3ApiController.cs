// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Controller.Capacity3ApiController
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

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
  [VersionedApiControllerCustomName(Area = "work", ResourceName = "capacities", ResourceVersion = 3)]
  [ControllerApiVersion(6.1)]
  public class Capacity3ApiController : CapacityApiControllerBase
  {
    [HttpGet]
    public Microsoft.TeamFoundation.Work.WebApi.TeamCapacity GetCapacitiesWithIdentityRefAndTotals(
      Guid iterationId)
    {
      this.TfsRequestContext.TraceEnter(290930, "AgileService", "AgileService", nameof (GetCapacitiesWithIdentityRefAndTotals));
      try
      {
        Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamCapacity capacitiesInternal = this.GetCapacitiesInternal(iterationId);
        IEnumerable<TeamMemberCapacityIdentityRef> capacityIdentityRefs = new ConversionHelper().ConvertToTeamMemberCapacityIdentityReferences(this.TfsRequestContext, iterationId, (IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamMemberCapacity>) capacitiesInternal.TeamMemberCapacityCollection, true, new Guid?(this.ProjectId), new Guid?(this.TeamId)).Where<TeamMemberCapacityIdentityRef>((Func<TeamMemberCapacityIdentityRef, bool>) (c => !string.IsNullOrEmpty(c.TeamMember?.DisplayName)));
        double totalCapacity = 0.0;
        int num = 0;
        ITeamWeekends weekends = this.GetTeamSettingsInternal().Weekends;
        foreach (TeamMemberCapacityIdentityRef capacityIdentityRef in capacityIdentityRefs)
        {
          capacityIdentityRef.Activities.ForEach<Microsoft.TeamFoundation.Work.WebApi.Activity>((Action<Microsoft.TeamFoundation.Work.WebApi.Activity>) (activity => totalCapacity += (double) activity.CapacityPerDay));
          if (capacityIdentityRef.DaysOff != null)
            num += TeamSettingUtils.GetWorkingDays(capacityIdentityRef.DaysOff, weekends);
        }
        return new Microsoft.TeamFoundation.Work.WebApi.TeamCapacity()
        {
          TeamMembers = capacityIdentityRefs,
          TotalCapacityPerDay = totalCapacity,
          TotalDaysOff = num
        };
      }
      finally
      {
        this.TfsRequestContext.TraceLeave(290931, "AgileService", "AgileService", nameof (GetCapacitiesWithIdentityRefAndTotals));
      }
    }

    [HttpGet]
    [ClientExample("GET__work_teamsettings_iterations__iterationId__capacities__teammemberId_.json", "Get a team member's capacity", null, null)]
    public TeamMemberCapacityIdentityRef GetCapacityWithIdentityRef(
      Guid iterationId,
      Guid teamMemberId)
    {
      this.TfsRequestContext.TraceEnter(290932, "AgileService", "AgileService", nameof (GetCapacityWithIdentityRef));
      try
      {
        Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamMemberCapacity capacityInternal = this.GetCapacityInternal(iterationId, teamMemberId);
        ConversionHelper conversionHelper = new ConversionHelper();
        IVssRequestContext tfsRequestContext = this.TfsRequestContext;
        Guid iterationId1 = iterationId;
        List<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamMemberCapacity> teamMemberCapacityList = new List<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamMemberCapacity>();
        teamMemberCapacityList.Add(capacityInternal);
        Guid? projectId = new Guid?(this.ProjectId);
        Guid? teamId = new Guid?(this.TeamId);
        TeamMemberCapacityIdentityRef capacityWithIdentityRef = conversionHelper.ConvertToTeamMemberCapacityIdentityReferences(tfsRequestContext, iterationId1, (IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamMemberCapacity>) teamMemberCapacityList, true, projectId, teamId).First<TeamMemberCapacityIdentityRef>();
        capacityWithIdentityRef.Links = this.GetReferenceLinks(capacityWithIdentityRef.Url, TeamSettingsApiControllerBase.CommonUrlLink.TeamSettings | TeamSettingsApiControllerBase.CommonUrlLink.Iterations | TeamSettingsApiControllerBase.CommonUrlLink.Iteration | TeamSettingsApiControllerBase.CommonUrlLink.Capacities, iterationId);
        this.AddClassificationNodeLink(capacityWithIdentityRef.Links, iterationId);
        return capacityWithIdentityRef;
      }
      finally
      {
        this.TfsRequestContext.TraceLeave(290933, "AgileService", "AgileService", nameof (GetCapacityWithIdentityRef));
      }
    }

    [HttpPatch]
    [ClientExample("PATCH__work_teamsettings_iterations__iterationId__capacities__teammemberId_.json", "Update a team member's capacity", null, null)]
    public TeamMemberCapacityIdentityRef UpdateCapacityWithIdentityRef(
      Guid iterationId,
      Guid teamMemberId,
      [FromBody] CapacityPatch patch)
    {
      this.TfsRequestContext.TraceEnter(290934, "AgileService", "AgileService", nameof (UpdateCapacityWithIdentityRef));
      try
      {
        ArgumentUtility.CheckForNull<CapacityPatch>(patch, nameof (patch));
        this.UpdateCapacityInternal(iterationId, teamMemberId, patch);
        return this.GetCapacityWithIdentityRef(iterationId, teamMemberId);
      }
      finally
      {
        this.TfsRequestContext.TraceLeave(290935, "AgileService", "AgileService", nameof (UpdateCapacityWithIdentityRef));
      }
    }

    [HttpPut]
    [ClientExample("PUT__work_teamsettings_iterations_iterationId_capacities.json", "Replace a team's capacity", null, null)]
    public IEnumerable<TeamMemberCapacityIdentityRef> ReplaceCapacitiesWithIdentityRef(
      Guid iterationId,
      [FromBody] IEnumerable<TeamMemberCapacityIdentityRef> capacities)
    {
      this.TfsRequestContext.TraceEnter(290936, "AgileService", "AgileService", nameof (ReplaceCapacitiesWithIdentityRef));
      try
      {
        ArgumentUtility.CheckForNull<IEnumerable<TeamMemberCapacityIdentityRef>>(capacities, nameof (capacities));
        IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamMemberCapacity> capacities1 = capacities.Select<TeamMemberCapacityIdentityRef, Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamMemberCapacity>((Func<TeamMemberCapacityIdentityRef, Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamMemberCapacity>) (updatedCapacity => new ConversionHelper().ConvertToServerTeamMemberCapacityIdentityReferences((Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamMemberCapacity) null, updatedCapacity)));
        Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamCapacity teamCapacity = this.ReplaceCapacitiesInternal(iterationId, capacities1);
        return new ConversionHelper().ConvertToTeamMemberCapacityIdentityReferences(this.TfsRequestContext, iterationId, (IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamMemberCapacity>) teamCapacity.TeamMemberCapacityCollection, true, new Guid?(this.ProjectId), new Guid?(this.TeamId)).Where<TeamMemberCapacityIdentityRef>((Func<TeamMemberCapacityIdentityRef, bool>) (c => !string.IsNullOrEmpty(c.TeamMember?.DisplayName)));
      }
      finally
      {
        this.TfsRequestContext.TraceEnter(290937, "AgileService", "AgileService", nameof (ReplaceCapacitiesWithIdentityRef));
      }
    }
  }
}
