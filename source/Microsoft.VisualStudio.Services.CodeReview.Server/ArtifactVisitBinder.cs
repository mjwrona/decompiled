// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.ArtifactVisitBinder
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  internal class ArtifactVisitBinder : ObjectBinder<ArtifactVisit>
  {
    private SqlColumnBinder ArtifactIdColumn = new SqlColumnBinder("ArtifactId");
    private SqlColumnBinder LastVisitDateColumn = new SqlColumnBinder("LastVisitedDate");
    private SqlColumnBinder PreviousLastVisitDateColumn = new SqlColumnBinder("PreviousLastVisitedDate");
    private SqlColumnBinder ViewedStateColumn = new SqlColumnBinder("ViewedState");

    protected override ArtifactVisit Bind()
    {
      DateTime? nullable1 = new DateTime?(this.LastVisitDateColumn.GetDateTime((IDataReader) this.Reader));
      DateTime? nullable2 = nullable1;
      DateTime minValue = DateTime.MinValue;
      if ((nullable2.HasValue ? (nullable2.HasValue ? (nullable2.GetValueOrDefault() == minValue ? 1 : 0) : 1) : 0) != 0)
        nullable1 = new DateTime?();
      DateTime? nullable3 = new DateTime?();
      if (this.PreviousLastVisitDateColumn.ColumnExists((IDataReader) this.Reader) && this.PreviousLastVisitDateColumn.GetDateTime((IDataReader) this.Reader) != DateTime.MinValue)
        nullable3 = new DateTime?(this.PreviousLastVisitDateColumn.GetDateTime((IDataReader) this.Reader));
      string str = this.ViewedStateColumn.GetString((IDataReader) this.Reader, true, (string) null);
      return new ArtifactVisit()
      {
        ArtifactId = this.ArtifactIdColumn.GetString((IDataReader) this.Reader, false),
        LastVisitedDate = nullable1,
        PreviousLastVisitedDate = nullable3,
        ViewedState = str
      };
    }
  }
}
