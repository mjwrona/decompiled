// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.DataAccess.PackageDataBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Server.DataAccess
{
  internal class PackageDataBinder : ObjectBinder<PackageData>
  {
    private SqlColumnBinder PackageColumn = new SqlColumnBinder("PackageType");
    private SqlColumnBinder PlatformColumn = new SqlColumnBinder("Platform");
    private SqlColumnBinder MajorVersionColumn = new SqlColumnBinder("MajorVersion");
    private SqlColumnBinder MinorVersionColumn = new SqlColumnBinder("MinorVersion");
    private SqlColumnBinder PatchVersionColumn = new SqlColumnBinder("PatchVersion");
    private SqlColumnBinder CreatedOnColumn = new SqlColumnBinder("CreatedOn");
    private SqlColumnBinder DataColumn = new SqlColumnBinder("Data");

    protected override PackageData Bind()
    {
      PackageMetadata package = new PackageMetadata();
      package.Type = this.PackageColumn.GetString((IDataReader) this.Reader, false);
      package.Platform = this.PlatformColumn.GetString((IDataReader) this.Reader, false);
      package.Version = new PackageVersion()
      {
        Major = this.MajorVersionColumn.GetInt32((IDataReader) this.Reader),
        Minor = this.MinorVersionColumn.GetInt32((IDataReader) this.Reader),
        Patch = this.PatchVersionColumn.GetInt32((IDataReader) this.Reader)
      };
      package.CreatedOn = this.CreatedOnColumn.GetDateTime((IDataReader) this.Reader);
      IDictionary<string, string> data = (IDictionary<string, string>) null;
      if (!this.DataColumn.IsNull((IDataReader) this.Reader))
        data = JsonUtility.Deserialize<IDictionary<string, string>>(this.DataColumn.GetBytes((IDataReader) this.Reader, false));
      return new PackageData(package, data);
    }
  }
}
