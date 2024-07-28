// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.ReleaseDefinitionBinder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class ReleaseDefinitionBinder : ReleaseManagementObjectBinderBase<ReleaseDefinition>
  {
    private SqlColumnBinder id = new SqlColumnBinder(nameof (Id));
    private SqlColumnBinder dataspaceId = new SqlColumnBinder(nameof (DataspaceId));
    private SqlColumnBinder name = new SqlColumnBinder(nameof (Name));
    private SqlColumnBinder variables = new SqlColumnBinder(nameof (Variables));
    private SqlColumnBinder releaseNameFormat = new SqlColumnBinder(nameof (ReleaseNameFormat));
    private SqlColumnBinder createdBy = new SqlColumnBinder(nameof (CreatedBy));
    private SqlColumnBinder createdOn = new SqlColumnBinder(nameof (CreatedOn));
    private SqlColumnBinder modifiedBy = new SqlColumnBinder(nameof (ModifiedBy));
    private SqlColumnBinder modifiedOn = new SqlColumnBinder(nameof (ModifiedOn));
    private SqlColumnBinder revision = new SqlColumnBinder(nameof (Revision));

    public ReleaseDefinitionBinder(
      ReleaseManagementSqlResourceComponentBase sqlComponent)
      : base(sqlComponent)
    {
    }

    protected override ReleaseDefinition Bind()
    {
      Guid guid = this.dataspaceId.ColumnExists((IDataReader) this.Reader) ? this.SqlComponent.GetDataspaceIdentifier(this.dataspaceId.GetInt32((IDataReader) this.Reader)) : Guid.Empty;
      ReleaseDefinition releaseDefinition = new ReleaseDefinition()
      {
        Id = this.id.GetInt32((IDataReader) this.Reader),
        ProjectId = guid,
        Name = this.name.GetString((IDataReader) this.Reader, false),
        ReleaseNameFormat = this.releaseNameFormat.GetString((IDataReader) this.Reader, string.Empty),
        CreatedBy = this.createdBy.GetGuid((IDataReader) this.Reader, false),
        CreatedOn = new DateTime?(this.createdOn.GetDateTime((IDataReader) this.Reader)),
        ModifiedBy = this.modifiedBy.GetGuid((IDataReader) this.Reader, false),
        ModifiedOn = new DateTime?(this.modifiedOn.GetDateTime((IDataReader) this.Reader)),
        Revision = this.revision.GetInt32((IDataReader) this.Reader, 1)
      };
      string str = this.variables.GetString((IDataReader) this.Reader, (string) null);
      if (str != null)
        VariablesUtility.FillVariables(ServerModelUtility.FromString<IDictionary<string, ConfigurationVariableValue>>(str), releaseDefinition.Variables);
      return releaseDefinition;
    }

    protected ref SqlColumnBinder Id => ref this.id;

    protected ref SqlColumnBinder DataspaceId => ref this.dataspaceId;

    protected ref SqlColumnBinder Name => ref this.name;

    protected ref SqlColumnBinder Variables => ref this.variables;

    protected ref SqlColumnBinder ReleaseNameFormat => ref this.releaseNameFormat;

    protected ref SqlColumnBinder CreatedBy => ref this.createdBy;

    protected ref SqlColumnBinder CreatedOn => ref this.createdOn;

    protected ref SqlColumnBinder ModifiedBy => ref this.modifiedBy;

    protected ref SqlColumnBinder ModifiedOn => ref this.modifiedOn;

    protected ref SqlColumnBinder Revision => ref this.revision;
  }
}
