// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.StageExportBinder
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Analytics
{
  internal class StageExportBinder : ObjectBinder<StageExportResult>
  {
    private SqlColumnBinder DocumentColumn = new SqlColumnBinder("Document");

    protected override StageExportResult Bind() => new StageExportResult()
    {
      Document = this.DocumentColumn.GetString((IDataReader) this.Reader, true)
    };
  }
}
