// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Plugins.Common.Utils.ProjectBacklogPermissionUtils
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Agile.Server.Services;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WebPlatform.Server;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Plugins.Common.Utils
{
  public static class ProjectBacklogPermissionUtils
  {
    public static bool HasAdvanceBacklogManagementPermission(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      IAgileProjectPermissionService service = requestContext.GetService<IAgileProjectPermissionService>();
      Microsoft.TeamFoundation.Core.WebApi.ProjectInfo project = requestContext.GetService<IRequestProjectService>().GetProject(requestContext);
      return project != null && service.GetAdvanceBacklogManagementPermission(requestContext, project.Id);
    }
  }
}
