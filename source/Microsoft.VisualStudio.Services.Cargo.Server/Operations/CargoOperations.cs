// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Operations.CargoOperations
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseSerializers;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Operations
{
  public static class CargoOperations
  {
    public static readonly ProtocolOperation CargoAddOperation = new ProtocolOperation((IProtocol) Protocol.Cargo, "Add", "1.0");
    public static readonly ProtocolOperation CargoViewOperation = new ProtocolOperation((IProtocol) Protocol.Cargo, "View", "1.0");
    public static readonly ProtocolOperation CargoDeleteOperation = new ProtocolOperation((IProtocol) Protocol.Cargo, "Delete", "1.0");
    public static readonly ProtocolOperation CargoRestoreToFeedOperation = new ProtocolOperation((IProtocol) Protocol.Cargo, "RestoreToFeed", "1.0");
    public static readonly ProtocolOperation CargoPermanentDeleteOperation = new ProtocolOperation((IProtocol) Protocol.Cargo, "PermanentDelete", "1.0");
    public static readonly ProtocolOperation CargoYankOperation = new ProtocolOperation((IProtocol) Protocol.Cargo, "Yank", "1.0");
    public static readonly ProtocolOperation CargoUnyankOperation = new ProtocolOperation((IProtocol) Protocol.Cargo, "Unyank", "1.0");
    public static readonly CommonProtocolOperations CommonOperations = new CommonProtocolOperations(CargoOperations.CargoViewOperation, CargoOperations.CargoDeleteOperation, CargoOperations.CargoRestoreToFeedOperation, CargoOperations.CargoYankOperation, CargoOperations.CargoUnyankOperation);
  }
}
