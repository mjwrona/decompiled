// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.WikiGitHelper
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebServer;
using System;
using System.IO;
using System.Linq;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  public static class WikiGitHelper
  {
    public static string GetWikiItemGitObjectId(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitVersionDescriptor version,
      string itemPath)
    {
      string wikiItemGitObjectId = (string) null;
      GitItemDescriptor itemDescriptor = new GitItemDescriptor()
      {
        Path = itemPath,
        RecursionLevel = VersionControlRecursionType.None,
        Version = version.Version,
        VersionType = version.VersionType
      };
      try
      {
        GitItem gitItem = GitItemUtility.RetrieveItemModel(requestContext, (UrlHelper) null, repository, itemDescriptor);
        if (gitItem != null)
        {
          if (string.Equals(itemPath, gitItem.Path))
            wikiItemGitObjectId = gitItem.ObjectId;
        }
      }
      catch (GitItemNotFoundException ex)
      {
      }
      return wikiItemGitObjectId;
    }

    public static VersionedPageContent GetVersionedWikiPageContent(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitVersionDescriptor versionDescriptor,
      string pagePath)
    {
      GitItemDescriptor itemDescriptor = new GitItemDescriptor()
      {
        Path = pagePath,
        RecursionLevel = VersionControlRecursionType.None,
        Version = versionDescriptor.Version,
        VersionType = versionDescriptor.VersionType
      };
      try
      {
        GitItem gitItem = GitItemUtility.RetrieveItemModels(requestContext, (UrlHelper) null, repository, itemDescriptor, true, false, long.MaxValue).FirstOrDefault<GitItem>();
        if (gitItem != null)
        {
          using (Stream fileContentStream = GitFileUtility.GetFileContentStream(repository, new Sha1Id(gitItem.ObjectId)))
            return new VersionedPageContent()
            {
              Content = VersionControlFileReader.ReadFileContent(fileContentStream, gitItem.ContentMetadata.Encoding),
              Version = gitItem.ObjectId.ToString()
            };
        }
      }
      catch (Exception ex)
      {
      }
      return (VersionedPageContent) null;
    }

    public static string GetVersionString(GitVersionDescriptor versionDescriptor) => string.Format("refs/heads/" + versionDescriptor.Version);
  }
}
