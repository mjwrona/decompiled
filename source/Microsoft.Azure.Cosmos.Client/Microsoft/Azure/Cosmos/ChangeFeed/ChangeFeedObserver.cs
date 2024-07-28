// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.ChangeFeedObserver
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed
{
  internal abstract class ChangeFeedObserver
  {
    public abstract Task OpenAsync(string leaseToken);

    public abstract Task CloseAsync(string leaseToken, ChangeFeedObserverCloseReason reason);

    public abstract Task ProcessChangesAsync(
      ChangeFeedObserverContextCore context,
      Stream stream,
      CancellationToken cancellationToken);
  }
}
