// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.TracerCICorrelationForProjectDetails
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4AA9C920-1627-4C01-9F3D-849A7BC9C916
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry
{
  public class TracerCICorrelationForProjectDetails : TracerCICorrelationForCollectionDetails
  {
    public TracerIndexingUnitData ProjectData { get; }

    public TracerCICorrelationForProjectDetails(
      string correlationId,
      string currentContext,
      TracerIndexingUnitData accountData,
      TracerIndexingUnitData collectionData,
      TracerIndexingUnitData projectData)
      : base(correlationId, currentContext, accountData, collectionData)
    {
      this.ProjectData = projectData;
    }

    public override IDictionary<string, object> GetAllCorrelationDetails()
    {
      IDictionary<string, object> correlationDetails = base.GetAllCorrelationDetails();
      if (this.ProjectData != null)
      {
        correlationDetails.Add("ProjectId", (object) this.ProjectData.Id);
        correlationDetails.Add("ProjectIndexingUnitId", (object) this.ProjectData.IndexingUnitId);
      }
      return correlationDetails;
    }
  }
}
