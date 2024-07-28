// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildLogs2Controller
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(2.3)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "logs", ResourceVersion = 2)]
  public class BuildLogs2Controller : BuildLogsController
  {
    [HttpGet]
    [ClientResponseType(typeof (List<string>), "GetBuildLogJson", "application/json")]
    [ClientResponseType(typeof (Stream), "GetBuildLogStream", "text/plain")]
    public override HttpResponseMessage GetBuildLog(
      int buildId,
      int logId,
      long? startLine = null,
      long? endLine = null,
      DefinitionType? type = null)
    {
      RequestMediaType formatRequested = MediaTypeFormatUtility.GetPrioritizedAcceptHeaders(this.Request, new List<RequestMediaType>()
      {
        RequestMediaType.Json,
        RequestMediaType.Text
      }).FirstOrDefault<RequestMediaType>();
      return this.GetBuildLog(buildId, logId, startLine, endLine, type, formatRequested);
    }
  }
}
