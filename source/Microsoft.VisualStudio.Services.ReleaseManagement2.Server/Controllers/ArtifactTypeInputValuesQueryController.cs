// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.ArtifactTypeInputValuesQueryController
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(2.2)]
  [ClientInternalUseOnly(false)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "inputvaluesquery")]
  public class ArtifactTypeInputValuesQueryController : ReleaseManagementProjectControllerBase
  {
    [HttpPost]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.None)]
    public InputValuesQuery GetInputValues([FromBody] InputValuesQuery query)
    {
      if (query == null)
        throw new InvalidRequestException(Resources.QueryCannotBeNull);
      return this.TfsRequestContext.GetService<ArtifactTypeInputValuesQueryService>().GetInputValues(this.TfsRequestContext, this.ProjectInfo, query);
    }
  }
}
