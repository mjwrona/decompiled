// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.NamedValue
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

namespace Microsoft.OData.UriParser
{
  public sealed class NamedValue
  {
    private readonly string name;
    private readonly LiteralToken value;

    public NamedValue(string name, LiteralToken value)
    {
      ExceptionUtils.CheckArgumentNotNull<LiteralToken>(value, nameof (value));
      this.name = name;
      this.value = value;
    }

    public string Name => this.name;

    public LiteralToken Value => this.value;
  }
}
