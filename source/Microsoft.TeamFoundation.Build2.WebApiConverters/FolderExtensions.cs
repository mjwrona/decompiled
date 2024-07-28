// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.FolderExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public static class FolderExtensions
  {
    public static Microsoft.TeamFoundation.Build.WebApi.Folder ToWebApiFolder(
      this Microsoft.TeamFoundation.Build2.Server.Folder srvFolder,
      IVssRequestContext requestContext,
      IdentityMap identityMap = null)
    {
      if (identityMap == null)
        identityMap = new IdentityMap(requestContext.GetService<IdentityService>());
      if (srvFolder == null)
        return (Microsoft.TeamFoundation.Build.WebApi.Folder) null;
      return new Microsoft.TeamFoundation.Build.WebApi.Folder()
      {
        Path = srvFolder.Path,
        Description = srvFolder.Description,
        CreatedOn = srvFolder.CreatedOn,
        CreatedBy = identityMap.GetIdentityRef(requestContext, srvFolder.CreatedBy),
        LastChangedDate = srvFolder.LastChangedDate,
        LastChangedBy = identityMap.GetIdentityRef(requestContext, srvFolder.LastChangedBy),
        Project = requestContext.GetTeamProjectReference(srvFolder.ProjectId)
      };
    }

    public static Microsoft.TeamFoundation.Build2.Server.Folder ToBuildServerFolder(
      this Microsoft.TeamFoundation.Build.WebApi.Folder webApiFolder)
    {
      if (webApiFolder == null)
        return (Microsoft.TeamFoundation.Build2.Server.Folder) null;
      Microsoft.TeamFoundation.Build2.Server.Folder buildServerFolder = new Microsoft.TeamFoundation.Build2.Server.Folder()
      {
        Path = webApiFolder.Path,
        Description = webApiFolder.Description,
        CreatedOn = webApiFolder.CreatedOn,
        LastChangedDate = webApiFolder.LastChangedDate,
        LastChangedBy = Guid.Empty,
        CreatedBy = Guid.Empty
      };
      if (webApiFolder.Project != null)
        buildServerFolder.ProjectId = webApiFolder.Project.Id;
      return buildServerFolder;
    }
  }
}
