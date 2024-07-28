// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.Vocabularies.EdmPropertyValue
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using System;

namespace Microsoft.OData.Edm.Vocabularies
{
  public class EdmPropertyValue : IEdmPropertyValue, IEdmDelayedValue
  {
    private readonly string name;
    private IEdmValue value;

    public EdmPropertyValue(string name)
    {
      EdmUtil.CheckArgumentNull<string>(name, nameof (name));
      this.name = name;
    }

    public EdmPropertyValue(string name, IEdmValue value)
    {
      EdmUtil.CheckArgumentNull<string>(name, nameof (name));
      EdmUtil.CheckArgumentNull<IEdmValue>(value, nameof (value));
      this.name = name;
      this.value = value;
    }

    public string Name => this.name;

    public IEdmValue Value
    {
      get => this.value;
      set
      {
        EdmUtil.CheckArgumentNull<IEdmValue>(value, nameof (value));
        this.value = this.value == null ? value : throw new InvalidOperationException(Strings.ValueHasAlreadyBeenSet);
      }
    }
  }
}
