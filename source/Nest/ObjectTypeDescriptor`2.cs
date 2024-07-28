// Decompiled with JetBrains decompiler
// Type: Nest.ObjectTypeDescriptor`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Diagnostics;

namespace Nest
{
  [DebuggerDisplay("{DebugDisplay}")]
  public class ObjectTypeDescriptor<TParent, TChild> : 
    ObjectPropertyDescriptorBase<ObjectTypeDescriptor<TParent, TChild>, IObjectProperty, TParent, TChild>,
    IObjectProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
    where TParent : class
    where TChild : class
  {
  }
}
