// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.HttpHandlers.PackageUpload.WriteProgressStream
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.IO;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.HttpHandlers.PackageUpload
{
  public class WriteProgressStream : ProgressStream
  {
    public WriteProgressStream(
      Stream stream,
      bool leaveOpen,
      IStopwatch stopWatch,
      ITimeProvider timeProvider,
      long bytesInterval)
      : base(stream, leaveOpen, stopWatch, timeProvider, bytesInterval)
    {
    }

    public override void Write(byte[] buffer, int offset, int count) => this.PerformWithTracking((Func<int>) (() =>
    {
      this.BaseStream.Write(buffer, offset, count);
      return count;
    }));
  }
}
