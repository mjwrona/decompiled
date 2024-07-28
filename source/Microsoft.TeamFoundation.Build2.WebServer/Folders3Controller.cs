// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.Folders3Controller
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  [ControllerApiVersion(3.0)]
  [VersionedApiControllerCustomName(Area = "build", ResourceName = "folders", ResourceVersion = 1)]
  public class Folders3Controller : BuildApiController
  {
    [HttpPut]
    public Microsoft.TeamFoundation.Build.WebApi.Folder CreateFolder([ClientQueryParameter] string path, Microsoft.TeamFoundation.Build.WebApi.Folder folder)
    {
      this.CheckRequestContent((object) folder);
      this.ValidateFolder(folder);
      IdentityMap identityMap = new IdentityMap(this.TfsRequestContext.GetService<IdentityService>());
      return this.TfsRequestContext.GetService<IBuildFolderService>().AddFolder(this.TfsRequestContext, this.ProjectId, path, folder.ToBuildServerFolder()).ToWebApiFolder(this.TfsRequestContext, identityMap);
    }

    [HttpGet]
    public virtual List<Microsoft.TeamFoundation.Build.WebApi.Folder> GetFolders(
      string path = "\\",
      Microsoft.TeamFoundation.Build.WebApi.FolderQueryOrder queryOrder = Microsoft.TeamFoundation.Build.WebApi.FolderQueryOrder.None)
    {
      IList<Microsoft.TeamFoundation.Build2.Server.Folder> folders = this.TfsRequestContext.GetService<IBuildFolderService>().GetFolders(this.TfsRequestContext, this.ProjectId, path + "\\**\\*", (Microsoft.TeamFoundation.Build2.Server.FolderQueryOrder) queryOrder);
      IdentityMap identityMap = new IdentityMap(this.TfsRequestContext.GetService<IdentityService>());
      Func<Microsoft.TeamFoundation.Build2.Server.Folder, Microsoft.TeamFoundation.Build.WebApi.Folder> selector = (Func<Microsoft.TeamFoundation.Build2.Server.Folder, Microsoft.TeamFoundation.Build.WebApi.Folder>) (x => x.ToWebApiFolder(this.TfsRequestContext, identityMap));
      return folders.Select<Microsoft.TeamFoundation.Build2.Server.Folder, Microsoft.TeamFoundation.Build.WebApi.Folder>(selector).ToList<Microsoft.TeamFoundation.Build.WebApi.Folder>();
    }

    [HttpPost]
    public Microsoft.TeamFoundation.Build.WebApi.Folder UpdateFolder([ClientQueryParameter] string path, Microsoft.TeamFoundation.Build.WebApi.Folder folder)
    {
      this.CheckRequestContent((object) folder);
      this.ValidateFolder(folder);
      IdentityMap identityMap = new IdentityMap(this.TfsRequestContext.GetService<IdentityService>());
      return this.TfsRequestContext.GetService<IBuildFolderService>().UpdateFolder(this.TfsRequestContext, this.ProjectId, path, folder.ToBuildServerFolder()).ToWebApiFolder(this.TfsRequestContext, identityMap);
    }

    [HttpDelete]
    public void DeleteFolder([ClientQueryParameter] string path) => this.TfsRequestContext.GetService<IBuildFolderService>().DeleteFolder(this.TfsRequestContext, this.ProjectId, path);

    private void ValidateFolder(Microsoft.TeamFoundation.Build.WebApi.Folder folder)
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Build.WebApi.Folder>(folder, nameof (folder));
      if (folder.Project == null)
        folder.Project = new TeamProjectReference()
        {
          Id = this.ProjectId
        };
      else if (folder.Project.Id != this.ProjectId)
        throw new RouteIdConflictException(Resources.WrongProjectSpecifiedForFolder((object) this.ProjectId, (object) folder.Project.Id));
    }
  }
}
