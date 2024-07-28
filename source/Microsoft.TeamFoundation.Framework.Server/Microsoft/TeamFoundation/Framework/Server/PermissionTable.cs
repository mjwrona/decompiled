// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.PermissionTable
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class PermissionTable
  {
    public static string AddSeparator(char separator, string token)
    {
      if (separator != char.MinValue && (token.Length == 0 || (int) token[token.Length - 1] != (int) separator))
        token += (string) (object) separator;
      return token;
    }

    public static string GetIndexableTokenFromToken(string token, char separator)
    {
      if (token.Length > 350)
        token = PermissionTable.AddSeparator(separator, token.Substring(0, 350));
      return token;
    }
  }
}
