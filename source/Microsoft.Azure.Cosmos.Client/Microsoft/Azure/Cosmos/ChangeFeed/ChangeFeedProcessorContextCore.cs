// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.ChangeFeedProcessorContextCore
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

namespace Microsoft.Azure.Cosmos.ChangeFeed
{
  internal sealed class ChangeFeedProcessorContextCore : ChangeFeedProcessorContext
  {
    private readonly ChangeFeedObserverContextCore changeFeedObserverContextCore;

    public ChangeFeedProcessorContextCore(
      ChangeFeedObserverContextCore changeFeedObserverContextCore)
    {
      this.changeFeedObserverContextCore = changeFeedObserverContextCore;
    }

    public override string LeaseToken => this.changeFeedObserverContextCore.LeaseToken;

    public override CosmosDiagnostics Diagnostics => this.changeFeedObserverContextCore.Diagnostics;

    public override Headers Headers => this.changeFeedObserverContextCore.Headers;
  }
}
