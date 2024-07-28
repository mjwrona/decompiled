// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Service.Risk.Contract.RiskEvaluationResponse
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce.Service.Risk.Contract
{
  public class RiskEvaluationResponse
  {
    public string Id { get; set; }

    public string event_type { get; set; }

    public string created_date { get; set; }

    public string last_modified_date { get; set; }

    public string decision { get; set; }

    public string reason_code { get; set; }

    public List<string> reasons { get; set; }

    public string token { get; set; }

    public Dictionary<string, Link> links { get; set; }

    public string object_type { get; set; }

    public string contract_version { get; set; }
  }
}
