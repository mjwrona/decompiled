// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataImport.ImportSettings
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataImport
{
  public class ImportSettings
  {
    public ImportSettings(
      IServicingContext servicingContext,
      IVssRequestContext deploymentContext,
      int sourceDatabaseId)
    {
      ArgumentUtility.CheckForNull<IServicingContext>(servicingContext, nameof (servicingContext));
      ArgumentUtility.CheckForNull<IVssRequestContext>(deploymentContext, nameof (deploymentContext));
      this.Logger = servicingContext.TFLogger;
      try
      {
        this.DatabaseProperties = deploymentContext.GetService<ITeamFoundationDatabaseManagementService>().GetDatabase(deploymentContext, sourceDatabaseId);
      }
      catch (Exception ex)
      {
        throw new Exception(string.Format("Error reading database properties from the source database: {0}", (object) sourceDatabaseId), ex);
      }
      this.PartitionId = this.QueryPartitionIdOnSourceDatabase();
    }

    internal ITeamFoundationDatabaseProperties DatabaseProperties { get; private set; }

    internal int PartitionId { get; private set; }

    internal ITFLogger Logger { get; private set; }

    internal virtual int QueryPartitionIdOnSourceDatabase()
    {
      using (DatabasePartitionComponent componentRaw = this.DatabaseProperties.SqlConnectionInfo.CreateComponentRaw<DatabasePartitionComponent>())
        return componentRaw.QueryOnlyPartition().PartitionId;
    }
  }
}
