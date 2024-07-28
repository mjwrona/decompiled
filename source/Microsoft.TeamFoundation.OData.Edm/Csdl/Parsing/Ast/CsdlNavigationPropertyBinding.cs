// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Csdl.Parsing.Ast.CsdlNavigationPropertyBinding
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm.Csdl.Parsing.Ast
{
  internal class CsdlNavigationPropertyBinding : CsdlElement
  {
    private readonly string path;
    private readonly string target;

    public CsdlNavigationPropertyBinding(string path, string target, CsdlLocation location)
      : base(location)
    {
      this.path = path;
      this.target = target;
    }

    public string Path => this.path;

    public string Target => this.target;
  }
}
