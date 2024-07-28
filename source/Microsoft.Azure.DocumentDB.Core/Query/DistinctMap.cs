// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Query.DistinctMap
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Azure.Documents.Query
{
  internal abstract class DistinctMap
  {
    public static DistinctMap Create(DistinctQueryType distinctQueryType, UInt192? previousHash)
    {
      switch (distinctQueryType)
      {
        case DistinctQueryType.None:
          throw new ArgumentException("distinctQueryType can not be None. This part of code is not supposed to be reachable. Please contact support to resolve this issue.");
        case DistinctQueryType.Unordered:
          return (DistinctMap) new DistinctMap.UnorderdDistinctMap();
        case DistinctQueryType.Ordered:
          return (DistinctMap) new DistinctMap.OrderedDistinctMap(previousHash.GetValueOrDefault());
        default:
          throw new ArgumentException(string.Format("Unrecognized DistinctQueryType: {0}.", (object) distinctQueryType));
      }
    }

    public abstract bool Add(JToken jToken, out UInt192? hash);

    protected static UInt192 GetHash(JToken jToken) => DistinctHash.GetHashToken(jToken);

    private sealed class OrderedDistinctMap : DistinctMap
    {
      private UInt192 lastHash;

      public OrderedDistinctMap(UInt192 lastHash) => this.lastHash = lastHash;

      public override bool Add(JToken jToken, out UInt192? hash)
      {
        hash = new UInt192?(DistinctMap.GetHash(jToken));
        UInt192 lastHash = this.lastHash;
        UInt192? nullable = hash;
        bool flag;
        if ((nullable.HasValue ? (lastHash != nullable.GetValueOrDefault() ? 1 : 0) : 1) != 0)
        {
          this.lastHash = hash.Value;
          flag = true;
        }
        else
          flag = false;
        return flag;
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
      private static readonly JToken Undefined;
      private const int UInt192Length = 24;
      private const int UInt128Length = 16;
      private const int ULongLength = 8;
      private const int UIntLength = 4;
      private readonly byte[] utf8Buffer = new byte[24];
      private readonly HashSet<double> numbers = new HashSet<double>();
      private readonly HashSet<uint> stringsLength4 = new HashSet<uint>();
      private readonly HashSet<ulong> stringLength8 = new HashSet<ulong>();
      private readonly HashSet<UInt128> stringLength16 = new HashSet<UInt128>();
      private readonly HashSet<UInt192> stringLength24 = new HashSet<UInt192>();
      private readonly HashSet<UInt192> stringLength24Plus = new HashSet<UInt192>();
      private readonly HashSet<UInt192> arrays = new HashSet<UInt192>();
      private readonly HashSet<UInt192> objects = new HashSet<UInt192>();
      private DistinctMap.SimpleValues simpleValues;

      public override bool Add(JToken jToken, out UInt192? hash)
      {
        hash = new UInt192?();
        if (jToken == DistinctMap.UnorderdDistinctMap.Undefined)
          return this.AddSimpleValue(DistinctMap.SimpleValues.Undefined);
        JTokenType type = jToken.Type;
        switch (type)
        {
          case JTokenType.Object:
            return this.AddObjectValue((JObject) jToken);
          case JTokenType.Array:
            return this.AddArrayValue((JArray) jToken);
          case JTokenType.Integer:
          case JTokenType.Float:
            return this.AddNumberValue((double) jToken);
          case JTokenType.String:
          case JTokenType.Date:
          case JTokenType.Guid:
          case JTokenType.Uri:
          case JTokenType.TimeSpan:
            return this.AddStringValue(jToken.ToString());
          case JTokenType.Boolean:
            return this.AddSimpleValue((bool) jToken ? DistinctMap.SimpleValues.True : DistinctMap.SimpleValues.False);
          case JTokenType.Null:
            return this.AddSimpleValue(DistinctMap.SimpleValues.Null);
          default:
            throw new ArgumentException(string.Format("Unexpected JTokenType of: {0}", (object) type));
        }
      }

      private bool AddNumberValue(double value) => this.numbers.Add(value);

      private bool AddSimpleValue(DistinctMap.SimpleValues value)
      {
        if ((this.simpleValues & value) != DistinctMap.SimpleValues.None)
          return false;
        this.simpleValues |= value;
        return true;
      }

      private bool AddStringValue(string value)
      {
        int byteCount = Encoding.UTF8.GetByteCount(value);
        bool flag;
        if (byteCount <= 24)
        {
          Array.Clear((Array) this.utf8Buffer, 0, this.utf8Buffer.Length);
          Encoding.UTF8.GetBytes(value, 0, value.Length, this.utf8Buffer, 0);
          flag = byteCount != 0 ? (byteCount > 4 ? (byteCount > 8 ? (byteCount > 16 ? this.stringLength24.Add(UInt192.FromByteArray(this.utf8Buffer)) : this.stringLength16.Add(UInt128.FromByteArray(this.utf8Buffer))) : this.stringLength8.Add(BitConverter.ToUInt64(this.utf8Buffer, 0))) : this.stringsLength4.Add(BitConverter.ToUInt32(this.utf8Buffer, 0))) : this.AddSimpleValue(DistinctMap.SimpleValues.EmptyString);
        }
        else
          flag = this.stringLength24Plus.Add(DistinctMap.GetHash((JToken) value));
        return flag;
      }

      private bool AddArrayValue(JArray array) => this.arrays.Add(DistinctMap.GetHash((JToken) array));

      private bool AddObjectValue(JObject jObject) => this.objects.Add(DistinctMap.GetHash((JToken) jObject));
    }
  }
}
