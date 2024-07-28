// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Catalog.Objects.WarehouseDatabase
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core.Catalog.Objects
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class WarehouseDatabase : ServerDatabase
  {
    public static readonly Guid ResourceTypeIdentifier = CatalogResourceTypes.WarehouseDatabase;

    protected override void Reset() => base.Reset();

    public SqlReportingInstance DatabaseInstance => this.GetParent<SqlReportingInstance>();

    public virtual string GetConnectionString() => this.DatabaseInstance != null && this.DatabaseInstance.Machine != null ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Data Source={0};Initial Catalog={1};Integrated Security=SSPI;", (object) this.DatabaseInstance.ServerName, (object) this.InitialCatalog) : (string) null;

    public static WarehouseDatabase Register(SqlReportingInstance instance, string databaseName)
    {
      ArgumentUtility.CheckForNull<SqlReportingInstance>(instance, nameof (instance));
      ArgumentUtility.CheckStringForNullOrEmpty(databaseName, nameof (databaseName));
      WarehouseDatabase warehouseDatabase = instance.WarehouseDatabases.FirstOrDefault<WarehouseDatabase>((Func<WarehouseDatabase, bool>) (w => VssStringComparer.DatabaseName.Equals(w.InitialCatalog, databaseName)));
      if (warehouseDatabase == null)
      {
        warehouseDatabase = instance.CreateChild<WarehouseDatabase>(databaseName);
        warehouseDatabase.InitialCatalog = databaseName;
      }
      return warehouseDatabase;
    }
  }
}
