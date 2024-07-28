// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.TfvcShelvesetUtility
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public static class TfvcShelvesetUtility
  {
    private static readonly string[] s_defaultPropertyFilters = new string[1]
    {
      "Microsoft.TeamFoundation.VersionControl.SymbolicLink"
    };

    public static ShelvesetVersionSpec GetShelvesetVersionSpecFromId(
      IVssRequestContext requestContext,
      string shelvesetId)
    {
      string name = (string) null;
      string owner = (string) null;
      TfvcShelvesetUtility.ParseNameId(requestContext, shelvesetId, out name, out owner);
      return new ShelvesetVersionSpec(name, owner);
    }

    public static string CreateShelvesetId(string name, string owner) => owner + ";" + name;

    public static void ParseNameId(
      IVssRequestContext requestContext,
      string shelvesetId,
      out string name,
      out string owner)
    {
      name = (string) null;
      owner = (string) null;
      int length = shelvesetId.IndexOf(';');
      name = length != -1 ? shelvesetId.Substring(0, length) : throw new InvalidArgumentValueException(nameof (shelvesetId));
      string ownerId = shelvesetId.Substring(length + 1);
      owner = TfsModelExtensions.ParseOwnerId(requestContext, ownerId);
    }

    public static IEnumerable<TfvcShelvesetRef> GetShelvesets(
      IVssRequestContext requestContext,
      UrlHelper urlHelper,
      TfvcShelvesetRequestData requestData,
      int top,
      int skip)
    {
      List<TfvcShelvesetRef> shelvesets = new List<TfvcShelvesetRef>();
      List<Shelveset> list = TfvcShelvesetUtility.GetShelvesets(requestContext, requestData.Name, requestData.Owner).OrderByDescending<Shelveset, DateTime>((Func<Shelveset, DateTime>) (ss => ss.CreationDate)).Skip<Shelveset>(skip).Take<Shelveset>(top).ToList<Shelveset>();
      for (int index = 0; index < list.Count; ++index)
      {
        TfvcShelvesetRef shallowTfvcShelveset = list[index].ToShallowTfvcShelveset(requestContext, requestData.MaxCommentLength.Value);
        shallowTfvcShelveset.Url = urlHelper.RestLink(requestContext, TfvcConstants.TfvcShelvesetLocationId, (object) new
        {
          shelvesetId = shallowTfvcShelveset.Id
        });
        shallowTfvcShelveset.Links = requestData.IncludeLinks ? shallowTfvcShelveset.GetShelvesetsReferenceLinks(requestContext, urlHelper) : (ReferenceLinks) null;
        shelvesets.Add(shallowTfvcShelveset);
      }
      return (IEnumerable<TfvcShelvesetRef>) shelvesets;
    }

    public static TfvcShelveset GetShelveset(
      IVssRequestContext requestContext,
      UrlHelper urlHelper,
      TfvcShelvesetRequestData requestData)
    {
      requestData.MaxChangeCount = new int?(Math.Max(0, requestData.MaxChangeCount.GetValueOrDefault()));
      TfvcShelvesetRequestData shelvesetRequestData = requestData;
      int? nullable1 = requestData.MaxCommentLength;
      int? nullable2 = new int?(Math.Max(0, nullable1 ?? 2000));
      shelvesetRequestData.MaxCommentLength = nullable2;
      Shelveset shelveset1 = TfvcShelvesetUtility.GetShelveset(requestContext, requestData.Name, requestData.Owner);
      IEnumerable<AssociatedWorkItem> associatedWorkItems = (IEnumerable<AssociatedWorkItem>) null;
      if (requestData.IncludeWorkItems)
        associatedWorkItems = shelveset1.GetWorkItems(requestContext);
      IEnumerable<TfvcChange> tfvcChanges = (IEnumerable<TfvcChange>) null;
      nullable1 = requestData.MaxChangeCount;
      int num1 = 0;
      if (nullable1.GetValueOrDefault() > num1 & nullable1.HasValue)
      {
        bool flag = false;
        ShelvesetVersionSpec shelvesetVersionSpec = new ShelvesetVersionSpec(shelveset1.Name, shelveset1.Owner);
        IVssRequestContext requestContext1 = requestContext;
        ShelvesetVersionSpec versionSpec = shelvesetVersionSpec;
        nullable1 = requestData.MaxChangeCount;
        int top = nullable1.Value;
        string[] defaultPropertyFilters = TfvcShelvesetUtility.s_defaultPropertyFilters;
        ref bool local = ref flag;
        UrlHelper url = urlHelper;
        tfvcChanges = TfsModelExtensions.GetShelvedChanges(requestContext1, "$/", RecursionType.Full, 2, versionSpec, top, 0, defaultPropertyFilters, out local, true, url);
      }
      Shelveset shelveset2 = shelveset1;
      IVssRequestContext requestContext2 = requestContext;
      IEnumerable<TfvcChange> changes = tfvcChanges;
      IEnumerable<AssociatedWorkItem> workItems = associatedWorkItems;
      nullable1 = requestData.MaxCommentLength;
      int maxCommentLength = nullable1.Value;
      int num2 = requestData.IncludeDetails ? 1 : 0;
      TfvcShelveset tfvcShelveset = shelveset2.ToTfvcShelveset(requestContext2, changes, workItems, maxCommentLength, num2 != 0);
      tfvcShelveset.Url = urlHelper.RestLink(requestContext, TfvcConstants.TfvcShelvesetLocationId, (object) new
      {
        shelvesetId = tfvcShelveset.Id
      });
      tfvcShelveset.Links = tfvcShelveset.GetShelvesetsReferenceLinks(requestContext, urlHelper);
      return tfvcShelveset;
    }

    public static Shelveset GetShelveset(
      IVssRequestContext requestContext,
      string name,
      string owner)
    {
      List<Shelveset> list = TfvcShelvesetUtility.GetShelvesets(requestContext, name, owner).ToList<Shelveset>();
      return list.Count != 0 ? list[0] : throw new ShelvesetNotFoundException(name, owner);
    }

    public static bool DeleteShelveset(IVssRequestContext requestContext, string shelvesetId)
    {
      string name;
      string owner;
      TfvcShelvesetUtility.ParseNameId(requestContext, shelvesetId, out name, out owner);
      TfvcShelvesetUtility.GetShelveset(requestContext, name, owner);
      requestContext.GetService<TeamFoundationVersionControlService>().DeleteShelveset(requestContext, name, owner);
      return true;
    }

    public static IEnumerable<AssociatedWorkItem> GetWorkItems(
      this Shelveset shelveset,
      IVssRequestContext requestContext)
    {
      return new WorkItemHelper(requestContext).QueryWorkItemInformation(shelveset.Links).ToWebApiWorkItems();
    }

    private static IEnumerable<Shelveset> GetShelvesets(
      IVssRequestContext requestContext,
      string name,
      string owner)
    {
      try
      {
        requestContext.TraceEnter(513135, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, nameof (GetShelvesets));
        return (IEnumerable<Shelveset>) requestContext.GetService<TeamFoundationVersionControlService>().QueryShelvesets(requestContext, name, owner);
      }
      finally
      {
        requestContext.TraceLeave(513140, WebApiConstants.TraceArea, WebApiTraceLayers.BusinessLogic, nameof (GetShelvesets));
      }
    }

    private static TfvcShelvesetRef ToShallowTfvcShelveset(
      this Shelveset shelveset,
      IVssRequestContext requestContext,
      int maxCommentLength)
    {
      string text = shelveset.Comment;
      bool flag = false;
      if (text != null && text.Length > maxCommentLength)
      {
        text = StringUtil.Truncate(text, maxCommentLength, false);
        flag = true;
      }
      TfvcShelvesetRef shallowTfvcShelveset = new TfvcShelvesetRef()
      {
        Name = shelveset.Name,
        Id = shelveset.Name + ";" + shelveset.ownerId.ToString(),
        Owner = new IdentityRef()
        {
          Id = shelveset.ownerId.ToString(),
          DisplayName = shelveset.OwnerDisplayName,
          UniqueName = TfsModelExtensions.ParseOwnerId(requestContext, shelveset.Owner)
        },
        CreatedDate = shelveset.CreationDate,
        Comment = text,
        CommentTruncated = flag
      };
      if (shelveset.ownerId != Guid.Empty)
      {
        shallowTfvcShelveset.Owner.Url = IdentityHelper.GetIdentityResourceUriString(requestContext, shelveset.ownerId);
        shallowTfvcShelveset.Owner.ImageUrl = IdentityHelper.GetImageResourceUrl(requestContext, shelveset.ownerId);
      }
      return shallowTfvcShelveset;
    }

    private static TfvcShelveset ToTfvcShelveset(
      this Shelveset shelveset,
      IVssRequestContext requestContext,
      IEnumerable<TfvcChange> changes,
      IEnumerable<AssociatedWorkItem> workItems,
      int maxCommentLength,
      bool includeDetails)
    {
      TfvcShelveset tfvcShelveset = new TfvcShelveset(shelveset.ToShallowTfvcShelveset(requestContext, maxCommentLength));
      if (includeDetails)
      {
        tfvcShelveset.PolicyOverride = new TfvcPolicyOverrideInfo(shelveset.PolicyOverrideComment, (IEnumerable<TfvcPolicyFailureInfo>) null);
        Microsoft.TeamFoundation.SourceControl.WebApi.CheckinNote[] webApiCheckInNotes = shelveset.CheckinNote.ToWebApiCheckInNotes();
        tfvcShelveset.Notes = webApiCheckInNotes ?? Array.Empty<Microsoft.TeamFoundation.SourceControl.WebApi.CheckinNote>();
      }
      tfvcShelveset.Changes = changes;
      tfvcShelveset.WorkItems = workItems;
      return tfvcShelveset;
    }
  }
}
