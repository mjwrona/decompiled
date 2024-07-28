// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.DocumentFeedResponse`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Microsoft.Azure.Cosmos
{
  internal class DocumentFeedResponse<T> : 
    IEnumerable<T>,
    IEnumerable,
    IDynamicMetaObjectProvider,
    IDocumentFeedResponse<T>
  {
    internal readonly string disallowContinuationTokenMessage;
    private readonly IEnumerable<T> inner;
    private readonly Dictionary<string, long> usageHeaders;
    private readonly Dictionary<string, long> quotaHeaders;
    private readonly bool useETagAsContinuation;
    private readonly IReadOnlyDictionary<string, Microsoft.Azure.Cosmos.Query.Core.Metrics.QueryMetrics> queryMetrics;
    private INameValueCollection responseHeaders;

    public DocumentFeedResponse()
    {
    }

    public DocumentFeedResponse(IEnumerable<T> result)
      : this()
    {
      this.inner = result != null ? result : Enumerable.Empty<T>();
    }

    internal DocumentFeedResponse(
      IEnumerable<T> result,
      int count,
      INameValueCollection responseHeaders,
      bool useETagAsContinuation = false,
      IReadOnlyDictionary<string, Microsoft.Azure.Cosmos.Query.Core.Metrics.QueryMetrics> queryMetrics = null,
      IClientSideRequestStatistics requestStats = null,
      string disallowContinuationTokenMessage = null,
      long responseLengthBytes = 0)
      : this(result)
    {
      this.Count = count;
      this.responseHeaders = responseHeaders.Clone();
      this.usageHeaders = new Dictionary<string, long>();
      this.quotaHeaders = new Dictionary<string, long>();
      this.useETagAsContinuation = useETagAsContinuation;
      this.queryMetrics = queryMetrics;
      this.disallowContinuationTokenMessage = disallowContinuationTokenMessage;
      this.ResponseLengthBytes = responseLengthBytes;
    }

    internal DocumentFeedResponse(
      IEnumerable<T> result,
      int count,
      INameValueCollection responseHeaders,
      long responseLengthBytes)
      : this(result, count, responseHeaders)
    {
      this.ResponseLengthBytes = responseLengthBytes;
    }

    internal DocumentFeedResponse(
      IEnumerable<T> result,
      int count,
      INameValueCollection responseHeaders,
      IClientSideRequestStatistics requestStats,
      long responseLengthBytes)
      : this(result, count, responseHeaders, false, (IReadOnlyDictionary<string, Microsoft.Azure.Cosmos.Query.Core.Metrics.QueryMetrics>) null, requestStats, (string) null, responseLengthBytes)
    {
    }

    internal IClientSideRequestStatistics RequestStatistics { get; private set; }

    internal long ResponseLengthBytes { get; private set; }

    public long DatabaseQuota => this.GetMaxQuotaHeader("databases");

    public long DatabaseUsage => this.GetCurrentQuotaHeader("databases");

    public long CollectionQuota => this.GetMaxQuotaHeader("collections");

    public long CollectionUsage => this.GetCurrentQuotaHeader("collections");

    public long UserQuota => this.GetMaxQuotaHeader("users");

    public long UserUsage => this.GetCurrentQuotaHeader("users");

    public long PermissionQuota => this.GetMaxQuotaHeader("permissions");

    public long PermissionUsage => this.GetCurrentQuotaHeader("permissions");

    public long CollectionSizeQuota => this.GetMaxQuotaHeader("collectionSize");

    public long CollectionSizeUsage => this.GetCurrentQuotaHeader("collectionSize");

    public long StoredProceduresQuota => this.GetMaxQuotaHeader("storedProcedures");

    public long StoredProceduresUsage => this.GetCurrentQuotaHeader("storedProcedures");

    public long TriggersQuota => this.GetMaxQuotaHeader("triggers");

    public long TriggersUsage => this.GetCurrentQuotaHeader("triggers");

    public long UserDefinedFunctionsQuota => this.GetMaxQuotaHeader("functions");

    public long UserDefinedFunctionsUsage => this.GetCurrentQuotaHeader("functions");

    public int Count { get; private set; }

    public string MaxResourceQuota => this.responseHeaders["x-ms-resource-quota"];

    public string CurrentResourceQuotaUsage => this.responseHeaders["x-ms-resource-usage"];

    public double RequestCharge => Helpers.GetHeaderValueDouble(this.responseHeaders, "x-ms-request-charge", 0.0);

    public string ActivityId => this.responseHeaders["x-ms-activity-id"];

    public string ResponseContinuation
    {
      get
      {
        if (this.disallowContinuationTokenMessage != null)
          throw new ArgumentException(this.disallowContinuationTokenMessage);
        return this.InternalResponseContinuation;
      }
      internal set
      {
        if (this.disallowContinuationTokenMessage != null)
          throw new ArgumentException(this.disallowContinuationTokenMessage);
        this.responseHeaders["x-ms-continuation"] = value;
      }
    }

    public string SessionToken => this.responseHeaders["x-ms-session-token"];

    public string ContentLocation => this.responseHeaders["x-ms-alt-content-path"];

    public string ETag => this.responseHeaders["etag"];

    internal INameValueCollection Headers
    {
      get => this.responseHeaders;
      set => this.responseHeaders = value;
    }

    public NameValueCollection ResponseHeaders => this.responseHeaders.ToNameValueCollection();

    public IReadOnlyDictionary<string, Microsoft.Azure.Cosmos.Query.Core.Metrics.QueryMetrics> QueryMetrics => this.queryMetrics;

    internal string InternalResponseContinuation => !this.useETagAsContinuation ? this.responseHeaders["x-ms-continuation"] : this.ETag;

    public string RequestDiagnosticsString
    {
      get
      {
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.AppendFormat("QueryMetrics: {0}", (object) this.QueryMetrics);
        stringBuilder.AppendLine();
        stringBuilder.AppendFormat("ClientSideRequestStatistics: {0}", (object) this.RequestStatistics);
        stringBuilder.AppendLine();
        return stringBuilder.ToString();
      }
    }

    internal bool UseETagAsContinuation => this.useETagAsContinuation;

    internal string DisallowContinuationTokenMessage => this.disallowContinuationTokenMessage;

    public IEnumerator<T> GetEnumerator() => this.inner.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.inner.GetEnumerator();

    DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter) => (DynamicMetaObject) new DocumentFeedResponse<T>.ResourceFeedDynamicObject(this, parameter);

    private long GetCurrentQuotaHeader(string headerName)
    {
      if (this.usageHeaders.Count == 0 && !string.IsNullOrEmpty(this.MaxResourceQuota) && !string.IsNullOrEmpty(this.CurrentResourceQuotaUsage))
        this.PopulateQuotaHeader(this.MaxResourceQuota, this.CurrentResourceQuotaUsage);
      long num;
      return this.usageHeaders.TryGetValue(headerName, out num) ? num : 0L;
    }

    private long GetMaxQuotaHeader(string headerName)
    {
      if (this.quotaHeaders.Count == 0 && !string.IsNullOrEmpty(this.MaxResourceQuota) && !string.IsNullOrEmpty(this.CurrentResourceQuotaUsage))
        this.PopulateQuotaHeader(this.MaxResourceQuota, this.CurrentResourceQuotaUsage);
      long num;
      return this.quotaHeaders.TryGetValue(headerName, out num) ? num : 0L;
    }

    private void PopulateQuotaHeader(string headerMaxQuota, string headerCurrentUsage)
    {
      string[] strArray1 = headerMaxQuota.Split(Constants.Quota.DelimiterChars, StringSplitOptions.RemoveEmptyEntries);
      string[] strArray2 = headerCurrentUsage.Split(Constants.Quota.DelimiterChars, StringSplitOptions.RemoveEmptyEntries);
      for (int index = 0; index < strArray1.Length; ++index)
      {
        if (string.Equals(strArray1[index], "databases", StringComparison.OrdinalIgnoreCase))
        {
          this.quotaHeaders.Add("databases", long.Parse(strArray1[index + 1], (IFormatProvider) CultureInfo.InvariantCulture));
          this.usageHeaders.Add("databases", long.Parse(strArray2[index + 1], (IFormatProvider) CultureInfo.InvariantCulture));
        }
        else if (string.Equals(strArray1[index], "collections", StringComparison.OrdinalIgnoreCase))
        {
          this.quotaHeaders.Add("collections", long.Parse(strArray1[index + 1], (IFormatProvider) CultureInfo.InvariantCulture));
          this.usageHeaders.Add("collections", long.Parse(strArray2[index + 1], (IFormatProvider) CultureInfo.InvariantCulture));
        }
        else if (string.Equals(strArray1[index], "users", StringComparison.OrdinalIgnoreCase))
        {
          this.quotaHeaders.Add("users", long.Parse(strArray1[index + 1], (IFormatProvider) CultureInfo.InvariantCulture));
          this.usageHeaders.Add("users", long.Parse(strArray2[index + 1], (IFormatProvider) CultureInfo.InvariantCulture));
        }
        else if (string.Equals(strArray1[index], "permissions", StringComparison.OrdinalIgnoreCase))
        {
          this.quotaHeaders.Add("permissions", long.Parse(strArray1[index + 1], (IFormatProvider) CultureInfo.InvariantCulture));
          this.usageHeaders.Add("permissions", long.Parse(strArray2[index + 1], (IFormatProvider) CultureInfo.InvariantCulture));
        }
        else if (string.Equals(strArray1[index], "collectionSize", StringComparison.OrdinalIgnoreCase))
        {
          this.quotaHeaders.Add("collectionSize", long.Parse(strArray1[index + 1], (IFormatProvider) CultureInfo.InvariantCulture));
          this.usageHeaders.Add("collectionSize", long.Parse(strArray2[index + 1], (IFormatProvider) CultureInfo.InvariantCulture));
        }
        else if (string.Equals(strArray1[index], "storedProcedures", StringComparison.OrdinalIgnoreCase))
        {
          this.quotaHeaders.Add("storedProcedures", long.Parse(strArray1[index + 1], (IFormatProvider) CultureInfo.InvariantCulture));
          this.usageHeaders.Add("storedProcedures", long.Parse(strArray2[index + 1], (IFormatProvider) CultureInfo.InvariantCulture));
        }
        else if (string.Equals(strArray1[index], "triggers", StringComparison.OrdinalIgnoreCase))
        {
          this.quotaHeaders.Add("triggers", long.Parse(strArray1[index + 1], (IFormatProvider) CultureInfo.InvariantCulture));
          this.usageHeaders.Add("triggers", long.Parse(strArray2[index + 1], (IFormatProvider) CultureInfo.InvariantCulture));
        }
        else if (string.Equals(strArray1[index], "functions", StringComparison.OrdinalIgnoreCase))
        {
          this.quotaHeaders.Add("functions", long.Parse(strArray1[index + 1], (IFormatProvider) CultureInfo.InvariantCulture));
          this.usageHeaders.Add("functions", long.Parse(strArray2[index + 1], (IFormatProvider) CultureInfo.InvariantCulture));
        }
      }
    }

    private class ResourceFeedDynamicObject : DynamicMetaObject
    {
      public ResourceFeedDynamicObject(DocumentFeedResponse<T> parent, Expression expression)
        : base(expression, BindingRestrictions.Empty, (object) parent)
      {
      }

      public override DynamicMetaObject BindConvert(ConvertBinder binder)
      {
        Type genericTypeDefinition = typeof (DocumentFeedResponse<bool>).GetGenericTypeDefinition();
        if (binder.Type != typeof (IEnumerable) && (!binder.Type.IsGenericType() || binder.Type.GetGenericTypeDefinition() != genericTypeDefinition && binder.Type.GetGenericTypeDefinition() != typeof (IEnumerable<string>).GetGenericTypeDefinition() && binder.Type.GetGenericTypeDefinition() != typeof (IQueryable<string>).GetGenericTypeDefinition()))
          return base.BindConvert(binder);
        Expression expression = (Expression) Expression.Convert(this.Expression, this.LimitType);
        return new DynamicMetaObject((Expression) Expression.Call(typeof (FeedResponseBinder).GetMethod("Convert", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).MakeGenericMethod(binder.Type.GetGenericArguments()[0]), expression), BindingRestrictions.GetTypeRestriction(this.Expression, this.LimitType));
      }
    }
  }
}
