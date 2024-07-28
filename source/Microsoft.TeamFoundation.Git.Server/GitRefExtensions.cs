// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitRefExtensions
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal static class GitRefExtensions
  {
    public static TfsGitRef GetDefaultOrAny(this IEnumerable<TfsGitRef> refs)
    {
      TfsGitRef defaultOrAny1 = (TfsGitRef) null;
      foreach (TfsGitRef defaultOrAny2 in refs)
      {
        if (defaultOrAny2.IsDefaultBranch)
          return defaultOrAny2;
        if (defaultOrAny1 == null || defaultOrAny2.Name.Equals("refs/heads/master"))
          defaultOrAny1 = defaultOrAny2;
      }
      return defaultOrAny1;
    }
  }
}
