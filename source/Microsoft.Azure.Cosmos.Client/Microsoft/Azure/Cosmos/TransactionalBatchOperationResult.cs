// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.TransactionalBatchOperationResult
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using Microsoft.Azure.Cosmos.Serialization.HybridRow;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.IO;
using Microsoft.Azure.Cosmos.Serialization.HybridRow.Layouts;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Documents;
using System;
using System.IO;
using System.Net;

namespace Microsoft.Azure.Cosmos
{
  public class TransactionalBatchOperationResult
  {
    internal TransactionalBatchOperationResult(HttpStatusCode statusCode) => this.StatusCode = statusCode;

    internal TransactionalBatchOperationResult(TransactionalBatchOperationResult other)
    {
      this.StatusCode = other.StatusCode;
      this.SubStatusCode = other.SubStatusCode;
      this.ETag = other.ETag;
      this.ResourceStream = other.ResourceStream;
      this.RequestCharge = other.RequestCharge;
      this.RetryAfter = other.RetryAfter;
      this.SessionToken = other.SessionToken;
      this.ActivityId = other.ActivityId;
    }

    protected TransactionalBatchOperationResult()
    {
    }

    public virtual HttpStatusCode StatusCode { get; private set; }

    public virtual bool IsSuccessStatusCode
    {
      get
      {
        int statusCode = (int) this.StatusCode;
        return statusCode >= 200 && statusCode <= 299;
      }
    }

    public virtual string ETag { get; internal set; }

    public virtual Stream ResourceStream { get; internal set; }

    public virtual TimeSpan RetryAfter { get; internal set; }

    internal virtual double RequestCharge { get; set; }

    internal virtual SubStatusCodes SubStatusCode { get; set; }

    internal virtual string SessionToken { get; set; }

    internal virtual string ActivityId { get; set; }

    internal ITrace Trace { get; set; }

    internal static Result ReadOperationResult(
      ReadOnlyMemory<byte> input,
      out TransactionalBatchOperationResult batchOperationResult)
    {
      RowBuffer row = new RowBuffer(input.Length);
      if (!row.ReadFrom(input.Span, HybridRowVersion.V1, (LayoutResolver) BatchSchemaProvider.BatchLayoutResolver))
      {
        batchOperationResult = (TransactionalBatchOperationResult) null;
        return Result.Failure;
      }
      RowReader reader = new RowReader(ref row);
      Result result = TransactionalBatchOperationResult.ReadOperationResult(ref reader, out batchOperationResult);
      if (result != Result.Success)
        return result;
      return batchOperationResult.StatusCode == (HttpStatusCode) 0 ? Result.Failure : Result.Success;
    }

    private static Result ReadOperationResult(
      ref RowReader reader,
      out TransactionalBatchOperationResult batchOperationResult)
    {
      batchOperationResult = new TransactionalBatchOperationResult();
      while (reader.Read())
      {
        switch (UtfAnyString.op_Implicit(reader.Path))
        {
          case "statusCode":
            int num1;
            Result result1 = reader.ReadInt32(out num1);
            if (result1 != Result.Success)
              return result1;
            batchOperationResult.StatusCode = (HttpStatusCode) num1;
            continue;
          case "subStatusCode":
            int num2;
            Result result2 = reader.ReadInt32(out num2);
            if (result2 != Result.Success)
              return result2;
            batchOperationResult.SubStatusCode = (SubStatusCodes) num2;
            continue;
          case "eTag":
            string str;
            Result result3 = reader.ReadString(out str);
            if (result3 != Result.Success)
              return result3;
            batchOperationResult.ETag = str;
            continue;
          case "resourceBody":
            byte[] buffer;
            Result result4 = reader.ReadBinary(out buffer);
            if (result4 != Result.Success)
              return result4;
            batchOperationResult.ResourceStream = (Stream) new MemoryStream(buffer, 0, buffer.Length, false, true);
            continue;
          case "requestCharge":
            double num3;
            Result result5 = reader.ReadFloat64(out num3);
            if (result5 != Result.Success)
              return result5;
            batchOperationResult.RequestCharge = Math.Round(num3, 2);
            continue;
          case "retryAfterMilliseconds":
            uint num4;
            Result result6 = reader.ReadUInt32(out num4);
            if (result6 != Result.Success)
              return result6;
            batchOperationResult.RetryAfter = TimeSpan.FromMilliseconds((double) num4);
            continue;
          default:
            continue;
        }
      }
      return Result.Success;
    }

    internal ResponseMessage ToResponseMessage(ContainerInternal cosmosContainerCore = null) => new ResponseMessage(this.StatusCode, new RequestMessage()
    {
      ContainerId = cosmosContainerCore?.Id,
      DatabaseId = cosmosContainerCore?.Database?.Id,
      Trace = (ITrace) null
    }, new Headers()
    {
      SubStatusCode = this.SubStatusCode,
      ETag = this.ETag,
      RetryAfter = new TimeSpan?(this.RetryAfter),
      RequestCharge = this.RequestCharge,
      Session = this.SessionToken,
      ActivityId = this.ActivityId
    }, (CosmosException) null, this.Trace ?? (ITrace) NoOpTrace.Singleton)
    {
      Content = this.ResourceStream
    };
  }
}
