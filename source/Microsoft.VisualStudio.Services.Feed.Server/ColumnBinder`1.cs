// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.ColumnBinder`1
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  internal class ColumnBinder<T> : ObjectBinder<T>
  {
    private SqlColumnBinder columnBinder;
    private readonly System.Func<IDataReader, T> binder;

    public ColumnBinder(string columnName, System.Func<IDataReader, T> binder)
    {
      this.columnBinder = new SqlColumnBinder(columnName);
      this.binder = binder;
    }

    protected override T Bind() => this.binder(this.BaseReader);
  }
}
