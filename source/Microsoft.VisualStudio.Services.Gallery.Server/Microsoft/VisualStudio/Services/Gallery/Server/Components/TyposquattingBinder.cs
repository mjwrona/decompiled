// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Components.TyposquattingBinder
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Components
{
  internal class TyposquattingBinder : ObjectBinder<TyposquattingRow>
  {
    protected SqlColumnBinder keywordType = new SqlColumnBinder("KeywordType");
    protected SqlColumnBinder keyWordValue = new SqlColumnBinder("KeywordValue");

    protected override TyposquattingRow Bind() => new TyposquattingRow()
    {
      KeywordType = this.keywordType.GetInt32((IDataReader) this.Reader),
      KeywordValue = this.keyWordValue.GetString((IDataReader) this.Reader, true)
    };
  }
}
