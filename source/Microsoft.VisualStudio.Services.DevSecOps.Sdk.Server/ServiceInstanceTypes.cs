// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.ServiceInstanceTypes
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEA81E2B-AB47-44C0-8043-66C0E1018997
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server
{
  public static class ServiceInstanceTypes
  {
    private const string DevSecOpsServiceInstanceTypeString = "00000059-0000-8888-8000-000000000000";
    private const string ReleaseMangementServiceInstanceTypeString = "0000000D-0000-8888-8000-000000000000";
    private const string AdvancedSecurityServiceInstanceTypeString = "00000071-0000-8888-8000-000000000000";
    internal static readonly Guid DevSecOpsServiceInstanceType = new Guid("00000059-0000-8888-8000-000000000000");
    internal static readonly Guid ReleaseMangementServiceInstanceType = new Guid("0000000D-0000-8888-8000-000000000000");
    internal static readonly Guid AdvancedSecurityServiceInstanceType = new Guid("00000071-0000-8888-8000-000000000000");
  }
}
