// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Exceptions.UnexpectedQueryPartitionProviderException
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Tracing;
using System;

namespace Microsoft.Azure.Cosmos.Query.Core.Exceptions
{
  internal sealed class UnexpectedQueryPartitionProviderException : QueryPartitionProviderException
  {
    public UnexpectedQueryPartitionProviderException()
    {
    }

    public UnexpectedQueryPartitionProviderException(string message)
      : base(message)
    {
    }

    public UnexpectedQueryPartitionProviderException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public override TResult Accept<TResult>(QueryExceptionVisitor<TResult> visitor, ITrace trace) => visitor.Visit(this, trace);
  }
}
