// Decompiled with JetBrains decompiler
// Type: Nest.RuntimeFieldsDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class RuntimeFieldsDescriptor<TDocument> : 
    IsADictionaryDescriptorBase<RuntimeFieldsDescriptor<TDocument>, RuntimeFields, Field, IRuntimeField>
    where TDocument : class
  {
    public RuntimeFieldsDescriptor()
      : base(new RuntimeFields())
    {
    }

    public RuntimeFieldsDescriptor<TDocument> RuntimeField(
      string name,
      FieldType type,
      Func<RuntimeFieldDescriptor, IRuntimeField> selector)
    {
      return this.Assign((Field) name, selector != null ? selector(new RuntimeFieldDescriptor(type)) : (IRuntimeField) null);
    }

    public RuntimeFieldsDescriptor<TDocument> RuntimeField(
      Expression<Func<TDocument, Field>> field,
      FieldType type,
      Func<RuntimeFieldDescriptor, IRuntimeField> selector)
    {
      return this.Assign((Field) (Expression) field, selector != null ? selector(new RuntimeFieldDescriptor(type)) : (IRuntimeField) null);
    }

    public RuntimeFieldsDescriptor<TDocument> RuntimeField(string name, FieldType type) => this.Assign((Field) name, (IRuntimeField) new RuntimeFieldDescriptor(type));

    public RuntimeFieldsDescriptor<TDocument> RuntimeField(
      Expression<Func<TDocument, Field>> field,
      FieldType type)
    {
      return this.Assign((Field) (Expression) field, (IRuntimeField) new RuntimeFieldDescriptor(type));
    }
  }
}
