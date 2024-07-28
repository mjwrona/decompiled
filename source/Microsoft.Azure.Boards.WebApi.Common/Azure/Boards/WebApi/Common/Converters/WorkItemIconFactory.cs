// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.Converters.WorkItemIconFactory
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.Azure.Boards.WebApi.Common.Converters
{
  public class WorkItemIconFactory
  {
    public static WorkItemIcon Create(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType wit,
      string icon,
      string color = null)
    {
      return new WorkItemIcon((ISecuredObject) wit)
      {
        Id = icon,
        Url = WitUrlHelper.GetWorkItemTypeIconUrl(requestContext, icon, color)
      };
    }

    public static WorkItemIcon Create(IVssRequestContext requestContext, string icon, string color = null) => WorkItemIconFactory.Create(requestContext, (Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType) null, icon, color);
  }
}
