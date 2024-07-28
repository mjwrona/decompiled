// Decompiled with JetBrains decompiler
// Type: Nest.JoinPropertyDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Diagnostics;

namespace Nest
{
  [DebuggerDisplay("{DebugDisplay}")]
  public class JoinPropertyDescriptor<T> : 
    PropertyDescriptorBase<JoinPropertyDescriptor<T>, IJoinProperty, T>,
    IJoinProperty,
    IProperty,
    IFieldMapping
    where T : class
  {
    public JoinPropertyDescriptor()
      : base(FieldType.Join)
    {
    }

    IRelations IJoinProperty.Relations { get; set; }

    public JoinPropertyDescriptor<T> Relations(
      Func<RelationsDescriptor, IPromise<IRelations>> selector)
    {
      return this.Assign<Func<RelationsDescriptor, IPromise<IRelations>>>(selector, (Action<IJoinProperty, Func<RelationsDescriptor, IPromise<IRelations>>>) ((a, v) => a.Relations = v != null ? v(new RelationsDescriptor())?.Value : (IRelations) null));
    }
  }
}
