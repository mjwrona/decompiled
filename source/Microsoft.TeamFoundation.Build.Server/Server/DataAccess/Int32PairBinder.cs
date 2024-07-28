// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.Int32PairBinder
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal sealed class Int32PairBinder : BuildObjectBinder<KeyValuePair<int, int>>
  {
    private SqlColumnBinder key = new SqlColumnBinder("Key");
    private SqlColumnBinder value = new SqlColumnBinder("Value");

    protected override KeyValuePair<int, int> Bind() => new KeyValuePair<int, int>(this.key.GetInt32((IDataReader) this.Reader), this.value.GetInt32((IDataReader) this.Reader));
  }
}
