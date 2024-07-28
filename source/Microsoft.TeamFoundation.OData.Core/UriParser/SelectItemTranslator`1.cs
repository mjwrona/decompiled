// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.SelectItemTranslator`1
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;

namespace Microsoft.OData.UriParser
{
  public abstract class SelectItemTranslator<T>
  {
    public virtual T Translate(WildcardSelectItem item) => throw new NotImplementedException();

    public virtual T Translate(PathSelectItem item) => throw new NotImplementedException();

    public virtual T Translate(NamespaceQualifiedWildcardSelectItem item) => throw new NotImplementedException();

    public virtual T Translate(ExpandedNavigationSelectItem item) => throw new NotImplementedException();

    public virtual T Translate(ExpandedReferenceSelectItem item) => throw new NotImplementedException();
  }
}
