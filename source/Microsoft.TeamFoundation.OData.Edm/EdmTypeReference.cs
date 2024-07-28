// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmTypeReference
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

namespace Microsoft.OData.Edm
{
  public abstract class EdmTypeReference : EdmElement, IEdmTypeReference, IEdmElement
  {
    private readonly IEdmType definition;
    private readonly bool isNullable;

    protected EdmTypeReference(IEdmType definition, bool isNullable)
    {
      EdmUtil.CheckArgumentNull<IEdmType>(definition, nameof (definition));
      this.definition = definition;
      this.isNullable = isNullable;
    }

    public bool IsNullable => this.isNullable;

    public IEdmType Definition => this.definition;

    public override string ToString() => this.ToTraceString();
  }
}
