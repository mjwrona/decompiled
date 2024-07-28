// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Search.ISearchTokenHelper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Search
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [CLSCompliant(false)]
  public interface ISearchTokenHelper
  {
    object[] GetTokens();

    bool IsFilterToken(object token);

    string GetOriginalTokenText(object token);

    string GetParsedTokenText(object token);

    uint GetTokenStartPosition(object token);

    uint GetParseError(object token);

    string GetFilterField(object token);

    string GetFilterValue(object token);

    uint GetFilterSeperatorPosition(object token);

    uint GetFilterTokenType(object token);
  }
}
