// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.NamespaceQualifiedWildcardSelectItem
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

namespace Microsoft.OData.UriParser
{
  public sealed class NamespaceQualifiedWildcardSelectItem : SelectItem
  {
    public NamespaceQualifiedWildcardSelectItem(string namespaceName)
    {
      ExceptionUtils.CheckArgumentNotNull<string>(namespaceName, nameof (namespaceName));
      this.Namespace = namespaceName;
    }

    public string Namespace { get; private set; }

    public override T TranslateWith<T>(SelectItemTranslator<T> translator) => translator.Translate(this);

    public override void HandleWith(SelectItemHandler handler) => handler.Handle(this);
  }
}
