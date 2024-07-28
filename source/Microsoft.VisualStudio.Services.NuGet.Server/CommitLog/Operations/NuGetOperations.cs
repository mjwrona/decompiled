// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.CommitLog.Operations.NuGetOperations
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseSerializers;

namespace Microsoft.VisualStudio.Services.NuGet.Server.CommitLog.Operations
{
  public static class NuGetOperations
  {
    public static readonly CommonProtocolOperations CommonOperations = new CommonProtocolOperations((ProtocolOperation) NuGetViewOperation.Instance, (ProtocolOperation) NuGetDeleteOperation.Instance, (ProtocolOperation) NuGetRestoreToFeedOperation.Instance, (ProtocolOperation) NuGetDelistOperation.Instance, (ProtocolOperation) NuGetRelistOperation.Instance);
  }
}
