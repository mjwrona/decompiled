// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.TestExecutionSecurityChecker
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.Test.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [CLSCompliant(false)]
  public class TestExecutionSecurityChecker : ITestExecutionSecurityChecker
  {
    public void CheckManageTestConfigurationsPermission(
      TestExecutionRequestContext context,
      string projectUri)
    {
      if (!this.HasProjectPermission(context, projectUri, AuthorizationProjectPermissions.ManageTestConfigurations))
        throw new TestExecutionAccessDeniedException(TestExecutionServiceResources.CannotManageTestConfigurations);
    }

    public void HasPublishTestResultsPermission(
      TestExecutionRequestContext context,
      string projectUri)
    {
      if (!this.HasProjectPermission(context, projectUri, AuthorizationProjectPermissions.PublishTestResults))
        throw new TestExecutionAccessDeniedException(TestExecutionServiceResources.CannotPublishTestResults);
    }

    public void HasGenericReadPermission(TestExecutionRequestContext context, string projectUri)
    {
      if (!this.HasProjectPermission(context, projectUri, AuthorizationProjectPermissions.GenericRead))
        throw new TestExecutionAccessDeniedException(TestExecutionServiceResources.CannotReadProject);
    }

    private bool HasProjectPermission(
      TestExecutionRequestContext requestContext,
      string projectUri,
      int permission)
    {
      IVssSecurityNamespace securityNamespace = requestContext.RequestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext.RequestContext, FrameworkSecurity.TeamProjectNamespaceId);
      string token1 = TeamProjectSecurityConstants.GetToken(projectUri);
      IVssRequestContext requestContext1 = requestContext.RequestContext;
      string token2 = token1;
      int requestedPermissions = permission;
      int num = securityNamespace.HasPermission(requestContext1, token2, requestedPermissions) ? 1 : 0;
      if (num != 0)
        return num != 0;
      requestContext.RequestContext.Trace(0, TraceLevel.Info, TestExecutionServiceConstants.TestExecutionServiceArea, TestExecutionServiceConstants.ControllerLayer, "HasProjectPermission failed for {0}. projectUri = {1}.", (object) permission, (object) projectUri);
      return num != 0;
    }
  }
}
