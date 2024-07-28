// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Builder.NavigationPropertyBindingConfiguration
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using System.Collections.Generic;
using System.Reflection;

namespace Microsoft.AspNet.OData.Builder
{
  public class NavigationPropertyBindingConfiguration
  {
    public NavigationPropertyBindingConfiguration(
      NavigationPropertyConfiguration navigationProperty,
      NavigationSourceConfiguration navigationSource)
      : this(navigationProperty, navigationSource, (IList<MemberInfo>) new MemberInfo[1]
      {
        (MemberInfo) navigationProperty.PropertyInfo
      })
    {
    }

    public NavigationPropertyBindingConfiguration(
      NavigationPropertyConfiguration navigationProperty,
      NavigationSourceConfiguration navigationSource,
      IList<MemberInfo> path)
    {
      if (navigationProperty == null)
        throw Error.ArgumentNull(nameof (navigationProperty));
      if (navigationSource == null)
        throw Error.ArgumentNull(nameof (navigationSource));
      if (path == null)
        throw Error.ArgumentNull(nameof (path));
      this.NavigationProperty = navigationProperty;
      this.TargetNavigationSource = navigationSource;
      this.Path = path;
    }

    public NavigationPropertyConfiguration NavigationProperty { get; private set; }

    public NavigationSourceConfiguration TargetNavigationSource { get; private set; }

    public IList<MemberInfo> Path { get; private set; }

    public string BindingPath => this.Path.ConvertBindingPath();
  }
}
