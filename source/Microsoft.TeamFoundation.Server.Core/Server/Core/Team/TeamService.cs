// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Team.TeamService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.IdentityImage;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core.Team
{
  internal class TeamService : ITeamService, IVssFrameworkService
  {
    private const string c_revertBackToUsingExpandedFlag = "Microsoft.VisualStudio.Services.TeamService.Ims.RevertBackToUsingExpanded";
    private const string c_teamServiceSafeguardBlockCreateTeamAndAdminOperations = "TeamService.Safeguard.BlockCreateTeamAndAdminOperations";
    private const int c_defaultMaxTeamsPerProject = 5000;
    private static readonly string[] DefaultTeamPropertyFilters = new string[1]
    {
      TeamConstants.TeamPropertyName
    };
    private static readonly string s_area = "Team";
    private static readonly string s_layer = "ITeamFoundationTeamService";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public WebApiTeam CreateTeam(
      IVssRequestContext requestContext,
      string projectUri,
      string name,
      string description)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (requestContext.IsFeatureEnabled("TeamService.Safeguard.BlockCreateTeamAndAdminOperations"))
        throw new TeamFoundationServiceException(Microsoft.TeamFoundation.Server.Core.Resources.TeamServiceSafeguardBlockCreateTeamAndAdminOperations_Error());
      using (requestContext.TraceBlock(290471, 290472, TeamService.s_area, TeamService.s_layer, nameof (CreateTeam)))
      {
        Microsoft.TeamFoundation.Server.Core.TeamsUtility.CheckTeamName(name);
        using (requestContext.TraceBlock(100140000, 100140001, TeamService.s_area, TeamService.s_layer, "ValidateMaxTeamsPerProject"))
        {
          List<WebApiTeam> list = this.QueryTeamsInProject(requestContext, ProjectInfo.GetProjectId(projectUri)).ToList<WebApiTeam>();
          requestContext.GetService<IVssRegistryService>();
          int maxTeamsAllowed = this.GetMaxTeamsAllowed(requestContext);
          if (list.Count >= maxTeamsAllowed)
            throw new TeamLimitExceededException(maxTeamsAllowed, list.Count);
        }
        TeamFoundationIdentityService service = requestContext.GetService<TeamFoundationIdentityService>();
        TeamFoundationIdentity applicationGroup = service.CreateApplicationGroup(requestContext, projectUri, name, description);
        new List<string>()
        {
          TeamConstants.TeamPropertyName
        };
        try
        {
          applicationGroup.SetProperty(IdentityPropertyScope.Local, TeamConstants.TeamPropertyName, (object) true);
          requestContext.Items.Add(TeamConstants.CalledFromTeamPlatformService, (object) true);
          try
          {
            service.UpdateIdentity(requestContext, applicationGroup, IdentityPropertyScope.Local);
            this.TryUpdateTeamCount(requestContext, ProjectInfo.GetProjectId(projectUri), out int _);
          }
          finally
          {
            requestContext.Items.Remove(TeamConstants.CalledFromTeamPlatformService);
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException(59999, TraceLevel.Info, TeamService.s_area, TeamService.s_layer, ex);
          service.DeleteApplicationGroup(requestContext, applicationGroup.Descriptor);
          throw;
        }
        return this.CreateWebApiTeamFromIdentity(requestContext, IdentityUtil.Convert(requestContext, applicationGroup), ());
      }
    }

    public void UpdateTeam(
      IVssRequestContext requestContext,
      Guid projectGuid,
      Guid teamGuid,
      Microsoft.TeamFoundation.Core.WebApi.Team.UpdateTeam newTeamProperties)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Core.WebApi.Team.UpdateTeam>(newTeamProperties, nameof (newTeamProperties));
      WebApiTeam teamByGuidInternal = this.GetTeamByGuidInternal(requestContext, teamGuid, false);
      if (teamByGuidInternal == null)
        throw new Microsoft.Azure.Devops.Teams.Service.TeamNotFoundException(teamGuid.ToString());
      if (!GroupHelpers.HasManageGroupMembershipPermission(requestContext, IdentityUtil.Convert(teamByGuidInternal.Identity)))
        throw new TeamUpdateInvalidPermissionException(FrameworkResources.PermissionIdentityManageMembership());
      Microsoft.TeamFoundation.Server.Core.TeamsUtility.CheckTeamName(newTeamProperties.Name);
      teamByGuidInternal.Identity.SetProperty("Account", (object) newTeamProperties.Name);
      teamByGuidInternal.Identity.SetProperty("Description", (object) newTeamProperties.Description);
      requestContext.GetService<IdentityService>().UpdateIdentities(requestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>()
      {
        teamByGuidInternal.Identity
      });
      requestContext.GetService<TeamFoundationEventService>().PublishNotification(requestContext, (object) new TeamChangedEvent(teamByGuidInternal, TeamChangeType.TeamUpdated));
    }

    public void DeleteTeam(IVssRequestContext requestContext, Guid teamId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(teamId, nameof (teamId));
      using (requestContext.TraceBlock(290527, 290528, TeamService.s_area, TeamService.s_layer, nameof (DeleteTeam)))
      {
        WebApiTeam teamByGuid = this.GetTeamByGuid(requestContext, teamId);
        if (teamByGuid == null)
          throw new Microsoft.Azure.Devops.Teams.Service.TeamNotFoundException(teamId.ToString());
        if (this.GetDefaultTeamId(requestContext, teamByGuid.ProjectId) == teamId)
          throw new CannotDeleteDefaultTeamException(teamByGuid.Name);
        try
        {
          requestContext.GetService<ITeamFoundationIdentityService>().DeleteApplicationGroup(requestContext, teamByGuid.Identity.Descriptor);
          requestContext.GetService<TeamFoundationEventService>().PublishNotification(requestContext, (object) new TeamChangedEvent(teamByGuid, TeamChangeType.TeamDeleted));
          this.TryUpdateTeamCount(requestContext, teamByGuid.ProjectId, out int _);
        }
        catch (FindGroupSidDoesNotExistException ex)
        {
          requestContext.TraceException(0, TeamService.s_area, TeamService.s_area, (Exception) ex);
        }
      }
    }

    public virtual IReadOnlyCollection<WebApiTeam> QueryTeamsInProject(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (PerformanceTimer.StartMeasure(requestContext, "TeamService.QueryTeamsInProject"))
      {
        using (requestContext.TraceBlock(290479, 290480, TeamService.s_area, TeamService.s_layer, nameof (QueryTeamsInProject)))
        {
          List<WebApiTeam> webApiTeamList = new List<WebApiTeam>();
          string projectUri = ProjectInfo.GetProjectUri(projectId);
          string projectName = this.GetProjectName(requestContext, projectId);
          foreach (TeamFoundationIdentity applicationGroup in requestContext.GetService<ITeamFoundationIdentityService>().ListApplicationGroups(requestContext, projectUri, ReadIdentityOptions.ExtendedProperties, (IEnumerable<string>) TeamService.DefaultTeamPropertyFilters, IdentityPropertyScope.Local))
          {
            if (applicationGroup.TryGetProperty(TeamConstants.TeamPropertyName, out object _))
              webApiTeamList.Add(this.CreateWebApiTeamFromIdentity(requestContext, IdentityUtil.Convert(requestContext, applicationGroup), (projectId, projectName)));
          }
          ClientTraceService service = requestContext.GetService<ClientTraceService>();
          ClientTraceData clientTraceData = new ClientTraceData();
          clientTraceData.Add("TeamsCountForProject", (object) webApiTeamList.Count);
          clientTraceData.Add("ProjectUri", (object) projectUri);
          IVssRequestContext requestContext1 = requestContext;
          string area = TeamService.s_area;
          string layer = TeamService.s_layer;
          ClientTraceData properties = clientTraceData;
          service.Publish(requestContext1, area, layer, properties);
          return (IReadOnlyCollection<WebApiTeam>) webApiTeamList;
        }
      }
    }

    public IReadOnlyCollection<WebApiTeam> QueryAllTeamsInCollection(
      IVssRequestContext requestContext)
    {
      requestContext.CheckProjectCollectionRequestContext();
      ITeamFoundationIdentityService service1 = requestContext.GetService<ITeamFoundationIdentityService>();
      IProjectService service2 = requestContext.GetService<IProjectService>();
      IEnumerable<TeamFoundationIdentity> foundationIdentities;
      using (requestContext.TraceBlock(290484, 290485, TeamService.s_area, TeamService.s_layer, "QueryAllTeamsInCollection.ReadIdentities"))
        foundationIdentities = (IEnumerable<TeamFoundationIdentity>) service1.ReadIdentities(requestContext, this.GetAllTeamIds(requestContext).ToArray());
      List<WebApiTeam> webApiTeamList = new List<WebApiTeam>();
      foreach (TeamFoundationIdentity identity in foundationIdentities)
      {
        if (identity != null && identity.IsActive)
        {
          string property = identity.GetProperty<string>("Domain", (string) null);
          if (service2.HasProjectPermission(requestContext, property, TeamProjectPermissions.GenericRead))
          {
            WebApiTeam teamFromIdentity = this.CreateWebApiTeamFromIdentity(requestContext, IdentityUtil.Convert(requestContext, identity), ());
            webApiTeamList.Add(teamFromIdentity);
          }
        }
      }
      return (IReadOnlyCollection<WebApiTeam>) webApiTeamList;
    }

    public IReadOnlyCollection<WebApiTeam> QueryMyTeamsInProject(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor,
      Guid projectId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ClientTraceData data = new ClientTraceData();
      IEnumerable<WebApiTeam> source = requestContext.TraceBlock<IEnumerable<WebApiTeam>>(290483, 290484, TeamService.s_area, TeamService.s_layer, nameof (QueryMyTeamsInProject), (Func<IEnumerable<WebApiTeam>>) (() =>
      {
        TeamFoundationIdentity readIdentity = requestContext.GetService<ITeamFoundationIdentityService>().ReadIdentities(requestContext.Elevate(), new IdentityDescriptor[1]
        {
          descriptor
        }, MembershipQuery.ExpandedUp, ReadIdentityOptions.None, (IEnumerable<string>) null)[0];
        if (readIdentity == null)
          return Enumerable.Empty<WebApiTeam>();
        IEnumerable<WebApiTeam> webApiTeams = this.FilterTeams(requestContext, readIdentity.MemberOf.ToArray<IdentityDescriptor>(), ProjectInfo.GetProjectUri(projectId));
        data.Add("QueryMyTeamsInProject.Filter(ms)", (object) requestContext.RequestTimer.LastTracedBlockSpan.TotalMilliseconds);
        return webApiTeams;
      }));
      data.Add("QueryMyTeamsInProject(ms)", (object) requestContext.RequestTimer.LastTracedBlockSpan.TotalMilliseconds);
      requestContext.GetService<ClientTraceService>().Publish(requestContext, TeamService.s_area, TeamService.s_layer, data);
      return (IReadOnlyCollection<WebApiTeam>) source.ToList<WebApiTeam>();
    }

    public IReadOnlyCollection<WebApiTeam> QueryMyTeamsInCollection(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (requestContext.TraceBlock(290481, 290482, TeamService.s_area, TeamService.s_layer, nameof (QueryMyTeamsInCollection)))
      {
        TeamFoundationIdentity readIdentity = requestContext.GetService<ITeamFoundationIdentityService>().ReadIdentities(requestContext, new IdentityDescriptor[1]
        {
          descriptor
        }, MembershipQuery.ExpandedUp, ReadIdentityOptions.None, (IEnumerable<string>) null)[0];
        return readIdentity == null ? (IReadOnlyCollection<WebApiTeam>) Enumerable.Empty<WebApiTeam>().ToList<WebApiTeam>() : (IReadOnlyCollection<WebApiTeam>) this.FilterTeams(requestContext, readIdentity.MemberOf.ToArray<IdentityDescriptor>()).ToList<WebApiTeam>();
      }
    }

    public int? GetTeamCountByProject(IVssRequestContext requestContext, Guid projectId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ProjectProperty projectProperty = requestContext.GetService<IProjectService>().GetProjectProperties(requestContext, projectId, TeamConstants.TeamCountProperty).FirstOrDefault<ProjectProperty>();
      int teamCount;
      return projectProperty != null ? (int?) projectProperty.Value : (!this.TryUpdateTeamCount(requestContext, projectId, out teamCount) ? new int?() : new int?(teamCount));
    }

    public (int teamCount, int teamsLimit) GetTeamCountAndLimitByProject(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return (this.GetTeamCountByProject(requestContext, projectId).GetValueOrDefault(), this.GetMaxTeamsAllowed(requestContext));
    }

    public IReadOnlyCollection<Microsoft.VisualStudio.Services.Identity.Identity> GetTeamAdmins(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity teamIdentity)
    {
      using (requestContext.TraceBlock(290475, 290476, TeamService.s_area, TeamService.s_layer, nameof (GetTeamAdmins)))
      {
        string teamSecurableToken = requestContext.GetService<IIdentitySecurityService>().GetTeamSecurableToken(requestContext, teamIdentity);
        IAccessControlList accessControlList = requestContext.GetService<SecuredTeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.IdentitiesNamespaceId).QueryAccessControlLists(requestContext, teamSecurableToken, (IEnumerable<IdentityDescriptor>) null, false, false).FirstOrDefault<IAccessControlList>();
        List<IdentityDescriptor> identityDescriptorList = new List<IdentityDescriptor>();
        if (accessControlList != null)
        {
          foreach (IAccessControlEntry accessControlEntry in accessControlList.AccessControlEntries)
          {
            if ((accessControlEntry.Allow & 8) == 8)
              identityDescriptorList.Add(accessControlEntry.Descriptor);
          }
        }
        return (IReadOnlyCollection<Microsoft.VisualStudio.Services.Identity.Identity>) ((IEnumerable<TeamFoundationIdentity>) requestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(requestContext, identityDescriptorList.ToArray())).Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (admin => admin != null)).Select<TeamFoundationIdentity, Microsoft.VisualStudio.Services.Identity.Identity>((Func<TeamFoundationIdentity, Microsoft.VisualStudio.Services.Identity.Identity>) (admin => IdentityUtil.Convert(requestContext, admin))).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      }
    }

    public WebApiTeam GetTeamByGuid(IVssRequestContext requestContext, Guid teamId) => this.GetTeamByGuidInternal(requestContext, teamId, true);

    private WebApiTeam GetTeamByGuidInternal(
      IVssRequestContext requestContext,
      Guid teamId,
      bool includeLocalProperties)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (requestContext.TraceBlock(290486, 290487, TeamService.s_area, TeamService.s_layer, nameof (GetTeamByGuidInternal)))
      {
        TeamFoundationIdentity identity = this.ReadTeamIdentityInternal<Guid>(requestContext, new Func<IVssRequestContext, Guid, IEnumerable<string>, TeamFoundationIdentity>(this.TeamRetrieverByGuid), teamId);
        return this.CreateWebApiTeamFromIdentity(requestContext, IdentityUtil.Convert(identity, includeLocalProperties), ());
      }
    }

    public WebApiTeam GetTeamByIdentityDescriptor(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (requestContext.TraceBlock(290488, 290489, TeamService.s_area, TeamService.s_layer, nameof (GetTeamByIdentityDescriptor)))
      {
        Func<IVssRequestContext, IdentityDescriptor, IEnumerable<string>, TeamFoundationIdentity> teamRetriever = (Func<IVssRequestContext, IdentityDescriptor, IEnumerable<string>, TeamFoundationIdentity>) ((context, id, filters) => context.GetService<ITeamFoundationIdentityService>().ReadIdentities(context, new IdentityDescriptor[1]
        {
          id
        }, MembershipQuery.None, ReadIdentityOptions.ExtendedProperties, filters, IdentityPropertyScope.Local)[0]);
        TeamFoundationIdentity identity = this.ReadTeamIdentityInternal<IdentityDescriptor>(requestContext, teamRetriever, descriptor);
        return this.CreateWebApiTeamFromIdentity(requestContext, IdentityUtil.Convert(requestContext, identity), ());
      }
    }

    public WebApiTeam GetTeamInProject(
      IVssRequestContext requestContext,
      Guid projectGuid,
      string teamIdOrName)
    {
      ProjectInfo project = requestContext.GetService<IProjectService>().GetProject(requestContext, projectGuid);
      WebApiTeam teamInProject = (WebApiTeam) null;
      Guid result;
      if (Guid.TryParseExact(teamIdOrName, "D", out result))
      {
        WebApiTeam teamByGuid = this.GetTeamByGuid(requestContext, result);
        teamInProject = TeamService.EnsureTeamBelongsToProject(requestContext, projectGuid, teamByGuid);
      }
      if (teamInProject == null)
      {
        TeamFoundationIdentity identity = this.ReadTeamIdentityInternal(requestContext, project.Uri, teamIdOrName);
        teamInProject = this.CreateWebApiTeamFromIdentity(requestContext, IdentityUtil.Convert(requestContext, identity), ());
      }
      return teamInProject;
    }

    public IReadOnlyCollection<WebApiTeam> GetTeamsByGuid(
      IVssRequestContext requestContext,
      IEnumerable<Guid> teamIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) teamIds, nameof (teamIds));
      using (requestContext.TraceBlock(290492, 290493, TeamService.s_area, TeamService.s_layer, nameof (GetTeamsByGuid)))
      {
        Func<Guid[], IEnumerable<string>, IEnumerable<TeamFoundationIdentity>> func = (Func<Guid[], IEnumerable<string>, IEnumerable<TeamFoundationIdentity>>) ((ids, filters) => (IEnumerable<TeamFoundationIdentity>) requestContext.GetService<ITeamFoundationIdentityService>().ReadIdentities(requestContext, ids, MembershipQuery.None, ReadIdentityOptions.ExtendedProperties, filters, IdentityPropertyScope.Local));
        List<WebApiTeam> teamsByGuid = new List<WebApiTeam>();
        Guid[] array = teamIds.ToArray<Guid>();
        string[] teamPropertyFilters = TeamService.DefaultTeamPropertyFilters;
        foreach (TeamFoundationIdentity identity in func(array, (IEnumerable<string>) teamPropertyFilters))
        {
          if (identity != null && identity.IsActive && identity.TryGetProperty(TeamConstants.TeamPropertyName, out object _))
            teamsByGuid.Add(this.CreateWebApiTeamFromIdentity(requestContext, IdentityUtil.Convert(requestContext, identity), ()));
        }
        return (IReadOnlyCollection<WebApiTeam>) teamsByGuid;
      }
    }

    public IReadOnlyCollection<Microsoft.VisualStudio.Services.Identity.Identity> ReadTeamMembers(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity teamIdentity,
      MembershipQuery queryMembership)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(teamIdentity, nameof (teamIdentity));
      using (requestContext.TraceBlock(290494, 290495, TeamService.s_area, TeamService.s_layer, nameof (ReadTeamMembers)))
      {
        ITeamFoundationIdentityService service = requestContext.GetService<ITeamFoundationIdentityService>();
        ICollection<IdentityDescriptor> source;
        if (queryMembership == MembershipQuery.Expanded && requestContext.ExecutionEnvironment.IsHostedDeployment && !requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.TeamService.Ims.RevertBackToUsingExpanded"))
        {
          source = (ICollection<IdentityDescriptor>) GroupHelpers.ExpandMembersRecursively(requestContext, teamIdentity.Descriptor);
        }
        else
        {
          if (requestContext.IsFeatureEnabled("Agile.Server.TeamService.AADGroupsBlockExpansion") && queryMembership == MembershipQuery.ExpandedDown)
            queryMembership = MembershipQuery.Expanded;
          source = service.ReadIdentities(requestContext, new IdentityDescriptor[1]
          {
            teamIdentity.Descriptor
          }, queryMembership, ReadIdentityOptions.None, (IEnumerable<string>) null)[0]?.Members;
        }
        return (IReadOnlyCollection<Microsoft.VisualStudio.Services.Identity.Identity>) (source == null || source.Count <= 0 ? (IEnumerable<TeamFoundationIdentity>) Array.Empty<TeamFoundationIdentity>() : (IEnumerable<TeamFoundationIdentity>) ((IEnumerable<TeamFoundationIdentity>) service.ReadIdentities(requestContext, source.ToArray<IdentityDescriptor>(), MembershipQuery.None, ReadIdentityOptions.None, (IEnumerable<string>) null)).Where<TeamFoundationIdentity>((Func<TeamFoundationIdentity, bool>) (x => x != null)).ToArray<TeamFoundationIdentity>()).Select<TeamFoundationIdentity, Microsoft.VisualStudio.Services.Identity.Identity>((Func<TeamFoundationIdentity, Microsoft.VisualStudio.Services.Identity.Identity>) (member => IdentityUtil.Convert(requestContext, member))).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      }
    }

    public void SetTeamImage(IVssRequestContext requestContext, Guid teamId, byte[] imageBytes)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(teamId, nameof (teamId));
      try
      {
        Microsoft.TeamFoundation.Server.Core.TeamsUtility.CheckChangeImagePermissions(teamId, requestContext);
        IdentityService service = requestContext.GetService<IdentityService>();
        Microsoft.VisualStudio.Services.Identity.Identity readIdentity = service.ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
        {
          teamId
        }, QueryMembership.None, (IEnumerable<string>) null)[0];
        if (readIdentity == null)
          throw new IdentityNotFoundException(teamId);
        if (!readIdentity.IsContainer)
          throw new ArgumentException(FrameworkResources.RequiresContainerIdentity((object) readIdentity.SubjectDescriptor));
        readIdentity.SetProperty("Microsoft.TeamFoundation.Identity.Image.Data", (object) imageBytes);
        readIdentity.SetProperty("Microsoft.TeamFoundation.Identity.Image.Type", (object) "image/png");
        readIdentity.SetProperty("Microsoft.TeamFoundation.Identity.Image.Id", (object) Guid.NewGuid().ToByteArray());
        readIdentity.SetProperty("Microsoft.TeamFoundation.Identity.CandidateImage.Data", (object) null);
        readIdentity.SetProperty("Microsoft.TeamFoundation.Identity.CandidateImage.UploadDate", (object) null);
        service.UpdateIdentities(requestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
        {
          readIdentity
        });
        IdentityImageService.InvalidateIdentityImage(requestContext, readIdentity);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(290477, TeamService.s_area, TeamService.s_layer, ex);
        throw new ArgumentException(ex.Message, "teamImageSet");
      }
    }

    public void RemoveTeamImage(IVssRequestContext requestContext, Guid teamId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(teamId, nameof (teamId));
      try
      {
        Microsoft.TeamFoundation.Server.Core.TeamsUtility.CheckChangeImagePermissions(teamId, requestContext);
        IdentityService service = requestContext.GetService<IdentityService>();
        Microsoft.VisualStudio.Services.Identity.Identity readIdentity = service.ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
        {
          teamId
        }, QueryMembership.None, (IEnumerable<string>) null)[0];
        if (readIdentity == null)
          throw new IdentityNotFoundException(teamId);
        if (!readIdentity.IsContainer)
          throw new ArgumentException(FrameworkResources.RequiresContainerIdentity((object) readIdentity.SubjectDescriptor));
        readIdentity.SetProperty("Microsoft.TeamFoundation.Identity.Image.Data", (object) null);
        readIdentity.SetProperty("Microsoft.TeamFoundation.Identity.Image.Id", (object) null);
        readIdentity.SetProperty("Microsoft.TeamFoundation.Identity.Image.Type", (object) null);
        readIdentity.SetProperty("Microsoft.TeamFoundation.Identity.CandidateImage.Data", (object) null);
        readIdentity.SetProperty("Microsoft.TeamFoundation.Identity.CandidateImage.UploadDate", (object) null);
        service.UpdateIdentities(requestContext, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
        {
          readIdentity
        });
        IdentityImageService.InvalidateIdentityImage(requestContext, readIdentity);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(290478, TeamService.s_area, TeamService.s_layer, ex);
        throw new ArgumentException(ex.Message, "teamImageDelete");
      }
    }

    public int GetMaxTeamsAllowed(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/TeamFoundationTeamService/MaxTeamPerProject", 5000);

    public bool IsDefaultTeam(IVssRequestContext requestContext, Guid teamId)
    {
      using (requestContext.TraceBlock(290473, 290474, TeamService.s_area, TeamService.s_layer, nameof (IsDefaultTeam)))
      {
        TeamFoundationIdentity readIdentity = requestContext.GetService<ITeamFoundationIdentityService>().ReadIdentities(requestContext, new Guid[1]
        {
          teamId
        }, MembershipQuery.None, ReadIdentityOptions.ExtendedProperties, (IEnumerable<string>) new string[1]
        {
          TeamConstants.TeamPropertyName
        }, IdentityPropertyScope.Local)[0];
        if (!readIdentity.TryGetProperty(TeamConstants.TeamPropertyName, out object _))
          return false;
        string attribute = readIdentity.GetAttribute("Domain", (string) null);
        return !string.IsNullOrEmpty(attribute) && object.Equals((object) this.GetDefaultTeamId(requestContext, ProjectInfo.GetProjectId(attribute)), (object) teamId);
      }
    }

    public Guid GetDefaultTeamId(IVssRequestContext requestContext, Guid projectId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      IProjectService service = requestContext.GetService<IProjectService>();
      try
      {
        ProjectProperty projectProperty = service.GetProjectProperties(requestContext, projectId, TeamConstants.DefaultTeamPropertyName).FirstOrDefault<ProjectProperty>();
        if (projectProperty != null)
        {
          if (!string.IsNullOrEmpty((string) projectProperty.Value))
            return new Guid((string) projectProperty.Value);
        }
      }
      catch (ProjectDoesNotExistException ex)
      {
        return Guid.Empty;
      }
      return Guid.Empty;
    }

    public virtual WebApiTeam GetDefaultTeam(IVssRequestContext requestContext, Guid projectId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      Guid defaultTeamId = this.GetDefaultTeamId(requestContext, projectId);
      WebApiTeam defaultTeam = (WebApiTeam) null;
      if (defaultTeamId != Guid.Empty)
        defaultTeam = this.GetTeamByGuid(requestContext, defaultTeamId);
      return defaultTeam;
    }

    public void SetDefaultTeamId(IVssRequestContext requestContext, Guid projectId, Guid teamId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForEmptyGuid(teamId, nameof (teamId));
      using (requestContext.TraceBlock(290496, 290497, TeamService.s_area, TeamService.s_layer, nameof (SetDefaultTeamId)))
      {
        WebApiTeam teamByGuid = this.GetTeamByGuid(requestContext, teamId);
        if (teamByGuid == null || teamByGuid.ProjectId != projectId)
          throw new Microsoft.Azure.Devops.Teams.Service.TeamNotFoundException(teamId.ToString());
        string projectUri = ProjectInfo.GetProjectUri(projectId);
        IProjectService service = requestContext.GetService<IProjectService>();
        service.CheckProjectPermission(requestContext, projectUri, TeamProjectPermissions.GenericWrite);
        service.SetProjectProperties(requestContext.Elevate(), projectId, (IEnumerable<ProjectProperty>) new ProjectProperty[1]
        {
          new ProjectProperty(TeamConstants.DefaultTeamPropertyName, (object) teamId.ToString())
        });
      }
    }

    public ISecuredObject GetSecuredTeamObject(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity teamIdentity)
    {
      using (requestContext.TraceBlock(290960, 290961, TeamService.s_area, TeamService.s_layer, nameof (GetSecuredTeamObject)))
      {
        this.UserHasPermission(requestContext, teamIdentity, 1);
        string teamSecurableToken = requestContext.GetService<IIdentitySecurityService>().GetTeamSecurableToken(requestContext, teamIdentity);
        return (ISecuredObject) new SecuredTeam(FrameworkSecurity.IdentitiesNamespaceId, 1, teamSecurableToken);
      }
    }

    public WebApiTeam GetTeamWithSecuredObject(
      IVssRequestContext requestContext,
      Guid projectGuid,
      string teamIdOrName)
    {
      WebApiTeam teamInProject = this.GetTeamInProject(requestContext, projectGuid, teamIdOrName);
      if (teamInProject == null)
        throw new Microsoft.Azure.Devops.Teams.Service.TeamNotFoundException(teamIdOrName);
      ISecuredObject securedTeamObject = this.GetSecuredTeamObject(requestContext, teamInProject.Identity);
      return new WebApiTeam(teamInProject, securedTeamObject);
    }

    public bool HasTeamPermission(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity teamIdentity,
      int requestedPermissions,
      bool alwaysAllowAdministrators)
    {
      using (requestContext.TraceBlock(290498, 290499, TeamService.s_area, TeamService.s_layer, nameof (HasTeamPermission)))
        return requestContext.GetService<IIdentitySecurityService>().HasTeamPermission(requestContext, teamIdentity, requestedPermissions, alwaysAllowAdministrators);
    }

    public bool UserHasPermission(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity teamIdentity,
      int permission)
    {
      using (requestContext.TraceBlock(290956, 290957, TeamService.s_area, TeamService.s_layer, nameof (UserHasPermission)))
      {
        string key = "UserHasPermissions_" + teamIdentity.Id.ToString() + "_" + permission.ToString();
        object obj;
        bool flag;
        if (!requestContext.Items.TryGetValue(key, out obj))
        {
          flag = this.HasTeamPermission(requestContext, teamIdentity, permission, true);
          requestContext.Items[key] = (object) flag;
        }
        else
          flag = (bool) obj;
        return flag;
      }
    }

    public bool UserIsTeamAdmin(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity teamIdentity)
    {
      using (requestContext.TraceBlock(290958, 290959, TeamService.s_area, TeamService.s_layer, nameof (UserIsTeamAdmin)))
        return requestContext.IsSystemContext || this.HasTeamPermission(requestContext, teamIdentity, 8, true);
    }

    [Obsolete("Don't use Team to store local properties. This service is only available because we still need to support on-prem migration steps (TeamFavoriteMigrator, AgileStepPerformer)")]
    public void SetTeamViewProperty(
      IVssRequestContext requestContext,
      Guid teamGuid,
      string propertyName,
      string propertyValue)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(teamGuid, nameof (teamGuid));
      ArgumentUtility.CheckStringForNullOrEmpty(propertyName, nameof (propertyName));
      WebApiTeam teamByGuid = this.GetTeamByGuid(requestContext, teamGuid);
      teamByGuid.Identity.SetProperty(propertyName, (object) propertyValue);
      new Microsoft.VisualStudio.Services.Identity.IdentityPropertyHelper().UpdateExtendedProperties(requestContext, IdentityPropertyScope.Local, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>()
      {
        teamByGuid.Identity
      });
    }

    [Obsolete("Don't use Team to store local properties. This service is only available because we still need to support on-prem migration steps (TeamFavoriteMigrator, AgileStepPerformer)")]
    public bool TryGetTeamViewProperty<PropertyType>(
      IVssRequestContext requestContext,
      Guid teamGuid,
      string propertyName,
      out PropertyType propertyValue)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(teamGuid, nameof (teamGuid));
      ArgumentUtility.CheckStringForNullOrEmpty(propertyName, nameof (propertyName));
      object obj;
      if (this.ReadTeamWithAdditionalLocalProperties(requestContext, teamGuid, new HashSet<string>()
      {
        propertyName
      }).Identity.TryGetProperty(propertyName, out obj) && obj is PropertyType propertyType)
      {
        propertyValue = propertyType;
        return true;
      }
      propertyValue = default (PropertyType);
      return false;
    }

    [Obsolete("Don't use Team to store local properties. This service is only available because we still need to support on-prem migration steps (TeamFavoriteMigrator, AgileStepPerformer)")]
    public void DeleteTeamViewProperties(
      IVssRequestContext requestContext,
      Guid teamGuid,
      IReadOnlyCollection<string> propertyNames)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(teamGuid, nameof (teamGuid));
      if (propertyNames == null || !propertyNames.Any<string>())
        return;
      if (propertyNames.Contains<string>(TeamConstants.TeamPropertyName, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
        throw new InvalidOperationException(TeamConstants.TeamPropertyName);
      WebApiTeam webApiTeam = this.ReadTeamWithAdditionalLocalProperties(requestContext, teamGuid, new HashSet<string>((IEnumerable<string>) propertyNames));
      foreach (string propertyName in (IEnumerable<string>) propertyNames)
        webApiTeam.Identity.SetProperty(propertyName, (object) null);
      new Microsoft.VisualStudio.Services.Identity.IdentityPropertyHelper().UpdateExtendedProperties(requestContext, IdentityPropertyScope.Local, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) new List<Microsoft.VisualStudio.Services.Identity.Identity>()
      {
        webApiTeam.Identity
      });
    }

    private List<Guid> GetAllTeamIds(IVssRequestContext requestContext)
    {
      requestContext.CheckProjectCollectionRequestContext();
      List<Guid> allTeamIds = new List<Guid>();
      using (TeamFoundationDataReader properties = requestContext.GetService<ITeamFoundationPropertyService>().GetProperties(requestContext, ArtifactKinds.LocalIdentity, (IEnumerable<string>) new string[1]
      {
        TeamConstants.TeamPropertyName
      }, new Guid?(Guid.Empty)))
      {
        foreach (ArtifactPropertyValue artifactPropertyValue in properties)
        {
          foreach (PropertyValue propertyValue in artifactPropertyValue.PropertyValues)
          {
            bool result;
            if (StringComparer.Ordinal.Equals(propertyValue.PropertyName, TeamConstants.TeamPropertyName) && ((!(propertyValue.Value is string str) ? 0 : (bool.TryParse(str, out result) ? 1 : 0)) & (result ? 1 : 0)) != 0)
              allTeamIds.Add(new Guid(artifactPropertyValue.Spec.Id));
          }
        }
      }
      return allTeamIds;
    }

    private IEnumerable<WebApiTeam> FilterTeams(
      IVssRequestContext requestContext,
      IdentityDescriptor[] groups)
    {
      using (requestContext.TraceBlock(290532, 290533, TeamService.s_area, TeamService.s_layer, nameof (FilterTeams)))
      {
        List<WebApiTeam> webApiTeamList = new List<WebApiTeam>();
        if (groups != null && groups.Length != 0)
        {
          foreach (TeamFoundationIdentity readIdentity in requestContext.GetService<ITeamFoundationIdentityService>().ReadIdentities(requestContext, groups, MembershipQuery.None, ReadIdentityOptions.ExtendedProperties, (IEnumerable<string>) TeamService.DefaultTeamPropertyFilters, IdentityPropertyScope.Local))
          {
            if (readIdentity != null && readIdentity.TryGetProperty(TeamConstants.TeamPropertyName, out object _))
              webApiTeamList.Add(this.CreateWebApiTeamFromIdentity(requestContext, IdentityUtil.Convert(requestContext, readIdentity), ()));
          }
        }
        return (IEnumerable<WebApiTeam>) webApiTeamList;
      }
    }

    private IEnumerable<WebApiTeam> FilterTeams(
      IVssRequestContext requestContext,
      IdentityDescriptor[] groups,
      string projectUri)
    {
      return (IEnumerable<WebApiTeam>) requestContext.TraceBlock<List<WebApiTeam>>(290534, 290535, TeamService.s_area, TeamService.s_layer, nameof (FilterTeams), (Func<List<WebApiTeam>>) (() =>
      {
        List<WebApiTeam> webApiTeamList = new List<WebApiTeam>();
        if (groups != null && groups.Length != 0)
        {
          ITeamFoundationIdentityService service = requestContext.GetService<ITeamFoundationIdentityService>();
          foreach (TeamFoundationIdentity identity in ((IEnumerable<TeamFoundationIdentity>) service.ReadIdentities(requestContext, groups, MembershipQuery.None, ReadIdentityOptions.ExtendedProperties, (IEnumerable<string>) TeamService.DefaultTeamPropertyFilters, IdentityPropertyScope.Local)).Intersect<TeamFoundationIdentity>((IEnumerable<TeamFoundationIdentity>) service.ListApplicationGroups(requestContext, projectUri, ReadIdentityOptions.ExtendedProperties, (IEnumerable<string>) TeamService.DefaultTeamPropertyFilters, IdentityPropertyScope.Local), (IEqualityComparer<TeamFoundationIdentity>) new TeamFoundationIdentityDisplayNameComparer()))
          {
            if (identity != null && identity.TryGetProperty(TeamConstants.TeamPropertyName, out object _))
              webApiTeamList.Add(this.CreateWebApiTeamFromIdentity(requestContext, IdentityUtil.Convert(requestContext, identity), ()));
          }
        }
        return webApiTeamList;
      }));
    }

    private TeamFoundationIdentity ReadTeamIdentityInternal<T>(
      IVssRequestContext requestContext,
      Func<IVssRequestContext, T, IEnumerable<string>, TeamFoundationIdentity> teamRetriever,
      T teamDescriptor,
      HashSet<string> additionalLocalPropertyNames = null)
    {
      if (requestContext.GetService<PlatformProjectService>().GetPreCreatedProjectId(requestContext) != Guid.Empty && !requestContext.IsFeatureEnabled("Project.PreCreation.DisableBypassCacheForDefaultTeam"))
        requestContext.RootContext.Items[RequestContextItemsKeys.BypassIdentityCacheWhenReadingByVsid] = (object) true;
      List<string> list = ((IEnumerable<string>) TeamService.DefaultTeamPropertyFilters).ToList<string>();
      if (additionalLocalPropertyNames != null)
        list.AddRange((IEnumerable<string>) additionalLocalPropertyNames);
      TeamFoundationIdentity foundationIdentity = teamRetriever(requestContext, teamDescriptor, (IEnumerable<string>) list);
      return foundationIdentity == null || !foundationIdentity.IsActive || !foundationIdentity.TryGetProperty(TeamConstants.TeamPropertyName, out object _) ? (TeamFoundationIdentity) null : foundationIdentity;
    }

    private TeamFoundationIdentity ReadTeamIdentityInternal(
      IVssRequestContext requestContext,
      string projectUri,
      string teamName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (requestContext.TraceBlock(290490, 290491, TeamService.s_area, TeamService.s_layer, nameof (ReadTeamIdentityInternal)))
      {
        Func<IVssRequestContext, string, IEnumerable<string>, TeamFoundationIdentity> teamRetriever = (Func<IVssRequestContext, string, IEnumerable<string>, TeamFoundationIdentity>) ((context, id, filters) =>
        {
          ITeamFoundationIdentityService service = context.GetService<ITeamFoundationIdentityService>();
          string str = projectUri + "\\" + id;
          IVssRequestContext requestContext1 = context;
          string factorValue = str;
          IEnumerable<string> propertyNameFilters = filters;
          return service.ReadIdentity(requestContext1, IdentitySearchFactor.LocalGroupName, factorValue, MembershipQuery.None, ReadIdentityOptions.ExtendedProperties, propertyNameFilters, IdentityPropertyScope.Local);
        });
        return this.ReadTeamIdentityInternal<string>(requestContext, teamRetriever, teamName);
      }
    }

    private bool TryUpdateTeamCount(
      IVssRequestContext requestContext,
      Guid projectId,
      out int teamCount)
    {
      try
      {
        using (requestContext.TraceBlock(290952, 290953, TeamService.s_area, TeamService.s_layer, nameof (TryUpdateTeamCount)))
        {
          teamCount = this.QueryTeamsInProject(requestContext.Elevate(), projectId).Count;
          requestContext.GetService<IProjectService>().SetProjectProperties(requestContext.Elevate(), projectId, (IEnumerable<ProjectProperty>) new ProjectProperty[1]
          {
            new ProjectProperty(TeamConstants.TeamCountProperty, (object) teamCount)
          });
          return true;
        }
      }
      catch (TeamFoundationServerException ex)
      {
        requestContext.TraceException(290875, TeamService.s_area, TeamService.s_layer, (Exception) ex);
        teamCount = -1;
        return false;
      }
    }

    private static WebApiTeam EnsureTeamBelongsToProject(
      IVssRequestContext requestContext,
      Guid projectGuid,
      WebApiTeam team)
    {
      if (team != null && projectGuid != team.ProjectId)
      {
        requestContext.TraceAlways(290809, TraceLevel.Error, TeamService.s_area, TeamService.s_layer, string.Format("Team '{0}' ({1}) does not belong to ", (object) team.Name, (object) team.Id) + string.Format("project with GUID: {0}", (object) projectGuid));
        team = (WebApiTeam) null;
      }
      return team;
    }

    private string GetProjectName(IVssRequestContext requestContext, Guid projectId)
    {
      string projectName1 = (string) null;
      string projectName2;
      if (requestContext.GetService<IProjectService>().TryGetProjectName(requestContext, projectId, out projectName2))
        projectName1 = projectName2;
      return projectName1;
    }

    internal WebApiTeam CreateWebApiTeamFromIdentity(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      (Guid projectId, string projectName) projectInfo = default ((Guid projectId, string projectName)))
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (identity == null)
        return (WebApiTeam) null;
      (Guid projectId, string projectName) tuple = projectInfo;
      Guid projectId;
      string projectName;
      if (tuple.projectId == new Guid() && tuple.projectName == (string) null)
      {
        projectId = ProjectInfo.GetProjectId(identity.GetProperty<string>("Domain", (string) null));
        projectName = this.GetProjectName(requestContext, projectId);
      }
      else
      {
        projectId = projectInfo.projectId;
        projectName = projectInfo.projectName;
      }
      WebApiTeam teamFromIdentity = new WebApiTeam();
      teamFromIdentity.Id = identity.Id;
      object obj1;
      teamFromIdentity.Name = identity.TryGetProperty("Account", out obj1) ? obj1.ToString() : (string) null;
      teamFromIdentity.Url = ServerCoreApiExtensions.GetCoreResourceUriString(requestContext, CoreConstants.TeamsLocationId, (object) new
      {
        projectId = projectId.ToString(),
        teamId = identity.Id
      });
      teamFromIdentity.Identity = identity;
      teamFromIdentity.ProjectId = projectId;
      teamFromIdentity.ProjectName = projectName;
      object obj2;
      teamFromIdentity.Description = identity.TryGetProperty("Description", out obj2) ? obj2.ToString() : (string) null;
      teamFromIdentity.IdentityUrl = IdentityHelper.GetIdentityResourceUriString(requestContext, identity.Id);
      return teamFromIdentity;
    }

    [Obsolete("Use ReadTeam instead. You should NOT need to read local properties")]
    private WebApiTeam ReadTeamWithAdditionalLocalProperties(
      IVssRequestContext requestContext,
      Guid teamId,
      HashSet<string> additionalPropertyNames)
    {
      TeamFoundationIdentity identity = this.ReadTeamIdentityInternal<Guid>(requestContext, new Func<IVssRequestContext, Guid, IEnumerable<string>, TeamFoundationIdentity>(this.TeamRetrieverByGuid), teamId, additionalPropertyNames);
      return this.CreateWebApiTeamFromIdentity(requestContext, IdentityUtil.Convert(requestContext, identity), ());
    }

    private TeamFoundationIdentity TeamRetrieverByGuid(
      IVssRequestContext requestContext,
      Guid teamId,
      IEnumerable<string> localPropertyNames)
    {
      return requestContext.GetService<ITeamFoundationIdentityService>().ReadIdentities(requestContext, new Guid[1]
      {
        teamId
      }, MembershipQuery.None, ReadIdentityOptions.ExtendedProperties, localPropertyNames, IdentityPropertyScope.Local)[0];
    }
  }
}
