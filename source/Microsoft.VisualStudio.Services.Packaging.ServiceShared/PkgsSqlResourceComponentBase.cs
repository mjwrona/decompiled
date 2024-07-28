// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PkgsSqlResourceComponentBase
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class PkgsSqlResourceComponentBase : TeamFoundationSqlResourceComponent
  {
    public PkgsSqlResourceComponentBase()
    {
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
      this.ContainerErrorCode = 50000;
    }

    protected virtual void BindFeedIdentity(
      FeedIdentity feedId,
      bool createMissingDataspace = false,
      string dataspaceName = "@dataspaceId",
      string feedIdName = "@feedId")
    {
      int dataspaceId = this.GetDataspaceId(feedId.ProjectId ?? Guid.Empty, "Packaging", createMissingDataspace);
      this.BindInt(dataspaceName, dataspaceId);
      this.BindGuid(feedIdName, feedId.Id);
    }

    protected virtual void BindProtocol(IProtocol protocol, string protocolName = "@protocol") => this.BindString(protocolName, protocol?.CorrectlyCasedName, 20, false, SqlDbType.VarChar);
  }
}
