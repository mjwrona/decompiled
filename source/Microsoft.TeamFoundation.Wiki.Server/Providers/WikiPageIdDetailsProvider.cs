// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.Providers.WikiPageIdDetailsProvider
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.Wiki.Server.Contracts;
using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Wiki.Server.Providers
{
  public class WikiPageIdDetailsProvider : IWikiPageIdDetailsProvider
  {
    public const int MinPageId = 1;
    public const int MaxPageId = 2147483647;
    public const int MaxPageBatchSize = 100;
    public const int MinPageBatchSize = 1;
    private readonly TimedCiEvent m_ciEvent;

    public WikiPageIdDetailsProvider(TimedCiEvent mCiEvent = null) => this.m_ciEvent = mCiEvent;

    public virtual WikiPageIdDetails GetPageIdDetails(
      IVssRequestContext requestContext,
      Guid projectId,
      int pageId,
      out WikiV2 wiki)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckBoundsInclusive(pageId, 1, int.MaxValue, nameof (pageId));
      using (new StopWatchHelper(this.m_ciEvent, nameof (GetPageIdDetails)))
      {
        WikiPageIdInfo idDetailsInternal = WikiPageIdDetailsProvider.GetPageIdDetailsInternal(requestContext, projectId, pageId);
        wiki = WikiV2Helper.GetWikiById(requestContext, projectId, idDetailsInternal.WikiId, this.m_ciEvent);
        if (wiki == null)
          throw new WikiPageNotFoundException(string.Format(Resources.WikiPageIdNotFound, (object) pageId));
        return new WikiPageIdDetails(idDetailsInternal);
      }
    }

    public WikiPageIdDetails GetPageIdDetails(
      IVssRequestContext requestContext,
      Guid projectId,
      WikiV2 wiki,
      int pageId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNull<WikiV2>(wiki, nameof (wiki));
      ArgumentUtility.CheckForEmptyGuid(wiki.Id, "wiki.Id");
      ArgumentUtility.CheckBoundsInclusive(pageId, 1, int.MaxValue, nameof (pageId));
      using (new StopWatchHelper(this.m_ciEvent, nameof (GetPageIdDetails)))
      {
        WikiPageIdInfo idDetailsInternal = WikiPageIdDetailsProvider.GetPageIdDetailsInternal(requestContext, projectId, pageId);
        if (idDetailsInternal.WikiId != wiki.Id)
          throw new WikiPageNotFoundException(string.Format(Resources.WikiPageIdNotFound, (object) pageId));
        return new WikiPageIdDetails(idDetailsInternal);
      }
    }

    public IList<WikiPageIdDetails> GetPageIdDetails(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid wikiId,
      GitVersionDescriptor wikiVersion,
      int? afterPageId,
      int batchSize)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForEmptyGuid(wikiId, nameof (wikiId));
      if (batchSize < 1)
        throw new NotSupportedException(string.Format("Minimum Batch Size should be :{0}", (object) 1));
      if (batchSize > 100)
        throw new NotSupportedException(string.Format("Maximum Batch Size should be :{0}", (object) 100));
      if (afterPageId.HasValue && afterPageId.Value <= 0)
        throw new NotSupportedException("Minimum Value of AfterPageId should be 0");
      ArgumentUtility.CheckForNull<GitVersionDescriptor>(wikiVersion, nameof (wikiVersion));
      if (wikiVersion.VersionType != GitVersionType.Branch)
        throw new NotSupportedException(string.Format("Only Branches are supported, given version descriptor:{0}", (object) wikiVersion.VersionType));
      ArgumentUtility.CheckStringForNullOrEmpty(wikiVersion.Version, "Wiki Version");
      string versionString = WikiGitHelper.GetVersionString(wikiVersion);
      afterPageId = !afterPageId.HasValue ? new int?(0) : afterPageId;
      List<WikiPageIdDetails> pagesForVersion = new List<WikiPageIdDetails>();
      IList<WikiPageWithId> pages;
      using (WikiComponent component = requestContext.CreateComponent<WikiComponent>())
        pages = component.GetPages(projectId, wikiId, versionString, afterPageId.Value, batchSize);
      if (pages != null)
        pages.ForEach<WikiPageWithId>((Action<WikiPageWithId>) (page => pagesForVersion.Add(new WikiPageIdDetails(page, wikiId, versionString))));
      return WikiPageIdDetailsProvider.GetLatestPageDetails(pagesForVersion);
    }

    public static IEnumerable<KeyValuePair<Guid, int>> ValidatePageIds(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<KeyValuePair<Guid, int>> WikiIdWithPageIds)
    {
      List<KeyValuePair<Guid, int>> keyValuePairList = new List<KeyValuePair<Guid, int>>();
      using (WikiComponent component = requestContext.CreateComponent<WikiComponent>())
      {
        List<int> list = WikiIdWithPageIds.Select<KeyValuePair<Guid, int>, int>((Func<KeyValuePair<Guid, int>, int>) (x => x.Value)).ToList<int>();
        List<WikiPageIdInfo> allPagesByPageIds = component.GetAllPagesByPageIds(projectId, (IEnumerable<int>) list);
        foreach (KeyValuePair<Guid, int> wikiIdWithPageId in WikiIdWithPageIds)
        {
          KeyValuePair<Guid, int> WikiIdWithPageId = wikiIdWithPageId;
          if (allPagesByPageIds.Any<WikiPageIdInfo>((Func<WikiPageIdInfo, bool>) (x => x.WikiId == WikiIdWithPageId.Key && x.PageId == WikiIdWithPageId.Value)))
            keyValuePairList.Add(WikiIdWithPageId);
        }
      }
      return (IEnumerable<KeyValuePair<Guid, int>>) keyValuePairList;
    }

    private static WikiPageIdInfo GetPageIdDetailsInternal(
      IVssRequestContext requestContext,
      Guid projectId,
      int pageId)
    {
      WikiPageIdInfo latestPageById;
      using (WikiComponent component = requestContext.CreateComponent<WikiComponent>())
        latestPageById = component.GetLatestPageById(projectId, pageId);
      return latestPageById != null ? latestPageById : throw new WikiPageNotFoundException(string.Format(Resources.WikiPageIdNotFound, (object) pageId));
    }

    private static IList<WikiPageIdDetails> GetLatestPageDetails(List<WikiPageIdDetails> allPages)
    {
      List<WikiPageIdDetails> latestPageDetails = new List<WikiPageIdDetails>();
      if (allPages != null && allPages.Count != 0)
      {
        int count = allPages.Count;
        allPages.Sort((Comparison<WikiPageIdDetails>) ((x, y) => x.PageId.CompareTo(y.PageId)));
        WikiPageIdDetails wikiPageIdDetails = allPages[0];
        for (int index = 1; index < count; ++index)
        {
          if (wikiPageIdDetails.PageId != allPages[index].PageId)
          {
            latestPageDetails.Add(wikiPageIdDetails);
            wikiPageIdDetails = allPages[index];
          }
          else
            wikiPageIdDetails = wikiPageIdDetails.AssociatedPushId > allPages[index].AssociatedPushId ? wikiPageIdDetails : allPages[index];
        }
        latestPageDetails.Add(wikiPageIdDetails);
      }
      return (IList<WikiPageIdDetails>) latestPageDetails;
    }
  }
}
