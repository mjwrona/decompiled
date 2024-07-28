// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.DataAccess.OccurrenceDataBinder
// Assembly: Microsoft.Azure.Pipelines.Deployment.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6B08F1AB-5B33-41F6-908E-9A985FD0544C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.Server.dll

using Microsoft.Azure.Pipelines.Deployment.Model;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.Azure.Pipelines.Deployment.DataAccess
{
  internal class OccurrenceDataBinder : ObjectBinder<OccurrenceData>
  {
    private SqlColumnBinder ScopeId = new SqlColumnBinder(nameof (ScopeId));
    private SqlColumnBinder Name = new SqlColumnBinder(nameof (Name));
    private SqlColumnBinder NoteName = new SqlColumnBinder(nameof (NoteName));
    private SqlColumnBinder Kind = new SqlColumnBinder(nameof (Kind));
    private SqlColumnBinder FileId = new SqlColumnBinder(nameof (FileId));
    private SqlColumnBinder ResourceId = new SqlColumnBinder(nameof (ResourceId));

    protected override OccurrenceData Bind() => new OccurrenceData()
    {
      ScopeId = this.ScopeId.GetGuid((IDataReader) this.Reader),
      NoteName = this.NoteName.GetString((IDataReader) this.Reader, false),
      Name = this.Name.GetString((IDataReader) this.Reader, false),
      Kind = (NoteKind) this.Kind.GetByte((IDataReader) this.Reader),
      FileId = this.FileId.GetNullableInt32((IDataReader) this.Reader),
      ResourceId = this.ResourceId.GetString((IDataReader) this.Reader, false)
    };
  }
}
