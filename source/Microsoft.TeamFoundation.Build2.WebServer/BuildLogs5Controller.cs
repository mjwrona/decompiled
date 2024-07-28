// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildLogs5Controller
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "logs", ResourceVersion = 2)]
  public class BuildLogs5Controller : BuildLogs4Controller
  {
    [HttpGet]
    [ClientResponseType(typeof (Stream), "GetBuildLog", "text/plain")]
    [ClientResponseType(typeof (Stream), "GetBuildLogZip", "application/zip")]
    [ClientResponseType(typeof (List<string>), "GetBuildLogLines", "application/json")]
    [MethodInformation(TimeoutSeconds = 1800)]
    [PublicProjectRequestRestrictions]
    public override HttpResponseMessage GetBuildLog(
      int buildId,
      int logId,
      long? startLine = null,
      long? endLine = null)
    {
      int buildId1 = buildId;
      int logId1 = logId;
      List<RequestMediaType> supportedTypes = new List<RequestMediaType>();
      supportedTypes.Add(RequestMediaType.Json);
      supportedTypes.Add(RequestMediaType.Text);
      supportedTypes.Add(RequestMediaType.Zip);
      long? startLine1 = startLine;
      long? endLine1 = endLine;
      return this.GetBuildLogInternal(buildId1, logId1, supportedTypes, startLine1, endLine1);
    }
  }
}
