// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.DataAccess.SingleInt32ValueBinder
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.Azure.Pipelines.Checks.Server.DataAccess
{
  internal class SingleInt32ValueBinder : ObjectBinder<int>
  {
    private SqlColumnBinder deletedCount;

    public SingleInt32ValueBinder(string columnName) => this.deletedCount = new SqlColumnBinder(columnName);

    protected override int Bind() => this.deletedCount.GetInt32((IDataReader) this.Reader, 0, 0);
  }
}
