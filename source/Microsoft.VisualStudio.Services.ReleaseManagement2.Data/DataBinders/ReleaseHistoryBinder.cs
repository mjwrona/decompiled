// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders.ReleaseHistoryBinder
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.WebApi;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataBinders
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "Binder is the right suffix as per VSSF convention.")]
  public class ReleaseHistoryBinder : ObjectBinder<ReleaseRevision>
  {
    private SqlColumnBinder id = new SqlColumnBinder("ReleaseId");
    private SqlColumnBinder definisionSnapshotRevision = new SqlColumnBinder("DefinitionSnapshotRevision");
    private SqlColumnBinder changedBy = new SqlColumnBinder("ChangedBy");
    private SqlColumnBinder changedDate = new SqlColumnBinder("ChangedDate");
    private SqlColumnBinder changeType = new SqlColumnBinder("ChangeType");
    private SqlColumnBinder changeDetails = new SqlColumnBinder("ChangeDetails");
    private SqlColumnBinder comment = new SqlColumnBinder("Comment");
    private SqlColumnBinder fileId = new SqlColumnBinder("FileId");

    protected override ReleaseRevision Bind()
    {
      ReleaseHistoryChangeTypes historyChangeTypes = (ReleaseHistoryChangeTypes) this.changeType.GetByte((IDataReader) this.Reader);
      string inputMessage = this.changeDetails.GetString((IDataReader) this.Reader, true);
      ReleaseRevisionChangeDetails revisionChangeDetails = ReleaseHistoryChangeDetailsTranslator.Translate(inputMessage);
      string str = ReleaseHistoryCommentGenerator.GetComment(inputMessage) ?? this.comment.GetString((IDataReader) this.Reader, true);
      return new ReleaseRevision()
      {
        ReleaseId = this.id.GetInt32((IDataReader) this.Reader),
        DefinitionSnapshotRevision = this.definisionSnapshotRevision.GetInt32((IDataReader) this.Reader),
        ChangedBy = new IdentityRef()
        {
          Id = this.changedBy.GetGuid((IDataReader) this.Reader, false).ToString()
        },
        ChangedDate = this.changedDate.GetDateTime((IDataReader) this.Reader),
        ChangeType = historyChangeTypes,
        ChangeDetails = revisionChangeDetails,
        Comment = str,
        FileId = this.fileId.GetInt32((IDataReader) this.Reader)
      };
    }
  }
}
