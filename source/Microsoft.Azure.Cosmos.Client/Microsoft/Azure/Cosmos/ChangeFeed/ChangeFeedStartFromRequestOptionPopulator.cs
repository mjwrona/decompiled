// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.ChangeFeedStartFromRequestOptionPopulator
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Globalization;

namespace Microsoft.Azure.Cosmos.ChangeFeed
{
  internal sealed class ChangeFeedStartFromRequestOptionPopulator : ChangeFeedStartFromVisitor
  {
    private const string IfNoneMatchAllHeaderValue = "*";
    private static readonly DateTime StartFromBeginningTime = DateTime.MinValue.ToUniversalTime();
    private readonly RequestMessage requestMessage;

    public ChangeFeedStartFromRequestOptionPopulator(RequestMessage requestMessage) => this.requestMessage = requestMessage ?? throw new ArgumentNullException(nameof (requestMessage));

    public override void Visit(ChangeFeedStartFromNow startFromNow) => this.requestMessage.Headers.IfNoneMatch = "*";

    public override void Visit(ChangeFeedStartFromTime startFromTime)
    {
      if (!(startFromTime.StartTime != ChangeFeedStartFromRequestOptionPopulator.StartFromBeginningTime))
        return;
      this.requestMessage.Headers.Add("If-Modified-Since", startFromTime.StartTime.ToString("r", (IFormatProvider) CultureInfo.InvariantCulture));
    }

    public override void Visit(
      ChangeFeedStartFromContinuation startFromContinuation)
    {
      this.requestMessage.Headers.IfNoneMatch = startFromContinuation.Continuation;
    }

    public override void Visit(ChangeFeedStartFromBeginning startFromBeginning)
    {
    }

    public override void Visit(
      ChangeFeedStartFromContinuationAndFeedRange startFromContinuationAndFeedRange)
    {
      if (startFromContinuationAndFeedRange.Etag == null)
        return;
      this.requestMessage.Headers.IfNoneMatch = startFromContinuationAndFeedRange.Etag;
    }
  }
}
