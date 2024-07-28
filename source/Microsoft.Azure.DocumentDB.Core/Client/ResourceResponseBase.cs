// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Client.ResourceResponseBase
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Collections;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;

namespace Microsoft.Azure.Documents.Client
{
  public abstract class ResourceResponseBase : IResourceResponseBase
  {
    internal DocumentServiceResponse response;
    private Dictionary<string, long> usageHeaders;
    private Dictionary<string, long> quotaHeaders;

    public ResourceResponseBase()
    {
    }

    internal ResourceResponseBase(DocumentServiceResponse response)
    {
      this.response = response;
      this.usageHeaders = new Dictionary<string, long>();
      this.quotaHeaders = new Dictionary<string, long>();
    }

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

    public long DocumentQuota => this.GetMaxQuotaHeader("documentsSize");

    public long DocumentUsage => this.GetCurrentQuotaHeader("documentsSize");

    public long StoredProceduresQuota => this.GetMaxQuotaHeader("storedProcedures");

    public long StoredProceduresUsage => this.GetCurrentQuotaHeader("storedProcedures");

    public long TriggersQuota => this.GetMaxQuotaHeader("triggers");

    public long TriggersUsage => this.GetCurrentQuotaHeader("triggers");

    public long UserDefinedFunctionsQuota => this.GetMaxQuotaHeader("functions");

    public long UserDefinedFunctionsUsage => this.GetCurrentQuotaHeader("functions");

    internal long DocumentCount => this.GetCurrentQuotaHeader("documentsCount");

    public string ActivityId => this.response.Headers["x-ms-activity-id"];

    public string SessionToken => this.response.Headers["x-ms-session-token"];

    public HttpStatusCode StatusCode => this.response.StatusCode;

    public string MaxResourceQuota => this.response.Headers["x-ms-resource-quota"];

    public string CurrentResourceQuotaUsage => this.response.Headers["x-ms-resource-usage"];

    public Stream ResponseStream => this.response.ResponseBody;

    public double RequestCharge => Helpers.GetHeaderValueDouble(this.response.Headers, "x-ms-request-charge", 0.0);

    public bool IsRUPerMinuteUsed => Helpers.GetHeaderValueByte(this.response.Headers, "x-ms-documentdb-is-ru-per-minute-used", (byte) 0) != (byte) 0;

    public NameValueCollection ResponseHeaders => this.response.ResponseHeaders;

    internal INameValueCollection Headers => this.response.Headers;

    public string ContentLocation => this.response.Headers["x-ms-alt-content-path"];

    public long IndexTransformationProgress => Helpers.GetHeaderValueLong(this.response.Headers, "x-ms-documentdb-collection-index-transformation-progress");

    public long LazyIndexingProgress => Helpers.GetHeaderValueLong(this.response.Headers, "x-ms-documentdb-collection-lazy-indexing-progress");

    public TimeSpan RequestLatency => this.response.RequestStats == null ? TimeSpan.Zero : this.response.RequestStats.RequestLatency;

    public string RequestDiagnosticsString => this.response.RequestStats == null ? string.Empty : this.response.RequestStats.ToString();

    internal IClientSideRequestStatistics RequestStatistics => this.response.RequestStats;

    internal long GetCurrentQuotaHeader(string headerName)
    {
      long num = 0;
      if (this.usageHeaders.Count == 0 && !string.IsNullOrEmpty(this.MaxResourceQuota) && !string.IsNullOrEmpty(this.CurrentResourceQuotaUsage))
        this.PopulateQuotaHeader(this.MaxResourceQuota, this.CurrentResourceQuotaUsage);
      return this.usageHeaders.TryGetValue(headerName, out num) ? num : 0L;
    }

    internal long GetMaxQuotaHeader(string headerName)
    {
      long num = 0;
      if (this.quotaHeaders.Count == 0 && !string.IsNullOrEmpty(this.MaxResourceQuota) && !string.IsNullOrEmpty(this.CurrentResourceQuotaUsage))
        this.PopulateQuotaHeader(this.MaxResourceQuota, this.CurrentResourceQuotaUsage);
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
        else if (string.Equals(strArray1[index], "documentsSize", StringComparison.OrdinalIgnoreCase))
        {
          this.quotaHeaders.Add("documentsSize", long.Parse(strArray1[index + 1], (IFormatProvider) CultureInfo.InvariantCulture));
          this.usageHeaders.Add("documentsSize", long.Parse(strArray2[index + 1], (IFormatProvider) CultureInfo.InvariantCulture));
        }
        else if (string.Equals(strArray1[index], "documentsCount", StringComparison.OrdinalIgnoreCase))
        {
          this.quotaHeaders.Add("documentsCount", long.Parse(strArray1[index + 1], (IFormatProvider) CultureInfo.InvariantCulture));
          this.usageHeaders.Add("documentsCount", long.Parse(strArray2[index + 1], (IFormatProvider) CultureInfo.InvariantCulture));
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
  }
}
