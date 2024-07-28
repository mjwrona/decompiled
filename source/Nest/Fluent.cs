// Decompiled with JetBrains decompiler
// Type: Nest.Fluent
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Runtime.CompilerServices;

namespace Nest
{
  internal static class Fluent
  {
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static TDescriptor Assign<TDescriptor, TInterface, TValue>(
      TDescriptor self,
      TValue value,
      Action<TInterface, TValue> assign)
      where TDescriptor : class, TInterface
    {
      assign((TInterface) self, value);
      return self;
    }
  }
}
