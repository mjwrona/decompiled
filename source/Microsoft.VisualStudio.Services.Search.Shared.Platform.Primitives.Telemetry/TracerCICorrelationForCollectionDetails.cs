// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.TracerCICorrelationForCollectionDetails
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4AA9C920-1627-4C01-9F3D-849A7BC9C916
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry
{
  public class TracerCICorrelationForCollectionDetails : TracerCICorrelationForOrganizationDetails
  {
    public TracerIndexingUnitData CollectionData { get; private set; }

    public TracerCICorrelationForCollectionDetails(
      string correlationId,
      string currentContext,
      TracerIndexingUnitData accountData,
      TracerIndexingUnitData collectionData)
      : base(correlationId, currentContext, accountData)
    {
      this.CollectionData = collectionData;
    }

    public override IDictionary<string, object> GetAllCorrelationDetails()
    {
      IDictionary<string, object> correlationDetails = base.GetAllCorrelationDetails();
      correlationDetails.Add("CollectionId", (object) this.CollectionData.Id);
      return correlationDetails;
    }
  }
}
