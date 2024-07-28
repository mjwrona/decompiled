// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.AnnotationColumns
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class AnnotationColumns : VersionControlObjectBinder<Annotation>
  {
    private SqlColumnBinder identityId = new SqlColumnBinder("IdentityId");
    private SqlColumnBinder annotationName = new SqlColumnBinder("AnnotationName");
    private SqlColumnBinder annotationValue = new SqlColumnBinder("AnnotationValue");
    private SqlColumnBinder comment = new SqlColumnBinder("Comment");
    private SqlColumnBinder annotatedItem = new SqlColumnBinder("AnnotatedItem");
    private SqlColumnBinder version = new SqlColumnBinder("VersionFrom");
    private SqlColumnBinder lastModifiedDate = new SqlColumnBinder("ModifiedDate");

    protected override Annotation Bind() => new Annotation()
    {
      identityId = this.identityId.GetInt32((IDataReader) this.Reader),
      AnnotationName = this.annotationName.GetString((IDataReader) this.Reader, false),
      AnnotationValue = this.annotationValue.GetString((IDataReader) this.Reader, true),
      Comment = this.comment.GetString((IDataReader) this.Reader, true),
      AnnotatedItem = this.annotatedItem.GetServerItem(this.Reader, true),
      Version = this.version.GetInt32((IDataReader) this.Reader, 0),
      LastModifiedDate = this.lastModifiedDate.GetDateTime((IDataReader) this.Reader)
    };
  }
}
