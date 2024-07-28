// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.ProcessDefinitions.Factories.PickListModelFactory
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models.ProcessDefinitions.Factories
{
  internal static class PickListModelFactory
  {
    public static PickListModel Create(IVssRequestContext requestContext, WorkItemPickList list)
    {
      PickListModel pickListModel = new PickListModel();
      pickListModel.Id = list.Id;
      pickListModel.Name = list.Name;
      pickListModel.Type = list.Type.ToString();
      pickListModel.Items = PickListItemModelFactory.CreateList(requestContext, (IList<WorkItemPickListMember>) new List<WorkItemPickListMember>((IEnumerable<WorkItemPickListMember>) list.Items));
      pickListModel.IsSuggested = list.IsSuggested(requestContext, Guid.Empty, 0);
      pickListModel.Url = PickListMetadataModelFactory.GetLocationUrlForPicklist(requestContext, list.Id);
      return pickListModel;
    }
  }
}
