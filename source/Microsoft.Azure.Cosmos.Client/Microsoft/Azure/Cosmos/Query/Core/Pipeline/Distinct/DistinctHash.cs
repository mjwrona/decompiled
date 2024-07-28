// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.Distinct.DistinctHash
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.CosmosElements.Numbers;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline.Distinct
{
  internal static class DistinctHash
  {
    private static readonly UInt128 RootHashSeed = UInt128.Create(13817665562395861687UL, 9819782338949992223UL);

    public static UInt128 GetHash(CosmosElement cosmosElement) => DistinctHash.GetHash(cosmosElement, DistinctHash.RootHashSeed);

    private static UInt128 GetHash(CosmosElement cosmosElement, UInt128 seed) => cosmosElement.Accept<UInt128, UInt128>((ICosmosElementVisitor<UInt128, UInt128>) DistinctHash.CosmosElementHasher.Singleton, seed);

    private sealed class CosmosElementHasher : ICosmosElementVisitor<UInt128, UInt128>
    {
      public static readonly DistinctHash.CosmosElementHasher Singleton = new DistinctHash.CosmosElementHasher();

      private CosmosElementHasher()
      {
      }

      public UInt128 Visit(CosmosArray cosmosArray, UInt128 seed)
      {
        UInt128 seed1 = seed == DistinctHash.RootHashSeed ? DistinctHash.CosmosElementHasher.RootCache.Array : MurmurHash3.Hash128<UInt128>(DistinctHash.CosmosElementHasher.HashSeeds.Array, seed);
        for (int index = 0; index < cosmosArray.Count; ++index)
        {
          CosmosElement cosmos = cosmosArray[index];
          if (!(cosmos is CosmosUndefined))
          {
            UInt128 input = DistinctHash.CosmosElementHasher.HashSeeds.ArrayIndex + (UInt128) index;
            seed1 = MurmurHash3.Hash128<UInt128>(cosmos.Accept<UInt128, UInt128>((ICosmosElementVisitor<UInt128, UInt128>) this, input), seed1);
          }
        }
        return seed1;
      }

      public UInt128 Visit(CosmosBinary cosmosBinary, UInt128 seed)
      {
        UInt128 seed1 = seed == DistinctHash.RootHashSeed ? DistinctHash.CosmosElementHasher.RootCache.Binary : MurmurHash3.Hash128<UInt128>(DistinctHash.CosmosElementHasher.HashSeeds.Binary, seed);
        return MurmurHash3.Hash128(cosmosBinary.Value.Span, seed1);
      }

      public UInt128 Visit(CosmosBoolean cosmosBoolean, UInt128 seed)
      {
        if (!(seed == DistinctHash.RootHashSeed))
          return MurmurHash3.Hash128<UInt128>(cosmosBoolean.Value ? DistinctHash.CosmosElementHasher.HashSeeds.True : DistinctHash.CosmosElementHasher.HashSeeds.False, seed);
        return !cosmosBoolean.Value ? DistinctHash.CosmosElementHasher.RootCache.False : DistinctHash.CosmosElementHasher.RootCache.True;
      }

      public UInt128 Visit(CosmosGuid cosmosGuid, UInt128 seed)
      {
        UInt128 seed1 = seed == DistinctHash.RootHashSeed ? DistinctHash.CosmosElementHasher.RootCache.Guid : MurmurHash3.Hash128<UInt128>(DistinctHash.CosmosElementHasher.HashSeeds.Guid, seed);
        return MurmurHash3.Hash128((ReadOnlySpan<byte>) cosmosGuid.Value.ToByteArray(), seed1);
      }

      public UInt128 Visit(CosmosNull cosmosNull, UInt128 seed) => seed == DistinctHash.RootHashSeed ? DistinctHash.CosmosElementHasher.RootCache.Null : MurmurHash3.Hash128<UInt128>(DistinctHash.CosmosElementHasher.HashSeeds.Null, seed);

      public UInt128 Visit(CosmosUndefined cosmosUndefined, UInt128 seed) => seed;

      public UInt128 Visit(CosmosNumber cosmosNumber, UInt128 seed) => cosmosNumber.Accept<UInt128, UInt128>((ICosmosNumberVisitor<UInt128, UInt128>) DistinctHash.CosmosNumberHasher.Singleton, seed);

      public UInt128 Visit(CosmosObject cosmosObject, UInt128 seed)
      {
        UInt128 seed1 = seed == DistinctHash.RootHashSeed ? DistinctHash.CosmosElementHasher.RootCache.Object : MurmurHash3.Hash128<UInt128>(DistinctHash.CosmosElementHasher.HashSeeds.Object, seed);
        UInt128 uint128_1 = (UInt128) 0;
        foreach (KeyValuePair<string, CosmosElement> keyValuePair in cosmosObject)
        {
          if (!(keyValuePair.Value is CosmosUndefined))
          {
            UInt128 input = MurmurHash3.Hash128(keyValuePair.Key, MurmurHash3.Hash128<UInt128>(DistinctHash.CosmosElementHasher.HashSeeds.String, DistinctHash.CosmosElementHasher.HashSeeds.PropertyName));
            UInt128 uint128_2 = keyValuePair.Value.Accept<UInt128, UInt128>((ICosmosElementVisitor<UInt128, UInt128>) this, input);
            uint128_1 ^= uint128_2;
          }
        }
        if (uint128_1 > (UInt128) 0)
          seed1 = MurmurHash3.Hash128<UInt128>(uint128_1, seed1);
        return seed1;
      }

      public UInt128 Visit(CosmosString cosmosString, UInt128 seed)
      {
        UInt128 seed1 = seed == DistinctHash.RootHashSeed ? DistinctHash.CosmosElementHasher.RootCache.String : MurmurHash3.Hash128<UInt128>(DistinctHash.CosmosElementHasher.HashSeeds.String, seed);
        UtfAnyString utfAnyString = cosmosString.Value;
        UInt128 uint128;
        if (!((UtfAnyString) ref utfAnyString).IsUtf8)
        {
          uint128 = MurmurHash3.Hash128(utfAnyString.ToString(), seed1);
        }
        else
        {
          Utf8Span span = ((UtfAnyString) ref utfAnyString).ToUtf8String().Span;
          uint128 = MurmurHash3.Hash128(((Utf8Span) ref span).Span, seed1);
        }
        return uint128;
      }

      private static class HashSeeds
      {
        public static readonly UInt128 Null = UInt128.Create(1405394163615191012UL, 1543768802584751688UL);
        public static readonly UInt128 False = UInt128.Create(13960685504699806732UL, 16860503165384839389UL);
        public static readonly UInt128 True = UInt128.Create(17901046280654761588UL, 8684215748539357277UL);
        public static readonly UInt128 String = UInt128.Create(7058617304298310907UL, 668815231925573341UL);
        public static readonly UInt128 Array = UInt128.Create(18038951709216981390UL, 11534933797263814933UL);
        public static readonly UInt128 Object = UInt128.Create(8625103211509509936UL, 4453805435564205129UL);
        public static readonly UInt128 ArrayIndex = UInt128.Create(18304161622583523737UL, 6565336862697309587UL);
        public static readonly UInt128 PropertyName = UInt128.Create(14489731331758041738UL, 8974516184215864884UL);
        public static readonly UInt128 Binary = UInt128.Create(6090024868464469100UL, 15343113819980920427UL);
        public static readonly UInt128 Guid = UInt128.Create(6031930220040752971UL, 8990839158481972401UL);
      }

      private static class RootCache
      {
        public static readonly UInt128 Null = MurmurHash3.Hash128<UInt128>(DistinctHash.CosmosElementHasher.HashSeeds.Null, DistinctHash.RootHashSeed);
        public static readonly UInt128 False = MurmurHash3.Hash128<UInt128>(DistinctHash.CosmosElementHasher.HashSeeds.False, DistinctHash.RootHashSeed);
        public static readonly UInt128 True = MurmurHash3.Hash128<UInt128>(DistinctHash.CosmosElementHasher.HashSeeds.True, DistinctHash.RootHashSeed);
        public static readonly UInt128 String = MurmurHash3.Hash128<UInt128>(DistinctHash.CosmosElementHasher.HashSeeds.String, DistinctHash.RootHashSeed);
        public static readonly UInt128 Array = MurmurHash3.Hash128<UInt128>(DistinctHash.CosmosElementHasher.HashSeeds.Array, DistinctHash.RootHashSeed);
        public static readonly UInt128 Object = MurmurHash3.Hash128<UInt128>(DistinctHash.CosmosElementHasher.HashSeeds.Object, DistinctHash.RootHashSeed);
        public static readonly UInt128 Binary = MurmurHash3.Hash128<UInt128>(DistinctHash.CosmosElementHasher.HashSeeds.Binary, DistinctHash.RootHashSeed);
        public static readonly UInt128 Guid = MurmurHash3.Hash128<UInt128>(DistinctHash.CosmosElementHasher.HashSeeds.Guid, DistinctHash.RootHashSeed);
      }
    }

    private sealed class CosmosNumberHasher : ICosmosNumberVisitor<UInt128, UInt128>
    {
      public static readonly DistinctHash.CosmosNumberHasher Singleton = new DistinctHash.CosmosNumberHasher();

      private CosmosNumberHasher()
      {
      }

      public UInt128 Visit(CosmosFloat32 cosmosFloat32, UInt128 seed)
      {
        UInt128 seed1 = seed == DistinctHash.RootHashSeed ? DistinctHash.CosmosNumberHasher.RootCache.Float32 : MurmurHash3.Hash128<UInt128>(DistinctHash.CosmosNumberHasher.HashSeeds.Float32, seed);
        float num = cosmosFloat32.GetValue();
        if ((double) num == 0.0)
          num = 0.0f;
        return MurmurHash3.Hash128<UInt128>((UInt128) BitConverter.DoubleToInt64Bits((double) num), seed1);
      }

      public UInt128 Visit(CosmosFloat64 cosmosFloat64, UInt128 seed)
      {
        UInt128 seed1 = seed == DistinctHash.RootHashSeed ? DistinctHash.CosmosNumberHasher.RootCache.Float64 : MurmurHash3.Hash128<UInt128>(DistinctHash.CosmosNumberHasher.HashSeeds.Float64, seed);
        double num = cosmosFloat64.GetValue();
        if (num == 0.0)
          num = 0.0;
        return MurmurHash3.Hash128<UInt128>((UInt128) BitConverter.DoubleToInt64Bits(num), seed1);
      }

      public UInt128 Visit(CosmosInt16 cosmosInt16, UInt128 seed)
      {
        UInt128 seed1 = seed == DistinctHash.RootHashSeed ? DistinctHash.CosmosNumberHasher.RootCache.Int16 : MurmurHash3.Hash128<UInt128>(DistinctHash.CosmosNumberHasher.HashSeeds.Int16, seed);
        return MurmurHash3.Hash128<short>(cosmosInt16.GetValue(), seed1);
      }

      public UInt128 Visit(CosmosInt32 cosmosInt32, UInt128 seed)
      {
        UInt128 seed1 = seed == DistinctHash.RootHashSeed ? DistinctHash.CosmosNumberHasher.RootCache.Int32 : MurmurHash3.Hash128<UInt128>(DistinctHash.CosmosNumberHasher.HashSeeds.Int32, seed);
        return MurmurHash3.Hash128<int>(cosmosInt32.GetValue(), seed1);
      }

      public UInt128 Visit(CosmosInt64 cosmosInt64, UInt128 seed)
      {
        UInt128 seed1 = seed == DistinctHash.RootHashSeed ? DistinctHash.CosmosNumberHasher.RootCache.Int64 : MurmurHash3.Hash128<UInt128>(DistinctHash.CosmosNumberHasher.HashSeeds.Int64, seed);
        return MurmurHash3.Hash128<long>(cosmosInt64.GetValue(), seed1);
      }

      public UInt128 Visit(CosmosInt8 cosmosInt8, UInt128 seed)
      {
        UInt128 seed1 = seed == DistinctHash.RootHashSeed ? DistinctHash.CosmosNumberHasher.RootCache.Int8 : MurmurHash3.Hash128<UInt128>(DistinctHash.CosmosNumberHasher.HashSeeds.Int8, seed);
        return MurmurHash3.Hash128<sbyte>(cosmosInt8.GetValue(), seed1);
      }

      public UInt128 Visit(CosmosNumber64 cosmosNumber64, UInt128 seed)
      {
        UInt128 seed1 = seed == DistinctHash.RootHashSeed ? DistinctHash.CosmosNumberHasher.RootCache.Number64 : MurmurHash3.Hash128<UInt128>(DistinctHash.CosmosNumberHasher.HashSeeds.Number64, seed);
        return MurmurHash3.Hash128<Number64.DoubleEx>(Number64.ToDoubleEx(cosmosNumber64.GetValue()), seed1);
      }

      public UInt128 Visit(CosmosUInt32 cosmosUInt32, UInt128 seed)
      {
        UInt128 seed1 = seed == DistinctHash.RootHashSeed ? DistinctHash.CosmosNumberHasher.RootCache.UInt32 : MurmurHash3.Hash128<UInt128>(DistinctHash.CosmosNumberHasher.HashSeeds.UInt32, seed);
        return MurmurHash3.Hash128<uint>(cosmosUInt32.GetValue(), seed1);
      }

      public static class HashSeeds
      {
        public static readonly UInt128 Number64 = UInt128.Create(2594329264833600554UL, 8722313501650687105UL);
        public static readonly UInt128 Float32 = UInt128.Create(9807803984582676502UL, 2156327268277075275UL);
        public static readonly UInt128 Float64 = UInt128.Create(7132374477586391968UL, 16854317498723058691UL);
        public static readonly UInt128 Int8 = UInt128.Create(2136918320115114UL, 9913327092325696695UL);
        public static readonly UInt128 Int16 = UInt128.Create(16690780031523298313UL, 3738192885879437632UL);
        public static readonly UInt128 Int32 = UInt128.Create(225422494785158769UL, 17687287347282575269UL);
        public static readonly UInt128 Int64 = UInt128.Create(17119232182842779192UL, 961293960192956521UL);
        public static readonly UInt128 UInt32 = UInt128.Create(8702152547565681518UL, 12432410556547049245UL);
      }

      public static class RootCache
      {
        public static readonly UInt128 Number64 = MurmurHash3.Hash128<UInt128>(DistinctHash.CosmosNumberHasher.HashSeeds.Number64, DistinctHash.RootHashSeed);
        public static readonly UInt128 Float32 = MurmurHash3.Hash128<UInt128>(DistinctHash.CosmosNumberHasher.HashSeeds.Float32, DistinctHash.RootHashSeed);
        public static readonly UInt128 Float64 = MurmurHash3.Hash128<UInt128>(DistinctHash.CosmosNumberHasher.HashSeeds.Float64, DistinctHash.RootHashSeed);
        public static readonly UInt128 Int8 = MurmurHash3.Hash128<UInt128>(DistinctHash.CosmosNumberHasher.HashSeeds.Int8, DistinctHash.RootHashSeed);
        public static readonly UInt128 Int16 = MurmurHash3.Hash128<UInt128>(DistinctHash.CosmosNumberHasher.HashSeeds.Int16, DistinctHash.RootHashSeed);
        public static readonly UInt128 Int32 = MurmurHash3.Hash128<UInt128>(DistinctHash.CosmosNumberHasher.HashSeeds.Int32, DistinctHash.RootHashSeed);
        public static readonly UInt128 Int64 = MurmurHash3.Hash128<UInt128>(DistinctHash.CosmosNumberHasher.HashSeeds.Int64, DistinctHash.RootHashSeed);
        public static readonly UInt128 UInt32 = MurmurHash3.Hash128<UInt128>(DistinctHash.CosmosNumberHasher.HashSeeds.UInt32, DistinctHash.RootHashSeed);
      }
    }
  }
}
