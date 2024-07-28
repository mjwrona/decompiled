// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.ReleaseBinder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class ReleaseBinder : ReleaseManagementObjectBinderBase<Release>
  {
    private readonly IVssRequestContext requestContext;
    private SqlColumnBinder id = new SqlColumnBinder("Id");
    private SqlColumnBinder dataspaceId = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder name = new SqlColumnBinder("Name");
    private SqlColumnBinder status = new SqlColumnBinder("Status");
    private SqlColumnBinder reason = new SqlColumnBinder("Reason");
    private SqlColumnBinder definitionId = new SqlColumnBinder("DefinitionId");
    private SqlColumnBinder definitionName = new SqlColumnBinder("DefinitionName");
    private SqlColumnBinder definitionPath = new SqlColumnBinder(nameof (definitionPath));
    private SqlColumnBinder targetEnvironmentId = new SqlColumnBinder("TargetEnvironmentId");
    private SqlColumnBinder definitionJson = new SqlColumnBinder("DefinitionJson");
    private SqlColumnBinder variables = new SqlColumnBinder("Variables");
    private SqlColumnBinder createdOn = new SqlColumnBinder("CreatedOn");
    private SqlColumnBinder createdBy = new SqlColumnBinder("CreatedBy");
    private SqlColumnBinder modifiedOn = new SqlColumnBinder("ModifiedOn");
    private SqlColumnBinder modifiedBy = new SqlColumnBinder("ModifiedBy");
    private SqlColumnBinder isDeferrred = new SqlColumnBinder("IsDeferred");
    private SqlColumnBinder deferredDateTime = new SqlColumnBinder("DeferredDateTime");
    private SqlColumnBinder keepForever = new SqlColumnBinder("KeepForever");
    private SqlColumnBinder isDeleted = new SqlColumnBinder("IsDeleted");
    private SqlColumnBinder description = new SqlColumnBinder("Description");
    private SqlColumnBinder releaseNameFormat = new SqlColumnBinder("ReleaseNameFormat");
    private SqlColumnBinder definitionSnapshotRevision = new SqlColumnBinder("DefinitionSnapshotRevision");

    public ReleaseBinder(
      IVssRequestContext requestContext,
      ReleaseManagementSqlResourceComponentBase sqlComponent)
      : base(sqlComponent)
    {
      this.requestContext = requestContext;
    }

    protected override Release Bind()
    {
      DateTime dateTime = this.deferredDateTime.GetDateTime((IDataReader) this.Reader);
      Guid guid = this.dataspaceId.ColumnExists((IDataReader) this.Reader) ? this.SqlComponent.GetDataspaceIdentifier(this.dataspaceId.GetInt32((IDataReader) this.Reader)) : Guid.Empty;
      Release release = new Release()
      {
        Id = this.id.GetInt32((IDataReader) this.Reader),
        ProjectId = guid,
        Name = this.name.GetString((IDataReader) this.Reader, false),
        Description = this.description.GetString((IDataReader) this.Reader, false),
        ReleaseDefinitionId = this.definitionId.GetInt32((IDataReader) this.Reader),
        ReleaseDefinitionName = this.definitionName.GetString((IDataReader) this.Reader, false),
        ReleaseDefinitionPath = PathHelper.DBPathToServerPath(this.definitionPath.GetString((IDataReader) this.Reader, string.Empty)),
        TargetEnvironmentId = this.targetEnvironmentId.GetInt32((IDataReader) this.Reader),
        CreatedOn = this.createdOn.GetDateTime((IDataReader) this.Reader),
        CreatedBy = this.createdBy.GetGuid((IDataReader) this.Reader, false),
        ModifiedOn = this.modifiedOn.GetDateTime((IDataReader) this.Reader),
        ModifiedBy = this.modifiedBy.GetGuid((IDataReader) this.Reader, false),
        IsDeferred = this.isDeferrred.GetBoolean((IDataReader) this.Reader),
        DeferredDateTime = new DateTime?(dateTime),
        KeepForever = this.keepForever.ColumnExists((IDataReader) this.Reader) && this.keepForever.GetBoolean((IDataReader) this.Reader, false),
        IsDeleted = this.isDeleted.ColumnExists((IDataReader) this.Reader) && this.isDeleted.GetBoolean((IDataReader) this.Reader, false),
        Reason = (ReleaseReason) this.reason.GetByte((IDataReader) this.Reader, (byte) 0, (byte) 0),
        ReleaseNameFormat = this.releaseNameFormat.GetString((IDataReader) this.Reader, (string) null),
        DefinitionSnapshotRevision = this.definitionSnapshotRevision.GetInt32((IDataReader) this.Reader, 0, 0),
        Status = (ReleaseStatus) this.status.GetByte((IDataReader) this.Reader)
      };
      if (dateTime == DateTime.MinValue)
        release.DeferredDateTime = new DateTime?();
      VariablesUtility.FillVariables(ServerModelUtility.FromString<IDictionary<string, ConfigurationVariableValue>>(this.variables.GetString((IDataReader) this.Reader, false)), release.Variables);
      this.FillVariableGroups(release);
      release.DefinitionSnapshot = ReleaseDefinitionSnapshotUtility.GetReleaseDefinitionSnapshot(this.requestContext, release, this.definitionJson.GetString((IDataReader) this.Reader, true));
      return release;
    }

    protected virtual void FillVariableGroups(Release release)
    {
    }
  }
}
