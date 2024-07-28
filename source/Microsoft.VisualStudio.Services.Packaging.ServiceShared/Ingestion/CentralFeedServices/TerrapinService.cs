// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices.TerrapinService
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices
{
  public class TerrapinService : ITerrapinService
  {
    private readonly IOrgLevelPackagingSetting<bool> shouldCallApiSetting;
    private readonly IHttpClient httpClient;
    private readonly IFactory<string> authTokenFactory;
    private readonly ITracerService tracerService;
    private readonly ITerrapinApiStrategy terrapinApiStrategy;
    private Uri terrapinUri;

    public TerrapinService(
      IFactory<string> authTokenFactory,
      IOrgLevelPackagingSetting<bool> shouldCallApiSetting,
      ITracerService tracerService,
      IHttpClient httpClient,
      ITerrapinApiStrategy terrapinApiStrategy)
    {
      this.authTokenFactory = authTokenFactory;
      this.shouldCallApiSetting = shouldCallApiSetting;
      this.tracerService = tracerService;
      this.httpClient = httpClient;
      this.terrapinApiStrategy = terrapinApiStrategy;
    }

    public async Task<TerrapinIngestionValidationResult> GetValidationStatusAsync(
      PackageUrlIdentity packageUrlIdentity,
      IEnumerable<UpstreamSourceInfo> sourceChain,
      bool isValidatingWriteOperation)
    {
      TerrapinService sendInTheThisObject = this;
      using (ITracerBlock traceBlock = sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (GetValidationStatusAsync)))
      {
        if (!sendInTheThisObject.shouldCallApiSetting.Get())
        {
          traceBlock.TraceInfo(new string[1]
          {
            "TpinSvcShortCircuit"
          }, "Not calling Terrapin API because the setting is disabled");
          return TerrapinIngestionValidationResult.Approved;
        }
        try
        {
          IEnumerable<UpstreamSourceInfo> source = sourceChain;
          UpstreamSourceInfo initialSource = source != null ? source.LastOrDefault<UpstreamSourceInfo>() : (UpstreamSourceInfo) null;
          if (initialSource == null)
          {
            traceBlock.TraceInfo(new string[1]
            {
              "TpinSvcShortCircuit"
            }, "Not calling Terrapin API because the source chain is empty (i.e. this is a direct push)");
            return TerrapinIngestionValidationResult.Approved;
          }
          if (initialSource.SourceType != PackagingSourceType.Public)
          {
            traceBlock.TraceInfo(new string[1]
            {
              "TpinSvcShortCircuit"
            }, "Not calling Terrapin API because the oldest entry in the source chain is not a public upstream " + initialSource.Serialize<UpstreamSourceInfo>());
            return TerrapinIngestionValidationResult.Approved;
          }
          if (!sendInTheThisObject.terrapinApiStrategy.CanCallTerrapinApi(traceBlock, initialSource))
            return TerrapinIngestionValidationResult.Approved;
          sendInTheThisObject.terrapinUri = sendInTheThisObject.terrapinApiStrategy.ConstructUri(packageUrlIdentity);
          using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, sendInTheThisObject.terrapinUri))
          {
            string parameter = sendInTheThisObject.authTokenFactory.Get();
            request.Headers.Authorization = string.IsNullOrWhiteSpace(parameter) ? (AuthenticationHeaderValue) null : new AuthenticationHeaderValue("Bearer", parameter);
            request.Headers.Add("CFS-Intent", isValidatingWriteOperation ? "Consume" : "Inspect");
            using (HttpResponseMessage response = await sendInTheThisObject.httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead))
            {
              string str;
              if (response.Content != null)
                str = await response.Content.ReadAsStringAsync();
              else
                str = (string) null;
              string responseBody = str;
              traceBlock.TraceInfoAlways(new string[1]
              {
                "TerrapinResponse"
              }, new
              {
                StatusCode = response.StatusCode,
                ReasonPhrase = response.ReasonPhrase,
                Body = responseBody
              }.Serialize(true));
              response.EnsureSuccessStatusCode();
              return sendInTheThisObject.terrapinApiStrategy.GetTerrapinValidationResult(responseBody);
            }
          }
        }
        catch (Exception ex)
        {
          traceBlock.TraceException(ex);
          return TerrapinIngestionValidationResult.Approved;
        }
      }
    }
  }
}
