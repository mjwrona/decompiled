// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Utils.JsonErrorLogger
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using System.Text;


#nullable enable
namespace Microsoft.VisualStudio.Services.Npm.Server.Utils
{
  public static class JsonErrorLogger
  {
    public static IJsonErrorLogger Create(
      ITracerBlock? tracer,
      string? packageNameForLogging,
      string? packageVersionForLogging,
      string? upstreamLocationForLogging,
      string callerSourceLocationContextForLogging)
    {
      StringBuilder stringBuilder = new StringBuilder(callerSourceLocationContextForLogging);
      if (!string.IsNullOrWhiteSpace(packageNameForLogging))
        stringBuilder.AppendFormat(", package name `{0}`", (object) packageNameForLogging);
      if (!string.IsNullOrWhiteSpace(packageVersionForLogging))
        stringBuilder.AppendFormat(", package version `{0}`", (object) packageVersionForLogging);
      if (!string.IsNullOrWhiteSpace(upstreamLocationForLogging))
        stringBuilder.AppendFormat(", from upstream `{0}`", (object) upstreamLocationForLogging);
      string context = stringBuilder.ToString();
      return tracer == null ? (IJsonErrorLogger) new RawJsonErrorLogger(context) : (IJsonErrorLogger) new TracerBlockJsonErrorLogger(tracer, context);
    }
  }
}
