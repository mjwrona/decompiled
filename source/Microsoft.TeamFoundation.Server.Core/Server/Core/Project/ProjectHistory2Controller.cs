// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Project.ProjectHistory2Controller
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Server.Core.Project
{
  [VersionedApiControllerCustomName("core", "projectHistory", 2)]
  [ControllerApiVersion(4.0)]
  public class ProjectHistory2Controller : ServerCoreApiController
  {
    [HttpGet]
    [ClientInternalUseOnly(false)]
    [ClientExample("LIST__ProjectHistory_minrevision_.json", "Get project history by revision number", null, null)]
    public IEnumerable<ProjectInfo> GetProjectHistoryEntries(long minRevision = 0) => ProjectsUtility.GetProjectHistoryEntries(this.TfsRequestContext, minRevision);
  }
}
