// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.ReleaseDefinitionRevisionBinder1
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.WebApi;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class ReleaseDefinitionRevisionBinder1 : ReleaseDefinitionRevisionBinder
  {
    private SqlColumnBinder id = new SqlColumnBinder("DefinitionId");
    private SqlColumnBinder revision = new SqlColumnBinder("DefinitionRevision");
    private SqlColumnBinder changedBy = new SqlColumnBinder("ChangedBy");
    private SqlColumnBinder changedDate = new SqlColumnBinder("ChangedDate");
    private SqlColumnBinder changeType = new SqlColumnBinder("ChangeType");
    private SqlColumnBinder fileId = new SqlColumnBinder("FileId");
    private SqlColumnBinder comment = new SqlColumnBinder("Comment");
    private SqlColumnBinder apiversion = new SqlColumnBinder("ApiVersion");

    protected override ReleaseDefinitionRevision Bind() => new ReleaseDefinitionRevision()
    {
      DefinitionId = this.id.GetInt32((IDataReader) this.Reader),
      Revision = this.revision.GetInt32((IDataReader) this.Reader),
      ChangedBy = new IdentityRef()
      {
        Id = this.changedBy.GetGuid((IDataReader) this.Reader, false).ToString()
      },
      ChangedDate = this.changedDate.GetDateTime((IDataReader) this.Reader),
      ChangeType = (AuditAction) this.changeType.GetInt32((IDataReader) this.Reader, 2, 2),
      FileId = this.fileId.GetInt32((IDataReader) this.Reader),
      Comment = this.comment.GetString((IDataReader) this.Reader, true),
      ApiVersion = this.apiversion.GetString((IDataReader) this.Reader, true)
    };
  }
}
