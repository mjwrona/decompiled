// Decompiled with JetBrains decompiler
// Type: Nest.RuntimeFieldsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class RuntimeFieldsDescriptor : 
    IsADictionaryDescriptorBase<RuntimeFieldsDescriptor, RuntimeFields, Field, IRuntimeField>
  {
    public RuntimeFieldsDescriptor()
      : base(new RuntimeFields())
    {
    }

    public RuntimeFieldsDescriptor RuntimeField(
      string name,
      FieldType type,
      Func<RuntimeFieldDescriptor, IRuntimeField> selector)
    {
      return this.Assign((Field) name, selector != null ? selector(new RuntimeFieldDescriptor(type)) : (IRuntimeField) null);
    }

    public RuntimeFieldsDescriptor RuntimeField(string name, FieldType type) => this.Assign((Field) name, (IRuntimeField) new RuntimeFieldDescriptor(type));
  }
}
