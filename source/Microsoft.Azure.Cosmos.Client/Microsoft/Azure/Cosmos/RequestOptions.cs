// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.RequestOptions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos
{
  public class RequestOptions
  {
    public string IfMatchEtag { get; set; }

    public string IfNoneMatchEtag { get; set; }

    public IReadOnlyDictionary<string, object> Properties { get; set; }

    public Action<Headers> AddRequestHeaders { get; set; }

    internal DistributedTracingOptions DistributedTracingOptions { get; set; }

    internal bool IsEffectivePartitionKeyRouting { get; set; }

    internal virtual ConsistencyLevel? BaseConsistencyLevel { get; set; }

    internal bool DisablePointOperationDiagnostics { get; set; }

    internal virtual void PopulateRequestOptions(RequestMessage request)
    {
      if (this.Properties != null)
      {
        foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) this.Properties)
          request.Properties[property.Key] = property.Value;
      }
      if (this.IfMatchEtag != null)
        request.Headers.Add("If-Match", this.IfMatchEtag);
      if (this.IfNoneMatchEtag != null)
        request.Headers.Add("If-None-Match", this.IfNoneMatchEtag);
      Action<Headers> addRequestHeaders = this.AddRequestHeaders;
      if (addRequestHeaders == null)
        return;
      addRequestHeaders(request.Headers);
    }

    public RequestOptions ShallowCopy() => this.MemberwiseClone() as RequestOptions;

    internal bool TryGetResourceUri(out Uri resourceUri)
    {
      object obj;
      if (this.Properties != null && this.Properties.TryGetValue("x-ms-resource-uri", out obj))
      {
        Uri uri = obj as Uri;
        resourceUri = !(uri == (Uri) null) && !uri.IsAbsoluteUri ? uri : throw new ArgumentException("x-ms-resource-uri must be a relative Uri of type System.Uri");
        return true;
      }
      resourceUri = (Uri) null;
      return false;
    }

    internal static void SetSessionToken(RequestMessage request, string sessionToken)
    {
      if (string.IsNullOrWhiteSpace(sessionToken))
        return;
      request.Headers.Add("x-ms-session-token", sessionToken);
    }
  }
}
