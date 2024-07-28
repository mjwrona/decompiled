// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.PeriodicPackageCatchUpJob
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public class PeriodicPackageCatchUpJob : AbstractPeriodicCatchUpJob
  {
    protected override IEntityType EntityType => (IEntityType) PackageEntityType.GetInstance();

    protected override int TracePoint => 1080294;

    public PeriodicPackageCatchUpJob()
      : base(Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory.GetInstance())
    {
    }

    internal PeriodicPackageCatchUpJob(IDataAccessFactory dataAccessFactory)
      : base(dataAccessFactory)
    {
    }

    internal override bool IsIndexingEnabled(IVssRequestContext requestContext) => requestContext.IsPackageIndexingEnabled();
  }
}
