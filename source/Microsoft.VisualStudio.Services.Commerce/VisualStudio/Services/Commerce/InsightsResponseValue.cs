// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.InsightsResponseValue
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class InsightsResponseValue
  {
    public InsightsResponseAuthorization authorization { get; set; }

    public string caller { get; set; }

    public string channels { get; set; }

    public IDictionary<string, string> claims { get; set; }

    public string correlationId { get; set; }

    public string description { get; set; }

    public string eventDataId { get; set; }

    public InsightsResponseLocalizedValue eventName { get; set; }

    public InsightsResponseLocalizedValue eventSource { get; set; }

    public InsightsHttpRequest httpRequest { get; set; }

    public string id { get; set; }

    public string level { get; set; }

    public string resourceGroupName { get; set; }

    public InsightsResponseLocalizedValue resourceProviderName { get; set; }

    public string resourceUri { get; set; }

    public string operationId { get; set; }

    public InsightsResponseLocalizedValue operationName { get; set; }

    public IDictionary<string, string> properties { get; set; }

    public InsightsResponseLocalizedValue status { get; set; }

    public InsightsResponseLocalizedValue subStatus { get; set; }

    public DateTime eventTimestamp { get; set; }

    public DateTime submissionTimestamp { get; set; }

    public string subscriptionId { get; set; }
  }
}
