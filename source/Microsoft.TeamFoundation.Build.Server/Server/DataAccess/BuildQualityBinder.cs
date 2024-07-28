// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.DataAccess.BuildQualityBinder
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Build.Server.DataAccess
{
  internal sealed class BuildQualityBinder : BuildObjectBinder<string>
  {
    private SqlColumnBinder quality = new SqlColumnBinder("Quality");

    protected override string Bind() => BuildQuality.TryConvertResIdToBuildQuality(this.quality.GetString((IDataReader) this.Reader, true));
  }
}
