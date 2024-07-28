// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosQueryResponseMessageHeaders
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using System;

namespace Microsoft.Azure.Cosmos
{
  internal class CosmosQueryResponseMessageHeaders : Headers
  {
    public CosmosQueryResponseMessageHeaders(
      string continauationToken,
      string disallowContinuationTokenMessage,
      ResourceType resourceType,
      string containerRid)
    {
      base.ContinuationToken = continauationToken;
      this.DisallowContinuationTokenMessage = disallowContinuationTokenMessage;
      this.ResourceType = resourceType;
      this.ContainerRid = containerRid;
    }

    internal string DisallowContinuationTokenMessage { get; }

    public override string ContinuationToken
    {
      get
      {
        if (this.DisallowContinuationTokenMessage != null)
          throw new ArgumentException(this.DisallowContinuationTokenMessage);
        return base.ContinuationToken;
      }
      internal set => throw new InvalidOperationException("To prevent the different aggregate context from impacting each other only allow updating the continuation token via clone method.");
    }

    internal virtual string ContainerRid { get; }

    internal virtual ResourceType ResourceType { get; }

    internal string InternalContinuationToken => base.ContinuationToken;

    internal CosmosQueryResponseMessageHeaders CloneKnownProperties() => this.CloneKnownProperties(this.InternalContinuationToken, this.DisallowContinuationTokenMessage);

    internal CosmosQueryResponseMessageHeaders CloneKnownProperties(
      string continauationToken,
      string disallowContinuationTokenMessage)
    {
      CosmosQueryResponseMessageHeaders responseMessageHeaders = new CosmosQueryResponseMessageHeaders(continauationToken, disallowContinuationTokenMessage, this.ResourceType, this.ContainerRid);
      responseMessageHeaders.RequestCharge = this.RequestCharge;
      responseMessageHeaders.ContentLength = this.ContentLength;
      responseMessageHeaders.ActivityId = this.ActivityId;
      responseMessageHeaders.ETag = this.ETag;
      responseMessageHeaders.Location = this.Location;
      responseMessageHeaders.RetryAfterLiteral = this.RetryAfterLiteral;
      responseMessageHeaders.SubStatusCodeLiteral = this.SubStatusCodeLiteral;
      responseMessageHeaders.ContentType = this.ContentType;
      responseMessageHeaders.QueryMetricsText = this.QueryMetricsText;
      responseMessageHeaders.IndexUtilizationText = this.IndexUtilizationText;
      return responseMessageHeaders;
    }

    internal static CosmosQueryResponseMessageHeaders ConvertToQueryHeaders(
      Headers sourceHeaders,
      ResourceType resourceType,
      string containerRid,
      int? substatusCode = null,
      string activityId = null)
    {
      if (sourceHeaders == null)
        return new CosmosQueryResponseMessageHeaders((string) null, (string) null, resourceType, containerRid);
      CosmosQueryResponseMessageHeaders queryHeaders = new CosmosQueryResponseMessageHeaders(sourceHeaders.ContinuationToken, (string) null, resourceType, containerRid);
      queryHeaders.RequestCharge = sourceHeaders.RequestCharge;
      queryHeaders.ContentLength = sourceHeaders.ContentLength;
      queryHeaders.ActivityId = sourceHeaders.ActivityId ?? activityId;
      queryHeaders.ETag = sourceHeaders.ETag;
      queryHeaders.Location = sourceHeaders.Location;
      queryHeaders.RetryAfterLiteral = sourceHeaders.RetryAfterLiteral;
      queryHeaders.SubStatusCodeLiteral = sourceHeaders.SubStatusCodeLiteral ?? (substatusCode.HasValue ? substatusCode.Value.ToString() : (string) null);
      queryHeaders.ContentType = sourceHeaders.ContentType;
      queryHeaders.QueryMetricsText = sourceHeaders.QueryMetricsText;
      queryHeaders.IndexUtilizationText = sourceHeaders.IndexUtilizationText;
      return queryHeaders;
    }
  }
}
