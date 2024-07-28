// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.IAsyncGitOperationDispatcher
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Git.Server
{
  [DefaultServiceImplementation(typeof (AsyncGitOperationDispatcher))]
  public interface IAsyncGitOperationDispatcher : IVssFrameworkService
  {
    void SendProgressNotification(
      IVssRequestContext requestContext,
      AsyncGitOperationNotification notification);

    void SendCompletionNotification(
      IVssRequestContext requestContext,
      AsyncGitOperationNotification notification);

    void SendFailureNotification(
      IVssRequestContext requestContext,
      AsyncGitOperationNotification notification);

    void SendTimeoutNotification(
      IVssRequestContext requestContext,
      AsyncGitOperationNotification notification);

    Task Subscribe(IVssRequestContext requestContext, int operationId, string connectionId);

    Task Unsubscribe(IVssRequestContext requestContext, int operationId, string connectionId);
  }
}
