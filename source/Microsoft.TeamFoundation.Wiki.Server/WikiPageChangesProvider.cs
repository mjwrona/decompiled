// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.WikiPageChangesProvider
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebServer;
using Microsoft.TeamFoundation.Wiki.WebApi;
using System.Collections.Generic;
using System.IO;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  public class WikiPageChangesProvider : IGitChangesProvider<WikiPageChange>
  {
    public IList<GitChange> GetChanges(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitVersionDescriptor versionDescriptor,
      string mappedPath,
      WikiPageChange wikiPageChange)
    {
      WikiPagePath wikiPagePath = wikiPageChange != null ? wikiPageChange.Path : throw new InvalidArgumentValueException("pagePath", Resources.InvalidParametersOrNull);
      GitPath gitPath1 = GitPath.GitMdPathFromWikiPagePath(wikiPagePath, mappedPath);
      GitPath gitPath2 = gitPath1.GitEquivalentFolderPath();
      WikiPagePath newPath = wikiPageChange.NewPath;
      GitPath gitPath3 = (GitPath) null;
      GitPath newGitPath = (GitPath) null;
      if (newPath != null)
      {
        gitPath3 = GitPath.GitMdPathFromWikiPagePath(newPath, mappedPath);
        newGitPath = gitPath3.GitEquivalentFolderPath();
      }
      else if (wikiPageChange.ChangeType == WikiChangeType.Rename)
        throw new InvalidArgumentValueException("wikiPageChange.NewPath", "NewPath cannot be null for Rename operation");
      WikiPage pageIfExisting = this.GetPageIfExisting(requestContext, repository, versionDescriptor, mappedPath, wikiPagePath);
      this.PreProcessWikiPageChange(requestContext, repository, versionDescriptor, wikiPageChange.ChangeType, wikiPagePath, newPath, mappedPath, pageIfExisting);
      IList<GitChange> changes = (IList<GitChange>) new List<GitChange>();
      WikiPageChange wikiPageChange1 = wikiPageChange;
      bool flag = true;
      VersionControlChangeType controlChangeType;
      switch (wikiPageChange1.ChangeType)
      {
        case WikiChangeType.Add:
          controlChangeType = VersionControlChangeType.Add;
          break;
        case WikiChangeType.Delete:
          if (PageHelper.IsPageWithoutAssociatedContent(pageIfExisting))
            flag = false;
          controlChangeType = VersionControlChangeType.Delete;
          GitChange folderDeleteChange = this.GetSiblingFolderDeleteChange(requestContext, repository, versionDescriptor, gitPath2);
          if (folderDeleteChange != null)
          {
            changes.Add(folderDeleteChange);
            break;
          }
          break;
        case WikiChangeType.Edit:
          if (PageHelper.IsPageWithoutAssociatedContent(pageIfExisting))
            flag = false;
          controlChangeType = VersionControlChangeType.Edit;
          break;
        case WikiChangeType.Rename:
          if (PageHelper.IsPageWithoutAssociatedContent(pageIfExisting))
            flag = false;
          if (wikiPageChange1.Path.Equals((object) wikiPageChange1.NewPath))
          {
            controlChangeType = VersionControlChangeType.None;
            break;
          }
          controlChangeType = VersionControlChangeType.Rename;
          GitChange folderRenameChange = this.GetSiblingFolderRenameChange(requestContext, repository, versionDescriptor, gitPath2, newGitPath);
          if (folderRenameChange != null)
          {
            changes.Add(folderRenameChange);
            break;
          }
          break;
        default:
          controlChangeType = VersionControlChangeType.None;
          break;
      }
      if (controlChangeType != VersionControlChangeType.None && flag)
      {
        string path;
        string str;
        if (controlChangeType == VersionControlChangeType.Rename)
        {
          path = gitPath3.Path;
          str = gitPath1.Path;
        }
        else
        {
          path = gitPath1.Path;
          str = (string) null;
        }
        IList<GitChange> gitChangeList = changes;
        GitChange gitChange = new GitChange();
        gitChange.ChangeType = controlChangeType;
        GitItem gitItem = new GitItem();
        gitItem.Path = path;
        gitChange.Item = gitItem;
        gitChange.NewContent = new ItemContent()
        {
          Content = wikiPageChange1.Content,
          ContentType = ItemContentType.RawText
        };
        gitChange.SourceServerItem = str;
        gitChangeList.Add(gitChange);
      }
      return changes;
    }

    private GitChange GetSiblingFolderRenameChange(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitVersionDescriptor versionDescriptor,
      GitPath originalGitPath,
      GitPath newGitPath)
    {
      if (!this.DoesSiblingFolderExist(requestContext, repository, versionDescriptor, originalGitPath))
        return (GitChange) null;
      GitChange folderRenameChange = new GitChange();
      folderRenameChange.ChangeType = VersionControlChangeType.Rename;
      GitItem gitItem = new GitItem();
      gitItem.Path = newGitPath.Path;
      folderRenameChange.Item = gitItem;
      folderRenameChange.SourceServerItem = originalGitPath.Path;
      return folderRenameChange;
    }

    private GitChange GetSiblingFolderDeleteChange(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitVersionDescriptor versionDescriptor,
      GitPath gitPath)
    {
      if (!this.DoesSiblingFolderExist(requestContext, repository, versionDescriptor, gitPath))
        return (GitChange) null;
      GitChange folderDeleteChange = new GitChange();
      folderDeleteChange.ChangeType = VersionControlChangeType.Delete;
      GitItem gitItem = new GitItem();
      gitItem.Path = gitPath.Path;
      folderDeleteChange.Item = gitItem;
      return folderDeleteChange;
    }

    private bool DoesSiblingFolderExist(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitVersionDescriptor versionDescriptor,
      GitPath gitPath)
    {
      try
      {
        GitItemDescriptor itemDescriptor = new GitItemDescriptor()
        {
          Path = gitPath.Path,
          RecursionLevel = VersionControlRecursionType.None,
          Version = versionDescriptor.Version,
          VersionType = versionDescriptor.VersionType,
          VersionOptions = versionDescriptor.VersionOptions
        };
        GitItemUtility.RetrieveItemModel(requestContext, (UrlHelper) null, repository, itemDescriptor);
        return true;
      }
      catch (GitItemNotFoundException ex)
      {
        return false;
      }
    }

    private void PreProcessWikiPageChange(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitVersionDescriptor versionDescriptor,
      WikiChangeType wikiChangeType,
      WikiPagePath currentWikiPagePath,
      WikiPagePath newWikiPagePath,
      string mappedPath,
      WikiPage existingPage)
    {
      bool isPathLengthCheckNeeded = wikiChangeType != WikiChangeType.Delete && wikiChangeType != WikiChangeType.Rename;
      this.ValidatePagePath(currentWikiPagePath.Path, isPathLengthCheckNeeded);
      switch (wikiChangeType)
      {
        case WikiChangeType.Add:
          if (this.AreAncestorsExisting(requestContext, repository, versionDescriptor, mappedPath, currentWikiPagePath))
            break;
          throw new WikiAncestorPageNotFoundException(string.Format(Resources.ErrorMessageAncestorPageNotFound, (object) currentWikiPagePath));
        case WikiChangeType.Delete:
        case WikiChangeType.Reorder:
          if (existingPage != null)
            break;
          throw new WikiPageNotFoundException(string.Format(Resources.ErrorMessage_PageNotFound, (object) currentWikiPagePath));
        case WikiChangeType.Edit:
          if (existingPage == null)
            throw new WikiPageNotFoundException(string.Format(Resources.ErrorMessage_PageNotFound, (object) currentWikiPagePath));
          if (!PageHelper.IsPageWithoutAssociatedContent(existingPage))
            break;
          throw new WikiPageContentUnavailableException(string.Format(Resources.ErrorMessagePageContentUnavailable, (object) currentWikiPagePath, (object) "/"));
        case WikiChangeType.Rename:
          this.ValidatePagePath(newWikiPagePath?.Path, true);
          if (!this.AreAncestorsExisting(requestContext, repository, versionDescriptor, mappedPath, newWikiPagePath))
            throw new WikiAncestorPageNotFoundException(string.Format(Resources.ErrorMessageAncestorPageNotFound, (object) newWikiPagePath));
          if (existingPage != null)
            break;
          throw new WikiPageRenameSourceNotFoundException(string.Format(Resources.ErrorMessagePageRenameSourceNotFound, (object) currentWikiPagePath));
      }
    }

    private void ValidatePagePath(string path, bool isPathLengthCheckNeeded)
    {
      string pageName = !string.IsNullOrEmpty(path) ? path.Substring(path.LastIndexOf("/") + 1) : string.Empty;
      if (string.IsNullOrEmpty(pageName))
        throw new InvalidArgumentValueException(nameof (path), Resources.WikiPagePathOrNameIsNullOrEmpty);
      if (!PathHelper.IsValidPageName(pageName))
        throw new InvalidArgumentValueException("pageName", string.Format(Resources.WikiPageNameIsInvalid, (object) string.Join(", ", PathConstants.ResourceNameInvalidCharacters)));
      if (isPathLengthCheckNeeded && path.Length > 235)
        throw new PathTooLongException(string.Format(Resources.WikiPagePathTooLong, (object) 235));
    }

    private bool AreAncestorsExisting(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitVersionDescriptor versionDescriptor,
      string mappedPath,
      WikiPagePath wikiPagePath)
    {
      if (wikiPagePath.IsWikiRootPage())
        return true;
      for (WikiPagePath parentPagePath = wikiPagePath.GetParentPagePath(); parentPagePath != null && !wikiPagePath.IsWikiRootPage(); parentPagePath = parentPagePath.GetParentPagePath())
      {
        if (this.GetPageIfExisting(requestContext, repository, versionDescriptor, mappedPath, parentPagePath) == null)
          return false;
      }
      return true;
    }

    private WikiPage GetPageIfExisting(
      IVssRequestContext requestContext,
      ITfsGitRepository repository,
      GitVersionDescriptor versionDescriptor,
      string mappedPath,
      WikiPagePath wikiPagePath)
    {
      WikiPagesProvider wikiPagesProvider = new WikiPagesProvider();
      try
      {
        return wikiPagesProvider.GetPage(requestContext, repository, repository.Key.RepoId, mappedPath, wikiPagePath, versionDescriptor, (IWikiPagesOrderReader) new WikiPagesOrderReader());
      }
      catch (WikiPageNotFoundException ex)
      {
        return (WikiPage) null;
      }
    }
  }
}
