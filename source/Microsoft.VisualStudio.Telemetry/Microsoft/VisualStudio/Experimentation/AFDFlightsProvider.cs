// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Experimentation.AFDFlightsProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.Services;
using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Cache;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Experimentation
{
  internal sealed class AFDFlightsProvider : CachedRemotePollerFlightsProviderBase<ActiveFlightsData>
  {
    private const int DefaultPollingIntervalInSecs = 1800000;
    private const int DefaultRequestTimeout = 60000;
    private const string DefaultGetMethod = "GET";
    private const string DefaultContentType = "application/json";
    private const string DefaultUrl = "https://visualstudio-devdiv-c2s.msedge.net/ab";
    private static readonly HttpRequestCachePolicy DefaultCachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
    internal static readonly Regex RegExpression = new Regex("[^a-zA-Z0-9-]");
    private readonly string flightsKey;
    private readonly IExperimentationFilterProvider filterProvider;
    private readonly IHttpWebRequestFactory httpWebRequestFactory;

    public AFDFlightsProvider(
      IKeyValueStorage keyValueStorage,
      string flightsKey,
      IFlightsStreamParser flightsStreamParser,
      IExperimentationFilterProvider filterProvider,
      IHttpWebRequestFactory httpWebRequestFactory)
      : base(keyValueStorage, flightsStreamParser, 1800000)
    {
      flightsKey.RequiresArgumentNotNullAndNotEmpty(nameof (flightsKey));
      filterProvider.RequiresArgumentNotNull<IExperimentationFilterProvider>(nameof (filterProvider));
      httpWebRequestFactory.RequiresArgumentNotNull<IHttpWebRequestFactory>(nameof (httpWebRequestFactory));
      this.filterProvider = filterProvider;
      this.flightsKey = flightsKey;
      this.httpWebRequestFactory = httpWebRequestFactory;
    }

    protected override void InternalDispose()
    {
    }

    protected override async Task<Stream> SendRemoteRequestInternalAsync()
    {
      IHttpWebRequest httpWebRequest = this.httpWebRequestFactory.Create("https://visualstudio-devdiv-c2s.msedge.net/ab");
      httpWebRequest.Method = "GET";
      httpWebRequest.CachePolicy = (RequestCachePolicy) AFDFlightsProvider.DefaultCachePolicy;
      httpWebRequest.ContentType = "application/json";
      httpWebRequest.AddHeaders(this.GetAllFilters());
      Stream stream = (Stream) null;
      try
      {
        CancellationTokenSource tokenSource = new CancellationTokenSource();
        Task.Delay(60000).ContinueWith((Action<Task>) (task => tokenSource.Cancel()), CancellationToken.None, TaskContinuationOptions.None, TaskScheduler.Default);
        IHttpWebResponse httpWebResponse = await httpWebRequest.GetResponseAsync(tokenSource.Token).ConfigureAwait(false);
        if (httpWebResponse.ErrorCode == Microsoft.VisualStudio.Telemetry.Services.ErrorCode.NoError)
          stream = httpWebResponse.GetResponseStream();
      }
      catch
      {
      }
      Stream stream1 = stream;
      stream = (Stream) null;
      return stream1;
    }

    protected override string BuildFlightsKey()
    {
      List<string> stringList = new List<string>();
      this.AddIfNotEmpty(stringList, this.filterProvider.GetFilterValue(Filters.ApplicationName));
      this.AddIfNotEmpty(stringList, this.filterProvider.GetFilterValue(Filters.ApplicationVersion));
      this.AddIfNotEmpty(stringList, this.filterProvider.GetFilterValue(Filters.BranchBuildFrom));
      this.AddIfNotEmpty(stringList, this.flightsKey);
      return string.Join("\\", (IEnumerable<string>) stringList);
    }

    private IEnumerable<KeyValuePair<string, string>> GetAllFilters()
    {
      yield return new KeyValuePair<string, string>("X-MSEdge-ClientID", AFDFlightsProvider.ProcessFilterValue(this.filterProvider.GetFilterValue(Filters.UserId)));
      yield return new KeyValuePair<string, string>("X-MSEdge-AppID", AFDFlightsProvider.ProcessFilterValue(this.filterProvider.GetFilterValue(Filters.ApplicationName)));
      yield return new KeyValuePair<string, string>("X-VisualStudio-Internal", AFDFlightsProvider.ProcessFilterValue(this.filterProvider.GetFilterValue(Filters.IsInternal)));
      yield return new KeyValuePair<string, string>("X-VisualStudio-Build", AFDFlightsProvider.ProcessFilterValue(this.filterProvider.GetFilterValue(Filters.ApplicationVersion)));
      yield return new KeyValuePair<string, string>("X-VisualStudio-BuildVersion", this.filterProvider.GetFilterValue(Filters.ApplicationVersion));
      yield return new KeyValuePair<string, string>("X-VisualStudio-Branch", AFDFlightsProvider.ProcessFilterValue(this.filterProvider.GetFilterValue(Filters.BranchBuildFrom)));
      yield return new KeyValuePair<string, string>("X-VisualStudio-SKU", AFDFlightsProvider.ProcessFilterValue(this.filterProvider.GetFilterValue(Filters.ApplicationSku)));
      yield return new KeyValuePair<string, string>("X-VisualStudio-ChannelId", AFDFlightsProvider.ProcessFilterValue(this.filterProvider.GetFilterValue(Filters.ChannelId)));
    }

    private static string ProcessFilterValue(string value) => AFDFlightsProvider.RegExpression.Replace(value, "-");

    private void AddIfNotEmpty(List<string> parts, string v)
    {
      if (string.IsNullOrEmpty(v))
        return;
      parts.Add(v);
    }
  }
}
