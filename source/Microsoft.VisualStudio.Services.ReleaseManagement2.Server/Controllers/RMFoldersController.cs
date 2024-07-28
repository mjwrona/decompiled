// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers.RMFoldersController
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Controllers
{
  [ControllerApiVersion(3.1)]
  [VersionedApiControllerCustomName(Area = "Release", ResourceName = "folders")]
  public class RMFoldersController : ReleaseManagementProjectControllerBase
  {
    [HttpPost]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.None)]
    [Obsolete("This method is no longer supported. Use CreateFolder with folder parameter API.")]
    public Folder CreateFolder(string path, Folder folder)
    {
      if (folder == null)
        throw new InvalidRequestException(Resources.FolderCannotBeNull);
      return this.GetService<FolderService>().AddFolder(this.TfsRequestContext, this.ProjectId, path, folder);
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "path is optional")]
    [HttpGet]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.None)]
    [PublicProjectRequestRestrictions]
    public IEnumerable<Folder> GetFolders(string path = "\\", FolderPathQueryOrder queryOrder = FolderPathQueryOrder.None)
    {
      IList<Folder> folders = this.GetService<FolderService>().GetFolders(this.TfsRequestContext, this.ProjectId, path, queryOrder);
      foreach (Folder folder in (IEnumerable<Folder>) folders)
        folder.SetSecuredObject(this.ProjectId);
      return (IEnumerable<Folder>) folders;
    }

    [HttpPatch]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.None)]
    public Folder UpdateFolder(string path, [FromBody] Folder folder) => this.GetService<FolderService>().UpdateFolder(this.TfsRequestContext, this.ProjectId, path, folder);

    [HttpDelete]
    [ReleaseManagementSecurityPermission(ReleaseManagementSecurityPermissions.None)]
    public void DeleteFolder(string path) => this.GetService<FolderService>().DeleteFolder(this.TfsRequestContext, this.ProjectId, path);
  }
}
