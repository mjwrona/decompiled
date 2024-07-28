// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Arriba.Model.Token
// Assembly: Microsoft.VisualStudio.Services.Search.Common.Arriba, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 29FBF982-8D5A-44EA-8073-2D46D60ABF28
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.Arriba.dll

namespace Microsoft.VisualStudio.Services.Search.Common.Arriba.Model
{
  internal class Token
  {
    public Token()
    {
      this.Prefix = string.Empty;
      this.Content = string.Empty;
    }

    public TokenType Type { get; set; }

    public string Prefix { get; set; }

    public string Content { get; set; }

    public override string ToString() => this.Prefix + this.Content;
  }
}
