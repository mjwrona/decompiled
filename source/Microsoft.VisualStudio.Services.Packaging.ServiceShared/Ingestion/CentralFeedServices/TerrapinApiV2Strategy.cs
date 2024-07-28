// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices.TerrapinApiV2Strategy
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
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices
{
  public class TerrapinApiV2Strategy : ITerrapinApiStrategy
  {
    private readonly IOrgLevelPackagingSetting<Uri> baseUriSetting;
    private Uri baseUri;

    public TerrapinApiV2Strategy(IOrgLevelPackagingSetting<Uri> baseUriSetting) => this.baseUriSetting = baseUriSetting;

    public bool CanCallTerrapinApi(ITracerBlock traceBlock, UpstreamSourceInfo initialSource)
    {
      Uri input = this.baseUriSetting.Get();
      this.baseUri = (object) input != null ? input.EnsurePathEndsInSlash() : (Uri) null;
      if (!(this.baseUri == (Uri) null))
        return true;
      traceBlock.TraceError("Not calling Terrapin API because the API base URI is not set even though the feature flag is on");
      return false;
    }

    public Uri ConstructUri(PackageUrlIdentity packageUrlIdentity) => new Uri(this.baseUri, Uri.EscapeDataString(this.ConstructPurlString(packageUrlIdentity)));

    public TerrapinIngestionValidationResult GetTerrapinValidationResult(string responseBody)
    {
      TerrapinApiV2Strategy.IngestionResponse ingestionResponse = JsonUtilities.Deserialize<TerrapinApiV2Strategy.IngestionResponse>(responseBody, true);
      return ingestionResponse.PolicyDecision.IsApproved ? TerrapinIngestionValidationResult.Approved : new TerrapinIngestionValidationResult(TerrapinIngestionValidationOverallResult.Denied, ((IEnumerable<TerrapinApiV2Strategy.IngestionResponsePolicyDecisionReason>) ingestionResponse.PolicyDecision.Reasons ?? Enumerable.Empty<TerrapinApiV2Strategy.IngestionResponsePolicyDecisionReason>()).Select<TerrapinApiV2Strategy.IngestionResponsePolicyDecisionReason, TerrapinIngestionValidationReason>((Func<TerrapinApiV2Strategy.IngestionResponsePolicyDecisionReason, TerrapinIngestionValidationReason>) (x => x.ConvertToValidationReason())));
    }

    private string ConstructPurlString(PackageUrlIdentity packageUrlIdentity)
    {
      string str1 = "pkg:" + packageUrlIdentity.Type;
      if (!string.IsNullOrEmpty(packageUrlIdentity.Namespace))
      {
        string[] source = packageUrlIdentity.Namespace.Trim('/').Split('/');
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        str1 = str1 + "/" + string.Join("/", ((IEnumerable<string>) source).Select<string, string>(TerrapinApiV2Strategy.\u003C\u003EO.\u003C0\u003E__EscapeDataString ?? (TerrapinApiV2Strategy.\u003C\u003EO.\u003C0\u003E__EscapeDataString = new Func<string, string>(Uri.EscapeDataString))));
      }
      string str2 = str1 + "/" + Uri.EscapeDataString(packageUrlIdentity.Name) + "@" + Uri.EscapeDataString(packageUrlIdentity.Version);
      if (packageUrlIdentity.Qualifiers.Any<KeyValuePair<string, string>>())
        str2 = str2 + "?" + string.Join("&", packageUrlIdentity.Qualifiers.Select<KeyValuePair<string, string>, string>((Func<KeyValuePair<string, string>, string>) (pair => Uri.EscapeDataString(pair.Key) + "=" + Uri.EscapeDataString(pair.Value))));
      return str2;
    }

    private class IngestionResponse
    {
      public TerrapinApiV2Strategy.IngestionResponsePolicyDecision PolicyDecision { get; set; }
    }

    private class IngestionResponsePolicyDecision
    {
      public bool IsApproved { get; set; }

      public IList<TerrapinApiV2Strategy.IngestionResponsePolicyDecisionReason> Reasons { get; set; }
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
