// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GvfsAuthHandler
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Web;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class GvfsAuthHandler : GvfsHttpHandler
  {
    private static readonly TimeSpan s_defaultAuthorizationLifetime = TimeSpan.FromHours(3.0);
    private static readonly TimeSpan s_unauthorizedLifetime = TimeSpan.FromMinutes(1.0);
    private static readonly RegistryQuery s_gvfsAuthLifeQuery = new RegistryQuery("/Service/Git/Settings/GVFS/AuthorizationLifetime", false);

    public GvfsAuthHandler()
    {
    }

    public GvfsAuthHandler(HttpContextBase context)
      : base(context)
    {
    }

    protected override string Layer => nameof (GvfsAuthHandler);

    protected override TimeSpan Timeout => TimeSpan.FromSeconds(30.0);

    internal override void ProcessGet(RepoNameKey nameKey)
    {
      GvfsAuthResponse gvfsAuthResponse = new GvfsAuthResponse()
      {
        UserId = this.RequestContext.GetUserId(),
        UserName = this.RequestContext.AuthenticatedUserName
      };
      ITfsGitRepository repository;
      int num = this.RequestContext.GetService<ITeamFoundationGitRepositoryService>().TryFindRepositoryByName(this.RequestContext, nameKey.ProjectName, nameKey.RepositoryName, out repository) ? 1 : 0;
      repository?.Dispose();
      if (num != 0)
      {
        gvfsAuthResponse.UserIsAuthorized = true;
        TimeSpan timeSpan = this.RequestContext.GetService<IVssRegistryService>().GetValue<TimeSpan>(this.RequestContext, in GvfsAuthHandler.s_gvfsAuthLifeQuery, GvfsAuthHandler.s_defaultAuthorizationLifetime);
        gvfsAuthResponse.ExpireDate = DateTimeOffset.UtcNow.Add(timeSpan);
      }
      else
      {
        gvfsAuthResponse.UserIsAuthorized = false;
        gvfsAuthResponse.ExpireDate = DateTimeOffset.UtcNow.Add(GvfsAuthHandler.s_unauthorizedLifetime);
      }
      this.HandlerHttpContext.Response.Write(gvfsAuthResponse.Serialize<GvfsAuthResponse>());
    }
  }
}
