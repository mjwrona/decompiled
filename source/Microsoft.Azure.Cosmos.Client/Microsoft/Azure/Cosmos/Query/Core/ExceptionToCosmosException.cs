// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.ExceptionToCosmosException
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.ChangeFeed;
using Microsoft.Azure.Cosmos.Query.Core.Exceptions;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Resource.CosmosExceptions;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using System;

namespace Microsoft.Azure.Cosmos.Query.Core
{
  internal sealed class ExceptionToCosmosException
  {
    public static bool TryCreateFromException(
      Exception exception,
      ITrace trace,
      out CosmosException cosmosException)
    {
      switch (exception)
      {
        case CosmosException cosmosException1:
          cosmosException = cosmosException1;
          return true;
        case DocumentClientException documentClientException:
          cosmosException = ExceptionToCosmosException.CreateFromDocumentClientException(documentClientException, trace);
          return true;
        case QueryException queryException:
          cosmosException = queryException.Accept<CosmosException>((QueryExceptionVisitor<CosmosException>) ExceptionToCosmosException.QueryExceptionConverter.Singleton, trace);
          return true;
        case ChangeFeedException changeFeedException:
          cosmosException = changeFeedException.Accept<CosmosException>((ChangeFeedExceptionVisitor<CosmosException>) ExceptionToCosmosException.ChangeFeedExceptionConverter.Singleton, trace);
          return true;
        case ExceptionWithStackTraceException exceptionWithStackTrace:
          return ExceptionToCosmosException.TryCreateFromExceptionWithStackTrace(exceptionWithStackTrace, trace, out cosmosException);
        default:
          if (exception.InnerException != null)
            return ExceptionToCosmosException.TryCreateFromException(exception.InnerException, trace, out cosmosException);
          cosmosException = (CosmosException) null;
          return false;
      }
    }

    private static CosmosException CreateFromDocumentClientException(
      DocumentClientException documentClientException,
      ITrace trace)
    {
      return CosmosExceptionFactory.Create(documentClientException, trace);
    }

    private static bool TryCreateFromExceptionWithStackTrace(
      ExceptionWithStackTraceException exceptionWithStackTrace,
      ITrace trace,
      out CosmosException cosmosException)
    {
      if (exceptionWithStackTrace.InnerException is DocumentClientException || exceptionWithStackTrace.InnerException is CosmosException)
        return ExceptionToCosmosException.TryCreateFromException(exceptionWithStackTrace.InnerException, trace, out cosmosException);
      if (!ExceptionToCosmosException.TryCreateFromException(exceptionWithStackTrace.InnerException, trace, out cosmosException))
        return false;
      cosmosException = CosmosExceptionFactory.Create(cosmosException.StatusCode, cosmosException.Message, exceptionWithStackTrace.StackTrace, cosmosException.Headers, cosmosException.Trace, cosmosException.Error, cosmosException.InnerException);
      return true;
    }

    private sealed class QueryExceptionConverter : QueryExceptionVisitor<CosmosException>
    {
      public static readonly ExceptionToCosmosException.QueryExceptionConverter Singleton = new ExceptionToCosmosException.QueryExceptionConverter();

      private QueryExceptionConverter()
      {
      }

      public override CosmosException Visit(
        MalformedContinuationTokenException malformedContinuationTokenException,
        ITrace trace)
      {
        Microsoft.Azure.Cosmos.Headers headers1 = new Microsoft.Azure.Cosmos.Headers()
        {
          SubStatusCode = SubStatusCodes.MalformedContinuationToken
        };
        string message = malformedContinuationTokenException.Message;
        Microsoft.Azure.Cosmos.Headers headers2 = headers1;
        string stackTrace = malformedContinuationTokenException.StackTrace;
        Exception exception = (Exception) malformedContinuationTokenException;
        ITrace trace1 = trace;
        Exception innerException = exception;
        return CosmosExceptionFactory.CreateBadRequestException(message, headers2, stackTrace, trace1, innerException: innerException);
      }

      public override CosmosException Visit(
        UnexpectedQueryPartitionProviderException unexpectedQueryPartitionProviderException,
        ITrace trace)
      {
        Microsoft.Azure.Cosmos.Headers headers = new Microsoft.Azure.Cosmos.Headers();
        Exception exception = (Exception) unexpectedQueryPartitionProviderException;
        ITrace trace1 = trace;
        Exception innerException = exception;
        return CosmosExceptionFactory.CreateInternalServerErrorException("CosmosException due to UnexpectedQueryPartitionProviderException", headers, trace: trace1, innerException: innerException);
      }

      public override CosmosException Visit(
        ExpectedQueryPartitionProviderException expectedQueryPartitionProviderException,
        ITrace trace)
      {
        string message = expectedQueryPartitionProviderException.Message;
        Microsoft.Azure.Cosmos.Headers headers = new Microsoft.Azure.Cosmos.Headers();
        string stackTrace = expectedQueryPartitionProviderException.StackTrace;
        Exception exception = (Exception) expectedQueryPartitionProviderException;
        ITrace trace1 = trace;
        Exception innerException = exception;
        return CosmosExceptionFactory.CreateBadRequestException(message, headers, stackTrace, trace1, innerException: innerException);
      }
    }

    private sealed class ChangeFeedExceptionConverter : ChangeFeedExceptionVisitor<CosmosException>
    {
      public static readonly ExceptionToCosmosException.ChangeFeedExceptionConverter Singleton = new ExceptionToCosmosException.ChangeFeedExceptionConverter();

      private ChangeFeedExceptionConverter()
      {
      }

      internal override CosmosException Visit(
        MalformedChangeFeedContinuationTokenException malformedChangeFeedContinuationTokenException,
        ITrace trace)
      {
        string message = malformedChangeFeedContinuationTokenException.Message;
        Microsoft.Azure.Cosmos.Headers headers = new Microsoft.Azure.Cosmos.Headers();
        string stackTrace = malformedChangeFeedContinuationTokenException.StackTrace;
        Exception exception = (Exception) malformedChangeFeedContinuationTokenException;
        ITrace trace1 = trace;
        Exception innerException = exception;
        return CosmosExceptionFactory.CreateBadRequestException(message, headers, stackTrace, trace1, innerException: innerException);
      }
    }
  }
}
