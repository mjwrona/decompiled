// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GvfsConfigHandler
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Microsoft.TeamFoundation.Git.Server
{
  [GvfsPublicProjectRequestRestrictions]
  internal class GvfsConfigHandler : GvfsHttpHandler
  {
    private const string c_layer = "GvfsConfigHandler";

    public GvfsConfigHandler()
    {
    }

    public GvfsConfigHandler(HttpContextBase context)
      : base(context)
    {
    }

    protected override string Layer => nameof (GvfsConfigHandler);

    protected override TimeSpan Timeout => TimeSpan.FromSeconds(30.0);

    internal override void ProcessGet(RepoNameKey nameKey)
    {
      ITeamFoundationGitRepositoryService service1 = this.RequestContext.GetService<ITeamFoundationGitRepositoryService>();
      using (ITfsGitRepository repo = service1.FindRepositoryByName(this.RequestContext, nameKey.ProjectName, nameKey.RepositoryName))
      {
        string repoGuid = repo.Key.RepoId.ToString();
        ITeamFoundationProxyService service2 = this.RequestContext.GetService<ITeamFoundationProxyService>();
        IEnumerable<CacheServerInfo> cacheServerInfos;
        try
        {
          IEnumerable<Microsoft.TeamFoundation.Core.WebApi.Proxy> source = service2.QueryProxies(this.RequestContext, (IList<string>) new List<string>()).Where<Microsoft.TeamFoundation.Core.WebApi.Proxy>((Func<Microsoft.TeamFoundation.Core.WebApi.Proxy, bool>) (proxy => !string.IsNullOrWhiteSpace(proxy.FriendlyName))).Where<Microsoft.TeamFoundation.Core.WebApi.Proxy>((Func<Microsoft.TeamFoundation.Core.WebApi.Proxy, bool>) (proxy => proxy.Url.EndsWith(repoGuid, StringComparison.OrdinalIgnoreCase)));
          cacheServerInfos = !source.Any<Microsoft.TeamFoundation.Core.WebApi.Proxy>((Func<Microsoft.TeamFoundation.Core.WebApi.Proxy, bool>) (p =>
          {
            if (!this.SiteEqualsRepoId(p, repo.Key.RepoId))
              return false;
            bool? siteDefault = p.SiteDefault;
            bool flag = true;
            return siteDefault.GetValueOrDefault() == flag & siteDefault.HasValue;
          })) ? source.Select<Microsoft.TeamFoundation.Core.WebApi.Proxy, CacheServerInfo>((Func<Microsoft.TeamFoundation.Core.WebApi.Proxy, CacheServerInfo>) (proxy => new CacheServerInfo(proxy.Url, proxy.FriendlyName, proxy.GlobalDefault.HasValue && proxy.GlobalDefault.Value))) : source.Select<Microsoft.TeamFoundation.Core.WebApi.Proxy, CacheServerInfo>((Func<Microsoft.TeamFoundation.Core.WebApi.Proxy, CacheServerInfo>) (proxy => new CacheServerInfo(proxy.Url, proxy.FriendlyName, this.SiteEqualsRepoId(proxy, repo.Key.RepoId) && proxy.SiteDefault.Value)));
        }
        catch (AccessCheckException ex)
        {
          this.RequestContext.TraceCatch(1013841, GitServerUtils.TraceArea, nameof (GvfsConfigHandler), (Exception) ex);
          cacheServerInfos = Enumerable.Empty<CacheServerInfo>();
        }
        this.HandlerHttpContext.Response.ContentType = "application/json";
        this.HandlerHttpContext.Response.Write(new GvfsConfigResponse()
        {
          AllowedGvfsClientVersions = this.GetVersionsOrDefault(repo.Settings.GvfsAllowedVersionRanges),
          CacheServers = cacheServerInfos
        }.Serialize<GvfsConfigResponse>(true));
      }
    }

    private bool SiteEqualsRepoId(Microsoft.TeamFoundation.Core.WebApi.Proxy proxy, Guid repoId)
    {
      Guid result;
      return Guid.TryParse(proxy.Site, out result) && result == repoId;
    }

    private IEnumerable<GvfsAllowedVersionRange> GetVersionsOrDefault(
      IReadOnlyCollection<GvfsAllowedVersionRange> gvfsAllowedVersionRanges)
    {
      if (gvfsAllowedVersionRanges != null && gvfsAllowedVersionRanges.Any<GvfsAllowedVersionRange>())
        return (IEnumerable<GvfsAllowedVersionRange>) gvfsAllowedVersionRanges;
      return (IEnumerable<GvfsAllowedVersionRange>) new GvfsAllowedVersionRange[1]
      {
        new GvfsAllowedVersionRange()
        {
          Min = new GvfsAllowedVersion()
          {
            Major = 0,
            Minor = 0,
            Build = 0,
            Revision = 0
          },
          Max = (GvfsAllowedVersion) null
        }
      };
    }
  }
}
