// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.CVS.CVSScanResponse
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.Ops.Cvs.Client.DataContracts;
using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Gallery.Server.Extension.ExtensionPostUploadProcessing.Validation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server.CVS
{
  internal class CVSScanResponse
  {
    private Job m_rawjob;
    private const string s_layer = "CVSScanResponse";

    public CVSScanResponse(Job job)
    {
      this.m_rawjob = job;
      this.Violations = (IEnumerable<string>) new List<string>();
      this.ValidationStatus = ValidationStatus.NotStarted;
      this.JobId = string.Empty;
      this.ParseRawJob();
    }

    public IEnumerable<string> Violations { get; private set; }

    public IEnumerable<string> Faults { get; private set; }

    public string FailureLogs { get; private set; }

    public ValidationStatus ValidationStatus { get; private set; }

    public string JobId { get; private set; }

    private void ParseRawJob()
    {
      if (this.m_rawjob == null)
        return;
      this.JobId = ((BaseResource) this.m_rawjob).Id;
      switch (this.m_rawjob.Status - 1)
      {
        case 0:
        case 1:
          this.ValidationStatus = ValidationStatus.InProgress;
          break;
        case 2:
          this.Violations = (IEnumerable<string>) this.GetViolations(this.m_rawjob.Determination);
          if (this.Violations.IsNullOrEmpty<string>())
          {
            this.ValidationStatus = ValidationStatus.Success;
            break;
          }
          this.ValidationStatus = ValidationStatus.Failure;
          break;
        case 3:
          this.FailureLogs = JsonConvert.SerializeObject((object) this.m_rawjob, Formatting.Indented, new JsonSerializerSettings()
          {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
          });
          this.Faults = (IEnumerable<string>) this.GetFaults(this.m_rawjob.ProviderResults);
          goto case 2;
        default:
          this.FailureLogs = JsonConvert.SerializeObject((object) this.m_rawjob, Formatting.Indented, new JsonSerializerSettings()
          {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
          });
          this.ValidationStatus = ValidationStatus.InternalFailure;
          this.Faults = (IEnumerable<string>) this.GetFaults(this.m_rawjob.ProviderResults);
          break;
      }
    }

    private List<string> GetViolations(ProviderResult determination)
    {
      List<string> violations = (List<string>) null;
      if (determination != null && determination.PolicyViolationCount > 0)
      {
        violations = new List<string>();
        foreach (PolicyViolation policyViolation in determination.PolicyViolations)
        {
          string str = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}", (object) policyViolation.CvsPolicy.Name);
          IEnumerable<ContentItem> contentItems = determination.ContentItems;
          ContentType? nullable1;
          ContentType? nullable2;
          if (contentItems == null)
          {
            nullable1 = new ContentType?();
            nullable2 = nullable1;
          }
          else
          {
            ContentItem contentItem = contentItems.FirstOrDefault<ContentItem>();
            if (contentItem == null)
            {
              nullable1 = new ContentType?();
              nullable2 = nullable1;
            }
            else
              nullable2 = new ContentType?(contentItem.ContentType);
          }
          nullable1 = nullable2;
          if (nullable1.GetValueOrDefault() == 1)
          {
            List<string> violatingText = this.GetViolatingText(policyViolation);
            if (!violatingText.IsNullOrEmpty<string>())
              str += string.Format((IFormatProvider) CultureInfo.InvariantCulture, ": {0} - {1}", (object) GalleryResources.TextViolationWords(), (object) string.Join(",", (IEnumerable<string>) violatingText));
          }
          violations.Add(str);
        }
      }
      return violations;
    }

    private List<string> GetViolatingText(PolicyViolation policyViolation)
    {
      Dictionary<long, string> dictionary = new Dictionary<long, string>();
      bool? nullable;
      if (policyViolation == null)
      {
        nullable = new bool?();
      }
      else
      {
        TraitDetection traitDetection = policyViolation.TraitDetection;
        if (traitDetection == null)
        {
          nullable = new bool?();
        }
        else
        {
          IEnumerable<Indicator> detectionIndicators = traitDetection.DetectionIndicators;
          nullable = detectionIndicators != null ? new bool?(detectionIndicators.IsNullOrEmpty<Indicator>()) : new bool?();
        }
      }
      if (((int) nullable ?? 1) == 0)
      {
        foreach (TextIndicator detectionIndicator in policyViolation.TraitDetection.DetectionIndicators)
          dictionary[detectionIndicator.Location] = detectionIndicator.Text;
      }
      return dictionary.Values.ToList<string>();
    }

    private List<string> GetFaults(IEnumerable<ProviderResult> providerResults)
    {
      List<string> faults = (List<string>) null;
      if (!providerResults.IsNullOrEmpty<ProviderResult>())
      {
        IEnumerable<ProviderResult> enumerable = providerResults.Where<ProviderResult>((Func<ProviderResult, bool>) (pr => !pr.Faults.IsNullOrEmpty<Fault>()));
        if (!enumerable.IsNullOrEmpty<ProviderResult>())
        {
          faults = new List<string>();
          foreach (ProviderResult providerResult in enumerable)
          {
            foreach (Fault fault in providerResult.Faults)
              faults.Add(fault.Detail);
          }
        }
      }
      return faults;
    }
  }
}
