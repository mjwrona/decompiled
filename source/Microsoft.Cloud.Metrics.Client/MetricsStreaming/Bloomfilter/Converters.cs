// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.MetricsStreaming.Bloomfilter.Converters
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Cloud.Metrics.Client.MetricsStreaming.Bloomfilter
{
  internal static class Converters
  {
    private static readonly Delegate[] BitConverters = new Delegate[5]
    {
      (Delegate) (rgb => rgb),
      Converters.CreateConverter<int>(),
      Converters.CreateConverter<uint>(),
      Converters.CreateConverter<long>(),
      Converters.CreateConverter<ulong>()
    };
    private static readonly Dictionary<Type, Delegate> TypeToBitConverter = new Dictionary<Type, Delegate>();

    static Converters()
    {
      foreach (Delegate bitConverter in Converters.BitConverters)
        Converters.TypeToBitConverter.Add(bitConverter.Method.GetParameters()[0].ParameterType, bitConverter);
    }

    public static Func<T, byte[]> GetConverter<T>()
    {
      Delegate converter;
      if (!Converters.TypeToBitConverter.TryGetValue(typeof (T), out converter))
        throw new ArgumentException("Unsupported type parameter: no pre-existing bit converter. Use an overload that includes a Func<T, byte[]> argument instead.");
      return (Func<T, byte[]>) converter;
    }

    private static Delegate CreateConverter<T>() => Delegate.CreateDelegate(typeof (Func<T, byte[]>), typeof (BitConverter).GetMethod("GetBytes", new Type[1]
    {
      typeof (T)
    }));
  }
}
