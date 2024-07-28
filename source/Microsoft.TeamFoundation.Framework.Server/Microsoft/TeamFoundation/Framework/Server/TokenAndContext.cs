// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TokenAndContext
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal struct TokenAndContext
  {
    public readonly string Token;
    public readonly object Context;

    public TokenAndContext(string token, object context = null)
    {
      this.Token = token;
      this.Context = context;
    }

    public static implicit operator TokenAndContext(string token) => new TokenAndContext(token);
  }
}
