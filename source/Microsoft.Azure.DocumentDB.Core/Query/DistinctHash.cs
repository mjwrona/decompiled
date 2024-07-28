// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Query.DistinctHash
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Routing;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.Documents.Query
{
  internal static class DistinctHash
  {
    private static readonly UInt192 RootHashSeed = UInt192.Create(15992047605174166762UL, 11514953832815889395UL, 4478289918784009099UL);
    private static readonly UInt192 NullHashSeed = UInt192.Create(12278193463599318509UL, 13575553817073638549UL, 12367631068517817386UL);
    private static readonly UInt192 FalseHashSeed = UInt192.Create(1429697140260995089UL, 12058514231935786610UL, 15213120655323614500UL);
    private static readonly UInt192 TrueHashSeed = UInt192.Create(4787996480607579680UL, 5989861620227925760UL, 6910469410427058089UL);
    private static readonly UInt192 NumberHashSeed = UInt192.Create(13151337925362354973UL, 1067564635031500007UL, 6409491933319599190UL);
    private static readonly UInt192 StringHashSeed = UInt192.Create(7496206448276951651UL, 676088056534543826UL, 12486218591645107815UL);
    private static readonly UInt192 ArrayHashSeed = UInt192.Create(16779609907233277819UL, 7151985556769501107UL, 16729802325368678337UL);
    private static readonly UInt192 ObjectHashSeed = UInt192.Create(2859240458868302001UL, 12963811524562026943UL, 7949939114162109242UL);
    private static readonly UInt192 ArrayIndexHashSeed = UInt192.Create(17548991362029881809UL, 6875091296931507438UL, 14055947870195278658UL);
    private static readonly UInt192 PropertyNameHashSeed = UInt192.Create(18405052666065471316UL, 14772279679115199733UL, 5168031347707020066UL);
    private static readonly JToken Undefined = (JToken) null;
    private const int UInt192LengthInBits = 192;
    private const int BitsPerByte = 8;
    private const int UInt192LengthInBytes = 24;

    public static UInt192 GetHashToken(JToken value) => DistinctHash.GetHashToken(value, DistinctHash.RootHashSeed);

    private static UInt192 GetHash(UInt192 value, UInt192 seed) => DistinctHash.GetHash(UInt192.ToByteArray(value), seed);

    private static UInt192 GetHash(byte[] bytes, UInt192 seed)
    {
      UInt128 uint128 = MurmurHash3.Hash128(bytes, bytes.Length, UInt128.Create(seed.GetLow(), seed.GetMid()));
      ulong high = MurmurHash3.Hash64(bytes, bytes.Length, seed.GetHigh());
      return UInt192.Create(uint128.GetLow(), uint128.GetHigh(), high);
    }

    private static UInt192 GetHashToken(JToken value, UInt192 seed)
    {
      if (value == DistinctHash.Undefined)
        return seed;
      JTokenType type = value.Type;
      switch (type)
      {
        case JTokenType.None:
        case JTokenType.Undefined:
          return DistinctHash.GetUndefinedHash(seed);
        case JTokenType.Object:
          return DistinctHash.GetObjectHash((JObject) value, seed);
        case JTokenType.Array:
          return DistinctHash.GetArrayHash((JArray) value, seed);
        case JTokenType.Integer:
        case JTokenType.Float:
          return DistinctHash.GetNumberHash((double) value, seed);
        case JTokenType.String:
        case JTokenType.Date:
        case JTokenType.Guid:
        case JTokenType.Uri:
        case JTokenType.TimeSpan:
          return DistinctHash.GetStringHash(value.ToString(), seed);
        case JTokenType.Boolean:
          return DistinctHash.GetBooleanHash((bool) value, seed);
        case JTokenType.Null:
          return DistinctHash.GetNullHash(seed);
        default:
          throw new ArgumentException(string.Format("Unexpected JTokenType of: {0}", (object) type));
      }
    }

    private static UInt192 GetUndefinedHash(UInt192 seed) => seed;

    private static UInt192 GetNullHash(UInt192 seed) => DistinctHash.GetHash(DistinctHash.NullHashSeed, seed);

    private static UInt192 GetBooleanHash(bool boolean, UInt192 seed) => DistinctHash.GetHash(boolean ? DistinctHash.TrueHashSeed : DistinctHash.FalseHashSeed, seed);

    private static UInt192 GetNumberHash(double number, UInt192 seed)
    {
      UInt192 hash = DistinctHash.GetHash(DistinctHash.NumberHashSeed, seed);
      return DistinctHash.GetHash((UInt192) BitConverter.DoubleToInt64Bits(number), hash);
    }

    private static UInt192 GetStringHash(string value, UInt192 seed)
    {
      UInt192 hash = DistinctHash.GetHash(DistinctHash.StringHashSeed, seed);
      return DistinctHash.GetHash(Encoding.UTF8.GetBytes(value), hash);
    }

    private static UInt192 GetArrayHash(JArray array, UInt192 seed)
    {
      UInt192 hash = DistinctHash.GetHash(DistinctHash.ArrayHashSeed, seed);
      for (int index = 0; index < array.Count; ++index)
      {
        JToken jtoken = array[index];
        UInt192 seed1 = DistinctHash.ArrayIndexHashSeed + (UInt192) index;
        hash = DistinctHash.GetHash(hash, DistinctHash.GetHashToken(jtoken, seed1));
      }
      return hash;
    }

    private static UInt192 GetObjectHash(JObject jObject, UInt192 seed)
    {
      UInt192 hash = DistinctHash.GetHash(DistinctHash.ObjectHashSeed, seed);
      UInt192 uint192 = (UInt192) 0;
      foreach (KeyValuePair<string, JToken> keyValuePair in jObject)
      {
        UInt192 hashToken1 = DistinctHash.GetHashToken((JToken) keyValuePair.Key, DistinctHash.PropertyNameHashSeed);
        UInt192 hashToken2 = DistinctHash.GetHashToken(keyValuePair.Value, hashToken1);
        uint192 ^= hashToken2;
      }
      if (uint192 > (UInt192) 0)
        hash = DistinctHash.GetHash(uint192, hash);
      return hash;
    }
  }
}
