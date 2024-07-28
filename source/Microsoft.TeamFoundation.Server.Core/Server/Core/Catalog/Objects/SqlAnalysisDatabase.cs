// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Catalog.Objects.SqlAnalysisDatabase
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
  public class SqlAnalysisDatabase : ServerDatabase
  {
    public static readonly Guid ResourceTypeIdentifier = CatalogResourceTypes.AnalysisDatabase;

    protected override void Reset() => base.Reset();

    public SqlAnalysisInstance DatabaseInstance => this.GetParent<SqlAnalysisInstance>();

    public WarehouseDatabase ReportingWarehouse
    {
      get => this.GetDependency<WarehouseDatabase>(nameof (ReportingWarehouse));
      set => this.SetDependency<WarehouseDatabase>(nameof (ReportingWarehouse), value);
    }

    public virtual string GetConnectionString() => this.DatabaseInstance != null && this.DatabaseInstance.Machine != null ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Data Source={0};Initial Catalog={1};Integrated Security=SSPI;", (object) this.DatabaseInstance.ServerName, (object) this.InitialCatalog) : (string) null;

    public static SqlAnalysisDatabase Register(SqlAnalysisInstance instance, string databaseName)
    {
      ArgumentUtility.CheckForNull<SqlAnalysisInstance>(instance, nameof (instance));
      ArgumentUtility.CheckStringForNullOrEmpty(databaseName, nameof (databaseName));
      SqlAnalysisDatabase analysisDatabase = instance.SqlAnalysisDatabases.FirstOrDefault<SqlAnalysisDatabase>((Func<SqlAnalysisDatabase, bool>) (w => VssStringComparer.DatabaseName.Equals(w.InitialCatalog, databaseName)));
      if (analysisDatabase == null)
      {
        analysisDatabase = instance.CreateChild<SqlAnalysisDatabase>(databaseName);
        analysisDatabase.InitialCatalog = databaseName;
      }
      return analysisDatabase;
    }

    public new static class Fields
    {
      public const string ReportingWarehouse = "ReportingWarehouse";
    }
  }
}
