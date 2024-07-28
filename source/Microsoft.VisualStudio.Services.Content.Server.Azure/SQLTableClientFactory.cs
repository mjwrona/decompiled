// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.SQLTableClientFactory
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public class SQLTableClientFactory : ITableClientFactory, IDisposable
  {
    private SQLTableAdapter sqlTableInstance;
    private string tableName;

    public SQLTableClientFactory(string tableName)
    {
      this.tableName = tableName;
      this.sqlTableInstance = new SQLTableAdapter(tableName);
    }

    public bool IsReadOnly => false;

    public bool RequiresVssRequestContext => true;

    public void Dispose() => this.Dispose(true);

    public IEnumerable<ITable> GetAllTables() => (IEnumerable<ITable>) new List<ITable>()
    {
      (ITable) this.sqlTableInstance
    };

    public ITable GetTable(string shardHint) => (ITable) this.sqlTableInstance;

    protected virtual void Dispose(bool disposing)
    {
    }
  }
}
