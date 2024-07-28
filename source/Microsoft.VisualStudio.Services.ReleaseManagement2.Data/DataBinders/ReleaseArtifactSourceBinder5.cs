// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.ReleaseArtifactSourceBinder5
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "Cannot avoid this as this is a sql binder")]
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class ReleaseArtifactSourceBinder5 : ReleaseArtifactSourceBinder4
  {
    private SqlColumnBinder sourceId = new SqlColumnBinder(nameof (SourceId));
    private SqlColumnBinder dataspaceId = new SqlColumnBinder(nameof (DataspaceId));
    private SqlColumnBinder artifactVersionName = new SqlColumnBinder(nameof (ArtifactVersionName));
    private SqlColumnBinder artifactVersionId = new SqlColumnBinder(nameof (ArtifactVersionId));

    public ReleaseArtifactSourceBinder5(
      ReleaseManagementSqlResourceComponentBase sqlComponent)
      : base(sqlComponent)
    {
    }

    protected override PipelineArtifactSource Bind()
    {
      string artifactTypeId = this.ArtifactTypeId.GetString((IDataReader) this.Reader, false);
      Guid guid = this.dataspaceId.ColumnExists((IDataReader) this.Reader) ? this.SqlComponent.GetDataspaceIdentifier(this.dataspaceId.GetInt32((IDataReader) this.Reader)) : Guid.Empty;
      PipelineArtifactSource pipelineArtifactSource1 = new PipelineArtifactSource();
      pipelineArtifactSource1.Id = this.Id.GetInt32((IDataReader) this.Reader);
      pipelineArtifactSource1.ProjectId = guid;
      pipelineArtifactSource1.SourceId = this.sourceId.GetString((IDataReader) this.Reader, false);
      pipelineArtifactSource1.ReleaseId = this.ReleaseId.GetInt32((IDataReader) this.Reader);
      pipelineArtifactSource1.ArtifactTypeId = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.GetNormalizedArtifactTypeIdAfterDbRead(artifactTypeId);
      pipelineArtifactSource1.Alias = this.SourceAlias.GetString((IDataReader) this.Reader, false);
      pipelineArtifactSource1.SourceBranch = this.Branch.GetString((IDataReader) this.Reader, string.Empty);
      pipelineArtifactSource1.IsPrimary = this.IsPrimary.GetBoolean((IDataReader) this.Reader, false);
      pipelineArtifactSource1.ArtifactVersion = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.ToString((object) new InputValue()
      {
        Value = this.artifactVersionId.GetString((IDataReader) this.Reader, true),
        DisplayValue = this.artifactVersionName.GetString((IDataReader) this.Reader, false)
      });
      PipelineArtifactSource pipelineArtifactSource2 = pipelineArtifactSource1;
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.FillSourceData(this.SourceData.GetString((IDataReader) this.Reader, false), pipelineArtifactSource2.SourceData);
      return pipelineArtifactSource2;
    }

    protected ref SqlColumnBinder SourceId => ref this.sourceId;

    protected ref SqlColumnBinder DataspaceId => ref this.dataspaceId;

    protected ref SqlColumnBinder ArtifactVersionName => ref this.artifactVersionName;

    protected ref SqlColumnBinder ArtifactVersionId => ref this.artifactVersionId;
  }
}
