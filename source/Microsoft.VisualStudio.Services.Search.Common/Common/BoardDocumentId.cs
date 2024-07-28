// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.BoardDocumentId
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.Common.Enums;
using System;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public class BoardDocumentId
  {
    private readonly string m_documentId;

    public BoardDocumentId(Guid teamId, Guid projectId, BoardType boardType) => this.m_documentId = BoardDocumentId.GetDocumentId(teamId, projectId, boardType);

    public override int GetHashCode() => this.m_documentId.GetHashCode();

    public override bool Equals(object obj) => obj is BoardDocumentId boardDocumentId && this.m_documentId == boardDocumentId.m_documentId;

    public override string ToString() => this.m_documentId;

    private static string GetDocumentId(Guid teamId, Guid projectId, BoardType boardType) => teamId.ToString() + "@" + (object) projectId + "@" + boardType.ToString();
  }
}
