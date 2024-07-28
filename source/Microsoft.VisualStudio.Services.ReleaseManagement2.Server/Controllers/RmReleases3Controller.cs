// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.RmReleases3Controller
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(2.2)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "releases", ResourceVersion = 3)]
  public class RmReleases3Controller : RmReleases2Controller
  {
    protected override Release LatestToIncoming(Release release)
    {
      if (release == null)
        throw new ArgumentNullException(nameof (release));
      if (release.Environments != null)
      {
        foreach (ReleaseEnvironment environment in (IEnumerable<ReleaseEnvironment>) release.Environments)
        {
          environment.ToNoPhasesFormat();
          environment.HandleCancelingStateBackCompatibility();
          environment.HandleGateCanceledStateBackCompatibility();
        }
      }
      if (release.Reason == ReleaseReason.PullRequest)
        release.Reason = ReleaseReason.ContinuousIntegration;
      return release;
    }
  }
}
