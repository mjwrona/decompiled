// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Implementations.CommitLog.Services.MavenCommitLogService
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ItemStore.Server.Common;
using Microsoft.VisualStudio.Services.Maven.Server.Contracts;
using Microsoft.VisualStudio.Services.Maven.Server.Implementations.CommitLog.OperationData;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseSerializers;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ItemStore;

namespace Microsoft.VisualStudio.Services.Maven.Server.Implementations.CommitLog.Services
{
  public class MavenCommitLogService : 
    CommitLogServiceBase,
    IMavenCommitLogService,
    ICommitLogService,
    IRetrievableCommitLogService,
    ICoreCommitLogService,
    IVssFrameworkService,
    IUnsafeMavenCommitLogService,
    IUnsafeCommitLogService
  {
    public MavenCommitLogService()
      : this((ITimeProvider) new DefaultTimeProvider())
    {
    }

    public MavenCommitLogService(ITimeProvider timeProvider)
      : base(timeProvider)
    {
    }

    public override string CommitType => Protocol.Maven.CommitLogItemType;

    public override void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    protected override IItemStore GetItemStore(IVssRequestContext requestContext) => (IItemStore) new TracingItemStore((IItemStore) requestContext.GetService<MavenItemStore>());

    protected override ICommitEntryDeserializer GetDeserializer(IVssRequestContext requestContext) => (ICommitEntryDeserializer) MavenOperationDeserializer.BootstrapCommitEntryDeserializer(requestContext);

    protected override ICommitEntrySerializer GetSerializer(IVssRequestContext requestContext) => (ICommitEntrySerializer) new CommitEntrySerializer((IProtocolSpecificCommitEntrySerializer) new MavenOperationSerializer());
  }
}
