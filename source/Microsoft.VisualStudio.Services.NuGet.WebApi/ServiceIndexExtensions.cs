// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.WebApi.ServiceIndexExtensions
// Assembly: Microsoft.VisualStudio.Services.NuGet.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9D44F181-506D-4445-A06B-7AA7FD5D22D8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.WebApi.dll

using System;
using System.Linq;

namespace Microsoft.VisualStudio.Services.NuGet.WebApi
{
  public static class ServiceIndexExtensions
  {
    public static Uri GetServiceOrDefault(this ServiceIndex serviceIndex, string serviceName)
    {
      ServiceEntry serviceEntry;
      if (serviceName.Contains("/"))
      {
        serviceEntry = serviceIndex.Resources.FirstOrDefault<ServiceEntry>((Func<ServiceEntry, bool>) (x => x.Type == serviceName));
      }
      else
      {
        string serviceNameWithSlash = serviceName + "/";
        serviceEntry = serviceIndex.Resources.FirstOrDefault<ServiceEntry>((Func<ServiceEntry, bool>) (x => x.Type == serviceName || x.Type.StartsWith(serviceNameWithSlash)));
      }
      return serviceEntry != null ? new Uri(serviceEntry.Id) : (Uri) null;
    }

    public static Uri GetService(this ServiceIndex serviceIndex, string serviceName)
    {
      Uri serviceOrDefault = serviceIndex.GetServiceOrDefault(serviceName);
      return !(serviceOrDefault == (Uri) null) ? serviceOrDefault : throw new Exception("Could not find URI for service " + serviceName);
    }

    public static bool HasService(this ServiceIndex serviceIndex, string serviceName) => serviceIndex.GetServiceOrDefault(serviceName) != (Uri) null;
  }
}
