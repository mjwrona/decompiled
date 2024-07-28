// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Client.TestPointState
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.TestManagement.Client
{
  [GenerateAllConstants(null)]
  public enum TestPointState
  {
    None = 0,
    Ready = 1,
    Completed = 2,
    NotReady = 3,
    InProgress = 4,
    MaxValue = 4,
  }
}
