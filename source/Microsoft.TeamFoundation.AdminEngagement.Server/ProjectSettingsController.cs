// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.AdminEngagement.WebApi.ProjectSettingsController
// Assembly: Microsoft.TeamFoundation.AdminEngagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4DC53F57-597F-449E-A165-8D6CA5E396C1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.AdminEngagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System.Web.Http;

namespace Microsoft.TeamFoundation.AdminEngagement.WebApi
{
  [ControllerApiVersion(5.1)]
  [VersionedApiControllerCustomName(Area = "ProjectSettings", ResourceName = "Project")]
  public class ProjectSettingsController : TfsProjectApiController
  {
    [HttpGet]
    [ActionName("GetProjectAdminString")]
    public AdminModel GetProjectAdminString() => new AdminModel()
    {
      AdminMessage = ""
    };
  }
}
