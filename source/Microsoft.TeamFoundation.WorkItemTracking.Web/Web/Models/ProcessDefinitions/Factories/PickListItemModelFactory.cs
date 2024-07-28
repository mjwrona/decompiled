// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.ProcessDefinitions.Factories.PickListItemModelFactory
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models.ProcessDefinitions.Factories
{
  internal static class PickListItemModelFactory
  {
    public static PickListItemModel Create(
      IVssRequestContext requestContext,
      WorkItemPickListMember member)
    {
      return new PickListItemModel()
      {
        Id = member.Id,
        Value = member.Value
      };
    }

    public static IList<PickListItemModel> CreateList(
      IVssRequestContext requestContext,
      IList<WorkItemPickListMember> members)
    {
      return (IList<PickListItemModel>) members.Select<WorkItemPickListMember, PickListItemModel>((Func<WorkItemPickListMember, PickListItemModel>) (item => PickListItemModelFactory.Create(requestContext, item))).ToList<PickListItemModel>();
    }
  }
}
