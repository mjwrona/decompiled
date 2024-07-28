// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.TfvcBranchesUtility
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
using System.Diagnostics;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  internal class TfvcBranchesUtility
  {
    public static void CreateBranch(IVssRequestContext requestContext, TfvcBranch branch)
    {
      if (branch == null || string.IsNullOrEmpty(branch.Path) || branch.Parent != null && string.IsNullOrEmpty(branch.Parent.Path))
        throw new ArgumentException(Resources.Get("InvalidParameters")).Expected(requestContext.ServiceName);
      requestContext.GetService<TeamFoundationVersionControlService>().CreateBranch(requestContext, branch.Parent == null ? (string) null : branch.Parent.Path, branch.Path, (VersionSpec) new LatestVersionSpec(), new Changeset(), new CheckinNotificationInfo(), (List<Mapping>) null);
    }

    public static void CreateBranchObject(
      IVssRequestContext requestContext,
      TfvcBranch branchObject)
    {
      if (branchObject == null || string.IsNullOrEmpty(branchObject.Path) || branchObject.Parent != null && string.IsNullOrEmpty(branchObject.Parent.Path))
        throw new ArgumentException(Resources.Get("InvalidParameters")).Expected(requestContext.ServiceName);
      BranchProperties branchProperties1 = new BranchProperties();
      branchProperties1.RootItem = new ItemIdentifier()
      {
        Item = branchObject.Path
      };
      ItemIdentifier itemIdentifier;
      if (branchObject.Parent != null)
        itemIdentifier = new ItemIdentifier()
        {
          Item = branchObject.Parent.Path
        };
      else
        itemIdentifier = (ItemIdentifier) null;
      branchProperties1.ParentBranch = itemIdentifier;
      BranchProperties branchProperties2 = branchProperties1;
      requestContext.GetService<TeamFoundationVersionControlService>().UpdateBranchObject(requestContext, branchProperties2, false);
    }

    public static TfvcBranch GetBranch(
      IVssRequestContext requestContext,
      UrlHelper urlHelper,
      string path,
      bool includeParent,
      bool includeChildren)
    {
      path = VersionControlPath.GetFullPath(path);
      if (path == "$/")
        throw new BranchNotFoundException(Resources.Format("ErrorTfvcBranchNotFound", (object) path));
      VersionControlPath.ValidatePath(path);
      TeamFoundationVersionControlService service = requestContext.GetService<TeamFoundationVersionControlService>();
      ItemIdentifier itemIdentifier = new ItemIdentifier();
      TfvcBranch branch = (TfvcBranch) null;
      itemIdentifier.ItemPathPair = ItemPathPair.FromServerItem(path);
      try
      {
        using (TeamFoundationDataReader foundationDataReader = service.QueryBranchObjects(requestContext, itemIdentifier, TfvcCommonUtility.ConvertVersionControlRecursionType(includeChildren ? VersionControlRecursionType.Full : VersionControlRecursionType.None)))
        {
          int num = 0;
          Dictionary<string, TfvcBranchesCollection> dictionary1 = new Dictionary<string, TfvcBranchesCollection>();
          Dictionary<string, TfvcBranch> dictionary2 = new Dictionary<string, TfvcBranch>();
          foreach (BranchObject branchObject in foundationDataReader.Current<StreamingCollection<BranchObject>>())
          {
            TfvcBranch webApiTfvcBranch = TfsModelExtensions.ToWebApiTfvcBranch(requestContext, branchObject, includeParent, true);
            if (includeChildren)
              webApiTfvcBranch.Children = (List<TfvcBranch>) new TfvcBranchesCollection();
            if (num == 0)
            {
              branch = webApiTfvcBranch;
              if (includeChildren)
                dictionary2.Add(branchObject.Properties.RootItem.Item, branch);
            }
            else
            {
              try
              {
                dictionary2.Add(branchObject.Properties.RootItem.Item, webApiTfvcBranch);
                TfvcBranchesCollection branchesCollection;
                if (!dictionary1.TryGetValue(branchObject.Properties.ParentBranch.Item, out branchesCollection))
                {
                  branchesCollection = new TfvcBranchesCollection();
                  dictionary1.Add(branchObject.Properties.ParentBranch.Item, branchesCollection);
                }
                branchesCollection.Add(webApiTfvcBranch);
              }
              catch (Exception ex)
              {
                requestContext.Trace(513197, TraceLevel.Error, TraceArea.Exceptions, TraceLayer.Command, string.Format("GetBranch exception while processing object. Exception: {0}", (object) ex));
              }
            }
            ++num;
          }
          foreach (string key in dictionary1.Keys)
            dictionary2[key].Children = (List<TfvcBranch>) dictionary1[key];
          if (branch == null)
            throw new BranchNotFoundException(Resources.Format("ErrorTfvcBranchNotFound", (object) path));
          branch.Url = urlHelper.RestLink(requestContext, TfvcConstants.TfvcBranchesLocationId, (object) new
          {
            path = path
          });
          return branch;
        }
      }
      catch (Exception ex)
      {
        requestContext.Trace(513197, TraceLevel.Error, TraceArea.Exceptions, TraceLayer.Command, string.Format("GetBranch has caught an exception.  Exception: {0}", (object) ex));
        throw;
      }
    }

    public static TfvcBranchRefsCollection QueryBranches(
      IVssRequestContext requestContext,
      UrlHelper urlHelper,
      string scopePath,
      VersionSpec version,
      bool includeDeleted,
      bool includeLinks)
    {
      VersionControlPath.ValidatePath(scopePath);
      TeamFoundationVersionControlService service = requestContext.GetService<TeamFoundationVersionControlService>();
      ItemIdentifier itemIdentifier1 = new ItemIdentifier();
      TfvcBranchRefsCollection branchRefsCollection = new TfvcBranchRefsCollection();
      itemIdentifier1.ItemPathPair = ItemPathPair.FromServerItem(scopePath);
      IVssRequestContext requestContext1 = requestContext;
      ItemIdentifier itemIdentifier2 = itemIdentifier1;
      VersionSpec version1 = version;
      using (TeamFoundationDataReader foundationDataReader = service.QueryBranchObjectsByPath(requestContext1, itemIdentifier2, version1))
      {
        foreach (BranchObject branchObject in foundationDataReader.Current<StreamingCollection<BranchObject>>())
        {
          if (includeDeleted || branchObject.Properties.RootItem.DeletionId == 0)
          {
            TfvcBranchRef apiTfvcBranchRef = TfsModelExtensions.ToWebApiTfvcBranchRef(requestContext, branchObject);
            apiTfvcBranchRef.Url = urlHelper.RestLink(requestContext, TfvcConstants.TfvcBranchesLocationId, (object) new
            {
              path = apiTfvcBranchRef.Path
            });
            apiTfvcBranchRef.Links = includeLinks ? apiTfvcBranchRef.GetBranchRefReferenceLinks(requestContext, urlHelper) : (ReferenceLinks) null;
            branchRefsCollection.Add(apiTfvcBranchRef);
          }
        }
        return branchRefsCollection;
      }
    }

    public static TfvcBranchesCollection QueryRootBranches(
      IVssRequestContext requestContext,
      UrlHelper urlHelper,
      string scopePath,
      bool includeParent,
      bool includeChildren,
      bool includeDeleted,
      bool includeLinks)
    {
      TeamFoundationVersionControlService service = requestContext.GetService<TeamFoundationVersionControlService>();
      ItemIdentifier itemIdentifier = (ItemIdentifier) null;
      if (!string.IsNullOrEmpty(scopePath))
        itemIdentifier = new ItemIdentifier()
        {
          ItemPathPair = ItemPathPair.FromServerItem(scopePath)
        };
      TfvcBranchesCollection branchesCollection1 = new TfvcBranchesCollection();
      using (TeamFoundationDataReader foundationDataReader = itemIdentifier == null ? service.QueryBranchObjects(requestContext, itemIdentifier, TfvcCommonUtility.ConvertVersionControlRecursionType(includeChildren ? VersionControlRecursionType.Full : VersionControlRecursionType.OneLevel)) : service.QueryBranchObjectsByPath(requestContext, itemIdentifier, (VersionSpec) new LatestVersionSpec()))
      {
        if (!includeChildren)
        {
          foreach (BranchObject branchObject in foundationDataReader.Current<StreamingCollection<BranchObject>>())
          {
            if (includeDeleted || branchObject.Properties.RootItem.DeletionId == 0)
            {
              TfvcBranch webApiTfvcBranch = TfsModelExtensions.ToWebApiTfvcBranch(requestContext, branchObject, false, includeDeleted);
              webApiTfvcBranch.Url = urlHelper.RestLink(requestContext, TfvcConstants.TfvcBranchesLocationId, (object) new
              {
                path = webApiTfvcBranch.Path
              });
              webApiTfvcBranch.Links = includeLinks ? webApiTfvcBranch.GetBranchReferenceLinks(requestContext, urlHelper) : (ReferenceLinks) null;
              branchesCollection1.Add(webApiTfvcBranch);
            }
          }
          return branchesCollection1;
        }
        Dictionary<string, TfvcBranchesCollection> dictionary1 = new Dictionary<string, TfvcBranchesCollection>();
        Dictionary<string, TfvcBranch> dictionary2 = new Dictionary<string, TfvcBranch>();
        TfvcBranchesCollection branchesCollection2 = new TfvcBranchesCollection();
        foreach (BranchObject branchObject in foundationDataReader.Current<StreamingCollection<BranchObject>>())
        {
          if (includeDeleted || branchObject.Properties.RootItem.DeletionId == 0)
          {
            TfvcBranch webApiTfvcBranch = TfsModelExtensions.ToWebApiTfvcBranch(requestContext, branchObject, includeParent, includeDeleted);
            webApiTfvcBranch.Url = urlHelper.RestLink(requestContext, TfvcConstants.TfvcBranchesLocationId, (object) new
            {
              path = webApiTfvcBranch.Path
            });
            webApiTfvcBranch.Children = (List<TfvcBranch>) new TfvcBranchesCollection();
            dictionary2.Add(branchObject.Properties.RootItem.Item, webApiTfvcBranch);
            if (branchObject.Properties.ParentBranch != null)
            {
              TfvcBranchesCollection branchesCollection3;
              if (!dictionary1.TryGetValue(branchObject.Properties.ParentBranch.Item, out branchesCollection3))
              {
                branchesCollection3 = new TfvcBranchesCollection();
                dictionary1.Add(branchObject.Properties.ParentBranch.Item, branchesCollection3);
              }
              branchesCollection3.Add(webApiTfvcBranch);
            }
            else
              branchesCollection2.Add(webApiTfvcBranch);
          }
        }
        foreach (string key in dictionary1.Keys)
        {
          try
          {
            dictionary2[key].Children = (List<TfvcBranch>) dictionary1[key];
          }
          catch (KeyNotFoundException ex)
          {
          }
        }
        if (includeLinks)
        {
          foreach (TfvcBranch tfvcBranch in (List<TfvcBranch>) branchesCollection2)
            tfvcBranch.Links = tfvcBranch.GetBranchReferenceLinks(requestContext, urlHelper);
        }
        return branchesCollection2;
      }
    }
  }
}
