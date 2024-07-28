// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.RmReleases6Controller
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Cannot avoid it")]
  [ControllerApiVersion(4.0)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "releases", ResourceVersion = 6)]
  public class RmReleases6Controller : RmReleases5Controller
  {
    protected override Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release LatestToIncoming(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release release)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      if (release.Reason == ReleaseReason.PullRequest)
        release.Reason = ReleaseReason.ContinuousIntegration;
      if (release.Environments != null)
        release.Environments.HandleGateCanceledStateBackCompatibility();
      return release;
    }

    protected override Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release IncomingToLatest(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release release,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Release serverRelease)
    {
      return release;
    }
  }
}
