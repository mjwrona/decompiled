// Decompiled with JetBrains decompiler
// Type: Nest.ExceptionExtensions
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  internal static class ExceptionExtensions
  {
    internal static T ThrowWhen<T>(
      this T @object,
      Func<T, bool> predicate,
      string exceptionMessage)
    {
      return !(predicate != null ? new bool?(predicate(@object)) : new bool?()).GetValueOrDefault(false) ? @object : throw new ArgumentException(exceptionMessage);
    }
  }
}
