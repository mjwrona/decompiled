// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.RMFolders2Controller
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "folders", ResourceVersion = 2)]
  public class RMFolders2Controller : RMFoldersController
  {
    [HttpPost]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.None)]
    public Folder CreateFolder([FromBody] Folder folder)
    {
      if (folder == null)
        throw new InvalidRequestException(Resources.FolderCannotBeNull);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(folder.Path, "Path");
      return this.GetService<FolderService>().AddFolder(this.TfsRequestContext, this.ProjectId, folder.Path, folder);
    }
  }
}
