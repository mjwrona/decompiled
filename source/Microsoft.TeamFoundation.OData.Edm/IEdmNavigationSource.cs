// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.IEdmNavigationSource
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
  public interface IEdmNavigationSource : IEdmNamedElement, IEdmElement
  {
    IEnumerable<IEdmNavigationPropertyBinding> NavigationPropertyBindings { get; }

    IEdmPathExpression Path { get; }

    IEdmType Type { get; }

    IEdmNavigationSource FindNavigationTarget(IEdmNavigationProperty navigationProperty);

    IEdmNavigationSource FindNavigationTarget(
      IEdmNavigationProperty navigationProperty,
      IEdmPathExpression bindingPath);

    IEnumerable<IEdmNavigationPropertyBinding> FindNavigationPropertyBindings(
      IEdmNavigationProperty navigationProperty);
  }
}
