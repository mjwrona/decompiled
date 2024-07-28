// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.CommitLog.NpmCommitLogService
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ItemStore.Server.Common;
using Microsoft.VisualStudio.Services.Npm.Server.ItemStore;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseSerializers;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.ItemStore;

namespace Microsoft.VisualStudio.Services.Npm.Server.CommitLog
{
  public class NpmCommitLogService : 
    CommitLogServiceBase,
    INpmCommitLogService,
    ICommitLogService,
    IRetrievableCommitLogService,
    ICoreCommitLogService,
    IVssFrameworkService,
    IUnsafeNpmCommitLogService,
    IUnsafeCommitLogService
  {
    public NpmCommitLogService()
      : this((ITimeProvider) new DefaultTimeProvider())
    {
    }

    public NpmCommitLogService(ITimeProvider timeProvider)
      : base(timeProvider)
    {
    }

    public override string CommitType => "NpmCommitLog";

    public override void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    protected override IItemStore GetItemStore(IVssRequestContext requestContext) => (IItemStore) new TracingItemStore((IItemStore) requestContext.GetService<NpmItemStore>());

    protected override ICommitEntryDeserializer GetDeserializer(IVssRequestContext requestContext) => (ICommitEntryDeserializer) NpmOperationDeserializer.BootstrapCommitEntryDeserializer(requestContext);

    protected override ICommitEntrySerializer GetSerializer(IVssRequestContext requestContext) => (ICommitEntrySerializer) new CommitEntrySerializer((IProtocolSpecificCommitEntrySerializer) new NpmOperationSerializer());
  }
}
