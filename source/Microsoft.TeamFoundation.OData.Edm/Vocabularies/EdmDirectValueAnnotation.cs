// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Vocabularies.EdmDirectValueAnnotation
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm.Vocabularies
{
  public class EdmDirectValueAnnotation : 
    EdmNamedElement,
    IEdmDirectValueAnnotation,
    IEdmNamedElement,
    IEdmElement
  {
    private readonly object value;
    private readonly string namespaceUri;

    public EdmDirectValueAnnotation(string namespaceUri, string name, object value)
      : this(namespaceUri, name)
    {
      EdmUtil.CheckArgumentNull<object>(value, nameof (value));
      this.value = value;
    }

    internal EdmDirectValueAnnotation(string namespaceUri, string name)
      : base(name)
    {
      EdmUtil.CheckArgumentNull<string>(namespaceUri, nameof (namespaceUri));
      this.namespaceUri = namespaceUri;
    }

    public string NamespaceUri => this.namespaceUri;

    public object Value => this.value;
  }
}
