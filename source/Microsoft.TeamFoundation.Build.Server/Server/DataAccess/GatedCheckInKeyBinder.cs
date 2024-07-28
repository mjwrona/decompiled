// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.GatedCheckInKeyBinder
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal sealed class GatedCheckInKeyBinder : BuildObjectBinder<GatedCheckInKey>
  {
    private SqlColumnBinder iv = new SqlColumnBinder("IV");
    private SqlColumnBinder key = new SqlColumnBinder("Key");
    private SqlColumnBinder blockSize = new SqlColumnBinder("BlockSize");

    protected override GatedCheckInKey Bind() => new GatedCheckInKey(this.key.GetBytes((IDataReader) this.Reader, false), this.iv.GetBytes((IDataReader) this.Reader, false), this.blockSize.GetInt32((IDataReader) this.Reader));
  }
}
