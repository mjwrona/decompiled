// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.CommitLog.PyPiCommitLogService
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ItemStore.Server.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseSerializers;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ItemStore;

namespace Microsoft.VisualStudio.Services.PyPi.Server.CommitLog
{
  public class PyPiCommitLogService : 
    CommitLogServiceBase,
    IPyPiCommitLogService,
    ICommitLogService,
    IRetrievableCommitLogService,
    ICoreCommitLogService,
    IVssFrameworkService,
    IUnsafePyPiCommitLogService,
    IUnsafeCommitLogService
  {
    public PyPiCommitLogService()
      : this((ITimeProvider) new DefaultTimeProvider())
    {
    }

    public PyPiCommitLogService(ITimeProvider timeProvider)
      : base(timeProvider)
    {
    }

    public override string CommitType => Protocol.PyPi.CommitLogItemType;

    public override void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    protected override IItemStore GetItemStore(IVssRequestContext requestContext) => (IItemStore) new TracingItemStore((IItemStore) requestContext.GetService<PyPiItemStore>());

    protected override ICommitEntryDeserializer GetDeserializer(IVssRequestContext requestContext) => (ICommitEntryDeserializer) PyPiCommitLogEntryDeserializer.BootstrapCommitEntryDeserializer(requestContext);

    protected override ICommitEntrySerializer GetSerializer(IVssRequestContext requestContext) => (ICommitEntrySerializer) new CommitEntrySerializer((IProtocolSpecificCommitEntrySerializer) new PyPiCommitLogEntrySerializer());
  }
}
