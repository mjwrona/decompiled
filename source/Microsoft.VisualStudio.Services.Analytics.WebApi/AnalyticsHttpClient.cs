// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.WebApi.AnalyticsHttpClient
// Assembly: Microsoft.VisualStudio.Services.Analytics.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F858B048-FFE8-4E5F-8EBC-9B25DAC23DF8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.WebApi.dll

using Microsoft.VisualStudio.Services.Analytics.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Analytics.WebApi
{
  [ResourceArea("{F47C4501-5E41-4A7C-B17B-19B7CEF00B91}")]
  public class AnalyticsHttpClient : AnalyticsHttpClientBase
  {
    private static readonly Dictionary<string, Type> ExceptionMap = new Dictionary<string, Type>()
    {
      ["StageStreamDisabledException"] = typeof (StageStreamDisabledException),
      ["StageTableInMaintenanceException"] = typeof (StageTableInMaintenanceException),
      ["StageStreamNotFoundException"] = typeof (StageStreamNotFoundException),
      ["StageStreamThrottledException"] = typeof (StageStreamThrottledException),
      ["StageKeysOnlyNotSupportedException"] = typeof (StageKeysOnlyNotSupportedException),
      ["StageFailedException"] = typeof (StageFailedException)
    };

    internal AnalyticsHttpClient()
      : base(new Uri("https://localhost"), new VssCredentials())
    {
    }

    public AnalyticsHttpClient(Uri baseUrl, VssCredentials credentials)
      : base(baseUrl, credentials)
    {
    }

    public AnalyticsHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings)
      : base(baseUrl, credentials, settings)
    {
    }

    public AnalyticsHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, handlers)
    {
    }

    public AnalyticsHttpClient(
      Uri baseUrl,
      VssCredentials credentials,
      VssHttpRequestSettings settings,
      params DelegatingHandler[] handlers)
      : base(baseUrl, credentials, settings, handlers)
    {
    }

    public AnalyticsHttpClient(Uri baseUrl, HttpMessageHandler pipeline, bool disposeHandler)
      : base(baseUrl, pipeline, disposeHandler)
    {
    }

    protected override IDictionary<string, Type> TranslatedExceptions { get; } = (IDictionary<string, Type>) AnalyticsHttpClient.ExceptionMap;

    public virtual async Task<IngestResult> StageRecordsAsync(
      string table,
      int providerShard,
      int stream,
      StagePostEnvelope envelope,
      object userState = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AnalyticsHttpClient analyticsHttpClient = this;
      IngestResult ingestResult;
      using (MemoryStream content = new MemoryStream())
      {
        using (TextWriter textWriter = (TextWriter) new StreamWriter((Stream) content))
        {
          JsonSerializer.Create(new JsonSerializerSettings()
          {
            NullValueHandling = NullValueHandling.Ignore
          }).Serialize(textWriter, (object) envelope);
          textWriter.Flush();
          content.Position = 0L;
          ingestResult = await analyticsHttpClient.StageRecordsAsync((Stream) content, table, providerShard, stream, userState, cancellationToken);
        }
      }
      return ingestResult;
    }
  }
}
