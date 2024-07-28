// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations.ElasticSearchPlatformBackupOperations
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Nest;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Implementations
{
  internal class ElasticSearchPlatformBackupOperations : 
    ElasticSearchBasePlatform,
    ISearchBackupPlatform
  {
    internal ElasticSearchPlatformBackupOperations(
      string elasticSearchConnectionString,
      string platformSettings,
      bool isOnPrem)
      : base(elasticSearchConnectionString, platformSettings, isOnPrem)
    {
    }

    internal ElasticSearchPlatformBackupOperations(IElasticClient elasticClient)
      : base(elasticClient)
    {
    }

    public CreateRepositoryResponse RegisterBackupRepository(
      ExecutionContext executionContext,
      string repoName)
    {
      if (string.IsNullOrWhiteSpace(repoName))
        throw new ArgumentException("repoName cannot be null or contain only whitespace");
      Tracer.TraceEnter(1082259, "Search Engine", "Search Engine", nameof (RegisterBackupRepository));
      bool flag = false;
      CreateRepositoryResponse response = (CreateRepositoryResponse) null;
      CreateRepositoryRequest registerRepositoryRequest = new CreateRepositoryRequest((Name) repoName)
      {
        Repository = (ISnapshotRepository) new AzureRepository()
        {
          Settings = (IAzureRepositorySettings) new AzureRepositorySettings()
          {
            Compress = new bool?(true),
            Container = executionContext.ServiceSettings.JobSettings.AzureContainerForBackup,
            BasePath = executionContext.ServiceSettings.JobSettings.BasePathInContainer
          }
        }
      };
      try
      {
        response = GenericInvoker.Instance.InvokeWithFaultCheck<CreateRepositoryResponse>((Func<CreateRepositoryResponse>) (() => this.ElasticClient.Snapshot.CreateRepository((ICreateRepositoryRequest) registerRepositoryRequest)), executionContext.FaultService, 2, 1000, new TraceMetaData(1082259, "Search Engine", "Search Engine"));
        flag = true;
      }
      finally
      {
        Tracer.TraceLeave(1082259, "Search Engine", "Search Engine", nameof (RegisterBackupRepository));
        if (flag)
        {
          response.ThrowOnInvalidOrFailedResponse();
          Tracer.TraceInfo(1082259, "Search Engine", "Search Engine", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Registered Repository '{0}'", (object) repoName));
        }
      }
      return response;
    }

    public GetRepositoryResponse GetBackupRepository(
      ExecutionContext executionContext,
      string repoName)
    {
      Tracer.TraceEnter(1082266, "Search Engine", "Search Engine", nameof (GetBackupRepository));
      bool flag = false;
      GetRepositoryResponse response = (GetRepositoryResponse) null;
      GetRepositoryRequest getRepositoryRequest = new GetRepositoryRequest((Names) repoName);
      try
      {
        response = GenericInvoker.Instance.InvokeWithFaultCheck<GetRepositoryResponse>((Func<GetRepositoryResponse>) (() => this.ElasticClient.Snapshot.GetRepository((IGetRepositoryRequest) getRepositoryRequest)), executionContext.FaultService, 2, 1000, new TraceMetaData(1082266, "Search Engine", "Search Engine"));
        flag = true;
      }
      finally
      {
        Tracer.TraceLeave(1082266, "Search Engine", "Search Engine", nameof (GetBackupRepository));
        if (flag)
        {
          response.ThrowOnInvalidOrFailedResponse();
          Tracer.TraceInfo(1082266, "Search Engine", "Search Engine", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Got the response for the repository '{0}' ", (object) repoName));
        }
      }
      return response;
    }

    public GetRepositoryResponse GetAllBackupRepositories(ExecutionContext executionContext)
    {
      Tracer.TraceEnter(1082263, "Search Engine", "Search Engine", nameof (GetAllBackupRepositories));
      bool flag = false;
      GetRepositoryResponse response = (GetRepositoryResponse) null;
      GetRepositoryRequest getAllRepositoriesRequest = new GetRepositoryRequest();
      try
      {
        response = GenericInvoker.Instance.InvokeWithFaultCheck<GetRepositoryResponse>((Func<GetRepositoryResponse>) (() => this.ElasticClient.Snapshot.GetRepository((IGetRepositoryRequest) getAllRepositoriesRequest)), executionContext.FaultService, 2, 1000, new TraceMetaData(1082263, "Search Engine", "Search Engine"));
        flag = true;
      }
      finally
      {
        Tracer.TraceLeave(1082263, "Search Engine", "Search Engine", nameof (GetAllBackupRepositories));
        if (flag)
        {
          response.ThrowOnInvalidOrFailedResponse();
          Tracer.TraceInfo(1082263, "Search Engine", "Search Engine", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Got all registered repositories "));
        }
      }
      return response;
    }

    public DeleteRepositoryResponse DeleteBackupRepository(
      ExecutionContext executionContext,
      string repoName)
    {
      if (string.IsNullOrWhiteSpace(repoName))
        throw new ArgumentException("repoName cannot be null or contain only whitespace");
      Tracer.TraceEnter(1082265, "Search Engine", "Search Engine", nameof (DeleteBackupRepository));
      bool flag = false;
      DeleteRepositoryResponse response = (DeleteRepositoryResponse) null;
      DeleteRepositoryRequest deleteRepositoryRequest = new DeleteRepositoryRequest((Names) repoName);
      try
      {
        response = GenericInvoker.Instance.InvokeWithFaultCheck<DeleteRepositoryResponse>((Func<DeleteRepositoryResponse>) (() => this.ElasticClient.Snapshot.DeleteRepository((IDeleteRepositoryRequest) deleteRepositoryRequest)), executionContext.FaultService, 2, 1000, new TraceMetaData(1082265, "Search Engine", "Search Engine"));
        flag = true;
      }
      finally
      {
        Tracer.TraceLeave(1082265, "Search Engine", "Search Engine", nameof (DeleteBackupRepository));
        if (flag)
        {
          response.ThrowOnInvalidOrFailedResponse();
          Tracer.TraceInfo(1082265, "Search Engine", "Search Engine", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Deleted Repository '{0}'", (object) repoName));
        }
      }
      return response;
    }

    public SnapshotResponse CreateSnapshot(
      ExecutionContext executionContext,
      string repoName,
      string snapshotName)
    {
      if (string.IsNullOrWhiteSpace(repoName))
        throw new ArgumentException("repoName cannot be null or contain only whitespace");
      if (string.IsNullOrWhiteSpace(snapshotName))
        throw new ArgumentException("snapshotName cannot be null or contain only whitespace");
      Tracer.TraceEnter(1082260, "Search Engine", "Search Engine", nameof (CreateSnapshot));
      bool flag = false;
      SnapshotResponse response = (SnapshotResponse) null;
      IReadOnlyCollection<CatIndicesRecord> records = this.GetIndices(executionContext).Records;
      List<IndexName> indices = new List<IndexName>();
      foreach (CatIndicesRecord catIndicesRecord in (IEnumerable<CatIndicesRecord>) records)
      {
        string index = catIndicesRecord.Index;
        if (!index.StartsWith(".marvel-", StringComparison.Ordinal))
          indices.Add((IndexName) index);
      }
      SnapshotRequest createSnapshotRequest = new SnapshotRequest((Name) repoName, (Name) snapshotName)
      {
        Indices = (Indices) Indices.Index((IEnumerable<IndexName>) indices),
        IgnoreUnavailable = new bool?(true),
        IncludeGlobalState = new bool?(false),
        Partial = new bool?(true),
        WaitForCompletion = new bool?(false)
      };
      try
      {
        response = GenericInvoker.Instance.InvokeWithFaultCheck<SnapshotResponse>((Func<SnapshotResponse>) (() => this.ElasticClient.Snapshot.Snapshot((ISnapshotRequest) createSnapshotRequest)), executionContext.FaultService, 2, 1000, new TraceMetaData(1082260, "Search Engine", "Search Engine"));
        flag = true;
      }
      finally
      {
        Tracer.TraceLeave(1082260, "Search Engine", "Search Engine", nameof (CreateSnapshot));
        if (flag)
        {
          response.ThrowOnInvalidOrFailedResponse();
          Tracer.TraceInfo(1082260, "Search Engine", "Search Engine", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Creating Snapshot '{0}' under repository '{1}'", (object) snapshotName, (object) repoName));
        }
      }
      return response;
    }

    public GetSnapshotResponse GetAllSnapshots(ExecutionContext executionContext, string repoName)
    {
      if (string.IsNullOrWhiteSpace(repoName))
        throw new ArgumentException("repoName cannot be null or contain only whitespace");
      Tracer.TraceEnter(1082264, "Search Engine", "Search Engine", nameof (GetAllSnapshots));
      bool flag = false;
      GetSnapshotResponse response = (GetSnapshotResponse) null;
      GetSnapshotRequest getAllSnapshotsRequest = new GetSnapshotRequest((Name) repoName, (Names) "_all");
      try
      {
        response = GenericInvoker.Instance.InvokeWithFaultCheck<GetSnapshotResponse>((Func<GetSnapshotResponse>) (() => this.ElasticClient.Snapshot.Get((IGetSnapshotRequest) getAllSnapshotsRequest)), executionContext.FaultService, 2, 1000, new TraceMetaData(1082264, "Search Engine", "Search Engine"));
        flag = true;
      }
      finally
      {
        Tracer.TraceLeave(1082264, "Search Engine", "Search Engine", nameof (GetAllSnapshots));
        if (flag)
        {
          response.ThrowOnInvalidOrFailedResponse();
          Tracer.TraceInfo(1082264, "Search Engine", "Search Engine", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Got all snapshots under repository '{0}'", (object) repoName));
        }
      }
      return response;
    }

    public Snapshot GetLatestSnapshot(ExecutionContext executionContext, string repoName)
    {
      GetSnapshotResponse allSnapshots = this.GetAllSnapshots(executionContext, repoName);
      return allSnapshots != null && allSnapshots.Snapshots.Any<Snapshot>() ? allSnapshots.Snapshots.OrderByDescending<Snapshot, DateTime>((Func<Snapshot, DateTime>) (i => i.StartTime)).ToList<Snapshot>().First<Snapshot>() : (Snapshot) null;
    }

    public SnapshotStatusResponse GetSnapshotStatus(
      ExecutionContext executionContext,
      string repoName,
      string snapshotName)
    {
      if (string.IsNullOrWhiteSpace(repoName))
        throw new ArgumentException("repoName cannot be null or contain only whitespace");
      if (string.IsNullOrWhiteSpace(snapshotName))
        throw new ArgumentException("snapshotName cannot be null or contain only whitespace");
      Tracer.TraceEnter(1082261, "Search Engine", "Search Engine", nameof (GetSnapshotStatus));
      bool flag = false;
      SnapshotStatusResponse response = (SnapshotStatusResponse) null;
      SnapshotStatusRequest getSnapshotStatusRequest = new SnapshotStatusRequest((Name) repoName, (Names) snapshotName);
      try
      {
        response = GenericInvoker.Instance.InvokeWithFaultCheck<SnapshotStatusResponse>((Func<SnapshotStatusResponse>) (() => this.ElasticClient.Snapshot.Status((ISnapshotStatusRequest) getSnapshotStatusRequest)), executionContext.FaultService, 2, 1000, new TraceMetaData(1082261, "Search Engine", "Search Engine"));
        flag = true;
      }
      finally
      {
        Tracer.TraceLeave(1082261, "Search Engine", "Search Engine", nameof (GetSnapshotStatus));
        if (flag)
        {
          response.ThrowOnInvalidOrFailedResponse();
          Tracer.TraceInfo(1082261, "Search Engine", "Search Engine", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Snapshotstatus of snapshot '{0}' under repository '{1}'", (object) snapshotName, (object) repoName));
        }
      }
      return response;
    }

    public DeleteSnapshotResponse DeleteSnapshot(
      ExecutionContext executionContext,
      string repoName,
      string snapshotName)
    {
      if (string.IsNullOrWhiteSpace(repoName))
        throw new ArgumentException("repoName cannot be null or contain only whitespace");
      if (string.IsNullOrWhiteSpace(snapshotName))
        throw new ArgumentException("snapshotName cannot be null or contain only whitespace");
      Tracer.TraceEnter(1082262, "Search Engine", "Search Engine", nameof (DeleteSnapshot));
      bool flag = false;
      DeleteSnapshotResponse response = (DeleteSnapshotResponse) null;
      DeleteSnapshotRequest deleteSnapshotRequest = new DeleteSnapshotRequest((Name) repoName, (Name) snapshotName);
      try
      {
        response = GenericInvoker.Instance.InvokeWithFaultCheck<DeleteSnapshotResponse>((Func<DeleteSnapshotResponse>) (() => this.ElasticClient.Snapshot.Delete((IDeleteSnapshotRequest) deleteSnapshotRequest)), executionContext.FaultService, 2, 1000, new TraceMetaData(1082262, "Search Engine", "Search Engine"));
        flag = true;
      }
      finally
      {
        Tracer.TraceLeave(1082262, "Search Engine", "Search Engine", nameof (DeleteSnapshot));
        if (flag)
        {
          response.ThrowOnInvalidOrFailedResponse();
          Tracer.TraceInfo(1082260, "Search Engine", "Search Engine", string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Deleting Snapshot '{0}' under repository '{1}'", (object) snapshotName, (object) repoName));
        }
      }
      return response;
    }
  }
}
