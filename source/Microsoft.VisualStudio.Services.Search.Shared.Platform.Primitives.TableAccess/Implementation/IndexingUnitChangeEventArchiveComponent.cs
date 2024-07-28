// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation.IndexingUnitChangeEventArchiveComponent
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Common;
using System;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Implementation
{
  public class IndexingUnitChangeEventArchiveComponent : TeamFoundationSqlResourceComponent
  {
    public IndexingUnitChangeEventArchiveComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public void IndexingUnitChangeEventConnect(string connectionString, int partitionId)
    {
      ISqlConnectionInfo connectionInfo = SqlConnectionInfoFactory.Create(connectionString);
      this.PartitionId = partitionId;
      this.Initialize(connectionInfo, 3600, 20, 1, 1, this.GetDefaultLogger(), (CircuitBreakerDatabaseProperties) null);
    }

    public void DeleteOldArchives(int keepOlderEventsInDays)
    {
      try
      {
        this.PrepareStoredProcedure("Search.prc_DeleteArchivedIndexingUnitChangeEvents");
        this.BindInt("@days", keepOlderEventsInDays);
        this.ExecuteNonQuery(false);
      }
      catch (Exception ex)
      {
        throw new TableAccessException(TableAcessErrorCodeEnum.UNEXPECTED_ERROR, ex, "Failed to delete from Change Events Archive table");
      }
    }
  }
}
