// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.TfsTeamService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Core.WebApi.Internal;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Client
{
  public class TfsTeamService : ITfsConnectionObject
  {
    private const string c_defaultTeamPropertyName = "Microsoft.TeamFoundation.Team.Default";
    private TfsConnection m_tfs;

    public void Initialize(TfsConnection tfsConnection) => this.m_tfs = tfsConnection;

    public TeamFoundationTeam CreateTeam(
      string projectId,
      string name,
      string description,
      IDictionary<string, object> properties)
    {
      if (!CssUtils.IsValidProjectName(name))
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, TFCommonResources.BAD_GROUP_NAME((object) name)));
      IIdentityManagementService2 service = this.m_tfs.GetService<IIdentityManagementService2>();
      IdentityDescriptor applicationGroup = service.CreateApplicationGroup(projectId, name, description);
      TeamFoundationIdentity foundationIdentity = service.ReadIdentity(applicationGroup, MembershipQuery.None, ReadIdentityOptions.None);
      foundationIdentity.SetProperty(IdentityPropertyScope.Local, TeamConstants.TeamPropertyName, (object) true);
      if (properties != null)
      {
        foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) properties)
          foundationIdentity.SetProperty(IdentityPropertyScope.Local, property.Key, property.Value);
      }
      service.UpdateExtendedProperties(foundationIdentity);
      this.m_tfs.GetService<ISecurityService>().GetSecurityNamespace(FrameworkSecurity.IdentitiesNamespaceId).SetPermissions(IdentityHelper.CreateSecurityToken(foundationIdentity), this.m_tfs.AuthorizedIdentity.Descriptor, 31, 0, false);
      return new TeamFoundationTeam(foundationIdentity);
    }

    public IEnumerable<TeamFoundationTeam> QueryTeams(string projectId)
    {
      TeamFoundationIdentity[] foundationIdentityArray = this.m_tfs.GetService<IIdentityManagementService2>().ListApplicationGroups(projectId, ReadIdentityOptions.ExtendedProperties, (IEnumerable<string>) new string[1]
      {
        TeamConstants.TeamPropertyName
      }, IdentityPropertyScope.Local);
      List<TeamFoundationTeam> teamFoundationTeamList = new List<TeamFoundationTeam>();
      foreach (TeamFoundationIdentity team in foundationIdentityArray)
      {
        if (team.TryGetProperty(TeamConstants.TeamPropertyName, out object _))
          teamFoundationTeamList.Add(new TeamFoundationTeam(team));
      }
      return (IEnumerable<TeamFoundationTeam>) teamFoundationTeamList;
    }

    public IEnumerable<TeamFoundationTeam> QueryTeams(IdentityDescriptor descriptor) => this.QueryTeams(descriptor, ((IEnumerable<string>) new string[1]
    {
      TeamConstants.TeamPropertyName
    }).ToList<string>());

    public IEnumerable<TeamFoundationTeam> QueryTeams(
      IdentityDescriptor descriptor,
      List<string> propertyNameFilters)
    {
      IIdentityManagementService2 service = this.m_tfs.GetService<IIdentityManagementService2>();
      TeamFoundationIdentity foundationIdentity = service.ReadIdentity(descriptor, MembershipQuery.Expanded, ReadIdentityOptions.None, (IEnumerable<string>) null, IdentityPropertyScope.None);
      List<TeamFoundationTeam> teamFoundationTeamList = new List<TeamFoundationTeam>();
      if (foundationIdentity != null)
      {
        if (propertyNameFilters == null)
          propertyNameFilters = new List<string>();
        if (!propertyNameFilters.Contains(TeamConstants.TeamPropertyName))
          propertyNameFilters.Add(TeamConstants.TeamPropertyName);
        foreach (TeamFoundationIdentity readIdentity in service.ReadIdentities(foundationIdentity.MemberOf, MembershipQuery.None, ReadIdentityOptions.ExtendedProperties, (IEnumerable<string>) propertyNameFilters, IdentityPropertyScope.Local))
        {
          if (readIdentity.TryGetProperty(TeamConstants.TeamPropertyName, out object _))
            teamFoundationTeamList.Add(new TeamFoundationTeam(readIdentity));
        }
      }
      return (IEnumerable<TeamFoundationTeam>) teamFoundationTeamList;
    }

    private TeamFoundationTeam ReadTeamInternal<T>(
      Func<T, IEnumerable<string>, TeamFoundationIdentity> teamRetriever,
      T teamDescriptor,
      List<string> propertyNameFilters)
    {
      propertyNameFilters?.Add(TeamConstants.TeamPropertyName);
      TeamFoundationIdentity team = teamRetriever(teamDescriptor, (IEnumerable<string>) propertyNameFilters);
      return team == null || !team.IsActive || !team.TryGetProperty(TeamConstants.TeamPropertyName, out object _) ? (TeamFoundationTeam) null : new TeamFoundationTeam(team);
    }

    public TeamFoundationTeam ReadTeam(
      IdentityDescriptor descriptor,
      List<string> propertyNameFilters)
    {
      return this.ReadTeamInternal<IdentityDescriptor>((Func<IdentityDescriptor, IEnumerable<string>, TeamFoundationIdentity>) ((id, filters) => this.m_tfs.GetService<IIdentityManagementService2>().ReadIdentity(id, MembershipQuery.None, ReadIdentityOptions.ExtendedProperties, filters, IdentityPropertyScope.Local)), descriptor, propertyNameFilters);
    }

    public TeamFoundationTeam ReadTeam(Guid teamId, List<string> propertyNameFilters) => this.ReadTeamInternal<Guid>((Func<Guid, IEnumerable<string>, TeamFoundationIdentity>) ((id, filters) => this.m_tfs.GetService<IIdentityManagementService2>().ReadIdentities(new Guid[1]
    {
      id
    }, MembershipQuery.None, ReadIdentityOptions.ExtendedProperties, filters, IdentityPropertyScope.Local)[0]), teamId, propertyNameFilters);

    public TeamFoundationTeam ReadTeam(
      string projectId,
      string teamName,
      List<string> propertyNameFilters)
    {
      return this.ReadTeamInternal<string>((Func<string, IEnumerable<string>, TeamFoundationIdentity>) ((id, filters) => this.m_tfs.GetService<IIdentityManagementService2>().ReadIdentity(IdentitySearchFactor.AccountName, projectId + "\\" + id, MembershipQuery.None, ReadIdentityOptions.ExtendedProperties, filters, IdentityPropertyScope.Local)), teamName, propertyNameFilters);
    }

    public Guid GetDefaultTeamId(string projectUri)
    {
      ArgumentUtility.CheckForNull<string>(projectUri, nameof (projectUri));
      ProjectProperty projectProperty = this.m_tfs.GetService<ICommonStructureService4>().GetProjectProperty(projectUri, "Microsoft.TeamFoundation.Team.Default");
      return projectProperty != null && !string.IsNullOrEmpty(projectProperty.Value) ? new Guid(projectProperty.Value) : Guid.Empty;
    }

    public TeamFoundationTeam GetDefaultTeam(string projectUri, List<string> propertyNameFilters)
    {
      Guid defaultTeamId = this.GetDefaultTeamId(projectUri);
      TeamFoundationTeam defaultTeam = (TeamFoundationTeam) null;
      if (defaultTeamId != Guid.Empty)
        defaultTeam = this.ReadTeam(defaultTeamId, propertyNameFilters);
      return defaultTeam;
    }

    public void SetDefaultTeam(TeamFoundationTeam team)
    {
      ArgumentUtility.CheckForNull<TeamFoundationTeam>(team, nameof (team));
      this.SetDefaultTeamId(team.Project, team.Identity.TeamFoundationId);
    }

    public void SetDefaultTeamId(string projectUri, Guid teamId)
    {
      ArgumentUtility.CheckForNull<string>(projectUri, nameof (projectUri));
      this.m_tfs.GetService<ICommonStructureService4>().SetProjectProperty(projectUri, "Microsoft.TeamFoundation.Team.Default", teamId.ToString());
    }

    public void UpdateTeam(TeamFoundationTeam team)
    {
      IIdentityManagementService2 service = this.m_tfs.GetService<IIdentityManagementService2>();
      if (team.IsNameDirty)
        service.UpdateApplicationGroup(team.Identity.Descriptor, GroupProperty.Name, team.Name);
      if (team.IsDescriptionDirty)
        service.UpdateApplicationGroup(team.Identity.Descriptor, GroupProperty.Description, team.Description);
      service.UpdateExtendedProperties(team.Identity);
    }
  }
}
