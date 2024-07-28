// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.ExtensionMethods
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System;
using System.Globalization;
using System.IO;

namespace Microsoft.Spatial
{
  internal static class ExtensionMethods
  {
    public static void WriteRoundtrippable(this TextWriter writer, double d) => writer.Write(d.ToString("R", (IFormatProvider) CultureInfo.InvariantCulture));

    internal static TResult? IfValidReturningNullable<TArg, TResult>(
      this TArg arg,
      Func<TArg, TResult> op)
      where TArg : class
      where TResult : struct
    {
      return (object) arg != null ? new TResult?(op(arg)) : new TResult?();
    }

    internal static TResult IfValid<TArg, TResult>(this TArg arg, Func<TArg, TResult> op)
      where TArg : class
      where TResult : class
    {
      return (object) arg != null ? op(arg) : default (TResult);
    }
  }
}
