// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.ReleaseReferenceBinder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Components;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "ReleaseReferenceBinder is the correct term")]
  public class ReleaseReferenceBinder : ReleaseManagementObjectBinderBase<ReleaseReference>
  {
    private SqlColumnBinder id = new SqlColumnBinder("Id");
    private SqlColumnBinder name = new SqlColumnBinder("Name");
    private SqlColumnBinder definitionId = new SqlColumnBinder("DefinitionId");
    private SqlColumnBinder description = new SqlColumnBinder("Description");
    private SqlColumnBinder createdOn = new SqlColumnBinder("CreatedOn");
    private SqlColumnBinder createdBy = new SqlColumnBinder("CreatedBy");

    public ReleaseReferenceBinder(
      ReleaseManagementSqlResourceComponentBase sqlComponent)
      : base(sqlComponent)
    {
    }

    protected override ReleaseReference Bind() => new ReleaseReference()
    {
      Id = this.id.GetInt32((IDataReader) this.Reader),
      Name = this.name.GetString((IDataReader) this.Reader, false),
      Description = this.description.GetString((IDataReader) this.Reader, false),
      ReleaseDefinitionId = this.definitionId.GetInt32((IDataReader) this.Reader),
      CreatedOn = this.createdOn.GetDateTime((IDataReader) this.Reader),
      CreatedBy = this.createdBy.GetGuid((IDataReader) this.Reader)
    };
  }
}
