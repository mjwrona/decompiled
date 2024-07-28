// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MachineManagement.WebApi.MachineRequestTypeFactory
// Assembly: Microsoft.VisualStudio.Services.MachineManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0CB85E58-B74B-46EE-B86D-9E028F77476B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.MachineManagement.WebApi.dll

namespace Microsoft.VisualStudio.Services.MachineManagement.WebApi
{
  public static class MachineRequestTypeFactory
  {
    public static MachineRequestType GetMachineRequestType(MachineRequestTypes requestType)
    {
      switch (requestType)
      {
        case MachineRequestTypes.CloudTest:
          return new MachineRequestType()
          {
            Name = "CloudTest",
            AllowMultipleRequestsOfTypePerHost = true
          };
        case MachineRequestTypes.DistributedTask:
          return new MachineRequestType()
          {
            Name = "DistributedTask",
            AllowMultipleRequestsOfTypePerHost = true
          };
        case MachineRequestTypes.XamlBuild:
          return new MachineRequestType()
          {
            Name = "XamlBuild",
            AllowMultipleRequestsOfTypePerHost = false
          };
        default:
          return (MachineRequestType) null;
      }
    }
  }
}
