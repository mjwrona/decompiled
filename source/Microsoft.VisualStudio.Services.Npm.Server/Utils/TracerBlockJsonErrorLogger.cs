// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Utils.TracerBlockJsonErrorLogger
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Newtonsoft.Json.Serialization;


#nullable enable
namespace Microsoft.VisualStudio.Services.Npm.Server.Utils
{
  public class TracerBlockJsonErrorLogger : IJsonErrorLogger
  {
    public TracerBlockJsonErrorLogger(ITracerBlock tracer, string context)
    {
      // ISSUE: reference to a compiler-generated field
      this.\u003Ctracer\u003EP = tracer;
      this.Context = context;
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }

    public string Context { get; }

    public void LogJsonError(ErrorContext errorContext) => this.\u003Ctracer\u003EP.TraceException(errorContext.Error, "Exception while processing npm package.json. JSON path: {0}, Context: [{1}]\n{2}", (object) errorContext.Path, (object) this.\u003Ccontext\u003EP, (object) errorContext.Error.ToStringWithCompressedStackTrace());
  }
}
