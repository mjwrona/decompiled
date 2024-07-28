// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.RmReleaseEnvironments4Controller
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(3.0)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "environments", ResourceVersion = 4)]
  public class RmReleaseEnvironments4Controller : RmReleaseEnvironments3Controller
  {
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Compat", Justification = "Right term")]
    protected override ReleaseEnvironment LatestToIncoming(ReleaseEnvironment environment)
    {
      environment.HandleGateCanceledStateBackCompatibility();
      return environment.HandleCancelingStateBackCompatibility();
    }
  }
}
