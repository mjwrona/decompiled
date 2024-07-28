// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Service.Risk.Contract.RiskEvaluationRequestPayload
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

namespace Microsoft.VisualStudio.Services.Commerce.Service.Risk.Contract
{
  public class RiskEvaluationRequestPayload
  {
    public const string EventTypeString = "purchase";
    public const string UnknownLabel = "unknown";

    public RiskEvaluationEventDetails event_details { get; set; }

    public string event_type { get; set; }
  }
}
