// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildFolderService
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public sealed class BuildFolderService : IBuildFolderService, IVssFrameworkService
  {
    public Folder AddFolder(
      IVssRequestContext requestContext,
      Guid projectId,
      string path,
      Folder folder)
    {
      using (requestContext.TraceScope("Service", nameof (AddFolder)))
      {
        this.ValidatePath(ref path);
        bool allowNumericFolderNames = requestContext.GetService<ITeamFoundationFeatureAvailabilityService>().IsFeatureEnabled(requestContext, "Build2.Service.AllowNumericFolderNames");
        this.ValidateFolderName(requestContext, projectId, path, allowNumericFolderNames);
        Guid userId = requestContext.GetUserId(true);
        Folder folder1;
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          folder1 = component.AddFolder(projectId, path, folder, userId);
        return folder1;
      }
    }

    public IList<Folder> GetFolders(
      IVssRequestContext requestContext,
      Guid projectId,
      string path,
      FolderQueryOrder queryOrder)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      using (requestContext.TraceScope("Service", nameof (GetFolders)))
      {
        if (path == "\\")
          path = "\\**\\*";
        else
          this.ValidatePath(ref path, true);
        IList<Folder> folders;
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          folders = component.GetFolders(projectId, path, queryOrder);
        return folders;
      }
    }

    public Folder UpdateFolder(
      IVssRequestContext requestContext,
      Guid projectId,
      string path,
      Folder folder)
    {
      using (requestContext.TraceScope("Service", nameof (UpdateFolder)))
      {
        if (requestContext.GetService<ITeamFoundationFeatureAvailabilityService>().IsFeatureEnabled(requestContext, "Build2.Service.DisallowToEditFolderWithoutPermissions"))
        {
          try
          {
            this.GetAllDefinitionsForAction(requestContext, projectId, path, BuildPermissions.ViewBuildDefinition);
            this.GetAllDefinitionsForAction(requestContext, projectId, path, BuildPermissions.EditBuildDefinition);
          }
          catch (AccessDeniedException ex)
          {
            throw new AccessDeniedException(BuildServerResources.CannotUpdateFolderDueToLackOfPermissions((object) path));
          }
        }
        this.ValidateFolder(folder);
        this.ValidatePath(ref path, allowRootPath: false);
        Folder folder1 = this.GetFolders(requestContext, folder.ProjectId, path, FolderQueryOrder.None).FirstOrDefault<Folder>();
        if (folder1 == null)
          throw new FolderNotFoundException(BuildServerResources.FolderNotFound((object) path));
        bool allowNumericFolderNames = requestContext.GetService<ITeamFoundationFeatureAvailabilityService>().IsFeatureEnabled(requestContext, "Build2.Service.AllowNumericFolderNames");
        if (folder.Path != null && folder1.Path != folder.Path)
          this.ValidateFolderName(requestContext, folder.ProjectId, folder.Path, allowNumericFolderNames);
        Guid userId = requestContext.GetUserId(true);
        Folder folder2 = (Folder) null;
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          folder2 = component.UpdateFolder(path, folder, userId, folder1.GetSecurityToken(), folder.GetSecurityToken());
        return folder2;
      }
    }

    public void DeleteFolder(IVssRequestContext requestContext, Guid projectId, string path)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
      this.ValidatePath(ref path, allowRootPath: false);
      using (requestContext.TraceScope("Service", nameof (DeleteFolder)))
      {
        Guid userId = requestContext.GetUserId(true);
        List<int> definitionsForAction;
        try
        {
          definitionsForAction = this.GetAllDefinitionsForAction(requestContext, projectId, path, BuildPermissions.ViewBuildDefinition);
        }
        catch (AccessDeniedException ex)
        {
          throw new AccessDeniedException(BuildServerResources.CannotDeleteFolderDueToHiddenDefinitions((object) path));
        }
        requestContext.GetService<IBuildDefinitionService>().DeleteDefinitions(requestContext, projectId, definitionsForAction);
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          component.DeleteFolder(projectId, path, userId);
      }
    }

    private List<int> GetAllDefinitionsForAction(
      IVssRequestContext requestContext,
      Guid projectId,
      string path,
      int requiredPermissionForAction)
    {
      string str1 = path;
      if (!str1.EndsWith("\\"))
        str1 += "\\";
      string str2 = str1 + "**\\*\\";
      IVssRequestContext context = requestContext.Elevate();
      IBuildDefinitionService service = context.GetService<IBuildDefinitionService>();
      IVssRequestContext requestContext1 = context;
      Guid projectId1 = projectId;
      string str3 = str2;
      DateTime? minLastModifiedTime = new DateTime?();
      DateTime? maxLastModifiedTime = new DateTime?();
      DateTime? minMetricsTime = new DateTime?();
      string path1 = str3;
      DateTime? builtAfter = new DateTime?();
      DateTime? notBuiltAfter = new DateTime?();
      Guid? taskIdFilter = new Guid?();
      int? processType = new int?();
      IEnumerable<BuildDefinition> definitions = service.GetDefinitions(requestContext1, projectId1, minLastModifiedTime: minLastModifiedTime, maxLastModifiedTime: maxLastModifiedTime, minMetricsTime: minMetricsTime, path: path1, builtAfter: builtAfter, notBuiltAfter: notBuiltAfter, taskIdFilter: taskIdFilter, processType: processType);
      BuildSecurityProvider securityProvider = new BuildSecurityProvider();
      foreach (BuildDefinition definition in definitions)
        securityProvider.CheckDefinitionPermission(requestContext, definition.ProjectId, (MinimalBuildDefinition) definition, requiredPermissionForAction, false);
      return definitions.Select<BuildDefinition, int>((Func<BuildDefinition, int>) (x => x.Id)).ToList<int>();
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }
  }
}
