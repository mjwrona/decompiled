// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.ReleaseListBinder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class ReleaseListBinder : ObjectBinder<Release>
  {
    private SqlColumnBinder id = new SqlColumnBinder("Id");
    private SqlColumnBinder name = new SqlColumnBinder("Name");
    private SqlColumnBinder status = new SqlColumnBinder("Status");
    private SqlColumnBinder createdOn = new SqlColumnBinder("CreatedOn");
    private SqlColumnBinder createdBy = new SqlColumnBinder("CreatedBy");
    private SqlColumnBinder modifiedOn = new SqlColumnBinder("ModifiedOn");

    protected override Release Bind() => new Release()
    {
      Id = this.id.GetInt32((IDataReader) this.Reader),
      Status = (ReleaseStatus) this.status.GetByte((IDataReader) this.Reader),
      Name = this.name.GetString((IDataReader) this.Reader, false),
      CreatedOn = this.createdOn.GetDateTime((IDataReader) this.Reader),
      CreatedBy = this.createdBy.GetGuid((IDataReader) this.Reader, false),
      ModifiedOn = this.modifiedOn.GetDateTime((IDataReader) this.Reader)
    };
  }
}
