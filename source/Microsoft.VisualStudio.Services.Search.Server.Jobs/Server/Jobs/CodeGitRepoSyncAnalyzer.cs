// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.CodeGitRepoSyncAnalyzer
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Code;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public class CodeGitRepoSyncAnalyzer : GitRepoSyncAnalyzer
  {
    public CodeGitRepoSyncAnalyzer(
      ExecutionContext executionContext,
      TraceMetaData traceMetaData,
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler)
      : base(executionContext, traceMetaData, indexingUnitChangeEventHandler)
    {
    }

    internal CodeGitRepoSyncAnalyzer(
      ExecutionContext executionContext,
      TraceMetaData traceMetaData,
      IDataAccessFactory dataAccessFactory,
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler,
      IndexMetadataStateAnalyser indexMetadataStateAnalyser)
      : base(executionContext, traceMetaData, dataAccessFactory, indexingUnitChangeEventHandler, indexMetadataStateAnalyser)
    {
    }

    internal override EntityFinalizerBase FinalizeHelper => (EntityFinalizerBase) new CollectionCodeFinalizeHelper();

    internal override List<GitRepository> GetTfsRepos()
    {
      IEnumerable<GitRepository> repositoriesAsync = this.GitHttpClient.GetRepositoriesAsync();
      return repositoriesAsync == null ? (List<GitRepository>) null : repositoriesAsync.ToList<GitRepository>();
    }

    protected override IEntityType EntityType => (IEntityType) CodeEntityType.GetInstance();
  }
}
