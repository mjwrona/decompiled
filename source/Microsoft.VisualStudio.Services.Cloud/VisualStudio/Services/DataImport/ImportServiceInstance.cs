// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DataImport.ImportServiceInstance
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.DataImport
{
  public class ImportServiceInstance
  {
    public ImportServiceInstance()
    {
    }

    public ImportServiceInstance(Microsoft.TeamFoundation.Framework.Server.ServiceInstance serviceInstance, int importVersion)
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Framework.Server.ServiceInstance>(serviceInstance, nameof (serviceInstance));
      this.ServiceType = serviceInstance.InstanceType;
      this.ServiceName = serviceInstance.Name;
      this.ServiceInstance = serviceInstance.InstanceId;
      this.ImportVersion = importVersion;
    }

    public ImportServiceInstance(
      Guid serviceType,
      string serviceName,
      Guid serviceInstance,
      int importVersion)
    {
      ArgumentUtility.CheckForEmptyGuid(serviceType, nameof (serviceType));
      ArgumentUtility.CheckStringForNullOrEmpty(serviceName, nameof (serviceName));
      ArgumentUtility.CheckForEmptyGuid(serviceInstance, nameof (serviceInstance));
      this.ServiceType = serviceType;
      this.ServiceName = serviceName;
      this.ServiceInstance = serviceInstance;
      this.ImportVersion = importVersion;
    }

    public Guid ServiceType { get; set; }

    public string ServiceName { get; set; }

    public Guid ServiceInstance { get; set; }

    public int ImportVersion { get; set; } = -1;

    public static ImportServiceInstance SpsServiceInstance(
      Guid serviceInstanceId,
      int importVersion = -1)
    {
      return new ImportServiceInstance(ServiceInstanceTypes.SPS, "SPS", serviceInstanceId, importVersion);
    }
  }
}
