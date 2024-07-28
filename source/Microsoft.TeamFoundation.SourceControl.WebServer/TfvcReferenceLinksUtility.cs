// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.TfvcReferenceLinksUtility
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.VersionControl.Server;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Linq;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public static class TfvcReferenceLinksUtility
  {
    public static ReferenceLinks GetBaseReferenceLinks(
      IVssRequestContext requestContext,
      UrlHelper urlHelper,
      string selfLink)
    {
      ReferenceLinks baseReferenceLinks = new ReferenceLinks();
      baseReferenceLinks.AddLink("self", selfLink);
      return baseReferenceLinks;
    }

    public static ReferenceLinks GetBranchRefReferenceLinks(
      this TfvcBranchRef tfvcBranchRef,
      IVssRequestContext requestContext,
      UrlHelper urlHelper)
    {
      ReferenceLinks baseReferenceLinks = TfvcReferenceLinksUtility.GetBaseReferenceLinks(requestContext, urlHelper, tfvcBranchRef.Url);
      if (tfvcBranchRef.Owner != null && tfvcBranchRef.Owner.Url != null)
        baseReferenceLinks.AddLink("owner", tfvcBranchRef.Owner.Url);
      return baseReferenceLinks;
    }

    public static ReferenceLinks GetBranchReferenceLinks(
      this TfvcBranch tfvcBranch,
      IVssRequestContext requestContext,
      UrlHelper urlHelper)
    {
      ReferenceLinks baseReferenceLinks = TfvcReferenceLinksUtility.GetBaseReferenceLinks(requestContext, urlHelper, tfvcBranch.Url);
      if (tfvcBranch.RelatedBranches != null && tfvcBranch.RelatedBranches.Count > 0)
      {
        foreach (TfvcShallowBranchRef relatedBranch in tfvcBranch.RelatedBranches)
          baseReferenceLinks.AddLink("relatedBranches", urlHelper.RestLink(requestContext, TfvcConstants.TfvcBranchesLocationId, (object) new
          {
            path = relatedBranch.Path
          }));
      }
      if (tfvcBranch.Parent != null)
        baseReferenceLinks.AddLink("relatedBranches", urlHelper.RestLink(requestContext, TfvcConstants.TfvcBranchesLocationId, (object) new
        {
          path = tfvcBranch.Parent.Path
        }));
      if (tfvcBranch.Children != null && tfvcBranch.Children.Count > 0)
      {
        foreach (TfvcBranch child in tfvcBranch.Children)
          baseReferenceLinks.AddLink("childBranches", urlHelper.RestLink(requestContext, TfvcConstants.TfvcBranchesLocationId, (object) new
          {
            path = child.Path
          }));
      }
      if (tfvcBranch.Owner != null && tfvcBranch.Owner.Url != null)
        baseReferenceLinks.AddLink("owner", tfvcBranch.Owner.Url);
      return baseReferenceLinks;
    }

    public static ReferenceLinks GetChangesetsReferenceLinks(
      this TfvcChangesetRef tfvcChangesetRef,
      IVssRequestContext requestContext,
      UrlHelper urlHelper)
    {
      string nameForChangeset = TfvcReferenceLinksUtility.GetProjectNameForChangeset(requestContext, tfvcChangesetRef.ChangesetId);
      ReferenceLinks baseReferenceLinks = TfvcReferenceLinksUtility.GetBaseReferenceLinks(requestContext, urlHelper, tfvcChangesetRef.Url);
      baseReferenceLinks.AddLink("changes", urlHelper.RestLink(requestContext, TfvcConstants.TfvcChangesetChangesLocationId, (object) new
      {
        id = tfvcChangesetRef.ChangesetId
      }));
      baseReferenceLinks.AddLink("workItems", urlHelper.RestLink(requestContext, TfvcConstants.TfvcChangesetWorkItemsLocationId, (object) new
      {
        id = tfvcChangesetRef.ChangesetId
      }));
      string href = UriUtility.Combine(UriUtility.EnsureEndsWithPathSeparator(requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, Guid.Empty, AccessMappingConstants.ClientAccessMappingMoniker)) + UriUtility.EnsureEndsWithPathSeparator(nameForChangeset) + "_versionControl", "/changeset/" + tfvcChangesetRef.ChangesetId.ToString(), true).ToString();
      baseReferenceLinks.AddLink("web", href);
      if (tfvcChangesetRef.Author != null && tfvcChangesetRef.Author.Url != null)
        baseReferenceLinks.AddLink("author", tfvcChangesetRef.Author.Url);
      if (tfvcChangesetRef.CheckedInBy != null && tfvcChangesetRef.CheckedInBy.Url != null)
        baseReferenceLinks.AddLink("checkedInBy", tfvcChangesetRef.CheckedInBy.Url);
      return baseReferenceLinks;
    }

    public static ReferenceLinks GetItemsReferenceLinks(
      this TfvcItem tfvcItem,
      IVssRequestContext requestContext,
      UrlHelper urlHelper)
    {
      return TfvcReferenceLinksUtility.GetBaseReferenceLinks(requestContext, urlHelper, tfvcItem.Url);
    }

    public static ReferenceLinks GetShelvesetsReferenceLinks(
      this TfvcShelvesetRef tfvcShelvesetRef,
      IVssRequestContext requestContext,
      UrlHelper urlHelper)
    {
      ReferenceLinks baseReferenceLinks = TfvcReferenceLinksUtility.GetBaseReferenceLinks(requestContext, urlHelper, tfvcShelvesetRef.Url);
      baseReferenceLinks.AddLink("changes", urlHelper.RestLink(requestContext, TfvcConstants.TfvcShelvesetChangesLocationId, (object) new
      {
        shelvesetId = tfvcShelvesetRef.Id
      }));
      baseReferenceLinks.AddLink("workItems", urlHelper.RestLink(requestContext, TfvcConstants.TfvcShelvesetWorkItemsLocationId, (object) new
      {
        shelvesetId = tfvcShelvesetRef.Id
      }));
      if (tfvcShelvesetRef.Owner != null && tfvcShelvesetRef.Owner.Url != null)
        baseReferenceLinks.AddLink("owner", tfvcShelvesetRef.Owner.Url);
      return baseReferenceLinks;
    }

    public static ReferenceLinks GetLabelsReferenceLinks(
      this TfvcLabelRef tfvcLabelRef,
      IVssRequestContext requestContext,
      UrlHelper urlHelper)
    {
      ReferenceLinks baseReferenceLinks = TfvcReferenceLinksUtility.GetBaseReferenceLinks(requestContext, urlHelper, tfvcLabelRef.Url);
      baseReferenceLinks.AddLink("items", urlHelper.RestLink(requestContext, TfvcConstants.TfvcLabelItemsLocationId, (object) new
      {
        labelId = tfvcLabelRef.Id
      }));
      if (tfvcLabelRef.Owner != null && tfvcLabelRef.Owner.Url != null)
        baseReferenceLinks.AddLink("owner", tfvcLabelRef.Owner.Url);
      return baseReferenceLinks;
    }

    private static string GetProjectNameForChangeset(
      IVssRequestContext requestContext,
      int changesetId)
    {
      string nameForChangeset = (string) null;
      Change change = TfvcChangesetUtility.GetChangesetChanges(requestContext, changesetId, 1, 0, false, out bool _).FirstOrDefault<Change>();
      if (change != null)
        nameForChangeset = VersionControlPath.GetTeamProjectName(change.Item.ServerItem);
      return nameForChangeset;
    }
  }
}
