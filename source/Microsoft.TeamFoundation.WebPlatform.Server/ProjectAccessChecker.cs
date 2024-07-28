// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WebPlatform.Server.ProjectAccessChecker
// Assembly: Microsoft.TeamFoundation.WebPlatform.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BDF91478-A3ED-4B5B-AA51-9473C7AE6182
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WebPlatform.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;

namespace Microsoft.TeamFoundation.WebPlatform.Server
{
  public class ProjectAccessChecker : IProjectAccessChecker
  {
    public void CheckProjectAccess(IVssRequestContext tfsRequestContext, ProjectInfo project)
    {
      IVssSecurityNamespace securityNamespace = tfsRequestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(tfsRequestContext, FrameworkSecurity.TeamProjectNamespaceId);
      string securityToken = tfsRequestContext.GetService<IProjectService>().GetSecurityToken(tfsRequestContext, project.Uri.ToString());
      securityNamespace.CheckPermission(tfsRequestContext, securityToken, securityNamespace.Description.ReadPermission);
    }
  }
}
