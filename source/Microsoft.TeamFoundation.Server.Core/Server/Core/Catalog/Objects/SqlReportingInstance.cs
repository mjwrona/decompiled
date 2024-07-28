// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Catalog.Objects.SqlReportingInstance
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Server.Core.Catalog.Objects
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class SqlReportingInstance : ServerInstance
  {
    public static readonly Guid ResourceTypeIdentifier = CatalogResourceTypes.SqlReportingInstance;
    private static readonly Type[] KnownChildTypes = new Type[1]
    {
      typeof (WarehouseDatabase)
    };
    private ICollection<WarehouseDatabase> m_WarehouseDatabases;

    protected override void Reset()
    {
      base.Reset();
      this.m_WarehouseDatabases = (ICollection<WarehouseDatabase>) null;
    }

    public override void Preload(CatalogBulkData bulkData)
    {
      base.Preload(bulkData);
      this.m_WarehouseDatabases = this.PreloadChildren<WarehouseDatabase>(bulkData);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public ICollection<WarehouseDatabase> WarehouseDatabases => this.GetChildCollection<WarehouseDatabase>(ref this.m_WarehouseDatabases);

    public static SqlReportingInstance Register(Machine machine, string instanceName) => ServerInstance.Register<SqlReportingInstance>(machine, instanceName, instanceName, machine.SqlReportingInstances);
  }
}
