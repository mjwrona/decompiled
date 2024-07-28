// Decompiled with JetBrains decompiler
// Type: HdrHistogram.Persistence.CountsDecoder
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace HdrHistogram.Persistence
{
  internal static class CountsDecoder
  {
    private static readonly IDictionary<int, ICountsDecoder> Decoders = (IDictionary<int, ICountsDecoder>) ((IEnumerable<ICountsDecoder>) new ICountsDecoder[4]
    {
      (ICountsDecoder) new ShortCountsDecoder(),
      (ICountsDecoder) new IntCountsDecoder(),
      (ICountsDecoder) new LongCountsDecoder(),
      (ICountsDecoder) new V2MaxWordSizeCountsDecoder()
    }).ToDictionary<ICountsDecoder, int>((Func<ICountsDecoder, int>) (cd => cd.WordSize));

    public static ICountsDecoder GetDecoderForWordSize(int wordSize) => CountsDecoder.Decoders[wordSize];
  }
}
