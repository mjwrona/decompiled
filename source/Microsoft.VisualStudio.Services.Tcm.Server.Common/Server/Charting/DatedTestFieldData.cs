// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Charting.DatedTestFieldData
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;

namespace Microsoft.TeamFoundation.TestManagement.Server.Charting
{
  public struct DatedTestFieldData
  {
    public DatedTestFieldData(DateTime date, TestFieldData value)
      : this()
    {
      this.Date = date;
      this.Value = value;
    }

    public DateTime Date { get; private set; }

    public TestFieldData Value { get; private set; }

    public Microsoft.TeamFoundation.TestManagement.WebApi.DatedTestFieldData ToWebApiModel() => new Microsoft.TeamFoundation.TestManagement.WebApi.DatedTestFieldData()
    {
      Date = this.Date,
      Value = this.Value.ToWebApiModel()
    };
  }
}
