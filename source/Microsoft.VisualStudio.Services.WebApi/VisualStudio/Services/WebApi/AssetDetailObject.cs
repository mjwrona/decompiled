// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.AssetDetailObject
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.WebApi
{
  public class AssetDetailObject
  {
    public string OfferMarketingUrlIdentifier { get; set; }

    public string PublisherNaturalIdentifier { get; set; }

    public string ServiceNaturalIdentifier { get; set; }

    public string ProductTypeNaturalIdentifier { get; set; }

    public string PublisherId { get; set; }

    public string PublisherName { get; set; }

    public string OfferId { get; set; }

    public Dictionary<string, PlanDetails> AnswersPerPlan { get; set; }

    public Dictionary<string, List<object>> ServicePlansByMarket { get; set; }

    public Dictionary<string, LangDetails> Languages { get; set; }

    public AnswersDetails Answers { get; set; }
  }
}
