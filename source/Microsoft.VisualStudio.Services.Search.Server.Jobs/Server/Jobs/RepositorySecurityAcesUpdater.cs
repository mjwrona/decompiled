// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.RepositorySecurityAcesUpdater
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Search.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public class RepositorySecurityAcesUpdater : SecurityAcesUpdaterBase
  {
    public RepositorySecurityAcesUpdater(ExecutionContext executionContext)
      : base(executionContext)
    {
      this.SecurityHashComputationKpiName = "RepositorySecurityHashComputationTime";
    }

    protected override void LoadSecurityNamespace(ExecutionContext executionContext) => SecurityChecksUtils.LoadRemoteSecurityNamespace(this.ExecutionContext.RequestContext, GitConstants.GitSecurityNamespaceId);

    public virtual byte[] GetSecurityHashCodeForRepository(Guid repositoryId, Guid projectId) => this.GetSecurityHashCodeWithFaultCheck((Func<IEnumerable<IAccessControlEntry>>) (() => SecurityChecksUtils.GetRepositoryAces(this.ExecutionContext.RequestContext, repositoryId, projectId)), 2);
  }
}
