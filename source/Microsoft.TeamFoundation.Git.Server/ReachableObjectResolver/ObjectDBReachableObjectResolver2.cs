// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.ReachableObjectResolver.ObjectDBReachableObjectResolver2
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Protocol.UploadPack.Filter;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server.ReachableObjectResolver
{
  internal class ObjectDBReachableObjectResolver2 : IReachableObjectResolver
  {
    private readonly IVssRequestContext m_requestContext;

    public ObjectDBReachableObjectResolver2(IVssRequestContext requestContext) => this.m_requestContext = requestContext;

    public ISet<Sha1Id> Resolve(
      ITfsGitRepository repository,
      ISet<Sha1Id> haves,
      ISet<Sha1Id> wants1,
      ICollection<Sha1Id> shallows,
      GitObjectFilter filter,
      bool wantsPreExpanded,
      out ISet<Sha1Id> foundHaves,
      IObserver<int> statusObserver)
    {
      ArgumentUtility.CheckForNull<ITfsGitRepository>(repository, nameof (repository));
      ArgumentUtility.CheckForNull<ISet<Sha1Id>>(haves, nameof (haves));
      ArgumentUtility.CheckForNull<ISet<Sha1Id>>(wants1, nameof (wants));
      ArgumentUtility.CheckForNull<ICollection<Sha1Id>>(shallows, nameof (shallows));
      HashSet<TfsGitObject> baseHaves;
      HashSet<TfsGitObject> startObjects = GitServerUtils.ExpandHavesAndWants(this.m_requestContext, repository, haves, (IEnumerable<Sha1Id>) wants1, shallows, wantsPreExpanded, out baseHaves);
      ObjectWalker objectWalker = new ObjectWalker();
      ISet<Sha1Id> foundIds = this.CreateHashSet(repository);
      objectWalker.Walk((IEnumerable<TfsGitObject>) baseHaves, (Func<Sha1Id, GitObjectType, bool>) ((id, objectType) => foundIds.Add(id)));
      ISet<Sha1Id> objectsToSend = this.CreateHashSet(repository);
      Func<Sha1Id, GitObjectType, ISet<Sha1Id>, bool> filterAction = (Func<Sha1Id, GitObjectType, ISet<Sha1Id>, bool>) null;
      filterAction = this.m_requestContext.IsFeatureEnabled("Git.DisallowShallowAndPartialClone") ? (Func<Sha1Id, GitObjectType, ISet<Sha1Id>, bool>) ((id, objectType, wants2) => filter.Include(objectType)) : (Func<Sha1Id, GitObjectType, ISet<Sha1Id>, bool>) ((id, objectType, wants3) => filter.Include(id, objectType, wants3));
      objectWalker.Walk((IEnumerable<TfsGitObject>) startObjects, (Func<Sha1Id, GitObjectType, bool>) ((id, objectType) =>
      {
        if (!filterAction(id, objectType, wants1) || foundIds.Contains(id) || !objectsToSend.Add(id))
          return false;
        statusObserver?.OnNext(objectsToSend.Count);
        return true;
      }));
      objectsToSend = filter.Filter(objectsToSend, wants1, repository);
      statusObserver?.OnNext(objectsToSend.Count);
      statusObserver?.OnCompleted();
      foundHaves = foundIds;
      return objectsToSend;
    }

    private ISet<Sha1Id> CreateHashSet(ITfsGitRepository repo) => repo.OdbSettings.UseShardedHashsetForROR ? (ISet<Sha1Id>) new ShardedHashSet<Sha1Id>() : (ISet<Sha1Id>) new HashSet<Sha1Id>();
  }
}
