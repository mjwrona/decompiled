// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.TracerIndexingUnitData
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4AA9C920-1627-4C01-9F3D-849A7BC9C916
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.dll

using System;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry
{
  public class TracerIndexingUnitData
  {
    public string Name { get; private set; }

    public string Id { get; private set; }

    public int IndexingUnitId { get; private set; }

    public TracerIndexingUnitData(string name, Guid id)
    {
      this.Name = name;
      this.Id = id.ToString();
      this.IndexingUnitId = -1;
    }

    public TracerIndexingUnitData(string name, Guid id, int indexingUnitId)
    {
      this.Name = name;
      this.Id = id.ToString();
      this.IndexingUnitId = indexingUnitId;
    }
  }
}
