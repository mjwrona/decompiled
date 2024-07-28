// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.GalleryInstallationTargetBinder3
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class GalleryInstallationTargetBinder3 : ObjectBinder<GalleryInstallationTarget3>
  {
    protected SqlColumnBinder ReferenceIdColumn = new SqlColumnBinder("ReferenceId");
    protected SqlColumnBinder TargetColumn = new SqlColumnBinder("Target");
    protected SqlColumnBinder VersionRangeColumn = new SqlColumnBinder("VersionRange");
    protected SqlColumnBinder MinInclusiveColumn = new SqlColumnBinder("MinInclusive");
    protected SqlColumnBinder MaxInclusiveColumn = new SqlColumnBinder("MaxInclusive");
    protected SqlColumnBinder MinVersionColumn = new SqlColumnBinder("MinVersion");
    protected SqlColumnBinder MaxVersionColumn = new SqlColumnBinder("MaxVersion");
    protected SqlColumnBinder ProductArchitecture = new SqlColumnBinder(nameof (ProductArchitecture));
    protected SqlColumnBinder ExtensionVersion = new SqlColumnBinder(nameof (ExtensionVersion));
    protected SqlColumnBinder TargetPlatform = new SqlColumnBinder(nameof (TargetPlatform));

    protected override GalleryInstallationTarget3 Bind()
    {
      GalleryInstallationTarget3 installationTarget3 = new GalleryInstallationTarget3();
      installationTarget3.ReferenceId = this.ReferenceIdColumn.GetGuid((IDataReader) this.Reader);
      installationTarget3.Target = this.TargetColumn.GetString((IDataReader) this.Reader, false);
      installationTarget3.TargetVersion = this.VersionRangeColumn.GetString((IDataReader) this.Reader, true);
      installationTarget3.MinInclusive = this.MinInclusiveColumn.GetBoolean((IDataReader) this.Reader, false);
      installationTarget3.MaxInclusive = this.MaxInclusiveColumn.GetBoolean((IDataReader) this.Reader, false);
      installationTarget3.MaxVersion = Version.Parse(this.MaxVersionColumn.GetString((IDataReader) this.Reader, false));
      installationTarget3.MinVersion = Version.Parse(this.MinVersionColumn.GetString((IDataReader) this.Reader, false));
      installationTarget3.ProductArchitecture = this.ProductArchitecture.GetString((IDataReader) this.Reader, true);
      installationTarget3.ExtensionVersion = this.ExtensionVersion.GetString((IDataReader) this.Reader, true);
      installationTarget3.TargetPlatform = this.TargetPlatform.GetString((IDataReader) this.Reader, true);
      return installationTarget3;
    }
  }
}
