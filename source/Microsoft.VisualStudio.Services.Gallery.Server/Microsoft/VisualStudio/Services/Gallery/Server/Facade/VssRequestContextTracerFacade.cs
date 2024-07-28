// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Facade.VssRequestContextTracerFacade
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Facade
{
  public sealed class VssRequestContextTracerFacade : ITracerFacade
  {
    private const string area = "gallery";
    private readonly ITraceRequest tracer;

    public VssRequestContextTracerFacade(ITraceRequest tracer) => this.tracer = tracer ?? throw new ArgumentNullException(nameof (tracer));

    public void TraceException(int tracepoint, string layer, Exception exception) => this.tracer.TraceException(tracepoint, TraceLevel.Error, "gallery", layer, exception);
  }
}
