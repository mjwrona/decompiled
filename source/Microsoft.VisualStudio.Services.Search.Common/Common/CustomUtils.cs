// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.CustomUtils
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public static class CustomUtils
  {
    public static string GetBranchNameWithoutPrefix(string branchRefPrefix, string branchName)
    {
      if (branchRefPrefix == null)
        throw new ArgumentNullException(nameof (branchRefPrefix));
      string nameWithoutPrefix = branchName != null ? branchName : throw new ArgumentNullException(nameof (branchName));
      if (branchName.StartsWith(branchRefPrefix, StringComparison.OrdinalIgnoreCase))
        nameWithoutPrefix = nameWithoutPrefix.Substring(branchRefPrefix.Length);
      return nameWithoutPrefix;
    }

    public static string GetRepoWebUrlFromRepo(GitRepository repo)
    {
      if (repo == null)
        throw new ArgumentNullException(nameof (repo));
      if (repo.Links == null)
        throw new ArgumentNullException(FormattableString.Invariant(FormattableStringFactory.Create("{0}.{1}", (object) nameof (repo), (object) repo.Links)));
      string href = repo.Links.Links["web"] is ReferenceLink link ? link.Href : (string) null;
      ArgumentUtility.CheckForNull<string>(href, "repoWebUrl");
      return new UriBuilder(href) { UserName = "" }.Uri.AbsoluteUri;
    }
  }
}
