// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.CommitLog.PyPiOperations
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseSerializers;

namespace Microsoft.VisualStudio.Services.PyPi.Server.CommitLog
{
  public static class PyPiOperations
  {
    public static readonly ProtocolOperation PyPiAddOperation = new ProtocolOperation((IProtocol) Protocol.PyPi, "Add", "1.0");
    public static readonly ProtocolOperation PyPiViewOperation = new ProtocolOperation((IProtocol) Protocol.PyPi, "View", "1.0");
    public static readonly ProtocolOperation PyPiDeleteOperation = new ProtocolOperation((IProtocol) Protocol.PyPi, "Delete", "1.0");
    public static readonly ProtocolOperation PyPiRestoreToFeedOperation = new ProtocolOperation((IProtocol) Protocol.PyPi, "RestoreToFeed", "1.0");
    public static readonly ProtocolOperation PyPiPermanentDeleteOperation = new ProtocolOperation((IProtocol) Protocol.PyPi, "PermanentDelete", "1.0");
    public static readonly CommonProtocolOperations CommonOperations = new CommonProtocolOperations(PyPiOperations.PyPiViewOperation, PyPiOperations.PyPiDeleteOperation, PyPiOperations.PyPiRestoreToFeedOperation, (ProtocolOperation) null, (ProtocolOperation) null);
  }
}
