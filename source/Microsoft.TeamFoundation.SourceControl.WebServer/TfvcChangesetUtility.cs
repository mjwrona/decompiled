// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.TfvcChangesetUtility
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.VersionControl.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public static class TfvcChangesetUtility
  {
    public static TfvcChangeset GetChangesetById(
      IVssRequestContext requestContext,
      UrlHelper urlHelper,
      int id,
      int maxChangeCount,
      bool toIncludeSourceRenames,
      bool toIncludeDetails,
      bool toIncludeWorkItems,
      int commentLength)
    {
      using (TeamFoundationDataReader foundationDataReader = requestContext.GetService<TeamFoundationVersionControlService>().QueryChangeset(requestContext, id, false, false, toIncludeSourceRenames))
        return TfvcChangesetUtility.GetChangesetModel(requestContext, urlHelper, foundationDataReader.Current<Changeset>(), maxChangeCount, toIncludeDetails, toIncludeWorkItems, true, commentLength);
    }

    public static TfvcChangeset GetChangesetModel(
      IVssRequestContext requestContext,
      UrlHelper urlHelper,
      Changeset changeset,
      int maxChanges,
      bool includeDetails,
      bool toIncludeWorkItems,
      bool generateDownloadUrls,
      int commentLength)
    {
      TfvcChangesContinuationToken nextToken = (TfvcChangesContinuationToken) null;
      IEnumerable<TfvcChange> changes = (IEnumerable<TfvcChange>) null;
      if (maxChanges > 0)
        changes = (IEnumerable<TfvcChange>) TfvcChangesetUtility.RetrieveChangesetChangesPaged(requestContext, urlHelper, changeset.ChangesetId, maxChanges, 0, (TfvcChangesContinuationToken) null, out nextToken);
      TfvcChangeset webApiChangeset = changeset.ToWebApiChangeset(requestContext, changes, nextToken != null, includeDetails, commentLength);
      webApiChangeset.Url = urlHelper.RestLink(requestContext, TfvcConstants.TfvcChangesetsLocationId, (object) new
      {
        id = changeset.ChangesetId
      });
      if (toIncludeWorkItems)
        webApiChangeset.WorkItems = TfvcChangesetUtility.GetChangesetWorkItems(requestContext, urlHelper, webApiChangeset.ChangesetId);
      return webApiChangeset;
    }

    public static List<TfvcChange> RetrieveChangesetChangesPaged(
      IVssRequestContext requestContext,
      UrlHelper urlHelper,
      int changesetId,
      int top,
      int skip,
      TfvcChangesContinuationToken continuationToken,
      out TfvcChangesContinuationToken nextToken)
    {
      List<TfvcChange> source1 = new List<TfvcChange>();
      nextToken = (TfvcChangesContinuationToken) null;
      using (TeamFoundationDataReader foundationDataReader = requestContext.GetService<TeamFoundationVersionControlService>().QueryChangesForChangeset(requestContext, changesetId, false, Math.Min(top + skip + 1, int.MaxValue), continuationToken == null ? (ItemSpec) null : new ItemSpec(continuationToken.ContinuationPath, RecursionType.Full), (string[]) null, true))
      {
        IEnumerable<Change> source2 = foundationDataReader.CurrentEnumerable<Change>();
        string empty = string.Empty;
        if (skip > 0)
          source2 = source2.Skip<Change>(skip);
        foreach (Change change in source2)
        {
          if (source1.Count == top)
          {
            nextToken = new TfvcChangesContinuationToken(changesetId, source1.Last<TfvcChange>().Item.Path);
            break;
          }
          TfvcChange webApiChangeModel = change.ToWebApiChangeModel();
          webApiChangeModel.Item.Url = urlHelper.RestLink(requestContext, TfvcConstants.TfvcItemsLocationId, (object) new Dictionary<string, object>()
          {
            {
              "path",
              (object) webApiChangeModel.Item.Path
            },
            {
              "versionType",
              (object) TfvcVersionType.Changeset
            },
            {
              "version",
              (object) changesetId
            }
          });
          string url1 = webApiChangeModel.Item.Url;
          if ((url1 != null ? (url1.IndexOf("+", StringComparison.Ordinal) > 0 ? 1 : 0) : 0) == 0)
          {
            string url2 = webApiChangeModel.Item.Url;
            if ((url2 != null ? (url2.IndexOf("%2b", StringComparison.OrdinalIgnoreCase) > 0 ? 1 : 0) : 0) == 0)
            {
              if (!webApiChangeModel.Item.IsFolder)
              {
                string url3 = webApiChangeModel.Item.Url;
                if ((url3 != null ? (url3.Length > 0 ? 1 : 0) : 0) == 0)
                  goto label_13;
              }
              else
                goto label_13;
            }
          }
          if (requestContext.IsFeatureEnabled("Tfvc.ItemUriUsePath"))
            webApiChangeModel.Item.Url = TfsModelExtensions.ParameterizeUnsafeUrlPaths(webApiChangeModel.Item.Url);
label_13:
          source1.Add(webApiChangeModel);
        }
      }
      return source1;
    }

    public static IEnumerable<Change> GetChangesetChanges(
      IVssRequestContext requestContext,
      int changesetId,
      int top,
      int skip,
      bool generateDownloadUrls,
      out bool hasMoreChanges)
    {
      requestContext.TraceEnter(513160, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, nameof (GetChangesetChanges));
      try
      {
        hasMoreChanges = false;
        int num = top + skip;
        using (TeamFoundationDataReader foundationDataReader = requestContext.GetService<TeamFoundationVersionControlService>().QueryChangesForChangeset(requestContext, changesetId, generateDownloadUrls, Math.Min(num + 1, int.MaxValue), (ItemSpec) null, (string[]) null, true))
        {
          IEnumerable<Change> source = foundationDataReader.CurrentEnumerable<Change>();
          if (skip > 0)
            source = source.Skip<Change>(skip);
          List<Change> changesetChanges = new List<Change>();
          foreach (Change change in source)
          {
            if (changesetChanges.Count == top)
            {
              hasMoreChanges = true;
              break;
            }
            changesetChanges.Add(change);
          }
          return (IEnumerable<Change>) changesetChanges;
        }
      }
      finally
      {
        requestContext.TraceLeave(513165, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, nameof (GetChangesetChanges));
      }
    }

    public static IEnumerable<AssociatedWorkItem> GetChangesetWorkItems(
      IVssRequestContext requestContext,
      UrlHelper urlHelper,
      int changesetId)
    {
      string changesetUri = TfvcChangesetUtility.GetChangesetUri(requestContext, changesetId);
      return new WorkItemHelper(requestContext).QueryWorkItemInformation(changesetUri).ToWebApiWorkItems();
    }

    public static string GetChangesetUri(IVssRequestContext requestContext, int changesetId) => LinkingUtilities.EncodeUri(new ArtifactId()
    {
      Tool = "VersionControl",
      ArtifactType = "Changeset",
      ToolSpecificId = changesetId.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)
    });

    public static TfvcChangesetsCollection QueryChangesets(
      IVssRequestContext requestContext,
      UrlHelper urlHelper,
      TfvcChangesetSearchCriteria searchCriteria,
      int top,
      int skip,
      int maxCommentLength,
      bool ascendingOrder,
      out bool moreChangesetsAvailable)
    {
      moreChangesetsAvailable = false;
      VersionSpec versionFrom = (VersionSpec) null;
      VersionSpec versionTo = (VersionSpec) null;
      TfvcChangesetsCollection changesetsCollection = new TfvcChangesetsCollection();
      if (searchCriteria.FromId > 0)
        versionFrom = (VersionSpec) new ChangesetVersionSpec(searchCriteria.FromId);
      else if (requestContext.ParseFromDate(searchCriteria.FromDate).HasValue)
        versionFrom = (VersionSpec) new DateVersionSpec()
        {
          Date = requestContext.ParseFromDate(searchCriteria.FromDate).Value
        };
      if (searchCriteria.ToId > 0)
        versionTo = (VersionSpec) new ChangesetVersionSpec(searchCriteria.ToId);
      else if (requestContext.ParseToDate(searchCriteria.ToDate).HasValue)
        versionTo = (VersionSpec) new DateVersionSpec()
        {
          Date = requestContext.ParseToDate(searchCriteria.ToDate).Value
        };
      if (top + skip == int.MaxValue)
        --top;
      if (searchCriteria.Mappings.Count == 0)
      {
        if (string.IsNullOrWhiteSpace(searchCriteria.ItemPath))
          searchCriteria.ItemPath = "$/";
        else if (!string.IsNullOrEmpty(searchCriteria.ItemPath))
          VersionControlPath.ValidatePath(searchCriteria.ItemPath);
        VersionSpec versionSpec = searchCriteria.versionDescriptor == null ? (VersionSpec) new LatestVersionSpec() : TfvcVersionSpecUtility.GetVersionSpec(requestContext, searchCriteria.versionDescriptor);
        TfvcItem tfvcItem = TfvcItemUtility.GetItem(requestContext, urlHelper, searchCriteria.ItemPath, versionSpec, DeletedState.Any);
        bool includeFiles = !tfvcItem.IsFolder;
        ItemSpec itemSpec = new ItemSpec(tfvcItem.Path, includeFiles ? RecursionType.None : RecursionType.Full);
        using (TeamFoundationDataReader foundationDataReader = requestContext.GetService<TeamFoundationVersionControlService>().QueryHistory(requestContext, (string) null, (string) null, itemSpec, versionSpec, searchCriteria.Author, versionFrom, versionTo, top + skip + 1, includeFiles, false, !searchCriteria.FollowRenames, ascendingOrder))
        {
          IEnumerable<Changeset> source = foundationDataReader.CurrentEnumerable<Changeset>();
          if (source != null)
          {
            if (skip > 0)
              source = source.Skip<Changeset>(skip);
            foreach (Changeset changeset in source)
            {
              if (changesetsCollection.Count == top)
              {
                moreChangesetsAvailable = true;
                break;
              }
              TfvcChangesetRef webApiChangeset = changeset.ToWebApiChangeset(requestContext, maxCommentLength);
              webApiChangeset.Url = urlHelper.RestLink(requestContext, TfvcConstants.TfvcChangesetsLocationId, (object) new
              {
                id = changeset.ChangesetId
              });
              webApiChangeset.Links = searchCriteria.IncludeLinks ? webApiChangeset.GetChangesetsReferenceLinks(requestContext, urlHelper) : (ReferenceLinks) null;
              changesetsCollection.Add(webApiChangeset);
            }
          }
        }
      }
      else
      {
        foreach (Changeset changeset in requestContext.GetService<TeamFoundationVersionControlService>().QueryChangesetRange(requestContext, (IEnumerable<TfvcMappingFilter>) searchCriteria.Mappings, versionFrom, versionTo, top + skip + 1, false))
        {
          if (changesetsCollection.Count == top)
          {
            moreChangesetsAvailable = true;
            break;
          }
          TfvcChangesetRef webApiChangeset = changeset.ToWebApiChangeset(requestContext, maxCommentLength);
          webApiChangeset.Url = urlHelper.RestLink(requestContext, TfvcConstants.TfvcChangesetsLocationId, (object) new
          {
            id = changeset.ChangesetId
          });
          webApiChangeset.Links = searchCriteria.IncludeLinks ? webApiChangeset.GetChangesetsReferenceLinks(requestContext, urlHelper) : (ReferenceLinks) null;
          changesetsCollection.Add(webApiChangeset);
        }
      }
      return changesetsCollection;
    }

    internal static int GetLatestChangesetId(IVssRequestContext requestContext) => requestContext.GetService<TeamFoundationVersionControlService>().GetLatestChangeset(requestContext);
  }
}
