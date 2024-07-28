// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TeamFoundationGitAsyncOperationService`3
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class TeamFoundationGitAsyncOperationService<TOperation, TParam, TStatus> : 
    ITeamFoundationGitAsyncOperationService<TOperation, TParam, TStatus>,
    IVssFrameworkService
    where TOperation : GitAsyncOperation<TParam, TStatus>, new()
  {
    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public TOperation CreateAsyncOperation(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      GitAsyncOperationType operationType,
      TParam parameters)
    {
      TeamFoundationIdentity foundationIdentity = IdentityHelper.Instance.GetCurrentTeamFoundationIdentity(requestContext);
      GitAsyncOperation asyncOperation;
      using (GitAsyncOperationComponent component = requestContext.CreateComponent<GitAsyncOperationComponent>())
      {
        GitAsyncOperationComponent operationComponent = component;
        RepoKey repoKey1 = repoKey;
        int operationType1 = (int) operationType;
        Guid teamFoundationId = foundationIdentity.TeamFoundationId;
        ref TParam local1 = ref parameters;
        string parameters1;
        if ((object) default (TParam) == null)
        {
          TParam obj = local1;
          ref TParam local2 = ref obj;
          if ((object) obj == null)
          {
            parameters1 = (string) null;
            goto label_5;
          }
          else
            local1 = ref local2;
        }
        parameters1 = local1.Serialize<TParam>(true);
label_5:
        asyncOperation = operationComponent.CreateAsyncOperation(repoKey1, (GitAsyncOperationType) operationType1, teamFoundationId, parameters1);
      }
      return this.ConvertToSpecificOperation(asyncOperation);
    }

    public TOperation GetAsyncOperationById(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      int operationId,
      bool throwIfNotFound = true)
    {
      using (GitAsyncOperationComponent component = requestContext.CreateComponent<GitAsyncOperationComponent>())
      {
        GitAsyncOperation operationByOperationId = component.GetAsyncOperationByOperationId(repoKey, operationId);
        if (operationByOperationId != null)
          return this.ConvertToSpecificOperation(operationByOperationId);
        if (throwIfNotFound)
          throw new GitAsyncOperationNotFoundException(operationId);
        return default (TOperation);
      }
    }

    public IEnumerable<TOperation> QueryAsyncOperationsByType(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      GitAsyncOperationType operationType)
    {
      IList<GitAsyncOperation> source = (IList<GitAsyncOperation>) new List<GitAsyncOperation>();
      using (GitAsyncOperationComponent component = requestContext.CreateComponent<GitAsyncOperationComponent>())
        source = (IList<GitAsyncOperation>) component.QueryAsyncOperationsByOperationType(repoKey, operationType);
      return source.Select<GitAsyncOperation, TOperation>((Func<GitAsyncOperation, TOperation>) (x => this.ConvertToSpecificOperation(x)));
    }

    public void UpdateAsyncOperationStatus(
      IVssRequestContext requestContext,
      RepoKey repoKey,
      int operationId,
      GitAsyncOperationStatus status,
      TStatus detailedStatus,
      GitAsyncOperationStatus? previousStatus = null)
    {
      using (GitAsyncOperationComponent component = requestContext.CreateComponent<GitAsyncOperationComponent>())
      {
        GitAsyncOperationComponent operationComponent = component;
        RepoKey repoKey1 = repoKey;
        int operationId1 = operationId;
        int status1 = (int) status;
        ref TStatus local1 = ref detailedStatus;
        string detailedStatus1;
        if ((object) default (TStatus) == null)
        {
          TStatus status2 = local1;
          ref TStatus local2 = ref status2;
          if ((object) status2 == null)
          {
            detailedStatus1 = (string) null;
            goto label_5;
          }
          else
            local1 = ref local2;
        }
        detailedStatus1 = local1.Serialize<TStatus>(true);
label_5:
        GitAsyncOperationStatus? previousStatus1 = previousStatus;
        operationComponent.UpdateAsyncOperationStatus(repoKey1, operationId1, (GitAsyncOperationStatus) status1, detailedStatus1, previousStatus1);
      }
    }

    private TOperation ConvertToSpecificOperation(GitAsyncOperation asyncOperation)
    {
      TParam obj = default (TParam);
      TStatus status = default (TStatus);
      if (!string.IsNullOrWhiteSpace(asyncOperation.Parameters))
        obj = JsonUtilities.Deserialize<TParam>(asyncOperation.Parameters, true);
      if (!string.IsNullOrWhiteSpace(asyncOperation.DetailedStatus))
        status = JsonUtilities.Deserialize<TStatus>(asyncOperation.DetailedStatus, true);
      TOperation specificOperation = new TOperation();
      specificOperation.RepositoryId = asyncOperation.RepositoryId;
      specificOperation.OperationId = asyncOperation.OperationId;
      specificOperation.Type = asyncOperation.Type;
      specificOperation.CreatorId = asyncOperation.CreatorId;
      specificOperation.CreationDate = asyncOperation.CreationDate;
      specificOperation.Parameters = obj;
      specificOperation.Status = asyncOperation.Status;
      specificOperation.DetailedStatus = status;
      return specificOperation;
    }
  }
}
