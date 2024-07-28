// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.TracerCICorrelationForTemporaryIndexingUnitDetails
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4AA9C920-1627-4C01-9F3D-849A7BC9C916
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.dll

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance")]
  public class TracerCICorrelationForTemporaryIndexingUnitDetails : 
    TracerCICorrelationForScopedIndexingUnitDetails,
    ITracerCICorrelationDetails
  {
    public TracerIndexingUnitData TemporaryIndexingUnitData { get; }

    public TracerCICorrelationForTemporaryIndexingUnitDetails(
      string correlationId,
      string currentContext,
      TracerIndexingUnitData accountData,
      TracerIndexingUnitData collectionData,
      TracerIndexingUnitData projectData,
      TracerIndexingUnitData repositoryData,
      TracerIndexingUnitData scopedIndexingUnitData,
      TracerIndexingUnitData tempIndexingUnitData)
      : base(correlationId, currentContext, accountData, collectionData, projectData, repositoryData, scopedIndexingUnitData)
    {
      this.TemporaryIndexingUnitData = tempIndexingUnitData;
    }

    public override IDictionary<string, object> GetAllCorrelationDetails()
    {
      IDictionary<string, object> correlationDetails = base.GetAllCorrelationDetails();
      correlationDetails.Add("TemporaryIndexingUnitId", (object) this.TemporaryIndexingUnitData.Id);
      return correlationDetails;
    }
  }
}
