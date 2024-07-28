// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Utils.RawJsonErrorLogger
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Newtonsoft.Json.Serialization;
using System.Diagnostics;


#nullable enable
namespace Microsoft.VisualStudio.Services.Npm.Server.Utils
{
  public class RawJsonErrorLogger : IJsonErrorLogger
  {
    public RawJsonErrorLogger(string context)
    {
      this.Context = context;
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }

    public string Context { get; }

    public void LogJsonError(ErrorContext errorContext) => TeamFoundationTracingService.TraceExceptionRaw(12000070, TraceLevel.Error, "npm", "PackageJsonParsing", errorContext.Error, "Exception while processing npm package.json. JSON path: {0}, Context: [{1}]\n{2}", (object) errorContext.Path, (object) this.\u003Ccontext\u003EP, (object) errorContext.Error.ToStringWithCompressedStackTrace());
  }
}
