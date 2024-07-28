// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.GalleryInstallationTargetBinder2
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class GalleryInstallationTargetBinder2 : ObjectBinder<GalleryInstallationTarget2>
  {
    protected SqlColumnBinder ReferenceIdColumn = new SqlColumnBinder("ReferenceId");
    protected SqlColumnBinder TargetColumn = new SqlColumnBinder("Target");
    protected SqlColumnBinder VersionRangeColumn = new SqlColumnBinder("VersionRange");
    protected SqlColumnBinder MinInclusiveColumn = new SqlColumnBinder("MinInclusive");
    protected SqlColumnBinder MaxInclusiveColumn = new SqlColumnBinder("MaxInclusive");
    protected SqlColumnBinder MinVersionColumn = new SqlColumnBinder("MinVersion");
    protected SqlColumnBinder MaxVersionColumn = new SqlColumnBinder("MaxVersion");
    protected SqlColumnBinder ProductArchitecture = new SqlColumnBinder(nameof (ProductArchitecture));

    protected override GalleryInstallationTarget2 Bind()
    {
      GalleryInstallationTarget2 installationTarget2 = new GalleryInstallationTarget2();
      installationTarget2.ReferenceId = this.ReferenceIdColumn.GetGuid((IDataReader) this.Reader);
      installationTarget2.Target = this.TargetColumn.GetString((IDataReader) this.Reader, false);
      installationTarget2.TargetVersion = this.VersionRangeColumn.GetString((IDataReader) this.Reader, true);
      installationTarget2.MinInclusive = this.MinInclusiveColumn.GetBoolean((IDataReader) this.Reader, false);
      installationTarget2.MaxInclusive = this.MaxInclusiveColumn.GetBoolean((IDataReader) this.Reader, false);
      installationTarget2.MaxVersion = Version.Parse(this.MaxVersionColumn.GetString((IDataReader) this.Reader, false));
      installationTarget2.MinVersion = Version.Parse(this.MinVersionColumn.GetString((IDataReader) this.Reader, false));
      installationTarget2.ProductArchitecture = this.ProductArchitecture.GetString((IDataReader) this.Reader, true);
      return installationTarget2;
    }
  }
}
