// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Vocabularies.EdmDirectValueAnnotationBinding
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm.Vocabularies
{
  public class EdmDirectValueAnnotationBinding : IEdmDirectValueAnnotationBinding
  {
    private readonly IEdmElement element;
    private readonly string namespaceUri;
    private readonly string name;
    private readonly object value;

    public EdmDirectValueAnnotationBinding(
      IEdmElement element,
      string namespaceUri,
      string name,
      object value)
    {
      EdmUtil.CheckArgumentNull<IEdmElement>(element, nameof (element));
      EdmUtil.CheckArgumentNull<string>(namespaceUri, nameof (namespaceUri));
      EdmUtil.CheckArgumentNull<string>(name, nameof (name));
      this.element = element;
      this.namespaceUri = namespaceUri;
      this.name = name;
      this.value = value;
    }

    public EdmDirectValueAnnotationBinding(IEdmElement element, string namespaceUri, string name)
    {
      EdmUtil.CheckArgumentNull<IEdmElement>(element, nameof (element));
      EdmUtil.CheckArgumentNull<string>(namespaceUri, nameof (namespaceUri));
      EdmUtil.CheckArgumentNull<string>(name, nameof (name));
      this.element = element;
      this.namespaceUri = namespaceUri;
      this.name = name;
    }

    public IEdmElement Element => this.element;

    public string NamespaceUri => this.namespaceUri;

    public string Name => this.name;

    public object Value => this.value;
  }
}
