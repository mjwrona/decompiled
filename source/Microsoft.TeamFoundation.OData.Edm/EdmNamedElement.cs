// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.EdmNamedElement
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System.Diagnostics;

namespace Microsoft.OData.Edm
{
  [DebuggerDisplay("Name:{Name}")]
  public abstract class EdmNamedElement : EdmElement, IEdmNamedElement, IEdmElement
  {
    private readonly string name;

    protected EdmNamedElement(string name)
    {
      EdmUtil.CheckArgumentNull<string>(name, nameof (name));
      this.name = name;
    }

    public string Name => this.name;
  }
}
