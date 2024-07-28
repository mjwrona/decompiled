// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.GatedCheckInTriggerExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class GatedCheckInTriggerExtensions
  {
    public static void ConvertPathFiltersPathsToProjectId(
      this GatedCheckInTrigger trigger,
      IVssRequestContext requestContext)
    {
      for (int index = 0; index < trigger.PathFilters.Count; ++index)
        trigger.PathFilters[index] = trigger.PathFilters[index].Substring(0, 1) + TFVCPathHelper.ConvertToPathWithProjectGuid(requestContext, trigger.PathFilters[index].Substring(1));
    }

    public static void ConvertPathFiltersPathsToProjectName(
      this GatedCheckInTrigger trigger,
      IVssRequestContext requestContext)
    {
      for (int index = 0; index < trigger.PathFilters.Count; ++index)
        trigger.PathFilters[index] = trigger.PathFilters[index].Substring(0, 1) + TFVCPathHelper.ConvertToPathWithProjectName(requestContext, trigger.PathFilters[index].Substring(1));
    }
  }
}
