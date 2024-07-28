// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Widgets.Utilities.OtherLinksInfo
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Widgets, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DD4C24BB-2646-4C82-B0E8-494FC53AC01D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Widgets.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.Widgets.Utilities
{
  public class OtherLinksInfo : ISecuredObject
  {
    public bool showTeamLinks;
    public string portalUrl;
    public string guidanceUrl;
    public string reportUrl;
    public int feedbackMode;
    public bool userHasTeamWritePermission;
    private Guid m_teamProjectId;

    public Guid NamespaceId => TeamProjectSecurityConstants.NamespaceId;

    public int RequiredPermissions => TeamProjectSecurityConstants.GenericRead;

    public string GetToken() => TeamProjectSecurityConstants.GetToken(ProjectInfo.GetProjectUri(this.m_teamProjectId));

    public OtherLinksInfo(Guid teamProjectId) => this.m_teamProjectId = teamProjectId;

    public static OtherLinksInfo CreateAndCheckPermission(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      OtherLinksInfo andCheckPermission = new OtherLinksInfo(projectId);
      IVssSecurityNamespace securityNamespace = requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, andCheckPermission.NamespaceId);
      DefaultSecurityNamespaceExtension.Instance.HasReadPermission(requestContext, securityNamespace, andCheckPermission.GetToken());
      return andCheckPermission;
    }
  }
}
