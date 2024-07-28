// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.GalleryInstallationTargetBinder
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class GalleryInstallationTargetBinder : ObjectBinder<GalleryInstallationTarget>
  {
    protected SqlColumnBinder referenceIdColumn = new SqlColumnBinder("ReferenceId");
    protected SqlColumnBinder TargetColumn = new SqlColumnBinder("Target");
    protected SqlColumnBinder versionRangeColumn = new SqlColumnBinder("VersionRange");
    protected SqlColumnBinder MinInclusiveColumn = new SqlColumnBinder("MinInclusive");
    protected SqlColumnBinder MaxInclusiveColumn = new SqlColumnBinder("MaxInclusive");
    protected SqlColumnBinder MinVersionColumn = new SqlColumnBinder("MinVersion");
    protected SqlColumnBinder MaxVersionColumn = new SqlColumnBinder("MaxVersion");

    protected override GalleryInstallationTarget Bind() => new GalleryInstallationTarget()
    {
      ReferenceId = this.referenceIdColumn.GetGuid((IDataReader) this.Reader),
      Target = this.TargetColumn.GetString((IDataReader) this.Reader, false),
      TargetVersion = this.versionRangeColumn.GetString((IDataReader) this.Reader, true),
      MinInclusive = this.MinInclusiveColumn.GetBoolean((IDataReader) this.Reader, false),
      MaxInclusive = this.MaxInclusiveColumn.GetBoolean((IDataReader) this.Reader, false),
      MaxVersion = Version.Parse(this.MaxVersionColumn.GetString((IDataReader) this.Reader, false)),
      MinVersion = Version.Parse(this.MinVersionColumn.GetString((IDataReader) this.Reader, false))
    };
  }
}
