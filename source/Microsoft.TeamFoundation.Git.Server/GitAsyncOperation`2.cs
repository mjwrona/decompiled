// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitAsyncOperation`2
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class GitAsyncOperation<TParam, TStatus>
  {
    public GitAsyncOperation()
    {
    }

    internal GitAsyncOperation(
      Guid repositoryId,
      int operationId,
      GitAsyncOperationType type,
      Guid creatorId,
      DateTime creationDate,
      TParam parameters,
      GitAsyncOperationStatus status,
      TStatus detailedStatus)
    {
      this.RepositoryId = repositoryId;
      this.OperationId = operationId;
      this.Type = type;
      this.CreatorId = creatorId;
      this.CreationDate = creationDate;
      this.Parameters = parameters;
      this.Status = status;
      this.DetailedStatus = detailedStatus;
    }

    public Guid RepositoryId { get; set; }

    public int OperationId { get; set; }

    public GitAsyncOperationType Type { get; set; }

    public Guid CreatorId { get; set; }

    public DateTime CreationDate { get; set; }

    public TParam Parameters { get; set; }

    public GitAsyncOperationStatus Status { get; set; }

    public TStatus DetailedStatus { get; set; }
  }
}
