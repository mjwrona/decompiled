// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.FolderBinder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "As per Vssf")]
  public class FolderBinder : ObjectBinder<Folder>
  {
    private SqlColumnBinder path = new SqlColumnBinder("Path");
    private SqlColumnBinder description = new SqlColumnBinder("Description");
    private SqlColumnBinder createdBy = new SqlColumnBinder("CreatedBy");
    private SqlColumnBinder createdOn = new SqlColumnBinder("CreatedOn");
    private SqlColumnBinder changedBy = new SqlColumnBinder("ChangedBy");
    private SqlColumnBinder changedOn = new SqlColumnBinder("ChangedOn");

    protected override Folder Bind() => new Folder()
    {
      Path = PathHelper.DBPathToServerPath(this.path.GetString((IDataReader) this.Reader, false)),
      Description = this.description.GetString((IDataReader) this.Reader, true),
      CreatedBy = new IdentityRef()
      {
        Id = this.createdBy.GetGuid((IDataReader) this.Reader).ToString("D")
      },
      CreatedOn = this.createdOn.GetDateTime((IDataReader) this.Reader),
      LastChangedBy = new IdentityRef()
      {
        Id = this.changedBy.GetGuid((IDataReader) this.Reader, true).ToString("D")
      },
      LastChangedDate = new DateTime?(this.changedOn.GetDateTime((IDataReader) this.Reader))
    };
  }
}
