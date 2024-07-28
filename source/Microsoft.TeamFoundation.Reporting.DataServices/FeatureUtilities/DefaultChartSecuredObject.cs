// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.FeatureUtilities.DefaultChartSecuredObject
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.Reporting.DataServices.FeatureUtilities
{
  public class DefaultChartSecuredObject : ISecuredObject
  {
    private Guid m_teamProjectId;

    public DefaultChartSecuredObject(Guid teamProjectId) => this.m_teamProjectId = teamProjectId;

    public Guid NamespaceId => TeamProjectSecurityConstants.NamespaceId;

    public int RequiredPermissions => TeamProjectSecurityConstants.GenericRead;

    public string GetToken() => TeamProjectSecurityConstants.GetToken(ProjectInfo.GetProjectUri(this.m_teamProjectId));

    public static DefaultChartSecuredObject GenerateAndCheckProjectReadObject(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      DefaultChartSecuredObject projectReadObject = new DefaultChartSecuredObject(projectId);
      IVssSecurityNamespace securityNamespace = requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, projectReadObject.NamespaceId);
      DefaultSecurityNamespaceExtension.Instance.HasReadPermission(requestContext, securityNamespace, projectReadObject.GetToken());
      return projectReadObject;
    }
  }
}
