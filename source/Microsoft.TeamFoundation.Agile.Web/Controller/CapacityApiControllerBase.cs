// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Controller.CapacityApiControllerBase
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

namespace Microsoft.TeamFoundation.Agile.Web.Controller
{
  public abstract class CapacityApiControllerBase : TeamSettingsApiControllerBase
  {
    protected Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamCapacity GetCapacitiesInternal(
      Guid iterationId)
    {
      ITeamSettings settingsInternal = this.GetTeamSettingsInternal();
      if (!settingsInternal.Iterations.Any<ITeamIteration>((Func<ITeamIteration, bool>) (i => i.IterationId == iterationId)))
        throw new IterationNotFoundException(iterationId.ToString());
      return this.TfsRequestContext.GetService<ITeamConfigurationService>().GetTeamIterationCapacity(this.TfsRequestContext, this.Team, settingsInternal, iterationId);
    }

    protected Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamMemberCapacity GetCapacityInternal(
      Guid iterationId,
      Guid teamMemberId)
    {
      ITeamSettings settingsInternal = this.GetTeamSettingsInternal();
      if (!settingsInternal.Iterations.Any<ITeamIteration>((Func<ITeamIteration, bool>) (i => i.IterationId == iterationId)))
        throw new IterationNotFoundException(iterationId.ToString());
      return this.TfsRequestContext.GetService<ITeamConfigurationService>().GetTeamIterationCapacity(this.TfsRequestContext, this.Team, settingsInternal, iterationId).TeamMemberCapacityCollection.FirstOrDefault<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamMemberCapacity>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamMemberCapacity, bool>) (x => x.TeamMemberId == teamMemberId)) ?? throw new CapacityNotSetException(iterationId, teamMemberId);
    }

    protected Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamMemberCapacity UpdateCapacityInternal(
      Guid iterationId,
      Guid teamMemberId,
      CapacityPatch patch)
    {
      if (patch.Activities == null && patch.DaysOff == null)
        throw new ArgumentException(nameof (patch));
      if (patch.Activities != null && patch.Activities.Count<Microsoft.TeamFoundation.Work.WebApi.Activity>() == 0)
        throw new ArgumentOutOfRangeException("patch.Activities");
      ITeamSettings settingsInternal = this.GetTeamSettingsInternal(true);
      ITeamConfigurationService service = this.TfsRequestContext.GetService<ITeamConfigurationService>();
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamCapacity iterationCapacity = service.GetTeamIterationCapacity(this.TfsRequestContext, this.Team, settingsInternal, iterationId);
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamMemberCapacity memberCapacities = new ConversionHelper().ConvertToServerTeamMemberCapacities(iterationCapacity, patch, teamMemberId);
      List<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamMemberCapacity> teamMemberCapacityList = new List<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamMemberCapacity>();
      teamMemberCapacityList.AddRange(iterationCapacity.TeamMemberCapacityCollection.Where<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamMemberCapacity>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamMemberCapacity, bool>) (tmc => tmc.TeamMemberId != teamMemberId)));
      teamMemberCapacityList.Add(memberCapacities);
      iterationCapacity.TeamMemberCapacityCollection = (IReadOnlyCollection<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamMemberCapacity>) teamMemberCapacityList;
      service.SaveTeamIterationCapacity(this.TfsRequestContext, this.Team, settingsInternal, iterationId, iterationCapacity);
      return memberCapacities;
    }

    protected Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamCapacity ReplaceCapacitiesInternal(
      Guid iterationId,
      IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamMemberCapacity> capacities)
    {
      ArgumentUtility.CheckForNull<IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamMemberCapacity>>(capacities, nameof (capacities));
      ITeamSettings settingsInternal = this.GetTeamSettingsInternal(true);
      ITeamConfigurationService service = this.TfsRequestContext.GetService<ITeamConfigurationService>();
      Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamCapacity iterationCapacity = service.GetTeamIterationCapacity(this.TfsRequestContext, this.Team, settingsInternal, iterationId);
      List<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamMemberCapacity> list = capacities.Select<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamMemberCapacity, Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamMemberCapacity>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamMemberCapacity, Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamMemberCapacity>) (patchedTeamMemberCapacity => new ConversionHelper().MergeServerTeamMemberCapacity(iterationCapacity.TeamMemberCapacityCollection.FirstOrDefault<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamMemberCapacity>((Func<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamMemberCapacity, bool>) (tmc => tmc.TeamMemberId == patchedTeamMemberCapacity.TeamMemberId)), patchedTeamMemberCapacity))).ToList<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamMemberCapacity>();
      iterationCapacity.TeamMemberCapacityCollection = (IReadOnlyCollection<Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamMemberCapacity>) list;
      service.SaveTeamIterationCapacity(this.TfsRequestContext, this.Team, settingsInternal, iterationId, iterationCapacity);
      return this.GetCapacitiesInternal(iterationId);
    }
  }
}
