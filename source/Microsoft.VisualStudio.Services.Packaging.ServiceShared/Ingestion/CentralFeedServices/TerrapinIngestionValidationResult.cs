// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices.TerrapinIngestionValidationResult
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices
{
  public class TerrapinIngestionValidationResult
  {
    public static TerrapinIngestionValidationResult Approved { get; } = new TerrapinIngestionValidationResult(TerrapinIngestionValidationOverallResult.Approved, (IEnumerable<TerrapinIngestionValidationReason>) ImmutableList<TerrapinIngestionValidationReason>.Empty);

    public TerrapinIngestionValidationResult(
      TerrapinIngestionValidationOverallResult overallResult,
      IEnumerable<TerrapinIngestionValidationReason> reasons)
    {
      this.OverallResult = overallResult;
      this.Reasons = reasons.ToImmutableList<TerrapinIngestionValidationReason>();
    }

    public TerrapinIngestionValidationOverallResult OverallResult { get; }

    public ImmutableList<TerrapinIngestionValidationReason> Reasons { get; }
  }
}
