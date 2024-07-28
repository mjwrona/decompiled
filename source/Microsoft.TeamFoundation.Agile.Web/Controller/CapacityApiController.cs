// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Controller.CapacityApiController
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
  [VersionedApiControllerCustomName(Area = "work", ResourceName = "capacities", ResourceVersion = 1)]
  public class CapacityApiController : CapacityApiControllerBase
  {
    [HttpGet]
    public IEnumerable<Microsoft.TeamFoundation.Work.WebApi.TeamMemberCapacity> GetCapacities(
      Guid iterationId)
    {
      this.TfsRequestContext.TraceEnter(290141, "AgileService", "AgileService", nameof (GetCapacities));
      try
      {
        Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamCapacity capacitiesInternal = this.GetCapacitiesInternal(iterationId);
        return new ConversionHelper().ConvertToTeamMemberCapacityReferences(this.TfsRequestContext, iterationId, (IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamMemberCapacity>) capacitiesInternal.TeamMemberCapacityCollection, true, new Guid?(this.ProjectId), new Guid?(this.TeamId)).Where<Microsoft.TeamFoundation.Work.WebApi.TeamMemberCapacity>((Func<Microsoft.TeamFoundation.Work.WebApi.TeamMemberCapacity, bool>) (c => !string.IsNullOrEmpty(c.TeamMember.DisplayName)));
      }
      finally
      {
        this.TfsRequestContext.TraceLeave(290142, "AgileService", "AgileService", nameof (GetCapacities));
      }
    }

    [HttpGet]
    public Microsoft.TeamFoundation.Work.WebApi.TeamMemberCapacity GetCapacity(
      Guid iterationId,
      Guid teamMemberId)
    {
      this.TfsRequestContext.TraceEnter(290143, "AgileService", "AgileService", nameof (GetCapacity));
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
        Microsoft.TeamFoundation.Work.WebApi.TeamMemberCapacity capacity = conversionHelper.ConvertToTeamMemberCapacityReferences(tfsRequestContext, iterationId1, (IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamMemberCapacity>) teamMemberCapacityList, true, projectId, teamId).First<Microsoft.TeamFoundation.Work.WebApi.TeamMemberCapacity>();
        capacity.Links = this.GetReferenceLinks(capacity.Url, TeamSettingsApiControllerBase.CommonUrlLink.TeamSettings | TeamSettingsApiControllerBase.CommonUrlLink.Iterations | TeamSettingsApiControllerBase.CommonUrlLink.Iteration | TeamSettingsApiControllerBase.CommonUrlLink.Capacities, iterationId);
        this.AddClassificationNodeLink(capacity.Links, iterationId);
        return capacity;
      }
      finally
      {
        this.TfsRequestContext.TraceLeave(290144, "AgileService", "AgileService", nameof (GetCapacity));
      }
    }

    [HttpPatch]
    public Microsoft.TeamFoundation.Work.WebApi.TeamMemberCapacity UpdateCapacity(
      Guid iterationId,
      Guid teamMemberId,
      [FromBody] CapacityPatch patch)
    {
      this.TfsRequestContext.TraceEnter(290145, "AgileService", "AgileService", nameof (UpdateCapacity));
      try
      {
        ArgumentUtility.CheckForNull<CapacityPatch>(patch, nameof (patch));
        this.UpdateCapacityInternal(iterationId, teamMemberId, patch);
        return this.GetCapacity(iterationId, teamMemberId);
      }
      finally
      {
        this.TfsRequestContext.TraceLeave(290146, "AgileService", "AgileService", nameof (UpdateCapacity));
      }
    }

    [HttpPut]
    public IEnumerable<Microsoft.TeamFoundation.Work.WebApi.TeamMemberCapacity> ReplaceCapacities(
      Guid iterationId,
      [FromBody] IEnumerable<Microsoft.TeamFoundation.Work.WebApi.TeamMemberCapacity> capacities)
    {
      this.TfsRequestContext.TraceEnter(290147, "AgileService", "AgileService", nameof (ReplaceCapacities));
      try
      {
        ArgumentUtility.CheckForNull<IEnumerable<Microsoft.TeamFoundation.Work.WebApi.TeamMemberCapacity>>(capacities, nameof (capacities));
        IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamMemberCapacity> capacities1 = capacities.Select<Microsoft.TeamFoundation.Work.WebApi.TeamMemberCapacity, Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamMemberCapacity>((Func<Microsoft.TeamFoundation.Work.WebApi.TeamMemberCapacity, Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamMemberCapacity>) (updatedCapacity => new ConversionHelper().ConvertToServerTeamMemberCapacity((Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamMemberCapacity) null, updatedCapacity)));
        Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamCapacity teamCapacity = this.ReplaceCapacitiesInternal(iterationId, capacities1);
        return new ConversionHelper().ConvertToTeamMemberCapacityReferences(this.TfsRequestContext, iterationId, (IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamMemberCapacity>) teamCapacity.TeamMemberCapacityCollection, true, new Guid?(this.ProjectId), new Guid?(this.TeamId)).Where<Microsoft.TeamFoundation.Work.WebApi.TeamMemberCapacity>((Func<Microsoft.TeamFoundation.Work.WebApi.TeamMemberCapacity, bool>) (c => !string.IsNullOrEmpty(c.TeamMember.DisplayName)));
      }
      finally
      {
        this.TfsRequestContext.TraceEnter(290148, "AgileService", "AgileService", nameof (ReplaceCapacities));
      }
    }
  }
}
