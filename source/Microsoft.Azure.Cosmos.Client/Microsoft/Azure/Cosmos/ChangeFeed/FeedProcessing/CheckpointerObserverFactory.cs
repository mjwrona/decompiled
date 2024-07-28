// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.FeedProcessing.CheckpointerObserverFactory
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;

namespace Microsoft.Azure.Cosmos.ChangeFeed.FeedProcessing
{
  internal sealed class CheckpointerObserverFactory : ChangeFeedObserverFactory
  {
    private readonly ChangeFeedObserverFactory observerFactory;
    private readonly bool withManualCheckpointing;

    public CheckpointerObserverFactory(
      ChangeFeedObserverFactory observerFactory,
      bool withManualCheckpointing)
    {
      this.observerFactory = observerFactory ?? throw new ArgumentNullException(nameof (observerFactory));
      this.withManualCheckpointing = withManualCheckpointing;
    }

    public override ChangeFeedObserver CreateObserver()
    {
      ChangeFeedObserver observer = (ChangeFeedObserver) new ObserverExceptionWrappingChangeFeedObserverDecorator(this.observerFactory.CreateObserver());
      return this.withManualCheckpointing ? observer : (ChangeFeedObserver) new AutoCheckpointer(observer);
    }
  }
}
