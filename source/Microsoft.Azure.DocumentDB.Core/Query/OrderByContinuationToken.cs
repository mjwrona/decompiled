// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Query.OrderByContinuationToken
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Newtonsoft.Json;
using System;
using System.Globalization;

namespace Microsoft.Azure.Documents.Query
{
  internal sealed class OrderByContinuationToken
  {
    public OrderByContinuationToken(
      CompositeContinuationToken compositeContinuationToken,
      QueryItem[] orderByItems,
      string rid,
      int skipCount,
      string filter)
    {
      if (compositeContinuationToken == null)
        throw new ArgumentNullException("compositeContinuationToken can not be null.");
      if (orderByItems == null)
        throw new ArgumentNullException("orderByItems can not be null.");
      if (orderByItems.Length == 0)
        throw new ArgumentException("orderByItems can not be empty.");
      if (string.IsNullOrWhiteSpace(rid))
        throw new ArgumentNullException("rid can not be null or empty or whitespace.");
      if (skipCount < 0)
        throw new ArgumentException("skipCount can not be negative.");
      this.CompositeContinuationToken = compositeContinuationToken;
      this.OrderByItems = orderByItems;
      this.Rid = rid;
      this.SkipCount = skipCount;
      this.Filter = filter;
    }

    [JsonProperty("compositeToken")]
    public CompositeContinuationToken CompositeContinuationToken { get; }

    [JsonProperty("orderByItems")]
    public QueryItem[] OrderByItems { get; }

    [JsonProperty("rid")]
    public string Rid { get; }

    [JsonProperty("skipCount")]
    public int SkipCount { get; }

    [JsonProperty("filter")]
    public string Filter { get; }

    public static OrderByContinuationToken Parse(string value)
    {
      OrderByContinuationToken orderByContinuationToken;
      if (!OrderByContinuationToken.TryParse(value, out orderByContinuationToken))
        throw new BadRequestException("Invalid OrderByContinuationToken: " + value);
      return orderByContinuationToken;
    }

    public static bool TryParse(
      string value,
      out OrderByContinuationToken orderByContinuationToken)
    {
      orderByContinuationToken = (OrderByContinuationToken) null;
      if (string.IsNullOrWhiteSpace(value))
        return false;
      try
      {
        orderByContinuationToken = JsonConvert.DeserializeObject<OrderByContinuationToken>(value);
        return true;
      }
      catch (JsonException ex)
      {
        DefaultTrace.TraceWarning(DateTime.UtcNow.ToString("o", (IFormatProvider) CultureInfo.InvariantCulture) + " Invalid continuation token " + value + " for Top~Component, exception: " + ex.Message);
        return false;
      }
    }
  }
}
