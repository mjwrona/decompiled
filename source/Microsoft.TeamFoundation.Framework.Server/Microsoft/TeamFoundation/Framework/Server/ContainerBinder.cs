// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ContainerBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.FileContainer;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ContainerBinder : ObjectBinder<Microsoft.VisualStudio.Services.FileContainer.FileContainer>
  {
    private SqlColumnBinder ContainerIdColumn = new SqlColumnBinder("ContainerId");
    private SqlColumnBinder ArtifactUriColumn = new SqlColumnBinder("ArtifactUri");
    private SqlColumnBinder SecurityTokenColumn = new SqlColumnBinder("SecurityToken");
    private SqlColumnBinder NameColumn = new SqlColumnBinder("Name");
    private SqlColumnBinder DescriptionColumn = new SqlColumnBinder("Description");
    private SqlColumnBinder SizeColumn = new SqlColumnBinder("Size");
    private SqlColumnBinder OptionsColumn = new SqlColumnBinder("Options");
    private SqlColumnBinder SigningKeyIdColumn = new SqlColumnBinder("SigningKeyId");
    private SqlColumnBinder CreatedByColumn = new SqlColumnBinder("CreatedBy");
    private SqlColumnBinder DateCreatedColumn = new SqlColumnBinder("DateCreated");

    protected override Microsoft.VisualStudio.Services.FileContainer.FileContainer Bind() => new Microsoft.VisualStudio.Services.FileContainer.FileContainer()
    {
      Id = this.ContainerIdColumn.GetInt64((IDataReader) this.Reader),
      ArtifactUri = new Uri(this.ArtifactUriColumn.GetString((IDataReader) this.Reader, false)),
      SecurityToken = this.SecurityTokenColumn.GetString((IDataReader) this.Reader, false),
      Name = this.NameColumn.GetString((IDataReader) this.Reader, false),
      Description = this.DescriptionColumn.GetString((IDataReader) this.Reader, true),
      Size = this.SizeColumn.GetInt64((IDataReader) this.Reader, -1L),
      Options = (ContainerOptions) this.OptionsColumn.GetByte((IDataReader) this.Reader),
      SigningKeyId = this.SigningKeyIdColumn.GetGuid((IDataReader) this.Reader, true),
      CreatedBy = this.CreatedByColumn.GetGuid((IDataReader) this.Reader, false),
      DateCreated = this.DateCreatedColumn.GetDateTime((IDataReader) this.Reader)
    };
  }
}
