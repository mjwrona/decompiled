// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.ObserverExceptionWrappingChangeFeedObserverDecorator
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed
{
  internal sealed class ObserverExceptionWrappingChangeFeedObserverDecorator : ChangeFeedObserver
  {
    private readonly ChangeFeedObserver changeFeedObserver;

    public ObserverExceptionWrappingChangeFeedObserverDecorator(
      ChangeFeedObserver changeFeedObserver)
    {
      this.changeFeedObserver = changeFeedObserver ?? throw new ArgumentNullException(nameof (changeFeedObserver));
    }

    public override Task CloseAsync(string leaseToken, ChangeFeedObserverCloseReason reason) => this.changeFeedObserver.CloseAsync(leaseToken, reason);

    public override Task OpenAsync(string leaseToken) => this.changeFeedObserver.OpenAsync(leaseToken);

    public override async Task ProcessChangesAsync(
      ChangeFeedObserverContextCore context,
      Stream stream,
      CancellationToken cancellationToken)
    {
      try
      {
        await this.changeFeedObserver.ProcessChangesAsync(context, stream, cancellationToken).ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        ChangeFeedProcessorContext context1 = (ChangeFeedProcessorContext) context;
        throw new ChangeFeedProcessorUserException(ex, context1);
      }
    }
  }
}
