// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Extensions.EnumExtensions
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Extensions
{
  public static class EnumExtensions
  {
    public static Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.ResourceLinkType ToResourceLinkType(
      this Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.ResourceLinkType linkType)
    {
      return (Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.ResourceLinkType) linkType;
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.ResourceLinkType ToResourceLinkType(
      this Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.ResourceLinkType linkType)
    {
      return (Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.ResourceLinkType) linkType;
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.LinkUpdateType ToLinkUpdateType(
      this Microsoft.TeamFoundation.WorkItemTracking.Common.LinkUpdateType linkType)
    {
      return (Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.LinkUpdateType) linkType;
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.Common.LinkUpdateType ToLinkUpdateType(
      this Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.LinkUpdateType linkType)
    {
      return (Microsoft.TeamFoundation.WorkItemTracking.Common.LinkUpdateType) linkType;
    }
  }
}
