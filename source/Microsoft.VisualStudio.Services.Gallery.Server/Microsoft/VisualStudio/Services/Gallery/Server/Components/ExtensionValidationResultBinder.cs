// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.ExtensionValidationResultBinder
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class ExtensionValidationResultBinder : ObjectBinder<ExtensionValidationResult>
  {
    protected SqlColumnBinder resultMessageColumn = new SqlColumnBinder("ResultMessage");
    protected SqlColumnBinder resultFileIdColumn = new SqlColumnBinder("ResultFileId");
    protected SqlColumnBinder lastUpdatedColumn = new SqlColumnBinder("LastUpdated");

    protected override ExtensionValidationResult Bind() => new ExtensionValidationResult()
    {
      LastUpdated = this.lastUpdatedColumn.GetDateTime((IDataReader) this.Reader),
      ResultMessage = this.resultMessageColumn.GetString((IDataReader) this.Reader, true),
      FileId = this.resultFileIdColumn.GetInt32((IDataReader) this.Reader, 0)
    };
  }
}
