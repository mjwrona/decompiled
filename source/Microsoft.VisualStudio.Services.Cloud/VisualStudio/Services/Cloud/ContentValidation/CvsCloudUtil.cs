// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ContentValidation.CvsCloudUtil
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.Ops.Cvs.Client.DataContracts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Cloud.ContentValidation
{
  internal static class CvsCloudUtil
  {
    public static readonly JsonSerializerSettings CallbackSerializationSettings = new JsonSerializerSettings()
    {
      ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
      DefaultValueHandling = DefaultValueHandling.Ignore
    };

    public static HashSet<string> ExtractUniqueFaultDescriptions(this Job job)
    {
      HashSet<string> faultDescriptions = new HashSet<string>();
      IEnumerable<ProviderResult> providerResults = job.ProviderResults;
      foreach (ProviderResult providerResult in providerResults != null ? providerResults.Where<ProviderResult>((Func<ProviderResult, bool>) (pr => pr != null)) : (IEnumerable<ProviderResult>) null)
      {
        if (providerResult.Faults != null)
        {
          foreach (Fault fault in providerResult.Faults)
          {
            string str = JsonConvert.SerializeObject((object) new
            {
              Detail = fault.Detail,
              ContentItemId = ((BaseResource) fault.ContentItem)?.Id,
              CreatedUtc = fault.CreatedUtc
            });
            faultDescriptions.Add(str);
          }
          providerResult.Faults = (IEnumerable<Fault>) null;
        }
      }
      return faultDescriptions;
    }

    public static IEnumerable<string> SerializeChunkedWithPiiSanitized(
      this Job callbackResult,
      ITraceRequest tracer,
      int chunkSize = 20480)
    {
      string fullResult = callbackResult.SerializeFullWithPiiSanitized(tracer);
      for (int i = 0; i < fullResult.Length; i += chunkSize)
        yield return fullResult.Substring(i, Math.Min(chunkSize, fullResult.Length - i));
    }

    public static string SerializeFullWithPiiSanitized(
      this Job callbackResult,
      ITraceRequest tracer)
    {
      if (callbackResult == null)
        tracer.TraceAlways(15289021, TraceLevel.Warning, nameof (CvsCloudUtil), nameof (SerializeFullWithPiiSanitized), "Attempted to serialize a null Job - this is unexpected.");
      ProviderResult determination = callbackResult.Determination;
      if (determination != null)
      {
        foreach (ContentItem contentItem in determination.ContentItems)
        {
          contentItem.ReporteeAddress = "[REDACTED]";
          try
          {
            ContentValidationIdentity validationIdentity = JsonConvert.DeserializeObject<ContentValidationIdentity>(contentItem.ReporteeName);
            if (validationIdentity.DisplayName != null)
              validationIdentity.DisplayName = "[REDACTED]";
            contentItem.ReporteeName = JsonConvert.SerializeObject((object) validationIdentity);
          }
          catch (JsonException ex)
          {
            tracer.TraceAlways(15289021, TraceLevel.Warning, nameof (CvsCloudUtil), nameof (SerializeFullWithPiiSanitized), "Failed to deserialize {0}: {1}", (object) "ContentValidationIdentity", (object) ex);
          }
          ContentValidationExternalId validationExternalId = JsonConvert.DeserializeObject<ContentValidationExternalId>(contentItem.ExternalId);
          validationExternalId.Token = "[REDACTED]";
          contentItem.ExternalId = JsonConvert.SerializeObject((object) validationExternalId);
        }
        determination.TraitDetections = (IEnumerable<TraitDetection>) null;
        determination.IntentDetections = (IEnumerable<IntentDetection>) null;
        determination.Package = (Package) null;
        if (determination.PolicyViolations != null)
        {
          foreach (PolicyViolation policyViolation in determination.PolicyViolations)
            policyViolation.TraitDetection.DetectionIndicators = (IEnumerable<Indicator>) null;
        }
      }
      callbackResult.Package = (Package) null;
      callbackResult.ProviderResults = (IEnumerable<ProviderResult>) null;
      return JsonConvert.SerializeObject((object) callbackResult, Formatting.None, CvsCloudUtil.CallbackSerializationSettings);
    }
  }
}
