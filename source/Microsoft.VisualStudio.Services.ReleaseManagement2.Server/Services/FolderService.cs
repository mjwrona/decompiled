// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.FolderService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Diagnostics;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Security;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
  public class FolderService : ReleaseManagement2ServiceBase
  {
    public const string RootFolderPath = "\\";
    private const string RootFolderRecursivePath = "\\**\\*";

    [StaticSafe]
    public static IList<char> PathDelimiters => (IList<char>) new char[2]
    {
      '\\',
      '/'
    };

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
    public Folder AddFolder(
      IVssRequestContext context,
      Guid projectId,
      string path,
      Folder folder)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      path = FolderValidator.ValidateAndSanitizePath(path);
      FolderValidator.CheckForFolderNamesWithOnlyDigits(context, path);
      if (folder == null)
        throw new ArgumentNullException(nameof (folder));
      using (ReleaseManagementTimer.Create(context, "Service", "FolderService.AddFolder", 1961225))
      {
        Func<FolderSqlComponent, Folder> action = (Func<FolderSqlComponent, Folder>) (component => component.AddFolder(projectId, path, folder, context.GetUserId(true)));
        Folder folder1 = context.ExecuteWithinUsingWithComponent<FolderSqlComponent, Folder>(action);
        FolderService.ResolveFolderIdentities(context, (IList<Folder>) new List<Folder>()
        {
          folder1
        });
        return folder1;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "optional param")]
    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
    public virtual IList<Folder> GetFolders(
      IVssRequestContext context,
      Guid projectId,
      string path,
      FolderPathQueryOrder queryOrder = FolderPathQueryOrder.None)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      path = !string.Equals(path, "\\", StringComparison.OrdinalIgnoreCase) ? FolderValidator.ValidateAndSanitizePath(path, true) : "\\**\\*";
      using (ReleaseManagementTimer.Create(context, "Service", "FolderService.GetFolder", 1961225))
      {
        Func<FolderSqlComponent, IList<Folder>> action = (Func<FolderSqlComponent, IList<Folder>>) (component => component.GetFolders(projectId, path, queryOrder));
        IList<Folder> folders = context.ExecuteWithinUsingWithComponent<FolderSqlComponent, IList<Folder>>(action);
        FolderService.ResolveFolderIdentities(context, folders);
        return folders;
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
    public Folder UpdateFolder(
      IVssRequestContext context,
      Guid projectId,
      string path,
      Folder folder)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      path = FolderValidator.ValidateAndSanitizePath(path, allowRootPath: false);
      folder.Path = FolderValidator.ValidateAndSanitizePath(folder.Path, allowRootPath: false);
      FolderValidator.CheckForFolderNamesWithOnlyDigits(context, folder.Path);
      Folder originalFolder = this.GetFolders(context, projectId, path).FirstOrDefault<Folder>();
      if (originalFolder == null)
        throw new FolderNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.FolderNotFound, (object) path));
      using (ReleaseManagementTimer.Create(context, "Service", "FolderService.UpdateFolder", 1961225))
      {
        Guid currentIdentity = context.GetUserId(true);
        Func<FolderSqlComponent, Folder> action = (Func<FolderSqlComponent, Folder>) (component => component.UpdateFolder(projectId, path, folder, currentIdentity, ReleaseManagementSecurityHelper.GetToken(projectId, originalFolder.Path), ReleaseManagementSecurityHelper.GetToken(projectId, folder.Path)));
        Folder folder1 = context.ExecuteWithinUsingWithComponent<FolderSqlComponent, Folder>(action);
        FolderService.ResolveFolderIdentities(context, (IList<Folder>) new List<Folder>()
        {
          folder1
        });
        return folder1;
      }
    }

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "As per VSSF service design this must be an instance member.")]
    public void DeleteFolder(IVssRequestContext context, Guid projectId, string path)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      path = FolderValidator.ValidateAndSanitizePath(path, allowRootPath: false);
      using (ReleaseManagementTimer.Create(context, "Service", "FolderService.UpdateFolder", 1961225))
      {
        Guid currentIdentity = context.GetUserId(true);
        string str = path;
        if (!str.EndsWith("\\", StringComparison.OrdinalIgnoreCase))
          str += "\\";
        string path1 = str + "**\\*\\";
        IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition> releaseDefinitions = context.GetService<ReleaseDefinitionsService>().GetReleaseDefinitions(context, projectId, (string) null, (string) null, (IEnumerable<string>) null, false, ReleaseDefinitionExpands.None, new DateTime?(), path: path1);
        if (releaseDefinitions == null)
          return;
        try
        {
          FolderService.DeleteDefinitions(context, projectId, releaseDefinitions, path);
          Action<FolderSqlComponent> action = (Action<FolderSqlComponent>) (component => component.DeleteFolder(projectId, path, currentIdentity));
          context.ExecuteWithinUsingWithComponent<FolderSqlComponent>(action);
        }
        catch (ReleaseFolderOperationException ex)
        {
          throw ex;
        }
      }
    }

    private static void DeleteDefinitions(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition> releaseDefinitions,
      string folderName)
    {
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition releaseDefinition in releaseDefinitions)
      {
        if (!requestContext.HasPermission(ReleaseManagementSecurityHelper.GetToken(projectId, releaseDefinition.Path, releaseDefinition.Id), ReleaseManagementSecurityPermissions.DeleteReleaseDefinition))
          throw new ReleaseFolderOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.FolderDeleteException, (object) folderName, (object) requestContext.GetRequestingUserDisplayName(), (object) releaseDefinition.Name));
      }
      foreach (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseDefinition releaseDefinition in releaseDefinitions)
        requestContext.GetService<ReleaseDefinitionsService>().SoftDeleteReleaseDefinition(requestContext, projectId, releaseDefinition.Id, "FolderDeleteOperation", true);
    }

    private static void ResolveFolderIdentities(IVssRequestContext context, IList<Folder> folders)
    {
      if (folders == null)
        return;
      string strB = Guid.Empty.ToString();
      HashSet<string> teamFoundationIds = new HashSet<string>(folders.Count);
      foreach (Folder folder in (IEnumerable<Folder>) folders)
      {
        if (folder != null)
        {
          if (folder.CreatedBy?.Id != null && string.Compare(folder.CreatedBy.Id, strB, StringComparison.OrdinalIgnoreCase) != 0)
            teamFoundationIds.Add(folder.CreatedBy.Id);
          if (folder.LastChangedBy?.Id != null && string.Compare(folder.LastChangedBy.Id, strB, StringComparison.OrdinalIgnoreCase) != 0)
            teamFoundationIds.Add(folder.LastChangedBy.Id);
        }
      }
      Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.IdentityHelper identityHelper = Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.IdentityHelper.GetIdentityHelper(context, (ICollection<string>) teamFoundationIds);
      foreach (Folder folder in (IEnumerable<Folder>) folders)
      {
        if (folder != null)
        {
          folder.CreatedBy = identityHelper.GetIdentity(folder.CreatedBy);
          if (folder.LastChangedBy != null)
            folder.LastChangedBy = identityHelper.GetIdentity(folder.LastChangedBy);
        }
      }
    }
  }
}
