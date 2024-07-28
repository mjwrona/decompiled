// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ResourceManagementComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ResourceManagementComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<ResourceManagementComponent>(1),
      (IComponentCreator) new ComponentCreator<ResourceManagementComponent2>(2),
      (IComponentCreator) new ComponentCreator<ResourceManagementComponent3>(3)
    }, "ResourceManagement");
    private static readonly string s_TryGetServiceVersionStmt = "IF OBJECT_ID('dbo.prc_GetServiceVersion', 'P') IS NOT NULL\r\nBEGIN\r\n    SELECT  CAST(1 AS BIT) AS SchemaExists\r\n\r\n    EXEC prc_GetServiceVersion @serviceName\r\nEND\r\nELSE\r\nBEGIN\r\n    SELECT  CAST(0 AS BIT) AS SchemaExists\r\nEND";

    public ResultCollection QueryServiceVersion()
    {
      this.PrepareStoredProcedure("prc_QueryServiceVersion");
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryServiceVersion", this.RequestContext);
      resultCollection.AddBinder<ServiceVersionEntry>((ObjectBinder<ServiceVersionEntry>) new ServiceVersionEntryColumns());
      return resultCollection;
    }

    public ServiceVersionEntry GetServiceVersion(string serviceName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(serviceName, nameof (serviceName));
      this.PrepareStoredProcedure("prc_GetServiceVersion");
      this.BindString("@serviceName", serviceName, serviceName.Length, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_GetServiceVersion", this.RequestContext);
      resultCollection.AddBinder<ServiceVersionEntry>((ObjectBinder<ServiceVersionEntry>) new ServiceVersionEntryColumns());
      return resultCollection.GetCurrent<ServiceVersionEntry>().Items.FirstOrDefault<ServiceVersionEntry>();
    }

    public ServiceVersionEntry TryGetServiceVersion(string serviceName, out bool schemaExists)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(serviceName, nameof (serviceName));
      this.PrepareSqlBatch(ResourceManagementComponent.s_TryGetServiceVersionStmt.Length);
      this.AddStatement(ResourceManagementComponent.s_TryGetServiceVersionStmt, 1);
      this.BindString("@serviceName", serviceName, serviceName.Length, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), nameof (TryGetServiceVersion), this.RequestContext);
      resultCollection.AddBinder<bool>((ObjectBinder<bool>) new SchemaExistsColumns());
      resultCollection.AddBinder<ServiceVersionEntry>((ObjectBinder<ServiceVersionEntry>) new ServiceVersionEntryColumns());
      ObjectBinder<bool> current = resultCollection.GetCurrent<bool>();
      schemaExists = current.MoveNext() ? current.Current : throw new UnexpectedDatabaseResultException(nameof (TryGetServiceVersion));
      ServiceVersionEntry serviceVersion = (ServiceVersionEntry) null;
      if (schemaExists)
      {
        resultCollection.NextResult();
        serviceVersion = resultCollection.GetCurrent<ServiceVersionEntry>().Items.FirstOrDefault<ServiceVersionEntry>();
      }
      return serviceVersion;
    }

    public void SetServiceVersion(string serviceName, int version, int minVersion)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(serviceName, nameof (serviceName));
      this.PrepareStoredProcedure("prc_SetServiceVersion");
      this.BindString("@serviceName", serviceName, serviceName.Length, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindInt("@version", version);
      this.BindInt("@minVersion", minVersion);
      this.ExecuteNonQuery();
    }
  }
}
