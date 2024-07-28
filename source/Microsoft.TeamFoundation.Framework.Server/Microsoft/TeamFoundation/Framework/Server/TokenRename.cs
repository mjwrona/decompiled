// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TokenRename
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class TokenRename
  {
    public string OldToken;
    public string NewToken;
    public bool Copy;
    public bool Recurse;

    public TokenRename()
    {
    }

    public TokenRename(string oldToken, string newToken, bool copy, bool recurse)
    {
      this.OldToken = oldToken;
      this.NewToken = newToken;
      this.Copy = copy;
      this.Recurse = recurse;
    }
  }
}
