// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.TfvcLabelUtility
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.VersionControl.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public static class TfvcLabelUtility
  {
    private static readonly ItemSpec[] s_itemSpecs = new ItemSpec[1]
    {
      new ItemSpec("$/", RecursionType.Full, 0)
    };

    public static int ParseLabelId(string strId)
    {
      int result;
      if (!int.TryParse(strId, out result))
        throw new InvalidArgumentValueException("labelId");
      return result;
    }

    public static List<TfvcLabelRef> GetLabels(
      IVssRequestContext requestContext,
      UrlHelper urlHelper,
      TfvcLabelRequestData requestData,
      int top,
      int skip)
    {
      string ownerId = TfsModelExtensions.ParseOwnerId(requestContext, requestData.Owner);
      using (TeamFoundationDataReader foundationDataReader = requestContext.GetService<TeamFoundationVersionControlService>().QueryLabels(requestContext, (string) null, (string) null, requestData.Name, requestData.LabelScope, ownerId, requestData.ItemLabelFilter, (VersionSpec) new LatestVersionSpec(), false, false, PathLength.Length399))
      {
        List<TfvcLabelRef> labels = new List<TfvcLabelRef>();
        foreach (VersionControlLabel label in foundationDataReader.Current<StreamingCollection<VersionControlLabel>>().Skip<VersionControlLabel>(skip).Take<VersionControlLabel>(top))
        {
          TfvcLabelRef webApiShallowLabel = label.ToWebApiShallowLabel(requestContext);
          webApiShallowLabel.Url = urlHelper.RestLink(requestContext, TfvcConstants.TfvcLabelsLocationId, (object) new
          {
            labelId = webApiShallowLabel.Id
          });
          webApiShallowLabel.Links = requestData.IncludeLinks ? webApiShallowLabel.GetLabelsReferenceLinks(requestContext, urlHelper) : (ReferenceLinks) null;
          labels.Add(webApiShallowLabel);
        }
        return labels;
      }
    }

    public static TfvcLabel GetLabel(
      IVssRequestContext requestContext,
      UrlHelper urlHelper,
      int labelId,
      TfvcLabelRequestData requestData)
    {
      VersionControlRequestContext controlRequestContext = new VersionControlRequestContext(requestContext);
      VersionControlLabel labelByLabelId = VersionControlLabel.FindLabelByLabelId(controlRequestContext, labelId);
      List<TfvcItem> items = (List<TfvcItem>) null;
      int? maxItemCount = requestData.MaxItemCount;
      int num = 0;
      if (maxItemCount.GetValueOrDefault() > num & maxItemCount.HasValue)
        items = TfvcLabelUtility.GetItems(controlRequestContext, requestContext, urlHelper, labelByLabelId.Name, labelByLabelId.Scope, requestData.MaxItemCount.Value, 0);
      TfvcLabel webApiLabel = labelByLabelId.ToWebApiLabel(requestContext, (IEnumerable<TfvcItem>) items);
      webApiLabel.Url = urlHelper.RestLink(requestContext, TfvcConstants.TfvcLabelsLocationId, (object) new
      {
        labelId = labelByLabelId.LabelId
      });
      webApiLabel.Links = webApiLabel.GetLabelsReferenceLinks(requestContext, urlHelper);
      return webApiLabel;
    }

    public static List<TfvcItem> GetItems(
      IVssRequestContext requestContext,
      UrlHelper urlHelper,
      int labelId,
      int top,
      int skip)
    {
      VersionControlRequestContext controlRequestContext = new VersionControlRequestContext(requestContext);
      VersionControlLabel labelByLabelId = VersionControlLabel.FindLabelByLabelId(controlRequestContext, labelId);
      return TfvcLabelUtility.GetItems(controlRequestContext, requestContext, urlHelper, labelByLabelId.Name, labelByLabelId.Scope, top, skip);
    }

    private static List<TfvcItem> GetItems(
      VersionControlRequestContext requestContext,
      IVssRequestContext tfsRequestContext,
      UrlHelper urlHelper,
      string name,
      string scope,
      int top,
      int skip)
    {
      List<TfvcItem> items = new List<TfvcItem>();
      using (CommandQueryItems<ItemSet, Item> commandQueryItems = new CommandQueryItems<ItemSet, Item>(requestContext))
      {
        commandQueryItems.Execute((Workspace) null, TfvcLabelUtility.s_itemSpecs, (VersionSpec) new LabelVersionSpec(name, scope), DeletedState.Any, ItemType.Any, false, 0);
        StreamingCollection<Item> source = (StreamingCollection<Item>) null;
        if (commandQueryItems.ItemSets.MoveNext())
          source = commandQueryItems.ItemSets.Current.Items;
        if (source != null)
          items = source.Skip<Item>(skip).Take<Item>(top).Select<Item, TfvcItem>((Func<Item, TfvcItem>) (x => x.ToWebApiItem(tfsRequestContext, urlHelper, true))).ToList<TfvcItem>();
      }
      return items;
    }
  }
}
