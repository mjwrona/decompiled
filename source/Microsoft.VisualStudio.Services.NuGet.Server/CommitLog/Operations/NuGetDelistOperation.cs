// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.CommitLog.Operations.NuGetDelistOperation
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using System;

namespace Microsoft.VisualStudio.Services.NuGet.Server.CommitLog.Operations
{
  public class NuGetDelistOperation : ProtocolOperation
  {
    private static readonly Lazy<NuGetDelistOperation> LazyInstance = new Lazy<NuGetDelistOperation>((Func<NuGetDelistOperation>) (() => new NuGetDelistOperation()));

    private NuGetDelistOperation()
      : base("NuGet", "Delist", "1.0")
    {
    }

    public static NuGetDelistOperation Instance { get; } = NuGetDelistOperation.LazyInstance.Value;
  }
}
