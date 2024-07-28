// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.CommitLog.Operations.NpmOperations
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseSerializers;

namespace Microsoft.VisualStudio.Services.Npm.Server.CommitLog.Operations
{
  public static class NpmOperations
  {
    public static readonly CommonProtocolOperations CommonOperations = new CommonProtocolOperations((ProtocolOperation) NpmViewOperation.Instance, (ProtocolOperation) NpmUnpublishOperation.Instance, (ProtocolOperation) NpmRestoreToFeedOperation.Instance, (ProtocolOperation) null, (ProtocolOperation) null);
  }
}
