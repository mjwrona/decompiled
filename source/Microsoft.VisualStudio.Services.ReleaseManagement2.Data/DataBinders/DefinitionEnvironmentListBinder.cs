// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.DefinitionEnvironmentListBinder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class DefinitionEnvironmentListBinder : 
    ReleaseManagementObjectBinderBase<DefinitionEnvironment>
  {
    private SqlColumnBinder id = new SqlColumnBinder("Id");
    private SqlColumnBinder dataspaceId = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder name = new SqlColumnBinder("Name");
    private SqlColumnBinder releaseDefinitionId = new SqlColumnBinder("ReleaseDefinitionId");
    private SqlColumnBinder rank = new SqlColumnBinder("Rank");
    private SqlColumnBinder retentionPolicy = new SqlColumnBinder("RetentionPolicy");
    private SqlColumnBinder retainBuild = new SqlColumnBinder("RetainBuild");
    private SqlColumnBinder ownerId = new SqlColumnBinder("OwnerId");

    public DefinitionEnvironmentListBinder(
      ReleaseManagementSqlResourceComponentBase sqlComponent)
      : base(sqlComponent)
    {
    }

    protected override DefinitionEnvironment Bind()
    {
      Guid guid = this.dataspaceId.ColumnExists((IDataReader) this.Reader) ? this.SqlComponent.GetDataspaceIdentifier(this.dataspaceId.GetInt32((IDataReader) this.Reader)) : Guid.Empty;
      DefinitionEnvironment definitionEnvironment = new DefinitionEnvironment()
      {
        Id = this.id.GetInt32((IDataReader) this.Reader),
        ProjectId = guid,
        Name = this.name.GetString((IDataReader) this.Reader, false),
        ReleaseDefinitionId = this.releaseDefinitionId.GetInt32((IDataReader) this.Reader),
        Rank = this.rank.GetInt32((IDataReader) this.Reader),
        OwnerId = this.ownerId.GetGuid((IDataReader) this.Reader, true, Guid.Empty)
      };
      string str = this.retentionPolicy.GetString((IDataReader) this.Reader, (string) null);
      definitionEnvironment.RetentionPolicy = string.IsNullOrWhiteSpace(str) ? (EnvironmentRetentionPolicy) null : JsonConvert.DeserializeObject<EnvironmentRetentionPolicy>(str);
      if (definitionEnvironment.RetentionPolicy != null)
        definitionEnvironment.RetentionPolicy.RetainBuild = this.retainBuild.ColumnExists((IDataReader) this.Reader) && this.retainBuild.GetBoolean((IDataReader) this.Reader, true);
      return definitionEnvironment;
    }
  }
}
