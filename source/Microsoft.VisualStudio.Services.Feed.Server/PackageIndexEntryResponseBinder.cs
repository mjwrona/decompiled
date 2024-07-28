// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.PackageIndexEntryResponseBinder
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.WebApi.Internal;
using System.Data;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  internal class PackageIndexEntryResponseBinder : ObjectBinder<PackageIndexEntryResponse>
  {
    private SqlColumnBinder packageId = new SqlColumnBinder("PackageId");
    private SqlColumnBinder packageVersionId = new SqlColumnBinder("PackageVersionId");
    private SqlColumnBinder created = new SqlColumnBinder("Created");

    protected override PackageIndexEntryResponse Bind() => new PackageIndexEntryResponse()
    {
      PackageId = this.packageId.GetGuid((IDataReader) this.Reader, false),
      PackageVersionId = this.packageVersionId.GetGuid((IDataReader) this.Reader, false),
      Created = this.created.ColumnExists((IDataReader) this.Reader) && this.created.GetBoolean((IDataReader) this.Reader, false)
    };
  }
}
