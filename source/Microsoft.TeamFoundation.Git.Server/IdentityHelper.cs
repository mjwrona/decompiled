// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.IdentityHelper
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class IdentityHelper
  {
    private static IdentityHelper s_instance = new IdentityHelper();

    internal static IdentityHelper Instance
    {
      get => IdentityHelper.s_instance;
      set => IdentityHelper.s_instance = value;
    }

    public virtual TeamFoundationIdentity GetCurrentTeamFoundationIdentity(
      IVssRequestContext requestContext)
    {
      return requestContext.GetService<ITeamFoundationIdentityService>().ReadRequestIdentity(requestContext);
    }

    public virtual string GetDisplayName(IVssRequestContext requestContext, Guid teamFoundationId)
    {
      ITeamFoundationIdentityService service = requestContext.GetService<ITeamFoundationIdentityService>();
      TeamFoundationIdentity userIdentity = this.GetUserIdentity(requestContext, service, teamFoundationId);
      return userIdentity != null ? userIdentity.DisplayName : Resources.Get("DisplayNameForInvalidIdentity");
    }

    public virtual TeamFoundationIdentity GetUserIdentity(
      IVssRequestContext requestContext,
      ITeamFoundationIdentityService identityService,
      Guid teamFoundationId)
    {
      TeamFoundationIdentity[] userIdentities = this.GetUserIdentities(requestContext, identityService, new Guid[1]
      {
        teamFoundationId
      });
      return userIdentities == null ? (TeamFoundationIdentity) null : ((IEnumerable<TeamFoundationIdentity>) userIdentities).FirstOrDefault<TeamFoundationIdentity>();
    }

    public virtual TeamFoundationIdentity[] GetUserIdentities(
      IVssRequestContext requestContext,
      ITeamFoundationIdentityService identityService,
      Guid[] teamFoundationIds)
    {
      return identityService.ReadIdentities(requestContext.Elevate(), teamFoundationIds);
    }

    public virtual IdentityDescriptor GetIdentityDescriptor(
      IVssRequestContext requestContext,
      Guid teamFoundationId)
    {
      ITeamFoundationIdentityService service = requestContext.GetService<ITeamFoundationIdentityService>();
      return this.GetUserIdentity(requestContext, service, teamFoundationId)?.Descriptor;
    }

    public virtual IdentityAndDate GetIdentityAndDate(
      IVssRequestContext requestContext,
      ITeamFoundationIdentityService identityService,
      Guid identityId)
    {
      TeamFoundationIdentity userIdentity = IdentityHelper.Instance.GetUserIdentity(requestContext, identityService, identityId);
      string name = userIdentity != null ? userIdentity.DisplayName : throw new GitIdentityNotFoundException(identityId.ToString());
      string email = identityService.GetPreferredEmailAddress(requestContext, userIdentity.TeamFoundationId) ?? "";
      return new IdentityAndDate(name + " <" + email + ">", name, email, DateTime.UtcNow, new TimeSpan());
    }
  }
}
