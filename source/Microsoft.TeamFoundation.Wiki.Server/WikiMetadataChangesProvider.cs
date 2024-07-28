// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.WikiMetadataChangesProvider
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebServer;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  public class WikiMetadataChangesProvider : IGitChangesProvider<WikiPageChange>
  {
    public IList<GitChange> GetChanges(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitVersionDescriptor versionDescriptor,
      string mappedPath,
      WikiPageChange wikiPageChange)
    {
      if (wikiPageChange == null)
        throw new InvalidArgumentValueException(nameof (wikiPageChange), Resources.InvalidParametersOrNull);
      IList<GitChange> changes = (IList<GitChange>) new List<GitChange>();
      WikiPageChange pageChange = wikiPageChange;
      string orderingFileContent1 = string.Empty;
      GitPath orderingFilePath1 = (GitPath) null;
      bool createNew1 = false;
      string orderingFileContent2 = string.Empty;
      GitPath orderingFilePath2 = (GitPath) null;
      bool createNew2 = false;
      IWikiPagesOrderProvider pagesOrderProvider = (IWikiPagesOrderProvider) new WikiPagesOrderProvider();
      using (WikiPagesOrderReader pagesOrderReader = new WikiPagesOrderReader())
      {
        switch (pageChange.ChangeType)
        {
          case WikiChangeType.Add:
            orderingFileContent2 = pagesOrderProvider.AddPageOrder(requestContext, repository, versionDescriptor, (IWikiPagesOrderReader) pagesOrderReader, pageChange.Path, pageChange.NewOrder.Value, mappedPath, out orderingFilePath2, out createNew2);
            break;
          case WikiChangeType.Delete:
            orderingFileContent2 = pagesOrderProvider.RemovePageOrder(requestContext, repository, versionDescriptor, (IWikiPagesOrderReader) pagesOrderReader, pageChange.Path, mappedPath, out orderingFilePath2, out createNew2);
            break;
          case WikiChangeType.Rename:
            if (this.IsTitleOnlyChange(pageChange))
            {
              orderingFileContent2 = pagesOrderProvider.ModifyPageTitle(requestContext, repository, versionDescriptor, (IWikiPagesOrderReader) pagesOrderReader, pageChange.NewPath, pageChange.Path, mappedPath, out orderingFilePath2, out createNew2);
              break;
            }
            orderingFileContent1 = pagesOrderProvider.RemovePageOrder(requestContext, repository, versionDescriptor, (IWikiPagesOrderReader) pagesOrderReader, pageChange.Path, mappedPath, out orderingFilePath1, out createNew1);
            orderingFileContent2 = pagesOrderProvider.AddPageOrder(requestContext, repository, versionDescriptor, (IWikiPagesOrderReader) pagesOrderReader, pageChange.NewPath, pageChange.NewOrder.Value, mappedPath, out orderingFilePath2, out createNew2);
            break;
          case WikiChangeType.Reorder:
            orderingFileContent2 = pagesOrderProvider.ModifyPageOrder(requestContext, repository, versionDescriptor, (IWikiPagesOrderReader) pagesOrderReader, pageChange.Path, pageChange.NewOrder.Value, mappedPath, out orderingFilePath2, out createNew2);
            break;
        }
      }
      if (orderingFilePath1 != null)
        changes.Add(this.GetOrderingFileChange(orderingFilePath1, orderingFileContent1, createNew1));
      if (orderingFilePath2 != null)
        changes.Add(this.GetOrderingFileChange(orderingFilePath2, orderingFileContent2, createNew2));
      return changes;
    }

    private bool IsTitleOnlyChange(WikiPageChange pageChange) => pageChange.Path != null && pageChange.NewPath != null && pageChange.Path.GetParentPagePath().Equals((object) pageChange.NewPath.GetParentPagePath());

    private GitChange GetOrderingFileChange(
      GitPath orderingFilePath,
      string orderingFileContent,
      bool createNew)
    {
      return !createNew && string.IsNullOrEmpty(orderingFileContent) ? this.GetDeleteChange(orderingFilePath) : this.GetAddOrEditChange(orderingFilePath, orderingFileContent, createNew);
    }

    private GitChange GetDeleteChange(GitPath orderingFilePath)
    {
      GitChange deleteChange = new GitChange();
      deleteChange.ChangeType = VersionControlChangeType.Delete;
      GitItem gitItem = new GitItem();
      gitItem.Path = orderingFilePath.Path;
      deleteChange.Item = gitItem;
      return deleteChange;
    }

    private GitChange GetAddOrEditChange(
      GitPath orderingFilePath,
      string orderingFileContent,
      bool createNew)
    {
      GitChange addOrEditChange = new GitChange();
      addOrEditChange.ChangeType = createNew ? VersionControlChangeType.Add : VersionControlChangeType.Edit;
      GitItem gitItem = new GitItem();
      gitItem.Path = orderingFilePath.Path;
      addOrEditChange.Item = gitItem;
      addOrEditChange.NewContent = new ItemContent()
      {
        Content = orderingFileContent,
        ContentType = ItemContentType.RawText
      };
      return addOrEditChange;
    }
  }
}
