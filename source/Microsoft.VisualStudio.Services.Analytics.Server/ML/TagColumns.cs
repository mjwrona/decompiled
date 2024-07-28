// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.ML.TagColumns
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.Model;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.Analytics.ML
{
  internal class TagColumns : ObjectBinder<Tag>
  {
    private SqlColumnBinder PartitionIdColumn = new SqlColumnBinder("PartitionId");
    private SqlColumnBinder TagSKColumn = new SqlColumnBinder("TagSK");
    private SqlColumnBinder TagNameColumn = new SqlColumnBinder("TagName");
    private SqlColumnBinder IsDeletedColumn = new SqlColumnBinder("IsDeleted");
    private SqlColumnBinder ProjectSKColumn = new SqlColumnBinder("ProjectSK");

    protected override Tag Bind() => new Tag()
    {
      PartitionId = this.PartitionIdColumn.GetInt32((IDataReader) this.Reader),
      TagSK = this.TagSKColumn.GetInt32((IDataReader) this.Reader),
      TagName = this.TagNameColumn.GetString((IDataReader) this.Reader, false),
      IsDeleted = this.IsDeletedColumn.GetBoolean((IDataReader) this.Reader),
      ProjectSK = new Guid?(this.ProjectSKColumn.GetGuid((IDataReader) this.Reader))
    };
  }
}
