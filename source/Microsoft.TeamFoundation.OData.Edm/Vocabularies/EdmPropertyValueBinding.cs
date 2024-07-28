// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Vocabularies.EdmPropertyValueBinding
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm.Vocabularies
{
  public class EdmPropertyValueBinding : EdmElement, IEdmPropertyValueBinding, IEdmElement
  {
    private readonly IEdmProperty boundProperty;
    private readonly IEdmExpression value;

    public EdmPropertyValueBinding(IEdmProperty boundProperty, IEdmExpression value)
    {
      EdmUtil.CheckArgumentNull<IEdmProperty>(boundProperty, nameof (boundProperty));
      EdmUtil.CheckArgumentNull<IEdmExpression>(value, nameof (value));
      this.boundProperty = boundProperty;
      this.value = value;
    }

    public IEdmProperty BoundProperty => this.boundProperty;

    public IEdmExpression Value => this.value;
  }
}
