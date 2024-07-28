// Decompiled with JetBrains decompiler
// Type: Validation.TypeInfoExtensions
// Assembly: Validation, Version=2.2.0.0, Culture=neutral, PublicKeyToken=2fc06f0d701809a7
// MVID: B008DAAB-8462-4DA1-958C-4C90CA316797
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Validation.dll

using System;
using System.Reflection;

namespace Validation
{
  internal static class TypeInfoExtensions
  {
    internal static Type[] GetGenericArguments(this TypeInfo type) => type.GenericTypeArguments;
  }
}
