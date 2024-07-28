// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.ChangeFeedStartFromVisitor
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

namespace Microsoft.Azure.Cosmos.ChangeFeed
{
  internal abstract class ChangeFeedStartFromVisitor
  {
    public abstract void Visit(ChangeFeedStartFromNow startFromNow);

    public abstract void Visit(ChangeFeedStartFromTime startFromTime);

    public abstract void Visit(
      ChangeFeedStartFromContinuation startFromContinuation);

    public abstract void Visit(ChangeFeedStartFromBeginning startFromBeginning);

    public abstract void Visit(
      ChangeFeedStartFromContinuationAndFeedRange startFromContinuationAndFeedRange);
  }
}
