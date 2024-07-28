// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.GitChangesProvider
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  public class GitChangesProvider : IGitChangesProvider<WikiPageChange>
  {
    public IList<GitChange> GetChanges(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitVersionDescriptor versionDescriptor,
      string mappedPath,
      WikiPageChange wikiPageChange)
    {
      List<GitChange> source = new List<GitChange>();
      if (wikiPageChange != null)
      {
        source.AddRange((IEnumerable<GitChange>) new WikiPageChangesProvider().GetChanges(requestContext, repository, versionDescriptor, mappedPath, wikiPageChange));
        source.AddRange((IEnumerable<GitChange>) new WikiMetadataChangesProvider().GetChanges(requestContext, repository, versionDescriptor, mappedPath, wikiPageChange));
      }
      return (IList<GitChange>) source.Where<GitChange>((Func<GitChange, bool>) (change => change != null)).ToList<GitChange>();
    }
  }
}
