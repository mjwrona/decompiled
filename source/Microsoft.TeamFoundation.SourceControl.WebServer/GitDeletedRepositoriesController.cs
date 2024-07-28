// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitDeletedRepositoriesController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  [ClientGroupByResource("Repositories")]
  public class GitDeletedRepositoriesController : GitApiController
  {
    [HttpGet]
    [ClientLocationId("2B6869C4-CB25-42B5-B7A3-0D3E6BE0A11A")]
    [ClientResponseType(typeof (IEnumerable<GitDeletedRepository>), null, null)]
    public HttpResponseMessage GetDeletedRepositories()
    {
      IList<TfsGitDeletedRepositoryInfo> source = this.TfsRequestContext.GetService<ITeamFoundationGitRepositoryService>().QueryDeletedRepositories(this.TfsRequestContext, new Guid?(this.ProjectId));
      Dictionary<Guid, TeamFoundationIdentity> guidToIdentity = new Dictionary<Guid, TeamFoundationIdentity>();
      IdentityLookupHelper.LoadIdentities(this.TfsRequestContext, source.Select<TfsGitDeletedRepositoryInfo, Guid>((Func<TfsGitDeletedRepositoryInfo, Guid>) (r => r.DeletedBy)), (IDictionary<Guid, TeamFoundationIdentity>) guidToIdentity);
      IList<GitDeletedRepository> collection = (IList<GitDeletedRepository>) new List<GitDeletedRepository>();
      foreach (TfsGitDeletedRepositoryInfo repo in (IEnumerable<TfsGitDeletedRepositoryInfo>) source)
        collection.Add(repo.ToWebApiItem(this.TfsRequestContext, (IDictionary<Guid, TeamFoundationIdentity>) guidToIdentity, this.ProjectInfo));
      return this.GenerateResponse<GitDeletedRepository>((IEnumerable<GitDeletedRepository>) collection);
    }
  }
}
