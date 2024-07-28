// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.BuildArtifacts4Controller
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
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "artifacts", ResourceVersion = 3)]
  public class BuildArtifacts4Controller : BuildArtifacts3Controller
  {
    [HttpGet]
    [PublicProjectRequestRestrictions]
    public override List<BuildArtifact> GetArtifacts(int buildId) => base.GetArtifacts(buildId);

    [HttpGet]
    [ClientResponseType(typeof (BuildArtifact), null, null)]
    [ClientResponseType(typeof (Stream), "GetArtifactContentZip", "application/zip")]
    [PublicProjectRequestRestrictions]
    public override HttpResponseMessage GetArtifact(int buildId, [ClientQueryParameter] string artifactName) => base.GetArtifact(buildId, artifactName);
  }
}
