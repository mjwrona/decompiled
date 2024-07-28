// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Pipeline.CodePipelineFailureHandler
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Maps;
using Microsoft.VisualStudio.Services.Search.Crawler.MetaDataCrawler.MetaDataCrawlSeed;
using Microsoft.VisualStudio.Services.Search.Indexer;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Api;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using System;
using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Pipeline
{
  [Export(typeof (ICorePipelineFailureHandler))]
  public class CodePipelineFailureHandler : IndexerBasePipelineFailureHandler
  {
    public override IEntityType SupportedEntityType => (IEntityType) CodeEntityType.GetInstance();

    public override bool IsItemLevelPersistenceSupported(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      return ((IndexingExecutionContext) coreIndexingExecutionContext).RepositoryIndexingUnit != null;
    }

    public override bool HandleError(
      CoreIndexingExecutionContext coreIndexingExecutionContext,
      Exception exception)
    {
      if (exception == null)
        throw new ArgumentNullException(nameof (exception));
      bool flag = false;
      if (IndexFaultMapManager.GetFaultMapper(typeof (ProjectNotFoundFaultMapper)).IsMatch(exception))
      {
        flag = true;
        coreIndexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Project has been deleted, queuing {0} to fix this.", (object) "PeriodicCatchUpJob")));
      }
      if (IndexFaultMapManager.GetFaultMapper(typeof (RepoDoesNotExistFaultMapper)).IsMatch(exception))
      {
        flag = true;
        coreIndexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Repository has been deleted, queuing {0} to fix this.", (object) "PeriodicCatchUpJob")));
      }
      if (IndexFaultMapManager.GetFaultMapper(typeof (TfvcScopePathConflictFaultMapper)).IsMatch(exception))
      {
        flag = true;
        coreIndexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("TFVC Scope Path conflict error detected, queuing {0} to fix this.", (object) "PeriodicCatchUpJob")));
      }
      if (IndexFaultMapManager.GetFaultMapper(typeof (TfsBranchNotFoundFaultMapper)).IsMatch(exception))
      {
        flag = true;
        coreIndexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Branch not found in TFS, queuing {0} to fix this.", (object) "PeriodicCatchUpJob")));
      }
      if (exception.GetBaseException() is BranchNotFoundException baseException)
      {
        flag = true;
        coreIndexingExecutionContext.Log.Append(FormattableString.Invariant(FormattableStringFactory.Create("Requested Branch {0} not found in repository, queuing {1} to fix this.", (object) baseException.BranchName, (object) "PeriodicCatchUpJob")));
      }
      if (flag)
        this.QueuePeriodicCatchUpJob(coreIndexingExecutionContext);
      return flag;
    }

    internal virtual void QueuePeriodicCatchUpJob(
      CoreIndexingExecutionContext coreIndexingExecutionContext)
    {
      coreIndexingExecutionContext.RequestContext.QueuePeriodicCatchUpJob(coreIndexingExecutionContext.ServiceSettings.JobSettings.PeriodicCatchUpJobDelayInSec);
    }
  }
}
