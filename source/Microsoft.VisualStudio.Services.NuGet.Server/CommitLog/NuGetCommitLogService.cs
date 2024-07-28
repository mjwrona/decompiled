// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.CommitLog.NuGetCommitLogService
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ItemStore.Server.Common;
using Microsoft.VisualStudio.Services.NuGet.Server.ItemStore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseSerializers;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ItemStore;

namespace Microsoft.VisualStudio.Services.NuGet.Server.CommitLog
{
  public class NuGetCommitLogService : 
    CommitLogServiceBase,
    INuGetCommitLogService,
    ICommitLogService,
    IRetrievableCommitLogService,
    ICoreCommitLogService,
    IVssFrameworkService,
    IUnsafeNuGetCommitLogService,
    IUnsafeCommitLogService
  {
    public NuGetCommitLogService()
      : this((ITimeProvider) new DefaultTimeProvider())
    {
    }

    public NuGetCommitLogService(ITimeProvider timeProvider)
      : base(timeProvider)
    {
    }

    public override string CommitType => Protocol.NuGet.CommitLogItemType;

    public override void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    protected override IItemStore GetItemStore(IVssRequestContext requestContext) => (IItemStore) new TracingItemStore((IItemStore) requestContext.GetService<NuGetItemStore>());

    protected override ICommitEntryDeserializer GetDeserializer(IVssRequestContext requestContext) => (ICommitEntryDeserializer) NuGetOperationDeserializer.BootstrapCommitEntryDeserializer(requestContext);

    protected override ICommitEntrySerializer GetSerializer(IVssRequestContext requestContext) => (ICommitEntrySerializer) new CommitEntrySerializer((IProtocolSpecificCommitEntrySerializer) new NuGetOperationSerializer());
  }
}
