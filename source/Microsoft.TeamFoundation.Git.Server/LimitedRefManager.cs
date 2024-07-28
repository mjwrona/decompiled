// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.LimitedRefManager
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class LimitedRefManager
  {
    private readonly ITfsGitRepository m_repository;
    private readonly IVssRequestContext m_requestContext;
    private readonly ISecurityHelper m_securityHelper;
    private readonly Func<List<string>, List<string>, TfsGitLimitedRefCriteria> m_updateCriteria;

    public LimitedRefManager(IVssRequestContext requestContext, ITfsGitRepository repository)
      : this(requestContext, repository, SecurityHelper.Instance)
    {
    }

    internal LimitedRefManager(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      ISecurityHelper securityHelper,
      Func<List<string>, List<string>, TfsGitLimitedRefCriteria> updateCriteria = null)
    {
      this.m_repository = repository;
      this.m_requestContext = requestContext;
      this.m_securityHelper = securityHelper;
      this.m_updateCriteria = updateCriteria ?? new Func<List<string>, List<string>, TfsGitLimitedRefCriteria>(this.UpdateLimitedRefCriteriaInDB);
    }

    public TfsGitLimitedRefCriteria UpdateLimitedRefCriteria(
      List<string> refExactMatch,
      List<string> refNamespaces)
    {
      refExactMatch = refExactMatch ?? new List<string>();
      refNamespaces = refNamespaces ?? new List<string>();
      LimitedRefManager.ValidateLimitedRefCriteria(refExactMatch, refNamespaces);
      this.m_securityHelper.CheckEditPoliciesPermission(this.m_requestContext, (RepoScope) this.m_repository.Key);
      return this.m_updateCriteria(refExactMatch, refNamespaces);
    }

    public TfsGitLimitedRefCriteria GetLimitedRefCriteria()
    {
      this.m_securityHelper.CheckReadPermission(this.m_requestContext, (RepoScope) this.m_repository.Key, this.m_repository.Name);
      using (GitCoreComponent gitCoreComponent = this.m_requestContext.CreateGitCoreComponent())
        return gitCoreComponent.ReadLimitedRefCriteria(this.m_repository.Key);
    }

    internal static void ValidateLimitedRefCriteria(
      List<string> refExactMatch,
      List<string> refNamespaces)
    {
      if (refExactMatch.Distinct<string>((IEqualityComparer<string>) StringComparer.Ordinal).Count<string>() != refExactMatch.Count<string>())
        throw new GitInvalidLimitedRefCriteriaException("LimitedRefCriteriaDuplicateExactMatchException");
      for (int index = 0; index < refNamespaces.Count; ++index)
        refNamespaces[index] = refNamespaces[index].EndsWith("/", StringComparison.Ordinal) ? refNamespaces[index] : refNamespaces[index] + "/";
      foreach (string str in refExactMatch)
      {
        foreach (string refNamespace in refNamespaces)
        {
          if (str.StartsWith(refNamespace, StringComparison.Ordinal))
            throw new GitInvalidLimitedRefCriteriaException(Resources.Format("LimitedRefCriteriaExactEntryInNamespaceException", (object) str, (object) refNamespace));
        }
      }
      foreach (string refNamespace1 in refNamespaces)
      {
        foreach (string refNamespace2 in refNamespaces)
        {
          if (!string.Equals(refNamespace1, refNamespace2, StringComparison.Ordinal) && refNamespace1.StartsWith(refNamespace2, StringComparison.Ordinal))
            throw new GitInvalidLimitedRefCriteriaException(Resources.Format("LimitedRefCriteriaNamespacesOverlapException", (object) refNamespace1, (object) refNamespace2));
        }
      }
    }

    private TfsGitLimitedRefCriteria UpdateLimitedRefCriteriaInDB(
      List<string> refExactMatch,
      List<string> refNamespaces)
    {
      using (GitCoreComponent gitCoreComponent = this.m_requestContext.CreateGitCoreComponent())
      {
        gitCoreComponent.UpdateGitLimitedRefCriteria(this.m_repository.Key, (IEnumerable<string>) refExactMatch, (IEnumerable<string>) refNamespaces);
        return gitCoreComponent.ReadLimitedRefCriteria(this.m_repository.Key);
      }
    }
  }
}
