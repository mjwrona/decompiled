// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitBlobProviderService
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class GitBlobProviderService : IVssFrameworkService
  {
    private const string c_layer = "GitBlobProviderService";

    public void ServiceStart(IVssRequestContext systemRC) => systemRC.TraceBlock(1013022, 14279, 14278, GitServerUtils.TraceArea, nameof (GitBlobProviderService), (Action) (() => this.BlobProvider = this.GetBlobProvider(systemRC)), nameof (ServiceStart));

    public void ServiceEnd(IVssRequestContext systemRC)
    {
    }

    protected virtual ITfsGitBlobProvider GetBlobProvider(IVssRequestContext systemRC) => systemRC.ExecutionEnvironment.IsOnPremisesDeployment ? (ITfsGitBlobProvider) new FileContainerGitBlobProvider() : (ITfsGitBlobProvider) new AzureGitBlobProvider();

    public ITfsGitBlobProvider BlobProvider { get; private set; }
  }
}
