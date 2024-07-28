// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices.TerrapinApiV1Strategy
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Settings;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices
{
  public class TerrapinApiV1Strategy : ITerrapinApiStrategy
  {
    private static readonly ImmutableDictionary<Uri, string> WellKnownUpstreams = new Dictionary<Uri, string>()
    {
      {
        new Uri("https://registry.npmjs.org/"),
        "npmjs"
      },
      {
        new Uri("https://api.nuget.org/v3/index.json"),
        "nuget"
      }
    }.ToImmutableDictionary<Uri, string>();
    private readonly IOrgLevelPackagingSetting<Uri> baseUriSetting;
    private string upstreamId;
    private Uri baseUri;

    public TerrapinApiV1Strategy(IOrgLevelPackagingSetting<Uri> baseUriSetting) => this.baseUriSetting = baseUriSetting;

    public bool CanCallTerrapinApi(ITracerBlock traceBlock, UpstreamSourceInfo initialSource)
    {
      Uri input = this.baseUriSetting.Get();
      this.baseUri = (object) input != null ? input.EnsurePathEndsInSlash() : (Uri) null;
      if (this.baseUri == (Uri) null)
      {
        traceBlock.TraceError("Not calling Terrapin API because the API base URI is not set even though the feature flag is on");
        return false;
      }
      string str;
      if (!TerrapinApiV1Strategy.WellKnownUpstreams.TryGetValue(new Uri(initialSource.Location), out str))
      {
        traceBlock.TraceInfo(new string[1]
        {
          "TpinSvcShortCircuit"
        }, "Not calling Terrapin API because upstream location " + initialSource.Location + " is not recognized as a well-known public upstream");
        return false;
      }
      this.upstreamId = str;
      return true;
    }

    public Uri ConstructUri(PackageUrlIdentity packageUrlIdentity) => new Uri(this.baseUri, string.Join("/", ((IEnumerable<string>) new string[6]
    {
      "packages",
      packageUrlIdentity.Type,
      this.upstreamId,
      packageUrlIdentity.Namespace ?? "-",
      packageUrlIdentity.Name,
      packageUrlIdentity.Version
    }).Select<string, string>(TerrapinApiV1Strategy.\u003C\u003EO.\u003C0\u003E__EscapeDataString ?? (TerrapinApiV1Strategy.\u003C\u003EO.\u003C0\u003E__EscapeDataString = new Func<string, string>(Uri.EscapeDataString)))));

    public TerrapinIngestionValidationResult GetTerrapinValidationResult(string responseBody)
    {
      TerrapinApiV1Strategy.IngestionResponse ingestionResponse = JsonUtilities.Deserialize<TerrapinApiV1Strategy.IngestionResponse>(responseBody, true);
      return new TerrapinIngestionValidationResult(ingestionResponse.PolicyDecision == null || ingestionResponse.PolicyDecision.Approved ? TerrapinIngestionValidationOverallResult.Approved : TerrapinIngestionValidationOverallResult.Denied, ((IEnumerable<TerrapinApiV1Strategy.IngestionResponsePolicyDecisionReason>) ingestionResponse.PolicyDecision?.Reasons ?? Enumerable.Empty<TerrapinApiV1Strategy.IngestionResponsePolicyDecisionReason>()).Select<TerrapinApiV1Strategy.IngestionResponsePolicyDecisionReason, TerrapinIngestionValidationReason>((Func<TerrapinApiV1Strategy.IngestionResponsePolicyDecisionReason, TerrapinIngestionValidationReason>) (x => x.ConvertToValidationReason())));
    }

    private class IngestionResponse
    {
      public TerrapinApiV1Strategy.IngestionResponsePolicyDecision PolicyDecision { get; set; }
    }

    private class IngestionResponsePolicyDecision
    {
      public bool Approved { get; set; }

      public IList<TerrapinApiV1Strategy.IngestionResponsePolicyDecisionReason> Reasons { get; set; }
    }

    private class IngestionResponsePolicyDecisionReason
    {
      public string Code { get; set; }

      public string Message { get; set; }

      public DateTime? QuarantinedUntilUtc { get; set; }

      public TerrapinIngestionValidationReason ConvertToValidationReason()
      {
        if (this.QuarantinedUntilUtc.HasValue)
          return (TerrapinIngestionValidationReason) new TerrapinIngestionValidationReasonQuarantine(this.Message, this.QuarantinedUntilUtc);
        return this.Code == "QuarantinedPendingAssessment" ? (TerrapinIngestionValidationReason) new TerrapinIngestionValidationReasonQuarantinePending(this.Message) : new TerrapinIngestionValidationReason(this.Code, this.Message);
      }
    }
  }
}
