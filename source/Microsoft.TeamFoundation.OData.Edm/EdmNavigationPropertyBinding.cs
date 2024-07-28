// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmNavigationPropertyBinding
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm
{
  public class EdmNavigationPropertyBinding : IEdmNavigationPropertyBinding
  {
    private IEdmNavigationProperty navigationProperty;
    private IEdmNavigationSource target;
    private IEdmPathExpression path;

    public EdmNavigationPropertyBinding(
      IEdmNavigationProperty navigationProperty,
      IEdmNavigationSource target)
    {
      this.navigationProperty = navigationProperty;
      this.target = target;
      this.path = (IEdmPathExpression) new EdmPathExpression(navigationProperty == null ? string.Empty : navigationProperty.Name);
    }

    public EdmNavigationPropertyBinding(
      IEdmNavigationProperty navigationProperty,
      IEdmNavigationSource target,
      IEdmPathExpression bindingPath)
    {
      this.navigationProperty = navigationProperty;
      this.target = target;
      this.path = bindingPath;
    }

    public IEdmNavigationProperty NavigationProperty => this.navigationProperty;

    public IEdmNavigationSource Target => this.target;

    public IEdmPathExpression Path => this.path;
  }
}
