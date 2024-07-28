// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmInclude
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm
{
  public class EdmInclude : IEdmInclude
  {
    private readonly string alias;
    private readonly string namespaceIncluded;

    public EdmInclude(string alias, string namespaceIncluded)
    {
      this.alias = alias;
      this.namespaceIncluded = namespaceIncluded;
    }

    public string Alias => this.alias;

    public string Namespace => this.namespaceIncluded;
  }
}
