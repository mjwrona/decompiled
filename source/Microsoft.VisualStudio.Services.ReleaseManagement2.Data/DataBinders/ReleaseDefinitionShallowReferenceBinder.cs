// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.ReleaseDefinitionShallowReferenceBinder
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
  public class ReleaseDefinitionShallowReferenceBinder : 
    ReleaseManagementObjectBinderBase<ReleaseDefinitionShallowReference>
  {
    private SqlColumnBinder id = new SqlColumnBinder("Id");
    private SqlColumnBinder name = new SqlColumnBinder("Name");
    private SqlColumnBinder pathColumn = new SqlColumnBinder("Path");
    private SqlColumnBinder dataspaceId = new SqlColumnBinder("DataspaceId");

    public ReleaseDefinitionShallowReferenceBinder(
      ReleaseManagementSqlResourceComponentBase sqlComponent)
      : base(sqlComponent)
    {
    }

    protected override ReleaseDefinitionShallowReference Bind()
    {
      Guid guid = this.dataspaceId.ColumnExists((IDataReader) this.Reader) ? this.SqlComponent.GetDataspaceIdentifier(this.dataspaceId.GetInt32((IDataReader) this.Reader)) : Guid.Empty;
      string path = this.pathColumn.ColumnExists((IDataReader) this.Reader) ? this.pathColumn.GetString((IDataReader) this.Reader, false) : (string) null;
      ReleaseDefinitionShallowReference shallowReference = new ReleaseDefinitionShallowReference();
      shallowReference.Id = this.id.GetInt32((IDataReader) this.Reader);
      shallowReference.Name = this.name.GetString((IDataReader) this.Reader, false);
      shallowReference.Path = PathHelper.DBPathToServerPath(path);
      shallowReference.ProjectId = guid;
      return shallowReference;
    }
  }
}
