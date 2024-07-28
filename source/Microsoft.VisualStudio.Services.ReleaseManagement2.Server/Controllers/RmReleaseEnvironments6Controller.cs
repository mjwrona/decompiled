// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.RmReleaseEnvironments6Controller
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Cannot avoid it")]
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "environments", ResourceVersion = 6)]
  public class RmReleaseEnvironments6Controller : RmReleaseEnvironments5Controller
  {
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Compat", Justification = "Right term")]
    protected override ReleaseEnvironment LatestToIncoming(ReleaseEnvironment environment) => environment;
  }
}
