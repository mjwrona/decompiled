// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.ReleaseDefinitionArtifactSourceBinder3
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class ReleaseDefinitionArtifactSourceBinder3 : ReleaseDefinitionArtifactSourceBinder2
  {
    private SqlColumnBinder isPrimary = new SqlColumnBinder("IsPrimary");
    private SqlColumnBinder dataspaceId = new SqlColumnBinder("DataspaceId");

    public ReleaseDefinitionArtifactSourceBinder3(
      ReleaseManagementSqlResourceComponentBase sqlComponent)
      : base(sqlComponent)
    {
    }

    protected override ReleaseDefinitionArtifactSourceMap Bind()
    {
      string artifactTypeId = this.ArtifactTypeId.GetString((IDataReader) this.Reader, false);
      Guid guid = this.dataspaceId.ColumnExists((IDataReader) this.Reader) ? this.SqlComponent.GetDataspaceIdentifier(this.dataspaceId.GetInt32((IDataReader) this.Reader)) : Guid.Empty;
      ReleaseDefinitionArtifactSourceMap artifactSourceMap = new ReleaseDefinitionArtifactSourceMap()
      {
        ReleaseDefinitionId = this.ReleaseDefinitionId.GetInt32((IDataReader) this.Reader),
        ProjectId = guid,
        SourceId = this.SourceId.GetString((IDataReader) this.Reader, false),
        Alias = this.Alias.GetString((IDataReader) this.Reader, false),
        ArtifactTypeId = ServerModelUtility.GetNormalizedArtifactTypeIdAfterDbRead(artifactTypeId),
        IsPrimary = this.isPrimary.GetBoolean((IDataReader) this.Reader)
      };
      ServerModelUtility.FillSourceData(this.SourceData.GetString((IDataReader) this.Reader, false), artifactSourceMap.SourceData);
      return artifactSourceMap;
    }
  }
}
