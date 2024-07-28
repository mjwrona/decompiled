// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.MalformedChangeFeedContinuationTokenException
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Tracing;
using System;

namespace Microsoft.Azure.Cosmos.ChangeFeed
{
  internal sealed class MalformedChangeFeedContinuationTokenException : ChangeFeedException
  {
    public MalformedChangeFeedContinuationTokenException()
    {
    }

    public MalformedChangeFeedContinuationTokenException(string message)
      : base(message)
    {
    }

    public MalformedChangeFeedContinuationTokenException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public override TResult Accept<TResult>(
      ChangeFeedExceptionVisitor<TResult> visitor,
      ITrace trace)
    {
      return visitor.Visit(this, trace);
    }
  }
}
