// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ServicePointAccessor
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Net;
using System.Runtime.InteropServices;

namespace Microsoft.Azure.Cosmos
{
  internal class ServicePointAccessor
  {
    private static readonly bool IsBrowser = RuntimeInformation.IsOSPlatform(OSPlatform.Create("BROWSER")) || RuntimeInformation.IsOSPlatform(OSPlatform.Create("WEBASSEMBLY"));
    private readonly ServicePoint servicePoint;

    private ServicePointAccessor(ServicePoint servicePoint)
    {
      this.servicePoint = servicePoint ?? throw new ArgumentNullException(nameof (servicePoint));
      this.TryDisableUseNagleAlgorithm();
    }

    internal static ServicePointAccessor FindServicePoint(Uri endpoint) => new ServicePointAccessor(ServicePointManager.FindServicePoint(endpoint));

    public int ConnectionLimit
    {
      get => this.servicePoint.ConnectionLimit;
      set => this.TrySetConnectionLimit(value);
    }

    private void TryDisableUseNagleAlgorithm()
    {
      try
      {
        this.servicePoint.UseNagleAlgorithm = false;
      }
      catch (PlatformNotSupportedException ex)
      {
        DefaultTrace.TraceWarning("ServicePoint.set_UseNagleAlgorithm - Platform does not support feature.");
      }
    }

    private void TrySetConnectionLimit(int connectionLimit)
    {
      if (ServicePointAccessor.IsBrowser)
        return;
      try
      {
        this.servicePoint.ConnectionLimit = connectionLimit;
      }
      catch (PlatformNotSupportedException ex)
      {
        DefaultTrace.TraceWarning("ServicePoint.set_ConnectionLimit - Platform does not support feature.");
      }
    }
  }
}
