// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Logger
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Diagnostics;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  public class Logger : ILogger
  {
    private readonly ITraceRequest _trace;
    private readonly string _area;
    private readonly string _layer;

    public Logger(ITraceRequest trace, string area, string layer)
    {
      this._trace = trace;
      this._area = area;
      this._layer = layer;
    }

    public void Log(TraceLevel level, int tracepointId, string message) => this._trace.Trace(tracepointId, level, this._area, this._layer, message);
  }
}
