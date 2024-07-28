// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades.ITracerBlock
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades
{
  public interface ITracerBlock : IDisposable
  {
    void TraceInfo(string message);

    void TraceInfo(string[] tags, string message);

    void TraceVerbose(string message);

    void TraceError(string message);

    void TraceException(Exception exception);

    void TraceConditionally(Func<string> messageFunc);

    void TraceConditionally(string[] tags, Func<string> messageFunc);

    IDisposable CreateTimeToFirstPageExclusionBlock();

    void TraceInfoAlways(string message);

    void TraceInfoAlways(string[] tags, string message);

    void TraceMarker(string markerId1, string markerId2 = null);

    void TraceException(Exception exception, string format, params object[] args);
  }
}
