// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.Utils.FeatureAvailabilityHelper
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi;

namespace Microsoft.VisualStudio.Services.Feed.Server.Utils
{
  public static class FeatureAvailabilityHelper
  {
    public static void ThrowIfUnsupportedProjectScope(
      IVssRequestContext requestContext,
      ProjectReference project)
    {
      if (project != (ProjectReference) null && !requestContext.IsFeatureEnabled("Packaging.Feed.ProjectScopedFeeds"))
        throw new FeatureDisabledException(Microsoft.VisualStudio.Services.Feed.Server.Resources.Error_ProjectScopedFeedsNotEnabled());
    }

    public static void ThrowIfProjectScope(ProjectReference project)
    {
      if (project != (ProjectReference) null)
        throw new FeatureDisabledException(Microsoft.VisualStudio.Services.Feed.Server.Resources.Error_ProjectScopedFeedsNotEnabledForThisResource());
    }
  }
}
