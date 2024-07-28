// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.ODataVersionConstraint
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Routing;

namespace Microsoft.AspNet.OData.Routing
{
  public class ODataVersionConstraint : IHttpRouteConstraint
  {
    internal const string ODataServiceVersionHeader = "OData-Version";
    internal const string ODataMaxServiceVersionHeader = "OData-MaxVersion";
    internal const string ODataMinServiceVersionHeader = "OData-MinVersion";
    internal const ODataVersion DefaultODataVersion = ODataVersion.V4;
    private const string PreviousODataVersionHeaderName = "DataServiceVersion";
    private const string PreviousODataMaxVersionHeaderName = "MaxDataServiceVersion";
    private const string PreviousODataMinVersionHeaderName = "MinDataServiceVersion";

    public bool Match(
      HttpRequestMessage request,
      IHttpRoute route,
      string parameterName,
      IDictionary<string, object> values,
      HttpRouteDirection routeDirection)
    {
      if (request == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (request));
      return routeDirection == HttpRouteDirection.UriGeneration || this.IsVersionMatch((IDictionary<string, IEnumerable<string>>) request.Headers.ToDictionary<KeyValuePair<string, IEnumerable<string>>, string, IEnumerable<string>>((Func<KeyValuePair<string, IEnumerable<string>>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, IEnumerable<string>>, IEnumerable<string>>) (kvp => kvp.Value)), request.ODataProperties().ODataServiceVersion, request.ODataProperties().ODataMaxServiceVersion);
    }

    public ODataVersionConstraint()
    {
      this.Version = ODataVersion.V4;
      this.IsRelaxedMatch = true;
    }

    public ODataVersion Version { get; private set; }

    public bool IsRelaxedMatch { get; set; }

    private bool IsVersionMatch(
      IDictionary<string, IEnumerable<string>> headers,
      ODataVersion? serviceVersion,
      ODataVersion? maxServiceVersion)
    {
      if (!this.ValidateVersionHeaders(headers))
        return false;
      ODataVersion? version = this.GetVersion(headers, serviceVersion, maxServiceVersion);
      return version.HasValue && version.Value >= this.Version;
    }

    private bool ValidateVersionHeaders(IDictionary<string, IEnumerable<string>> headers)
    {
      bool flag1 = headers.ContainsKey("DataServiceVersion") || headers.ContainsKey("MinDataServiceVersion") || headers.ContainsKey("MaxDataServiceVersion");
      bool flag2 = headers.ContainsKey("MaxDataServiceVersion") && !headers.ContainsKey("DataServiceVersion") && !headers.ContainsKey("MinDataServiceVersion");
      bool flag3 = headers.ContainsKey("OData-MaxVersion");
      if (!this.IsRelaxedMatch)
        return !flag1;
      return !flag1 || flag3 & flag2;
    }

    private ODataVersion? GetVersion(
      IDictionary<string, IEnumerable<string>> headers,
      ODataVersion? serviceVersion,
      ODataVersion? maxServiceVersion)
    {
      int headerCount1 = ODataVersionConstraint.GetHeaderCount("OData-Version", headers);
      int headerCount2 = ODataVersionConstraint.GetHeaderCount("OData-MaxVersion", headers);
      if (headerCount1 == 1 && serviceVersion.HasValue)
        return serviceVersion;
      if (headerCount1 == 0 && headerCount2 == 1 && maxServiceVersion.HasValue)
        return maxServiceVersion;
      return headerCount1 == 0 && headerCount2 == 0 ? new ODataVersion?(this.Version) : new ODataVersion?();
    }

    private static int GetHeaderCount(
      string headerName,
      IDictionary<string, IEnumerable<string>> headers)
    {
      IEnumerable<string> source;
      return headers.TryGetValue(headerName, out source) ? source.Count<string>() : 0;
    }
  }
}
