// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachinePoolPropertiesDefaultValues
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  public static class MachinePoolPropertiesDefaultValues
  {
    public const int MinNumberOfMachinesInPool = 5;
    public const int MinNumberOfMachinesInNestedPool = 1;
    public const string DefaultDisconnectTimeout = "00:15:00";
    public const int DefaultNotStartedTimeoutMinutes = 15;
    public const int DefaultMachineReimageLaterTimeoutMinutes = 60;
    public const int DefaultMachineReimageLaterStaggerRangeMinutes = 60;
    public const int RequestAssignmentAttemptsLimit = 10;
    public const int DefaultReimageOrchestrationTimeoutMinutes = 1440;
    public const int RamDiskSizeGB = 10;
  }
}
