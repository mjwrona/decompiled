// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanning.SuitePointCountQueryParameters2
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server.TestPlanning
{
  internal class SuitePointCountQueryParameters2
  {
    public List<byte> LastResultState { get; set; }

    public List<byte> PointOutcomes { get; set; }
  }
}
