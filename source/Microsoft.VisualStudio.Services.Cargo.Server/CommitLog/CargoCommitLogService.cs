// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.CommitLog.CargoCommitLogService
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ItemStore.Server.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseSerializers;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ItemStore;
using System.Diagnostics.CodeAnalysis;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.CommitLog
{
  [ExcludeFromCodeCoverage]
  public class CargoCommitLogService : 
    CommitLogServiceBase,
    ICargoCommitLogService,
    ICommitLogService,
    IRetrievableCommitLogService,
    ICoreCommitLogService,
    IVssFrameworkService,
    IUnsafeCargoCommitLogService,
    IUnsafeCommitLogService
  {
    public CargoCommitLogService()
      : this((ITimeProvider) new DefaultTimeProvider())
    {
    }

    public CargoCommitLogService(ITimeProvider timeProvider)
      : base(timeProvider)
    {
    }

    public override string CommitType => Protocol.Cargo.CommitLogItemType;

    public override void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    protected override IItemStore GetItemStore(IVssRequestContext requestContext) => (IItemStore) new TracingItemStore((IItemStore) requestContext.GetService<CargoItemStore>());

    protected override ICommitEntryDeserializer GetDeserializer(IVssRequestContext requestContext) => (ICommitEntryDeserializer) CargoOperationDeserializer.BootstrapCommitEntryDeserializer(requestContext);

    protected override ICommitEntrySerializer GetSerializer(IVssRequestContext requestContext) => (ICommitEntrySerializer) new CommitEntrySerializer((IProtocolSpecificCommitEntrySerializer) new CargoOperationSerializer());
  }
}
