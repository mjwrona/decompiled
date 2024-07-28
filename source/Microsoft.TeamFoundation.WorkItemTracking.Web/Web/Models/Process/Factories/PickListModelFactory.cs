// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Process.Factories.PickListModelFactory
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Process.Factories
{
  internal static class PickListModelFactory
  {
    public static PickList Create(IVssRequestContext requestContext, WorkItemPickList list)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<WorkItemPickList>(list, nameof (list));
      PickList pickList = new PickList();
      pickList.Id = list.Id;
      pickList.Name = list.Name;
      pickList.Type = list.Type.ToString();
      pickList.Items = PickListItemModelFactory.CreateList((IList<WorkItemPickListMember>) new List<WorkItemPickListMember>((IEnumerable<WorkItemPickListMember>) list.Items));
      pickList.IsSuggested = list.IsSuggested(requestContext, Guid.Empty, 0);
      pickList.Url = PickListMetadataModelFactory.GetLocationUrlForPicklist(requestContext, list.Id);
      return pickList;
    }
  }
}
