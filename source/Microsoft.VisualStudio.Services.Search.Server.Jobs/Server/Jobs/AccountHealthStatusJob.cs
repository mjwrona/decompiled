// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.AccountHealthStatusJob
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Constants;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public class AccountHealthStatusJob : AbstractHealthStatusJob
  {
    public AccountHealthStatusJob()
      : this(Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.DataAccessFactory.GetInstance())
    {
    }

    [Info("InternalForTestPurpose")]
    internal AccountHealthStatusJob(IDataAccessFactory dataAccessFactory)
      : base(dataAccessFactory, JobConstants.AccountHealthStatusJobId)
    {
    }

    internal override HealthStatusRecord GetDefaultHealthStatusRecord(
      IVssRequestContext requestContext)
    {
      throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("No record found in input table for collection {0}", (object) requestContext.GetCollectionID())));
    }

    internal override void ValidateHealthStatusRecord(HealthStatusRecord healthStatusRecord)
    {
      if (healthStatusRecord == null || healthStatusRecord.Data.EntityType == null || healthStatusRecord.Data.EntityType == AllEntityType.GetInstance())
        throw new SearchServiceException(FormattableString.Invariant(FormattableStringFactory.Create("Invalid EntityType for {0}", (object) nameof (AccountHealthStatusJob))));
    }
  }
}
