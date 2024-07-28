// Decompiled with JetBrains decompiler
// Type: HdrHistogram.Utilities.TypeHelper
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HdrHistogram.Utilities
{
  internal static class TypeHelper
  {
    public static ConstructorInfo GetConstructor(Type type, Type[] ctorArgTypes) => type.GetTypeInfo().DeclaredConstructors.FirstOrDefault<ConstructorInfo>((Func<ConstructorInfo, bool>) (ctor => TypeHelper.IsParameterMatch(ctor, ctorArgTypes)));

    private static bool IsParameterMatch(ConstructorInfo ctor, Type[] expectedParamters) => ((IEnumerable<ParameterInfo>) ctor.GetParameters()).Select<ParameterInfo, Type>((Func<ParameterInfo, Type>) (p => p.ParameterType)).ToArray<Type>().IsSequenceEqual<Type>(expectedParamters);
  }
}
