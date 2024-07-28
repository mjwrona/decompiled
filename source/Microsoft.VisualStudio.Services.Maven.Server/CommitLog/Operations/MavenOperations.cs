// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.CommitLog.Operations.MavenOperations
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Maven.Server.Implementations.CommitLog.Operations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseSerializers;

namespace Microsoft.VisualStudio.Services.Maven.Server.CommitLog.Operations
{
  public static class MavenOperations
  {
    public static readonly CommonProtocolOperations CommonOperations = new CommonProtocolOperations((ProtocolOperation) MavenViewOperation.Instance, (ProtocolOperation) MavenDeleteOperation.Instance, (ProtocolOperation) MavenRestoreToFeedOperation.Instance, (ProtocolOperation) null, (ProtocolOperation) null);
  }
}
