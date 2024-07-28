// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.CssNodes.Aux
// Assembly: Microsoft.Azure.Boards.CssNodes, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D887A041-2C68-42E5-BA83-E261159AB40A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.CssNodes.dll

using System;

namespace Microsoft.Azure.Boards.CssNodes
{
  public static class Aux
  {
    public static string NormalizeString(string s, bool allowEmpty)
    {
      if (allowEmpty)
        return s != null ? s.Trim() : string.Empty;
      s = s != null ? s.Trim() : throw new ArgumentNullException(nameof (s));
      return s.Length != 0 ? s : throw new ArgumentNullException(nameof (s));
    }
  }
}
