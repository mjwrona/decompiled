// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Types.Team.ITeamService
// Assembly: Microsoft.TeamFoundation.Server.Types, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC707FA3-32BF-41E4-BD8A-1BB971125382
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Types.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.Types.Team
{
  [DefaultServiceImplementation("Microsoft.TeamFoundation.Server.Core.Team.TeamService, Microsoft.TeamFoundation.Server.Core")]
  public interface ITeamService : IVssFrameworkService
  {
    WebApiTeam CreateTeam(
      IVssRequestContext requestContext,
      string projectUri,
      string name,
      string description);

    void UpdateTeam(
      IVssRequestContext requestContext,
      Guid projectGuid,
      Guid teamGuid,
      Microsoft.TeamFoundation.Core.WebApi.Team.UpdateTeam newTeamProperties);

    void DeleteTeam(IVssRequestContext requestContext, Guid teamId);

    IReadOnlyCollection<WebApiTeam> QueryTeamsInProject(
      IVssRequestContext requestContext,
      Guid projectId);

    IReadOnlyCollection<WebApiTeam> QueryAllTeamsInCollection(IVssRequestContext requestContext);

    IReadOnlyCollection<WebApiTeam> QueryMyTeamsInProject(
      IVssRequestContext requestContext,
      IdentityDescriptor userIdentityDescriptor,
      Guid projectId);

    IReadOnlyCollection<WebApiTeam> QueryMyTeamsInCollection(
      IVssRequestContext requestContext,
      IdentityDescriptor userIdentityDescriptor);

    int? GetTeamCountByProject(IVssRequestContext requestContext, Guid projectId);

    WebApiTeam GetTeamInProject(
      IVssRequestContext requestContext,
      Guid projectGuid,
      string teamIdOrName);

    WebApiTeam GetTeamByIdentityDescriptor(
      IVssRequestContext requestContext,
      IdentityDescriptor identityDescriptor);

    WebApiTeam GetTeamByGuid(IVssRequestContext requestContext, Guid teamGuid);

    IReadOnlyCollection<WebApiTeam> GetTeamsByGuid(
      IVssRequestContext requestContext,
      IEnumerable<Guid> teamIds);

    IReadOnlyCollection<Microsoft.VisualStudio.Services.Identity.Identity> ReadTeamMembers(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity teamIdentity,
      MembershipQuery queryMembership);

    void SetTeamImage(IVssRequestContext requestContext, Guid teamId, byte[] imageBytes);

    void RemoveTeamImage(IVssRequestContext requestContext, Guid teamId);

    (int teamCount, int teamsLimit) GetTeamCountAndLimitByProject(
      IVssRequestContext requestContext,
      Guid projectId);

    Guid GetDefaultTeamId(IVssRequestContext requestContext, Guid projectId);

    void SetDefaultTeamId(IVssRequestContext requestContext, Guid projectId, Guid teamId);

    WebApiTeam GetDefaultTeam(IVssRequestContext requestContext, Guid projectId);

    bool IsDefaultTeam(IVssRequestContext requestContext, Guid teamId);

    ISecuredObject GetSecuredTeamObject(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity teamIdentity);

    WebApiTeam GetTeamWithSecuredObject(
      IVssRequestContext requestContext,
      Guid projectGuid,
      string teamIdOrName);

    bool HasTeamPermission(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity teamIdentity,
      int requestedPermissions,
      bool alwaysAllowAdministrators);

    bool UserIsTeamAdmin(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity teamIdentity);

    IReadOnlyCollection<Microsoft.VisualStudio.Services.Identity.Identity> GetTeamAdmins(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity teamIdentity);

    bool UserHasPermission(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity teamIdentity,
      int permission);

    [Obsolete("Don't use Team to store local properties. This service is only available because we still need to support on-prem migration steps (TeamFavoriteMigrator, AgileStepPerformer)")]
    void SetTeamViewProperty(
      IVssRequestContext requestContext,
      Guid teamGuid,
      string propertyName,
      string propertyValue);

    [Obsolete("Don't use Team to store local properties. This service is only available because we still need to support on-prem migration steps (TeamFavoriteMigrator, AgileStepPerformer)")]
    bool TryGetTeamViewProperty<PropertyType>(
      IVssRequestContext requestContext,
      Guid teamGuid,
      string propertyName,
      out PropertyType propertyValue);

    [Obsolete("Don't use Team to store local properties. This service is only available because we still need to support on-prem migration steps (TeamFavoriteMigrator, AgileStepPerformer)")]
    void DeleteTeamViewProperties(
      IVssRequestContext requestContext,
      Guid teamGuid,
      IReadOnlyCollection<string> propertyNames);
  }
}
