// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.WebApiBatchConverter
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public class WebApiBatchConverter
  {
    private IVssRequestContext context;
    private IdentityMap identities;
    private AgentPoolQueueCache queues;
    private BuildRepositoryCache repos;
    private bool resolveExternalIdentities;

    public WebApiBatchConverter(IVssRequestContext requestContext, bool resolveExternalIdentities)
    {
      this.context = requestContext;
      this.resolveExternalIdentities = resolveExternalIdentities;
      this.identities = new IdentityMap(requestContext.GetService<IdentityService>());
      this.queues = new AgentPoolQueueCache(requestContext);
      this.repos = new BuildRepositoryCache(requestContext);
    }

    public Microsoft.TeamFoundation.Build.WebApi.BuildDefinition ConvertBuildDefinition(
      Microsoft.TeamFoundation.Build2.Server.BuildDefinition source,
      Version apiVersion,
      bool getRepositoryNames = false)
    {
      return source.ToWebApiBuildDefinition(this.context, apiVersion, this.identities, this.queues, this.repos, false, getRepositoryNames);
    }

    public Microsoft.TeamFoundation.Build.WebApi.Build ConvertBuild(
      BuildData source,
      Version apiVersion,
      bool updateReferencesAndLinks = false)
    {
      return source.ToWebApiBuild(this.context, apiVersion, this.identities, this.queues, this.repos, updateReferencesAndLinks, resolveExternalIdentities: this.resolveExternalIdentities);
    }

    public void PrimeRepos(
      IEnumerable<Tuple<Guid, Microsoft.TeamFoundation.Build2.Server.BuildRepository>> loadedRepos)
    {
      this.repos.PrimeCache(loadedRepos);
    }
  }
}
