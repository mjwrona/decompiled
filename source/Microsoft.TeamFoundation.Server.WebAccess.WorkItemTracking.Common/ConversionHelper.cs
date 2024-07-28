// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.ConversionHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Identity;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public class ConversionHelper
  {
    private AgileTracingUtils m_tracingUtils;

    public ConversionHelper(AgileTracingUtils tracingUtils = null) => this.m_tracingUtils = tracingUtils == null ? new AgileTracingUtils() : tracingUtils;

    public IEnumerable<Microsoft.TeamFoundation.Work.WebApi.DateRange> ConvertToWebApiDateRange(
      IEnumerable<DateRange> dateRange)
    {
      if (dateRange == null || !dateRange.Any<DateRange>())
        return Enumerable.Empty<Microsoft.TeamFoundation.Work.WebApi.DateRange>();
      List<Microsoft.TeamFoundation.Work.WebApi.DateRange> webApiDateRange = new List<Microsoft.TeamFoundation.Work.WebApi.DateRange>();
      foreach (DateRange dateRange1 in dateRange)
        webApiDateRange.Add(new Microsoft.TeamFoundation.Work.WebApi.DateRange()
        {
          Start = dateRange1.Start,
          End = dateRange1.End
        });
      return (IEnumerable<Microsoft.TeamFoundation.Work.WebApi.DateRange>) webApiDateRange;
    }

    public IEnumerable<DateRange> ConvertToServerDateRange(IEnumerable<Microsoft.TeamFoundation.Work.WebApi.DateRange> dateRange)
    {
      if (dateRange == null || !dateRange.Any<Microsoft.TeamFoundation.Work.WebApi.DateRange>())
        return Enumerable.Empty<DateRange>();
      List<DateRange> serverDateRange = new List<DateRange>();
      foreach (Microsoft.TeamFoundation.Work.WebApi.DateRange dateRange1 in dateRange)
        serverDateRange.Add(new DateRange()
        {
          Start = dateRange1.Start,
          End = dateRange1.End
        });
      return (IEnumerable<DateRange>) serverDateRange;
    }

    public TeamMemberCapacity ConvertToServerTeamMemberCapacities(
      TeamCapacity capacity,
      CapacityPatch patch,
      Guid teamMemberId)
    {
      TeamMemberCapacity existingTeamMemberCapacity = capacity.TeamMemberCapacityCollection.FirstOrDefault<TeamMemberCapacity>((Func<TeamMemberCapacity, bool>) (x => x.TeamMemberId == teamMemberId));
      Microsoft.TeamFoundation.Work.WebApi.TeamMemberCapacity teamMemberCapacity = new Microsoft.TeamFoundation.Work.WebApi.TeamMemberCapacity();
      teamMemberCapacity.Activities = patch.Activities;
      teamMemberCapacity.DaysOff = patch.DaysOff;
      teamMemberCapacity.TeamMember = new Member()
      {
        Id = teamMemberId
      };
      return this.ConvertToServerTeamMemberCapacity(existingTeamMemberCapacity, teamMemberCapacity);
    }

    public TeamMemberCapacity ConvertToServerTeamMemberCapacity(
      TeamMemberCapacity existingTeamMemberCapacity,
      Microsoft.TeamFoundation.Work.WebApi.TeamMemberCapacity teamMemberCapacity)
    {
      if (existingTeamMemberCapacity == null)
        existingTeamMemberCapacity = new TeamMemberCapacity();
      return new TeamMemberCapacity()
      {
        Activities = teamMemberCapacity.Activities != null ? (IList<Activity>) teamMemberCapacity.Activities.Where<Microsoft.TeamFoundation.Work.WebApi.Activity>((Func<Microsoft.TeamFoundation.Work.WebApi.Activity, bool>) (y => y != null)).Select<Microsoft.TeamFoundation.Work.WebApi.Activity, Activity>((Func<Microsoft.TeamFoundation.Work.WebApi.Activity, Activity>) (y => new Activity()
        {
          CapacityPerDay = y.CapacityPerDay,
          Name = y.Name
        })).ToList<Activity>() : existingTeamMemberCapacity.Activities,
        DaysOffDates = teamMemberCapacity.DaysOff != null ? (IList<DateRange>) teamMemberCapacity.DaysOff.Where<Microsoft.TeamFoundation.Work.WebApi.DateRange>((Func<Microsoft.TeamFoundation.Work.WebApi.DateRange, bool>) (y => y != null)).Select<Microsoft.TeamFoundation.Work.WebApi.DateRange, DateRange>((Func<Microsoft.TeamFoundation.Work.WebApi.DateRange, DateRange>) (y => new DateRange()
        {
          End = y.End,
          Start = y.Start
        })).ToList<DateRange>() : existingTeamMemberCapacity.DaysOffDates,
        TeamMemberId = teamMemberCapacity.TeamMember.Id
      };
    }

    public TeamMemberCapacity MergeServerTeamMemberCapacity(
      TeamMemberCapacity existingTeamMemberCapacity,
      TeamMemberCapacity teamMemberCapacity)
    {
      if (existingTeamMemberCapacity == null)
        existingTeamMemberCapacity = new TeamMemberCapacity();
      return new TeamMemberCapacity()
      {
        Activities = teamMemberCapacity.Activities != null ? teamMemberCapacity.Activities : existingTeamMemberCapacity.Activities,
        DaysOffDates = teamMemberCapacity.DaysOffDates != null ? teamMemberCapacity.DaysOffDates : existingTeamMemberCapacity.DaysOffDates,
        TeamMemberId = teamMemberCapacity.TeamMemberId
      };
    }

    public TeamMemberCapacity ConvertToServerTeamMemberCapacityIdentityReferences(
      TeamMemberCapacity existingTeamMemberCapacity,
      TeamMemberCapacityIdentityRef teamMemberCapacity)
    {
      if (existingTeamMemberCapacity == null)
        existingTeamMemberCapacity = new TeamMemberCapacity();
      if (teamMemberCapacity.TeamMember.Id == null)
        throw new ArgumentException(Resources.Validation_CapacityMissingUserId);
      return new TeamMemberCapacity()
      {
        Activities = teamMemberCapacity.Activities != null ? (IList<Activity>) teamMemberCapacity.Activities.Where<Microsoft.TeamFoundation.Work.WebApi.Activity>((Func<Microsoft.TeamFoundation.Work.WebApi.Activity, bool>) (y => y != null)).Select<Microsoft.TeamFoundation.Work.WebApi.Activity, Activity>((Func<Microsoft.TeamFoundation.Work.WebApi.Activity, Activity>) (y => new Activity()
        {
          CapacityPerDay = y.CapacityPerDay,
          Name = y.Name
        })).ToList<Activity>() : existingTeamMemberCapacity.Activities,
        DaysOffDates = teamMemberCapacity.DaysOff != null ? (IList<DateRange>) teamMemberCapacity.DaysOff.Where<Microsoft.TeamFoundation.Work.WebApi.DateRange>((Func<Microsoft.TeamFoundation.Work.WebApi.DateRange, bool>) (y => y != null)).Select<Microsoft.TeamFoundation.Work.WebApi.DateRange, DateRange>((Func<Microsoft.TeamFoundation.Work.WebApi.DateRange, DateRange>) (y => new DateRange()
        {
          End = y.End,
          Start = y.Start
        })).ToList<DateRange>() : existingTeamMemberCapacity.DaysOffDates,
        TeamMemberId = new Guid(teamMemberCapacity.TeamMember.Id)
      };
    }

    public IReadOnlyCollection<Microsoft.TeamFoundation.Work.WebApi.TeamMemberCapacity> ConvertToTeamMemberCapacityReferences(
      IVssRequestContext requestContext,
      Guid iterationId,
      IEnumerable<TeamMemberCapacity> teamMemberCapacities,
      bool addUrls = false,
      Guid? projectId = null,
      Guid? teamId = null)
    {
      ArgumentUtility.CheckForNull<IEnumerable<TeamMemberCapacity>>(teamMemberCapacities, nameof (teamMemberCapacities));
      if (!teamMemberCapacities.Any<TeamMemberCapacity>())
        return (IReadOnlyCollection<Microsoft.TeamFoundation.Work.WebApi.TeamMemberCapacity>) new List<Microsoft.TeamFoundation.Work.WebApi.TeamMemberCapacity>();
      using (this.m_tracingUtils.TraceBlock(requestContext, 910703, 910704, memberName: nameof (ConvertToTeamMemberCapacityReferences)))
      {
        IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> identities = this.FetchIdentities(requestContext, teamMemberCapacities.Select<TeamMemberCapacity, Guid>((Func<TeamMemberCapacity, Guid>) (_ => _.TeamMemberId)));
        return (IReadOnlyCollection<Microsoft.TeamFoundation.Work.WebApi.TeamMemberCapacity>) teamMemberCapacities.Select<TeamMemberCapacity, Microsoft.TeamFoundation.Work.WebApi.TeamMemberCapacity>((Func<TeamMemberCapacity, Microsoft.TeamFoundation.Work.WebApi.TeamMemberCapacity>) (tmc => this.ConvertToWebApiTeamMemberCapacityReference(requestContext, iterationId, identities, tmc, addUrls, projectId, teamId))).ToList<Microsoft.TeamFoundation.Work.WebApi.TeamMemberCapacity>();
      }
    }

    public IReadOnlyCollection<TeamMemberCapacityIdentityRef> ConvertToTeamMemberCapacityIdentityReferences(
      IVssRequestContext requestContext,
      Guid iterationId,
      IEnumerable<TeamMemberCapacity> teamMemberCapacities,
      bool addUrls = false,
      Guid? projectId = null,
      Guid? teamId = null)
    {
      ArgumentUtility.CheckForNull<IEnumerable<TeamMemberCapacity>>(teamMemberCapacities, nameof (teamMemberCapacities));
      if (!teamMemberCapacities.Any<TeamMemberCapacity>())
        return (IReadOnlyCollection<TeamMemberCapacityIdentityRef>) new List<TeamMemberCapacityIdentityRef>();
      using (this.m_tracingUtils.TraceBlock(requestContext, 910703, 910704, memberName: nameof (ConvertToTeamMemberCapacityIdentityReferences)))
      {
        IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> identities = this.FetchIdentities(requestContext, teamMemberCapacities.Select<TeamMemberCapacity, Guid>((Func<TeamMemberCapacity, Guid>) (_ => _.TeamMemberId)));
        return (IReadOnlyCollection<TeamMemberCapacityIdentityRef>) teamMemberCapacities.Select<TeamMemberCapacity, TeamMemberCapacityIdentityRef>((Func<TeamMemberCapacity, TeamMemberCapacityIdentityRef>) (tmc => this.ConvertToWebApiTeamMemberCapacityIdentityReference(requestContext, iterationId, identities, tmc, addUrls, projectId, teamId))).ToList<TeamMemberCapacityIdentityRef>();
      }
    }

    internal IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> FetchIdentities(
      IVssRequestContext requestContext,
      IEnumerable<Guid> identities)
    {
      using (this.m_tracingUtils.TraceBlock(requestContext, 910705, 910706, memberName: nameof (FetchIdentities)))
      {
        IdentityService service = requestContext.GetService<IdentityService>();
        List<Guid> list = identities.Distinct<Guid>().ToList<Guid>();
        IVssRequestContext requestContext1 = requestContext;
        List<Guid> identityIds = list;
        IList<Microsoft.VisualStudio.Services.Identity.Identity> source = service.ReadIdentities(requestContext1, (IList<Guid>) identityIds, QueryMembership.None, (IEnumerable<string>) null);
        Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> distinctIdentites = new Dictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>();
        foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in source.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (_ => _ != null && _.IsActive)))
        {
          if (!distinctIdentites.ContainsKey(identity.Id))
            distinctIdentites.Add(identity.Id, identity);
          else
            requestContext.TraceAlways(403453, TraceLevel.Error, "Agile", TfsTraceLayers.BusinessLogic, "Duplicate identities with Id: {0}, masterId: {1}, allInputs: {2}", (object) identity.Id, (object) identity.MasterId, (object) string.Join<Guid>(",", (IEnumerable<Guid>) list));
        }
        IEnumerable<Guid> guids = list.Where<Guid>((Func<Guid, bool>) (id => !distinctIdentites.ContainsKey(id)));
        if (guids.Count<Guid>() > 0)
          requestContext.Trace(403454, TraceLevel.Info, "Agile", TfsTraceLayers.BusinessLogic, "Requested identities with IDs [{0}], but failed to find identities [{1}]. They are either missing or inactive. All returned Identities: {2}", (object) string.Join<Guid>(",", (IEnumerable<Guid>) list), (object) string.Join<Guid>(",", guids), (object) string.Join(",", source.Select<Microsoft.VisualStudio.Services.Identity.Identity, string>((Func<Microsoft.VisualStudio.Services.Identity.Identity, string>) (identity =>
          {
            if (identity == null)
              return "null";
            return string.Format("{{ DisplayName: {0}, Id: {1}, MasterId: {2}, IsActive: {3} }}", (object) identity.DisplayName, (object) identity.Id, (object) identity.MasterId, (object) identity.IsActive);
          }))));
        else
          requestContext.Trace(403454, TraceLevel.Info, "Agile", TfsTraceLayers.BusinessLogic, "Requested identities with IDs [{0}]. All returned Identities: {1}", (object) string.Join<Guid>(",", (IEnumerable<Guid>) list), (object) string.Join(",", source.Select<Microsoft.VisualStudio.Services.Identity.Identity, string>((Func<Microsoft.VisualStudio.Services.Identity.Identity, string>) (identity =>
          {
            if (identity == null)
              return "null";
            return string.Format("{{ DisplayName: {0}, Id: {1}, MasterId: {2}, IsActive: {3} }}", (object) identity.DisplayName, (object) identity.Id, (object) identity.MasterId, (object) identity.IsActive);
          }))));
        return (IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity>) distinctIdentites;
      }
    }

    internal Microsoft.TeamFoundation.Work.WebApi.TeamMemberCapacity ConvertToWebApiTeamMemberCapacityReference(
      IVssRequestContext requestContext,
      Guid iterationId,
      IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> identities,
      TeamMemberCapacity serverTeamMemberCapacity,
      bool addUrls = false,
      Guid? projectId = null,
      Guid? teamId = null)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      identities.TryGetValue(serverTeamMemberCapacity.TeamMemberId, out identity);
      if (identity == null)
        requestContext.Trace(403454, TraceLevel.Info, "Agile", TfsTraceLayers.BusinessLogic, "Converting team member capacity for iteration {0}. No identity found for team member with ID {1}. Available identities: [{2}]", (object) iterationId.ToString(), (object) serverTeamMemberCapacity.TeamMemberId.ToString(), (object) string.Join(",", identities.Values.Select<Microsoft.VisualStudio.Services.Identity.Identity, string>((Func<Microsoft.VisualStudio.Services.Identity.Identity, string>) (i => i.Id.ToString()))));
      Microsoft.TeamFoundation.Work.WebApi.TeamMemberCapacity capacityReference = new Microsoft.TeamFoundation.Work.WebApi.TeamMemberCapacity();
      capacityReference.TeamMember = this.ConvertToWebApiMember(requestContext, identity, addUrls);
      if (serverTeamMemberCapacity.Activities == null)
        capacityReference.Activities = Enumerable.Empty<Microsoft.TeamFoundation.Work.WebApi.Activity>();
      else
        capacityReference.Activities = serverTeamMemberCapacity.Activities.Select<Activity, Microsoft.TeamFoundation.Work.WebApi.Activity>((Func<Activity, Microsoft.TeamFoundation.Work.WebApi.Activity>) (c => new Microsoft.TeamFoundation.Work.WebApi.Activity()
        {
          CapacityPerDay = c.CapacityPerDay,
          Name = c.Name
        }));
      if (serverTeamMemberCapacity.DaysOffDates == null)
        capacityReference.DaysOff = Enumerable.Empty<Microsoft.TeamFoundation.Work.WebApi.DateRange>();
      else
        capacityReference.DaysOff = serverTeamMemberCapacity.DaysOffDates.Select<DateRange, Microsoft.TeamFoundation.Work.WebApi.DateRange>((Func<DateRange, Microsoft.TeamFoundation.Work.WebApi.DateRange>) (x => new Microsoft.TeamFoundation.Work.WebApi.DateRange()
        {
          Start = x.Start,
          End = x.End
        }));
      if (addUrls)
      {
        ILocationService service = requestContext.GetService<ILocationService>();
        Dictionary<string, object> routeValues = this.GetRouteValues(projectId.Value, teamId.Value, iterationId, serverTeamMemberCapacity.TeamMemberId);
        capacityReference.Url = service.GetResourceUri(requestContext, "work", TeamSettingsApiConstants.CapacityLocationId, (object) routeValues, true).ToString();
      }
      return capacityReference;
    }

    internal TeamMemberCapacityIdentityRef ConvertToWebApiTeamMemberCapacityIdentityReference(
      IVssRequestContext requestContext,
      Guid iterationId,
      IDictionary<Guid, Microsoft.VisualStudio.Services.Identity.Identity> identities,
      TeamMemberCapacity serverTeamMemberCapacity,
      bool addUrls = false,
      Guid? projectId = null,
      Guid? teamId = null)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      identities.TryGetValue(serverTeamMemberCapacity.TeamMemberId, out identity);
      if (identity == null)
        requestContext.Trace(403454, TraceLevel.Info, "Agile", TfsTraceLayers.BusinessLogic, "Converting team member capacity for iteration {0}. No identity found for team member with ID {1}. Available identities: [{2}]", (object) iterationId.ToString(), (object) serverTeamMemberCapacity.TeamMemberId.ToString(), (object) string.Join(",", identities.Values.Select<Microsoft.VisualStudio.Services.Identity.Identity, string>((Func<Microsoft.VisualStudio.Services.Identity.Identity, string>) (i => i.Id.ToString()))));
      TeamMemberCapacityIdentityRef identityReference = new TeamMemberCapacityIdentityRef();
      identityReference.TeamMember = identity.ToIdentityRef(requestContext);
      if (serverTeamMemberCapacity.Activities == null)
        identityReference.Activities = Enumerable.Empty<Microsoft.TeamFoundation.Work.WebApi.Activity>();
      else
        identityReference.Activities = serverTeamMemberCapacity.Activities.Select<Activity, Microsoft.TeamFoundation.Work.WebApi.Activity>((Func<Activity, Microsoft.TeamFoundation.Work.WebApi.Activity>) (c => new Microsoft.TeamFoundation.Work.WebApi.Activity()
        {
          CapacityPerDay = c.CapacityPerDay,
          Name = c.Name
        }));
      if (serverTeamMemberCapacity.DaysOffDates == null)
        identityReference.DaysOff = Enumerable.Empty<Microsoft.TeamFoundation.Work.WebApi.DateRange>();
      else
        identityReference.DaysOff = serverTeamMemberCapacity.DaysOffDates.Select<DateRange, Microsoft.TeamFoundation.Work.WebApi.DateRange>((Func<DateRange, Microsoft.TeamFoundation.Work.WebApi.DateRange>) (x => new Microsoft.TeamFoundation.Work.WebApi.DateRange()
        {
          Start = x.Start,
          End = x.End
        }));
      if (addUrls)
      {
        ILocationService service = requestContext.GetService<ILocationService>();
        Dictionary<string, object> routeValues = this.GetRouteValues(projectId.Value, teamId.Value, iterationId, serverTeamMemberCapacity.TeamMemberId);
        identityReference.Url = service.GetResourceUri(requestContext, "work", TeamSettingsApiConstants.CapacityLocationId, (object) routeValues, true).ToString();
      }
      return identityReference;
    }

    internal Member ConvertToWebApiMember(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      bool addUrls)
    {
      string str1 = (identity != null ? identity.GetLegacyDistinctDisplayName() : (string) null) ?? string.Empty;
      string str2 = identity != null ? IdentityHelper.GetUniqueName(identity) : string.Empty;
      return new Member()
      {
        Id = identity != null ? identity.Id : Guid.Empty,
        DisplayName = str1,
        UniqueName = str2,
        Url = !addUrls || identity == null ? string.Empty : IdentityHelper.GetIdentityResourceUriString(requestContext, identity.Id),
        ImageUrl = !addUrls || identity == null ? string.Empty : IdentityHelper.GetImageResourceUrl(requestContext, identity.Id)
      };
    }

    public static object ConvertPotentialWitIdentityRef(object value) => value is WorkItemIdentity ? (object) (value as WorkItemIdentity).ToWitIdentityRef() : value;

    private Dictionary<string, object> GetRouteValues(
      Guid projectId,
      Guid teamId,
      Guid iterationId,
      Guid teamMemberId)
    {
      Dictionary<string, object> routeValues = new Dictionary<string, object>()
      {
        {
          nameof (iterationId),
          (object) iterationId
        },
        {
          nameof (teamMemberId),
          (object) teamMemberId
        }
      };
      if (projectId != Guid.Empty)
      {
        routeValues["project"] = (object) projectId;
        if (teamId != Guid.Empty)
          routeValues["team"] = (object) teamId;
      }
      return routeValues;
    }

    internal JsObject[] ToJson(IEnumerable<Microsoft.TeamFoundation.Work.WebApi.DateRange> enumerable) => enumerable.Select<Microsoft.TeamFoundation.Work.WebApi.DateRange, JsObject>((Func<Microsoft.TeamFoundation.Work.WebApi.DateRange, JsObject>) (x =>
    {
      return new JsObject()
      {
        {
          "start",
          (object) x.Start
        },
        {
          "end",
          (object) x.End
        }
      };
    })).ToArray<JsObject>();

    internal JsObject[] ToJson(IEnumerable<Microsoft.TeamFoundation.Work.WebApi.TeamMemberCapacity> enumerable) => enumerable.Select<Microsoft.TeamFoundation.Work.WebApi.TeamMemberCapacity, JsObject>((Func<Microsoft.TeamFoundation.Work.WebApi.TeamMemberCapacity, JsObject>) (x =>
    {
      return new JsObject()
      {
        {
          "activities",
          (object) this.ToJson(x.Activities)
        },
        {
          "daysOff",
          (object) this.ToJson(x.DaysOff)
        },
        {
          "teamMember",
          (object) this.ToJson(x.TeamMember)
        }
      };
    })).ToArray<JsObject>();

    private JsObject ToJson(Member teamMember)
    {
      JsObject json = new JsObject();
      json.Add("id", (object) teamMember.Id);
      json.Add("displayName", (object) teamMember.DisplayName);
      json.Add("uniqueName", (object) teamMember.UniqueName);
      json.Add("imageUrl", (object) teamMember.ImageUrl);
      json.Add("url", (object) teamMember.Url);
      return json;
    }

    private JsObject[] ToJson(IEnumerable<Microsoft.TeamFoundation.Work.WebApi.Activity> activities) => activities.Select<Microsoft.TeamFoundation.Work.WebApi.Activity, JsObject>((Func<Microsoft.TeamFoundation.Work.WebApi.Activity, JsObject>) (x => this.ToJson(x))).ToArray<JsObject>();

    private JsObject ToJson(Microsoft.TeamFoundation.Work.WebApi.Activity x)
    {
      JsObject json = new JsObject();
      json.Add("name", (object) x.Name);
      json.Add("capacityPerDay", (object) x.CapacityPerDay);
      return json;
    }
  }
}
