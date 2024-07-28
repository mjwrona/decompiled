// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.ProtocolPackageVersionIdentityBinder
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  internal class ProtocolPackageVersionIdentityBinder : ObjectBinder<ProtocolPackageVersionIdentity>
  {
    private static SqlColumnBinder protocol = new SqlColumnBinder("ProtocolType");
    private static SqlColumnBinder packageName = new SqlColumnBinder("NormalizedPackageName");
    private static SqlColumnBinder packageId = new SqlColumnBinder("PackageId");
    private static SqlColumnBinder version = new SqlColumnBinder("NormalizedPackageVersion");
    private static SqlColumnBinder versionId = new SqlColumnBinder("PackageVersionId");
    private readonly ProtocolPackageVersionIdentityBinder.BindOptions bindOptions;

    public ProtocolPackageVersionIdentityBinder(
      ProtocolPackageVersionIdentityBinder.BindOptions bindOptions = ProtocolPackageVersionIdentityBinder.BindOptions.None)
    {
      this.bindOptions = bindOptions;
    }

    protected override ProtocolPackageVersionIdentity Bind() => this.bindOptions.HasFlag((Enum) ProtocolPackageVersionIdentityBinder.BindOptions.IncludePackageIdAndVersionId) ? new ProtocolPackageVersionIdentity(ProtocolPackageVersionIdentityBinder.protocol.GetString((IDataReader) this.Reader, false), ProtocolPackageVersionIdentityBinder.packageName.GetString((IDataReader) this.Reader, false), ProtocolPackageVersionIdentityBinder.version.GetString((IDataReader) this.Reader, false), ProtocolPackageVersionIdentityBinder.packageId.GetGuid((IDataReader) this.Reader, false), ProtocolPackageVersionIdentityBinder.versionId.GetGuid((IDataReader) this.Reader, false)) : new ProtocolPackageVersionIdentity(ProtocolPackageVersionIdentityBinder.protocol.GetString((IDataReader) this.Reader, false), ProtocolPackageVersionIdentityBinder.packageName.GetString((IDataReader) this.Reader, false), ProtocolPackageVersionIdentityBinder.version.GetString((IDataReader) this.Reader, false));

    public enum BindOptions
    {
      None,
      IncludePackageIdAndVersionId,
    }
  }
}
