// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestExecution.Server.DefaultTestExecutionSecurityManager
// Assembly: Microsoft.VisualStudio.Services.TE.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BC2680F-A5FB-41BE-A4CF-F78BF7AC3E02
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.TE.Server.Common.dll

using System;

namespace Microsoft.TeamFoundation.TestExecution.Server
{
  [CLSCompliant(false)]
  public class DefaultTestExecutionSecurityManager : ISecurityManager
  {
    private ITestExecutionSecurityChecker _securityChecker;

    public DefaultTestExecutionSecurityManager() => this.SecurityChecker = (ITestExecutionSecurityChecker) new TestExecutionSecurityChecker();

    public DefaultTestExecutionSecurityManager(ITestExecutionSecurityChecker securityChecker) => this.SecurityChecker = securityChecker;

    public ITestExecutionSecurityChecker SecurityChecker
    {
      internal get => this._securityChecker;
      set => this._securityChecker = value;
    }

    public void CheckPermission(
      TestExecutionRequestContext context,
      DTAPermissionType type,
      string projectUri)
    {
      switch (type)
      {
        case DTAPermissionType.AgentsCreate:
        case DTAPermissionType.AgentsDelete:
        case DTAPermissionType.AutomationRunsCreate:
          this._securityChecker.CheckManageTestConfigurationsPermission(context, projectUri);
          break;
        case DTAPermissionType.AgentsGet:
          this._securityChecker.HasGenericReadPermission(context, projectUri);
          break;
        case DTAPermissionType.SlicesUpdate:
          this._securityChecker.HasPublishTestResultsPermission(context, projectUri);
          break;
      }
    }
  }
}
