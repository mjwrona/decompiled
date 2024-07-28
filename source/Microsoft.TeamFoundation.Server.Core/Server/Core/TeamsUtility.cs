// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TeamsUtility
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types.Team;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core
{
  public static class TeamsUtility
  {
    public static WebApiTeam GetTeam(
      IVssRequestContext requestContext,
      TeamProject project,
      string teamIdOrName,
      bool setSecuredObject = false)
    {
      ITeamService service = requestContext.GetService<ITeamService>();
      return (!setSecuredObject ? service.GetTeamInProject(requestContext, project.Id, teamIdOrName) : service.GetTeamWithSecuredObject(requestContext, project.Id, teamIdOrName)) ?? throw new Microsoft.Azure.Devops.Teams.Service.TeamNotFoundException(teamIdOrName);
    }

    public static IEnumerable<IdentityRef> GetTeamMembers(
      IVssRequestContext requestContext,
      Guid teamId,
      int top,
      int skip)
    {
      ITeamFoundationIdentityService service = requestContext.GetService<ITeamFoundationIdentityService>();
      TeamFoundationIdentity readIdentity = service.ReadIdentities(requestContext, new Guid[1]
      {
        teamId
      }, MembershipQuery.Expanded, ReadIdentityOptions.None, (IEnumerable<string>) null)[0];
      IdentityDescriptor[] identityDescriptorArray;
      if (readIdentity == null)
      {
        identityDescriptorArray = (IdentityDescriptor[]) null;
      }
      else
      {
        ICollection<IdentityDescriptor> members = readIdentity.Members;
        identityDescriptorArray = members != null ? members.ToArray<IdentityDescriptor>() : (IdentityDescriptor[]) null;
      }
      IdentityDescriptor[] descriptors = identityDescriptorArray;
      return descriptors != null ? ((IEnumerable<TeamFoundationIdentity>) service.ReadIdentities(requestContext, descriptors)).Skip<TeamFoundationIdentity>(skip).Take<TeamFoundationIdentity>(top).Select<TeamFoundationIdentity, IdentityRef>((Func<TeamFoundationIdentity, IdentityRef>) (x => x.ToIdentityRef(requestContext))) : Enumerable.Empty<IdentityRef>();
    }

    public static void CheckTeamName(string name)
    {
      if (!Microsoft.Azure.Devops.Teams.Service.TeamsUtility.IsValidTeamName(name))
        throw new InvalidTeamNameException(FrameworkResources.InvalidTeamName((object) name));
    }

    public static void CheckChangeImagePermissions(Guid teamId, IVssRequestContext requestContext)
    {
      TeamFoundationIdentity readIdentity = requestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(requestContext, new Guid[1]
      {
        teamId
      })[0];
      if (readIdentity == null)
        throw new IdentityNotFoundException(teamId);
      if (readIdentity.IsContainer)
      {
        if (GroupHelpers.IsCurrentHostWellKnownGroup(readIdentity))
          throw new InvalidOperationException(FrameworkResources.CannotChangeWellKnownPicture());
        if (!GroupHelpers.HasManageGroupMembershipPermission(requestContext, readIdentity))
          throw new InvalidOperationException(FrameworkResources.CannotChangePicturePermissions());
      }
      else if (!object.Equals((object) requestContext.GetUserId(), (object) readIdentity.TeamFoundationId))
        throw new InvalidOperationException(FrameworkResources.CannotChangeOtherUserPicture());
    }
  }
}
