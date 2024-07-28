// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachineManagementMessageTypes
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  public static class MachineManagementMessageTypes
  {
    public const string Namespace = "http://schemas.microsoft.com/visualstudio/2012/services/machinemanagement";
    public const string PoolUpdateAction = "http://schemas.microsoft.com/visualstudio/2012/services/machinemanagement/UpdatePool";
    public const string ProcessRequestAction = "http://schemas.microsoft.com/visualstudio/2012/services/machinemanagement/ProcessRequest";
    public const string SendDiagSasUriAction = "http://schemas.microsoft.com/visualstudio/2012/services/machinemanagement/SendDiagSasUri";
    public const string FinishRequestAction = "http://schemas.microsoft.com/visualstudio/2012/services/machinemanagement/FinishRequest";
    public const string CleanupMachineAction = "http://schemas.microsoft.com/visualstudio/2012/services/machinemanagement/CleanupMachine";
    public const string AbortRequestAction = "http://schemas.microsoft.com/visualstudio/2012/services/machinemanagement/AbortRequest";
    public const string RequestTimedOutAction = "http://schemas.microsoft.com/visualstudio/2012/services/machinemanagement/RequestTimedOut";
  }
}
