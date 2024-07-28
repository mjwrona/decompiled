// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Vocabularies.EdmTypedDirectValueAnnotationBinding`1
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm.Vocabularies
{
  public class EdmTypedDirectValueAnnotationBinding<T> : 
    EdmNamedElement,
    IEdmDirectValueAnnotationBinding
  {
    private readonly IEdmElement element;
    private readonly T value;

    public EdmTypedDirectValueAnnotationBinding(IEdmElement element, T value)
      : base(ExtensionMethods.TypeName<T>.LocalName)
    {
      this.element = element;
      this.value = value;
    }

    public IEdmElement Element => this.element;

    public string NamespaceUri => "http://schemas.microsoft.com/ado/2011/04/edm/internal";

    public object Value => (object) this.value;
  }
}
