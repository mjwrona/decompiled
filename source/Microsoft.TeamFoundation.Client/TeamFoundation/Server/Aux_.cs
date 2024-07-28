// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Aux
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;

namespace Microsoft.TeamFoundation.Server
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
