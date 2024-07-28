// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildLogs4Controller
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "logs", ResourceVersion = 2)]
  [ClientGroupByResource("builds")]
  public class BuildLogs4Controller : BuildLogs3Controller
  {
    [HttpGet]
    [ClientResponseType(typeof (List<BuildLog>), null, null)]
    [ClientResponseType(typeof (Stream), "GetBuildLogsZip", "application/zip")]
    [MethodInformation(TimeoutSeconds = 1800)]
    [PublicProjectRequestRestrictions]
    public override HttpResponseMessage GetBuildLogs(int buildId) => base.GetBuildLogs(buildId);

    [HttpGet]
    [ClientResponseType(typeof (Stream), "GetBuildLog", "text/plain")]
    [ClientResponseType(typeof (List<string>), "GetBuildLogLines", "application/json")]
    [MethodInformation(TimeoutSeconds = 1800)]
    [PublicProjectRequestRestrictions]
    public override HttpResponseMessage GetBuildLog(
      int buildId,
      int logId,
      long? startLine = null,
      long? endLine = null)
    {
      return base.GetBuildLog(buildId, logId, startLine, endLine);
    }
  }
}
