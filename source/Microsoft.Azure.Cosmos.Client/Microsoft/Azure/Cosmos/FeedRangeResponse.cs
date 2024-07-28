// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.FeedRangeResponse
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;

namespace Microsoft.Azure.Cosmos
{
  internal static class FeedRangeResponse
  {
    public static ResponseMessage CreateSuccess(
      ResponseMessage responseMessage,
      FeedRangeContinuation feedRangeContinuation)
    {
      if (responseMessage == null)
        throw new ArgumentNullException(nameof (responseMessage));
      if (feedRangeContinuation == null)
        throw new ArgumentNullException(nameof (feedRangeContinuation));
      responseMessage.Headers.ContinuationToken = !feedRangeContinuation.IsDone ? feedRangeContinuation.ToString() : (string) null;
      return responseMessage;
    }

    public static ResponseMessage CreateFailure(ResponseMessage responseMessage)
    {
      if (responseMessage == null)
        throw new ArgumentNullException(nameof (responseMessage));
      responseMessage.Headers.ContinuationToken = (string) null;
      return responseMessage;
    }
  }
}
