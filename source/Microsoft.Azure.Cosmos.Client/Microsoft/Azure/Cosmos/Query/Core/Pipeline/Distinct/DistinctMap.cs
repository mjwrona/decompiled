// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Query.Core.Pipeline.Distinct.DistinctMap
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Core.Utf8;
using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.CosmosElements.Numbers;
using Microsoft.Azure.Cosmos.Query.Core.Exceptions;
using Microsoft.Azure.Cosmos.Query.Core.Monads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.Azure.Cosmos.Query.Core.Pipeline.Distinct
{
  internal abstract class DistinctMap
  {
    public static TryCatch<DistinctMap> TryCreate(
      DistinctQueryType distinctQueryType,
      CosmosElement distinctMapContinuationToken)
    {
      switch (distinctQueryType)
      {
        case DistinctQueryType.None:
          throw new ArgumentException("distinctQueryType can not be None. This part of code is not supposed to be reachable. Please contact support to resolve this issue.");
        case DistinctQueryType.Unordered:
          return DistinctMap.UnorderdDistinctMap.TryCreate(distinctMapContinuationToken);
        case DistinctQueryType.Ordered:
          return DistinctMap.OrderedDistinctMap.TryCreate(distinctMapContinuationToken);
        default:
          throw new ArgumentException(string.Format("Unrecognized DistinctQueryType: {0}.", (object) distinctQueryType));
      }
    }

    public abstract bool Add(CosmosElement cosmosElement, out UInt128 hash);

    public abstract string GetContinuationToken();

    public abstract CosmosElement GetCosmosElementContinuationToken();

    private sealed class OrderedDistinctMap : DistinctMap
    {
      private UInt128 lastHash;

      private OrderedDistinctMap(UInt128 lastHash) => this.lastHash = lastHash;

      public override bool Add(CosmosElement cosmosElement, out UInt128 hash)
      {
        hash = DistinctHash.GetHash(cosmosElement);
        bool flag;
        if (this.lastHash != hash)
        {
          this.lastHash = hash;
          flag = true;
        }
        else
          flag = false;
        return flag;
      }

      public override string GetContinuationToken() => this.lastHash.ToString();

      public override CosmosElement GetCosmosElementContinuationToken() => (CosmosElement) CosmosBinary.Create((ReadOnlyMemory<byte>) UInt128.ToByteArray(this.lastHash));

      public static TryCatch<DistinctMap> TryCreate(CosmosElement requestContinuationToken)
      {
        UInt128 uInt128;
        if (requestContinuationToken != (CosmosElement) null)
        {
          switch (requestContinuationToken)
          {
            case CosmosString cosmosString:
              if (!UInt128.TryParse(UtfAnyString.op_Implicit(cosmosString.Value), out uInt128))
                return TryCatch<DistinctMap>.FromException((Exception) new MalformedContinuationTokenException(string.Format("Malformed {0} continuation token: {1}.", (object) nameof (OrderedDistinctMap), (object) requestContinuationToken)));
              break;
            case CosmosBinary cosmosBinary:
              if (!UInt128.TryCreateFromByteArray(cosmosBinary.Value.Span, out uInt128))
                return TryCatch<DistinctMap>.FromException((Exception) new MalformedContinuationTokenException(string.Format("Malformed {0} continuation token: {1}.", (object) nameof (OrderedDistinctMap), (object) requestContinuationToken)));
              break;
            default:
              throw new ArgumentOutOfRangeException(string.Format("Unknown {0} type. {1}.", (object) nameof (requestContinuationToken), (object) requestContinuationToken.GetType()));
          }
        }
        else
          uInt128 = new UInt128();
        return TryCatch<DistinctMap>.FromResult((DistinctMap) new DistinctMap.OrderedDistinctMap(uInt128));
      }
    }

    [Flags]
    private enum SimpleValues
    {
      None = 0,
      Undefined = 1,
      Null = 2,
      False = 4,
      True = 8,
      EmptyString = 16, // 0x00000010
      EmptyArray = 32, // 0x00000020
      EmptyObject = 64, // 0x00000040
    }

    private sealed class UnorderdDistinctMap : DistinctMap
    {
      private const int UInt128Length = 16;
      private const int ULongLength = 8;
      private const int UIntLength = 4;
      private readonly HashSet<Number64> numbers;
      private readonly HashSet<uint> stringsLength4;
      private readonly HashSet<ulong> stringsLength8;
      private readonly HashSet<UInt128> stringsLength16;
      private readonly HashSet<UInt128> stringsLength16Plus;
      private readonly HashSet<UInt128> arrays;
      private readonly HashSet<UInt128> objects;
      private DistinctMap.SimpleValues simpleValues;

      private UnorderdDistinctMap(
        HashSet<Number64> numbers,
        HashSet<uint> stringsLength4,
        HashSet<ulong> stringsLength8,
        HashSet<UInt128> stringsLength16,
        HashSet<UInt128> stringsLength16Plus,
        HashSet<UInt128> arrays,
        HashSet<UInt128> objects,
        DistinctMap.SimpleValues simpleValues)
      {
        this.numbers = numbers ?? throw new ArgumentNullException(nameof (numbers));
        this.stringsLength4 = stringsLength4 ?? throw new ArgumentNullException(nameof (stringsLength4));
        this.stringsLength8 = stringsLength8 ?? throw new ArgumentNullException(nameof (stringsLength8));
        this.stringsLength16 = stringsLength16 ?? throw new ArgumentNullException(nameof (stringsLength16));
        this.stringsLength16Plus = stringsLength16Plus ?? throw new ArgumentNullException(nameof (stringsLength16Plus));
        this.arrays = arrays ?? throw new ArgumentNullException(nameof (arrays));
        this.objects = objects ?? throw new ArgumentNullException(nameof (objects));
        this.simpleValues = simpleValues;
      }

      public override bool Add(CosmosElement cosmosElement, out UInt128 hash)
      {
        hash = new UInt128();
        switch (cosmosElement)
        {
          case CosmosArray array:
            return this.AddArrayValue(array);
          case CosmosBoolean cosmosBoolean:
            return this.AddSimpleValue(cosmosBoolean.Value ? DistinctMap.SimpleValues.True : DistinctMap.SimpleValues.False);
          case CosmosNull _:
            return this.AddSimpleValue(DistinctMap.SimpleValues.Null);
          case CosmosNumber cosmosNumber:
            return this.AddNumberValue(cosmosNumber.Value);
          case CosmosObject cosmosObject:
            return this.AddObjectValue(cosmosObject);
          case CosmosString cosmosString:
            return this.AddStringValue(UtfAnyString.op_Implicit(cosmosString.Value));
          default:
            throw new ArgumentOutOfRangeException(string.Format("Unexpected {0}: {1}", (object) "CosmosElement", (object) cosmosElement));
        }
      }

      public override string GetContinuationToken() => this.GetCosmosElementContinuationToken().ToString();

      public override CosmosElement GetCosmosElementContinuationToken() => (CosmosElement) CosmosObject.Create((IReadOnlyDictionary<string, CosmosElement>) new Dictionary<string, CosmosElement>()
      {
        {
          "Numbers",
          (CosmosElement) CosmosArray.Create((IEnumerable<CosmosElement>) this.numbers.Select<Number64, CosmosNumber64>((Func<Number64, CosmosNumber64>) (x => CosmosNumber64.Create(x))))
        },
        {
          "StringsLength4",
          (CosmosElement) CosmosArray.Create((IEnumerable<CosmosElement>) this.stringsLength4.Select<uint, CosmosUInt32>((Func<uint, CosmosUInt32>) (x => CosmosUInt32.Create(x))))
        },
        {
          "StringsLength8",
          (CosmosElement) CosmosArray.Create((IEnumerable<CosmosElement>) this.stringsLength8.Select<ulong, CosmosInt64>((Func<ulong, CosmosInt64>) (x => CosmosInt64.Create((long) x))))
        },
        {
          "StringsLength16",
          (CosmosElement) CosmosArray.Create((IEnumerable<CosmosElement>) this.stringsLength16.Select<UInt128, CosmosBinary>((Func<UInt128, CosmosBinary>) (x => CosmosBinary.Create((ReadOnlyMemory<byte>) UInt128.ToByteArray(x)))))
        },
        {
          "StringsLength16+",
          (CosmosElement) CosmosArray.Create((IEnumerable<CosmosElement>) this.stringsLength16Plus.Select<UInt128, CosmosBinary>((Func<UInt128, CosmosBinary>) (x => CosmosBinary.Create((ReadOnlyMemory<byte>) UInt128.ToByteArray(x)))))
        },
        {
          "Arrays",
          (CosmosElement) CosmosArray.Create((IEnumerable<CosmosElement>) this.arrays.Select<UInt128, CosmosBinary>((Func<UInt128, CosmosBinary>) (x => CosmosBinary.Create((ReadOnlyMemory<byte>) UInt128.ToByteArray(x)))))
        },
        {
          "Object",
          (CosmosElement) CosmosArray.Create((IEnumerable<CosmosElement>) this.objects.Select<UInt128, CosmosBinary>((Func<UInt128, CosmosBinary>) (x => CosmosBinary.Create((ReadOnlyMemory<byte>) UInt128.ToByteArray(x)))))
        },
        {
          "SimpleValues",
          (CosmosElement) CosmosString.Create(this.simpleValues.ToString())
        }
      });

      private bool AddNumberValue(Number64 value) => this.numbers.Add(value);

      private bool AddSimpleValue(DistinctMap.SimpleValues value)
      {
        if ((this.simpleValues & value) != DistinctMap.SimpleValues.None)
          return false;
        this.simpleValues |= value;
        return true;
      }

      private unsafe bool AddStringValue(string value)
      {
        int byteCount = Encoding.UTF8.GetByteCount(value);
        bool flag;
        if (byteCount <= 16)
        {
          // ISSUE: untyped stack allocation
          Span<byte> span = new Span<byte>((void*) __untypedstackalloc(new IntPtr(16)), 16);
          Encoding.UTF8.GetBytes(value, span);
          flag = byteCount != 0 ? (byteCount > 4 ? (byteCount > 8 ? this.stringsLength16.Add(UInt128.FromByteArray((ReadOnlySpan<byte>) span)) : this.stringsLength8.Add(MemoryMarshal.Read<ulong>((ReadOnlySpan<byte>) span))) : this.stringsLength4.Add(MemoryMarshal.Read<uint>((ReadOnlySpan<byte>) span))) : this.AddSimpleValue(DistinctMap.SimpleValues.EmptyString);
        }
        else
          flag = this.stringsLength16Plus.Add(DistinctHash.GetHash((CosmosElement) CosmosString.Create(value)));
        return flag;
      }

      private bool AddArrayValue(CosmosArray array) => this.arrays.Add(DistinctHash.GetHash((CosmosElement) array));

      private bool AddObjectValue(CosmosObject cosmosObject) => this.objects.Add(DistinctHash.GetHash((CosmosElement) cosmosObject));

      public static TryCatch<DistinctMap> TryCreate(CosmosElement continuationToken)
      {
        HashSet<Number64> numbers = new HashSet<Number64>();
        HashSet<uint> stringsLength4 = new HashSet<uint>();
        HashSet<ulong> stringsLength8 = new HashSet<ulong>();
        HashSet<UInt128> stringsLength16 = new HashSet<UInt128>();
        HashSet<UInt128> stringsLength16Plus = new HashSet<UInt128>();
        HashSet<UInt128> arrays = new HashSet<UInt128>();
        HashSet<UInt128> objects = new HashSet<UInt128>();
        DistinctMap.SimpleValues result = DistinctMap.SimpleValues.None;
        if (continuationToken != (CosmosElement) null)
        {
          if (!(continuationToken is CosmosObject hashDictionary))
            return TryCatch<DistinctMap>.FromException((Exception) new MalformedContinuationTokenException("UnorderdDistinctMap continuation token was malformed."));
          CosmosArray typedCosmosElement1;
          if (!hashDictionary.TryGetValue<CosmosArray>("Numbers", out typedCosmosElement1))
            return TryCatch<DistinctMap>.FromException((Exception) new MalformedContinuationTokenException("UnorderdDistinctMap continuation token was malformed."));
          foreach (CosmosElement cosmosElement in typedCosmosElement1)
          {
            if (!(cosmosElement is CosmosNumber64 cosmosNumber64))
              return TryCatch<DistinctMap>.FromException((Exception) new MalformedContinuationTokenException("UnorderdDistinctMap continuation token was malformed."));
            numbers.Add(cosmosNumber64.GetValue());
          }
          CosmosArray typedCosmosElement2;
          if (!hashDictionary.TryGetValue<CosmosArray>("StringsLength4", out typedCosmosElement2))
            return TryCatch<DistinctMap>.FromException((Exception) new MalformedContinuationTokenException("UnorderdDistinctMap continuation token was malformed."));
          foreach (CosmosElement cosmosElement in typedCosmosElement2)
          {
            if (!(cosmosElement is CosmosUInt32 cosmosUint32))
              return TryCatch<DistinctMap>.FromException((Exception) new MalformedContinuationTokenException("UnorderdDistinctMap continuation token was malformed."));
            stringsLength4.Add(cosmosUint32.GetValue());
          }
          CosmosArray typedCosmosElement3;
          if (!hashDictionary.TryGetValue<CosmosArray>("StringsLength8", out typedCosmosElement3))
            return TryCatch<DistinctMap>.FromException((Exception) new MalformedContinuationTokenException("UnorderdDistinctMap continuation token was malformed."));
          foreach (CosmosElement cosmosElement in typedCosmosElement3)
          {
            if (!(cosmosElement is CosmosInt64 cosmosInt64))
              return TryCatch<DistinctMap>.FromException((Exception) new MalformedContinuationTokenException("UnorderdDistinctMap continuation token was malformed."));
            stringsLength8.Add((ulong) cosmosInt64.GetValue());
          }
          stringsLength16 = DistinctMap.UnorderdDistinctMap.Parse128BitHashes(hashDictionary, "StringsLength16");
          stringsLength16Plus = DistinctMap.UnorderdDistinctMap.Parse128BitHashes(hashDictionary, "StringsLength16+");
          arrays = DistinctMap.UnorderdDistinctMap.Parse128BitHashes(hashDictionary, "Arrays");
          objects = DistinctMap.UnorderdDistinctMap.Parse128BitHashes(hashDictionary, "Object");
          if (!(hashDictionary["SimpleValues"] is CosmosString cosmosString))
            return TryCatch<DistinctMap>.FromException((Exception) new MalformedContinuationTokenException("UnorderdDistinctMap continuation token was malformed."));
          if (!Enum.TryParse<DistinctMap.SimpleValues>(UtfAnyString.op_Implicit(cosmosString.Value), out result))
            return TryCatch<DistinctMap>.FromException((Exception) new MalformedContinuationTokenException("UnorderdDistinctMap continuation token was malformed."));
        }
        return TryCatch<DistinctMap>.FromResult((DistinctMap) new DistinctMap.UnorderdDistinctMap(numbers, stringsLength4, stringsLength8, stringsLength16, stringsLength16Plus, arrays, objects, result));
      }

      private static HashSet<UInt128> Parse128BitHashes(
        CosmosObject hashDictionary,
        string propertyName)
      {
        HashSet<UInt128> uint128Set = new HashSet<UInt128>();
        CosmosArray typedCosmosElement;
        if (!hashDictionary.TryGetValue<CosmosArray>(propertyName, out typedCosmosElement))
          throw new MalformedContinuationTokenException("UnorderdDistinctMap continuation token was malformed.");
        foreach (CosmosElement cosmosElement in typedCosmosElement)
        {
          if (!(cosmosElement is CosmosBinary cosmosBinary))
            throw new MalformedContinuationTokenException("UnorderdDistinctMap continuation token was malformed.");
          UInt128 uint128 = UInt128.FromByteArray(cosmosBinary.Value.Span);
          uint128Set.Add(uint128);
        }
        return uint128Set;
      }

      private static class PropertyNames
      {
        public const string Numbers = "Numbers";
        public const string StringsLength4 = "StringsLength4";
        public const string StringsLength8 = "StringsLength8";
        public const string StringsLength16 = "StringsLength16";
        public const string StringsLength16Plus = "StringsLength16+";
        public const string Arrays = "Arrays";
        public const string Object = "Object";
        public const string SimpleValues = "SimpleValues";
      }
    }
  }
}
