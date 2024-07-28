// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.Catalog.Objects.SqlAnalysisInstance
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Client.Catalog.Objects
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class SqlAnalysisInstance : ServerInstance
  {
    public static readonly Guid ResourceTypeIdentifier = CatalogResourceTypes.SqlAnalysisInstance;
    private static readonly Type[] KnownChildTypes = new Type[1]
    {
      typeof (SqlAnalysisDatabase)
    };
    private ICollection<SqlAnalysisDatabase> m_SqlAnalysisDatabases;

    protected override void Reset()
    {
      base.Reset();
      this.m_SqlAnalysisDatabases = (ICollection<SqlAnalysisDatabase>) null;
    }

    public override void Preload(CatalogBulkData bulkData)
    {
      base.Preload(bulkData);
      this.m_SqlAnalysisDatabases = this.PreloadChildren<SqlAnalysisDatabase>(bulkData);
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public ICollection<SqlAnalysisDatabase> SqlAnalysisDatabases => this.GetChildCollection<SqlAnalysisDatabase>(ref this.m_SqlAnalysisDatabases);

    public static SqlAnalysisInstance Register(Machine machine, string instanceName) => ServerInstance.Register<SqlAnalysisInstance>(machine, instanceName, instanceName, machine.SqlAnalysisInstances);
  }
}
