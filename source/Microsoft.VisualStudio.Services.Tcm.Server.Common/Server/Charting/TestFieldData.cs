// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Charting.TestFieldData
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server.Charting
{
  public class TestFieldData
  {
    public TestFieldData(Dictionary<string, object> dimensions, long measure)
    {
      this.Dimensions = dimensions;
      this.Measure = measure;
    }

    public Dictionary<string, object> Dimensions { get; private set; }

    public long Measure { get; private set; }

    public Microsoft.TeamFoundation.TestManagement.WebApi.TestFieldData ToWebApiModel() => new Microsoft.TeamFoundation.TestManagement.WebApi.TestFieldData()
    {
      Dimensions = this.Dimensions,
      Measure = this.Measure
    };
  }
}
