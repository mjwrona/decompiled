// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Server.BacklogObjectBinder`1
// Assembly: Microsoft.TeamFoundation.Agile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4912F51-3FCA-4D2B-A7B5-CF15E2F3B46B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Agile.Server
{
  public abstract class BacklogObjectBinder<T> : ObjectBinder<T>
  {
    protected override sealed T Bind() => this.Bind(this.Reader);

    public abstract T Bind(IDataReader reader);

    protected IDataReader Reader => this.BaseReader;

    public IEnumerable<T> BindAll(IDataReader reader)
    {
      while (reader.Read())
        yield return this.Bind(reader);
    }
  }
}
