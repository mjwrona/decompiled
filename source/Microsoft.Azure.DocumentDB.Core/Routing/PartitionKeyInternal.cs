// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Routing.PartitionKeyInternal
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.SharedFiles.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.Azure.Documents.Routing
{
  [JsonConverter(typeof (PartitionKeyInternalJsonConverter))]
  [SuppressMessage("", "AvoidMultiLineComments", Justification = "Multi line business logic")]
  internal sealed class PartitionKeyInternal : 
    IComparable<PartitionKeyInternal>,
    IEquatable<PartitionKeyInternal>,
    ICloneable
  {
    private readonly IReadOnlyList<IPartitionKeyComponent> components;
    private static readonly PartitionKeyInternal NonePartitionKey = new PartitionKeyInternal();
    private static readonly PartitionKeyInternal EmptyPartitionKey = new PartitionKeyInternal((IReadOnlyList<IPartitionKeyComponent>) new IPartitionKeyComponent[0]);
    private static readonly PartitionKeyInternal InfinityPartitionKey = new PartitionKeyInternal((IReadOnlyList<IPartitionKeyComponent>) new InfinityPartitionKeyComponent[1]
    {
      new InfinityPartitionKeyComponent()
    });
    private static readonly PartitionKeyInternal UndefinedPartitionKey = new PartitionKeyInternal((IReadOnlyList<IPartitionKeyComponent>) new UndefinedPartitionKeyComponent[1]
    {
      new UndefinedPartitionKeyComponent()
    });
    private const int MaxPartitionKeyBinarySize = 336;
    private static readonly Int128 MaxHashV2Value = new Int128(new byte[16]
    {
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      byte.MaxValue,
      (byte) 63
    });
    public static readonly string MinimumInclusiveEffectivePartitionKey = PartitionKeyInternal.ToHexEncodedBinaryString((IReadOnlyList<IPartitionKeyComponent>) new IPartitionKeyComponent[0]);
    public static readonly string MaximumExclusiveEffectivePartitionKey = PartitionKeyInternal.ToHexEncodedBinaryString((IReadOnlyList<IPartitionKeyComponent>) new InfinityPartitionKeyComponent[1]
    {
      new InfinityPartitionKeyComponent()
    });

    public static PartitionKeyInternal InclusiveMinimum => PartitionKeyInternal.EmptyPartitionKey;

    public static PartitionKeyInternal ExclusiveMaximum => PartitionKeyInternal.InfinityPartitionKey;

    public static PartitionKeyInternal Empty => PartitionKeyInternal.EmptyPartitionKey;

    public static PartitionKeyInternal None => PartitionKeyInternal.NonePartitionKey;

    public static PartitionKeyInternal Undefined => PartitionKeyInternal.UndefinedPartitionKey;

    public IReadOnlyList<IPartitionKeyComponent> Components => this.components;

    private PartitionKeyInternal() => this.components = (IReadOnlyList<IPartitionKeyComponent>) null;

    public PartitionKeyInternal(IReadOnlyList<IPartitionKeyComponent> values) => this.components = values != null ? values : throw new ArgumentNullException(nameof (values));

    public static PartitionKeyInternal FromObjectArray(IEnumerable<object> values, bool strict)
    {
      if (values == null)
        throw new ArgumentNullException(nameof (values));
      List<IPartitionKeyComponent> values1 = new List<IPartitionKeyComponent>();
      foreach (object obj in values)
      {
        switch (obj)
        {
          case null:
            values1.Add((IPartitionKeyComponent) NullPartitionKeyComponent.Value);
            continue;
          case Microsoft.Azure.Documents.Undefined _:
            values1.Add((IPartitionKeyComponent) UndefinedPartitionKeyComponent.Value);
            continue;
          case bool flag:
            values1.Add((IPartitionKeyComponent) new BoolPartitionKeyComponent(flag));
            continue;
          case string _:
            values1.Add((IPartitionKeyComponent) new StringPartitionKeyComponent((string) obj));
            continue;
          default:
            if (PartitionKeyInternal.IsNumeric(obj))
            {
              values1.Add((IPartitionKeyComponent) new NumberPartitionKeyComponent(Convert.ToDouble(obj, (IFormatProvider) CultureInfo.InvariantCulture)));
              continue;
            }
            switch (obj)
            {
              case MinNumber _:
                values1.Add((IPartitionKeyComponent) MinNumberPartitionKeyComponent.Value);
                continue;
              case MaxNumber _:
                values1.Add((IPartitionKeyComponent) MaxNumberPartitionKeyComponent.Value);
                continue;
              case MinString _:
                values1.Add((IPartitionKeyComponent) MinStringPartitionKeyComponent.Value);
                continue;
              case MaxString _:
                values1.Add((IPartitionKeyComponent) MaxStringPartitionKeyComponent.Value);
                continue;
              default:
                if (strict)
                  throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, RMResources.UnsupportedPartitionKeyComponentValue, obj));
                values1.Add((IPartitionKeyComponent) UndefinedPartitionKeyComponent.Value);
                continue;
            }
        }
      }
      return new PartitionKeyInternal((IReadOnlyList<IPartitionKeyComponent>) values1);
    }

    public object[] ToObjectArray() => this.Components.Select<IPartitionKeyComponent, object>((Func<IPartitionKeyComponent, object>) (component => component.ToObject())).ToArray<object>();

    public static PartitionKeyInternal FromJsonString(string partitionKey)
    {
      if (string.IsNullOrWhiteSpace(partitionKey))
        throw new JsonSerializationException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, RMResources.UnableToDeserializePartitionKeyValue, (object) partitionKey));
      JsonSerializerSettings settings = new JsonSerializerSettings()
      {
        DateParseHandling = DateParseHandling.None
      };
      return JsonConvert.DeserializeObject<PartitionKeyInternal>(partitionKey, settings);
    }

    public string ToJsonString() => JsonConvert.SerializeObject((object) this, new JsonSerializerSettings()
    {
      StringEscapeHandling = StringEscapeHandling.EscapeNonAscii,
      Formatting = Formatting.None
    });

    public bool Contains(PartitionKeyInternal nestedPartitionKey)
    {
      if (this.Components.Count > nestedPartitionKey.Components.Count)
        return false;
      for (int index = 0; index < this.Components.Count; ++index)
      {
        if (this.Components[index].CompareTo(nestedPartitionKey.Components[index]) != 0)
          return false;
      }
      return true;
    }

    public static PartitionKeyInternal Max(PartitionKeyInternal key1, PartitionKeyInternal key2) => key1 == null || key2 != null && key1.CompareTo(key2) < 0 ? key2 : key1;

    public static PartitionKeyInternal Min(PartitionKeyInternal key1, PartitionKeyInternal key2) => key1 == null || key2 != null && key1.CompareTo(key2) > 0 ? key2 : key1;

    public static string GetMinInclusiveEffectivePartitionKey(
      int partitionIndex,
      int partitionCount,
      PartitionKeyDefinition partitionKeyDefinition,
      bool useHashV2asDefault = false)
    {
      if (partitionKeyDefinition.Paths.Count > 0 && partitionKeyDefinition.Kind != PartitionKind.Hash)
        throw new NotImplementedException("Cannot figure out range boundaries");
      if (partitionCount <= 0)
        throw new ArgumentException("Invalid partition count", nameof (partitionCount));
      if (partitionIndex < 0 || partitionIndex >= partitionCount)
        throw new ArgumentException("Invalid partition index", nameof (partitionIndex));
      if (partitionIndex == 0)
        return PartitionKeyInternal.MinimumInclusiveEffectivePartitionKey;
      PartitionKeyDefinitionVersion definitionVersion = useHashV2asDefault ? PartitionKeyDefinitionVersion.V2 : PartitionKeyDefinitionVersion.V1;
      switch ((PartitionKeyDefinitionVersion) ((int) partitionKeyDefinition.Version ?? (int) definitionVersion))
      {
        case PartitionKeyDefinitionVersion.V1:
          return PartitionKeyInternal.ToHexEncodedBinaryString((IReadOnlyList<IPartitionKeyComponent>) new IPartitionKeyComponent[1]
          {
            (IPartitionKeyComponent) new NumberPartitionKeyComponent((double) ((long) uint.MaxValue / (long) partitionCount * (long) partitionIndex))
          });
        case PartitionKeyDefinitionVersion.V2:
          byte[] bytes = (PartitionKeyInternal.MaxHashV2Value / (Int128) partitionCount * (Int128) partitionIndex).Bytes;
          Array.Reverse((Array) bytes);
          return PartitionKeyInternal.HexConvert.ToHex(bytes, 0, bytes.Length);
        default:
          throw new InternalServerErrorException("Unexpected PartitionKeyDefinitionVersion");
      }
    }

    public static string GetMaxExclusiveEffectivePartitionKey(
      int partitionIndex,
      int partitionCount,
      PartitionKeyDefinition partitionKeyDefinition,
      bool useHashV2asDefault = false)
    {
      if (partitionKeyDefinition.Paths.Count > 0 && partitionKeyDefinition.Kind != PartitionKind.Hash)
        throw new NotImplementedException("Cannot figure out range boundaries");
      if (partitionCount <= 0)
        throw new ArgumentException("Invalid partition count", nameof (partitionCount));
      if (partitionIndex < 0 || partitionIndex >= partitionCount)
        throw new ArgumentException("Invalid partition index", nameof (partitionIndex));
      if (partitionIndex == partitionCount - 1)
        return PartitionKeyInternal.MaximumExclusiveEffectivePartitionKey;
      PartitionKeyDefinitionVersion definitionVersion = useHashV2asDefault ? PartitionKeyDefinitionVersion.V2 : PartitionKeyDefinitionVersion.V1;
      switch ((PartitionKeyDefinitionVersion) ((int) partitionKeyDefinition.Version ?? (int) definitionVersion))
      {
        case PartitionKeyDefinitionVersion.V1:
          return PartitionKeyInternal.ToHexEncodedBinaryString((IReadOnlyList<IPartitionKeyComponent>) new IPartitionKeyComponent[1]
          {
            (IPartitionKeyComponent) new NumberPartitionKeyComponent((double) ((long) uint.MaxValue / (long) partitionCount * (long) (partitionIndex + 1)))
          });
        case PartitionKeyDefinitionVersion.V2:
          byte[] bytes = (PartitionKeyInternal.MaxHashV2Value / (Int128) partitionCount * (Int128) (partitionIndex + 1)).Bytes;
          Array.Reverse((Array) bytes);
          return PartitionKeyInternal.HexConvert.ToHex(bytes, 0, bytes.Length);
        default:
          throw new InternalServerErrorException("Unexpected PartitionKeyDefinitionVersion");
      }
    }

    public int CompareTo(PartitionKeyInternal other)
    {
      if (other == null)
        throw new ArgumentNullException(nameof (other));
      if (other.components == null || this.components == null)
      {
        IReadOnlyList<IPartitionKeyComponent> components1 = this.components;
        int count1 = components1 != null ? components1.Count : 0;
        IReadOnlyList<IPartitionKeyComponent> components2 = other.components;
        int count2 = components2 != null ? components2.Count : 0;
        return Math.Sign(count1 - count2);
      }
      for (int index = 0; index < Math.Min(this.Components.Count, other.Components.Count); ++index)
      {
        int typeOrdinal1 = this.Components[index].GetTypeOrdinal();
        int typeOrdinal2 = other.Components[index].GetTypeOrdinal();
        if (typeOrdinal1 != typeOrdinal2)
          return Math.Sign(typeOrdinal1 - typeOrdinal2);
        int num = this.Components[index].CompareTo(other.Components[index]);
        if (num != 0)
          return Math.Sign(num);
      }
      return Math.Sign(this.Components.Count - other.Components.Count);
    }

    public bool Equals(PartitionKeyInternal other)
    {
      if (other == null)
        return false;
      return this == other || this.CompareTo(other) == 0;
    }

    public override bool Equals(object other) => this.Equals(other as PartitionKeyInternal);

    public override int GetHashCode() => this.Components.Aggregate<IPartitionKeyComponent, int>(0, (Func<int, IPartitionKeyComponent, int>) ((current, value) => current * 397 ^ value.GetHashCode()));

    public override string ToString() => JsonConvert.SerializeObject((object) this);

    public object Clone() => (object) new PartitionKeyInternal(this.Components);

    private static string ToHexEncodedBinaryString(IReadOnlyList<IPartitionKeyComponent> components)
    {
      byte[] numArray = new byte[336];
      using (MemoryStream output = new MemoryStream(numArray))
      {
        using (BinaryWriter binaryWriter = new BinaryWriter((Stream) output))
        {
          for (int index = 0; index < components.Count; ++index)
            components[index].WriteForBinaryEncoding(binaryWriter);
          return PartitionKeyInternal.HexConvert.ToHex(numArray, 0, (int) output.Position);
        }
      }
    }

    [Obsolete]
    internal static PartitionKeyInternal FromHexEncodedBinaryString(string hexEncodedBinaryString)
    {
      List<IPartitionKeyComponent> values = new List<IPartitionKeyComponent>();
      byte[] byteArray = PartitionKeyInternal.HexStringToByteArray(hexEncodedBinaryString);
      int num = 0;
      while (num < byteArray.Length)
      {
        switch ((PartitionKeyComponentType) Enum.Parse(typeof (PartitionKeyComponentType), byteArray[num++].ToString((IFormatProvider) CultureInfo.InvariantCulture)))
        {
          case PartitionKeyComponentType.Undefined:
            values.Add((IPartitionKeyComponent) UndefinedPartitionKeyComponent.Value);
            continue;
          case PartitionKeyComponentType.Null:
            values.Add((IPartitionKeyComponent) NullPartitionKeyComponent.Value);
            continue;
          case PartitionKeyComponentType.False:
            values.Add((IPartitionKeyComponent) new BoolPartitionKeyComponent(false));
            continue;
          case PartitionKeyComponentType.True:
            values.Add((IPartitionKeyComponent) new BoolPartitionKeyComponent(true));
            continue;
          case PartitionKeyComponentType.MinNumber:
            values.Add((IPartitionKeyComponent) MinNumberPartitionKeyComponent.Value);
            continue;
          case PartitionKeyComponentType.Number:
            values.Add(NumberPartitionKeyComponent.FromHexEncodedBinaryString(byteArray, ref num));
            continue;
          case PartitionKeyComponentType.MaxNumber:
            values.Add((IPartitionKeyComponent) MaxNumberPartitionKeyComponent.Value);
            continue;
          case PartitionKeyComponentType.MinString:
            values.Add((IPartitionKeyComponent) MinStringPartitionKeyComponent.Value);
            continue;
          case PartitionKeyComponentType.String:
            values.Add(StringPartitionKeyComponent.FromHexEncodedBinaryString(byteArray, ref num));
            continue;
          case PartitionKeyComponentType.MaxString:
            values.Add((IPartitionKeyComponent) MaxStringPartitionKeyComponent.Value);
            continue;
          case PartitionKeyComponentType.Infinity:
            values.Add((IPartitionKeyComponent) new InfinityPartitionKeyComponent());
            continue;
          default:
            continue;
        }
      }
      return new PartitionKeyInternal((IReadOnlyList<IPartitionKeyComponent>) values);
    }

    public string GetEffectivePartitionKeyString(
      PartitionKeyDefinition partitionKeyDefinition,
      bool strict = true)
    {
      if (this.components == null)
        throw new ArgumentException(RMResources.TooFewPartitionKeyComponents);
      if (this.Equals(PartitionKeyInternal.EmptyPartitionKey))
        return PartitionKeyInternal.MinimumInclusiveEffectivePartitionKey;
      if (this.Equals(PartitionKeyInternal.InfinityPartitionKey))
        return PartitionKeyInternal.MaximumExclusiveEffectivePartitionKey;
      if (this.Components.Count < partitionKeyDefinition.Paths.Count)
        throw new ArgumentException(RMResources.TooFewPartitionKeyComponents);
      if (this.Components.Count > partitionKeyDefinition.Paths.Count & strict)
        throw new ArgumentException(RMResources.TooManyPartitionKeyComponents);
      if (partitionKeyDefinition.Kind != PartitionKind.Hash)
        return PartitionKeyInternal.ToHexEncodedBinaryString(this.Components);
      switch ((PartitionKeyDefinitionVersion) ((int) partitionKeyDefinition.Version ?? 1))
      {
        case PartitionKeyDefinitionVersion.V1:
          return this.GetEffectivePartitionKeyForHashPartitioning();
        case PartitionKeyDefinitionVersion.V2:
          return this.GetEffectivePartitionKeyForHashPartitioningV2();
        default:
          throw new InternalServerErrorException("Unexpected PartitionKeyDefinitionVersion");
      }
    }

    private string GetEffectivePartitionKeyForHashPartitioning()
    {
      IPartitionKeyComponent[] array = this.Components.ToArray<IPartitionKeyComponent>();
      for (int index = 0; index < array.Length; ++index)
        array[index] = this.Components[index].Truncate();
      double num;
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (BinaryWriter binaryWriter = new BinaryWriter((Stream) memoryStream))
        {
          for (int index = 0; index < array.Length; ++index)
            array[index].WriteForHashing(binaryWriter);
          num = (double) MurmurHash3.Hash32(CustomTypeExtensions.GetBuffer(memoryStream), memoryStream.Length);
        }
      }
      IPartitionKeyComponent[] components = new IPartitionKeyComponent[this.Components.Count + 1];
      components[0] = (IPartitionKeyComponent) new NumberPartitionKeyComponent(num);
      for (int index = 0; index < array.Length; ++index)
        components[index + 1] = array[index];
      return PartitionKeyInternal.ToHexEncodedBinaryString((IReadOnlyList<IPartitionKeyComponent>) components);
    }

    private string GetEffectivePartitionKeyForHashPartitioningV2()
    {
      byte[] bytes = (byte[]) null;
      using (MemoryStream memoryStream = new MemoryStream())
      {
        using (BinaryWriter binaryWriter = new BinaryWriter((Stream) memoryStream))
        {
          for (int index = 0; index < this.Components.Count; ++index)
            this.Components[index].WriteForHashingV2(binaryWriter);
          bytes = UInt128.ToByteArray(MurmurHash3.Hash128(CustomTypeExtensions.GetBuffer(memoryStream), (int) memoryStream.Length, UInt128.MinValue));
          Array.Reverse((Array) bytes);
          bytes[0] &= (byte) 63;
        }
      }
      return PartitionKeyInternal.HexConvert.ToHex(bytes, 0, bytes.Length);
    }

    private static bool IsNumeric(object value)
    {
      switch (value)
      {
        case sbyte _:
        case byte _:
        case short _:
        case ushort _:
        case int _:
        case uint _:
        case long _:
        case ulong _:
        case float _:
        case double _:
          return true;
        default:
          return value is Decimal;
      }
    }

    private static byte[] HexStringToByteArray(string hex)
    {
      int length = hex.Length;
      byte[] byteArray = length % 2 == 0 ? new byte[length / 2] : throw new ArgumentException("Hex string should be even length", nameof (hex));
      for (int startIndex = 0; startIndex < length; startIndex += 2)
        byteArray[startIndex / 2] = Convert.ToByte(hex.Substring(startIndex, 2), 16);
      return byteArray;
    }

    public static string GetMiddleRangeEffectivePartitionKey(
      string minInclusive,
      string maxExclusive,
      PartitionKeyDefinition partitionKeyDefinition)
    {
      if (partitionKeyDefinition.Kind != PartitionKind.Hash)
        throw new InvalidOperationException("Can determine middle of range only for hash partitioning.");
      switch ((PartitionKeyDefinitionVersion) ((int) partitionKeyDefinition.Version ?? 1))
      {
        case PartitionKeyDefinitionVersion.V1:
          long num = 0;
          long maxValue = (long) uint.MaxValue;
          if (!minInclusive.Equals(PartitionKeyInternal.MinimumInclusiveEffectivePartitionKey, StringComparison.Ordinal))
            num = (long) ((NumberPartitionKeyComponent) PartitionKeyInternal.FromHexEncodedBinaryString(minInclusive).Components[0]).Value;
          if (!maxExclusive.Equals(PartitionKeyInternal.MaximumExclusiveEffectivePartitionKey, StringComparison.Ordinal))
            maxValue = (long) ((NumberPartitionKeyComponent) PartitionKeyInternal.FromHexEncodedBinaryString(maxExclusive).Components[0]).Value;
          return PartitionKeyInternal.ToHexEncodedBinaryString((IReadOnlyList<IPartitionKeyComponent>) new NumberPartitionKeyComponent[1]
          {
            new NumberPartitionKeyComponent((double) ((num + maxValue) / 2L))
          });
        case PartitionKeyDefinitionVersion.V2:
          Int128 int128_1 = (Int128) 0;
          if (!minInclusive.Equals(PartitionKeyInternal.MinimumInclusiveEffectivePartitionKey, StringComparison.Ordinal))
          {
            byte[] byteArray = PartitionKeyInternal.HexStringToByteArray(minInclusive);
            Array.Reverse((Array) byteArray);
            int128_1 = new Int128(byteArray);
          }
          Int128 int128_2 = PartitionKeyInternal.MaxHashV2Value;
          if (!maxExclusive.Equals(PartitionKeyInternal.MaximumExclusiveEffectivePartitionKey, StringComparison.Ordinal))
          {
            byte[] byteArray = PartitionKeyInternal.HexStringToByteArray(maxExclusive);
            Array.Reverse((Array) byteArray);
            int128_2 = new Int128(byteArray);
          }
          byte[] bytes = (int128_1 + (int128_2 - int128_1) / (Int128) 2).Bytes;
          Array.Reverse((Array) bytes);
          return PartitionKeyInternal.HexConvert.ToHex(bytes, 0, bytes.Length);
        default:
          throw new InternalServerErrorException("Unexpected PartitionKeyDefinitionVersion");
      }
    }

    public static string[] GetNEqualRangeEffectivePartitionKeys(
      string minInclusive,
      string maxExclusive,
      PartitionKeyDefinition partitionKeyDefinition,
      int numberOfSubRanges)
    {
      if (partitionKeyDefinition.Kind != PartitionKind.Hash)
        throw new InvalidOperationException("Can determine " + (object) numberOfSubRanges + " ranges only for hash partitioning.");
      if (numberOfSubRanges <= 0)
        throw new InvalidOperationException("Number of sub ranges " + (object) numberOfSubRanges + " cannot be zero or negative");
      switch ((PartitionKeyDefinitionVersion) ((int) partitionKeyDefinition.Version ?? 1))
      {
        case PartitionKeyDefinitionVersion.V1:
          long num = 0;
          long maxValue = (long) uint.MaxValue;
          if (!minInclusive.Equals(PartitionKeyInternal.MinimumInclusiveEffectivePartitionKey, StringComparison.Ordinal))
            num = (long) ((NumberPartitionKeyComponent) PartitionKeyInternal.FromHexEncodedBinaryString(minInclusive).Components[0]).Value;
          if (!maxExclusive.Equals(PartitionKeyInternal.MaximumExclusiveEffectivePartitionKey, StringComparison.Ordinal))
            maxValue = (long) ((NumberPartitionKeyComponent) PartitionKeyInternal.FromHexEncodedBinaryString(maxExclusive).Components[0]).Value;
          if (maxValue - num < (long) numberOfSubRanges)
            throw new InvalidOperationException("Insufficient range width to produce " + (object) numberOfSubRanges + " equal sub ranges.");
          string[] effectivePartitionKeys1 = new string[numberOfSubRanges - 1];
          for (int index = 1; index < numberOfSubRanges; ++index)
            effectivePartitionKeys1[index - 1] = PartitionKeyInternal.ToHexEncodedBinaryString((IReadOnlyList<IPartitionKeyComponent>) new NumberPartitionKeyComponent[1]
            {
              new NumberPartitionKeyComponent((double) (num + (long) index * ((maxValue - num) / (long) numberOfSubRanges)))
            });
          return effectivePartitionKeys1;
        case PartitionKeyDefinitionVersion.V2:
          Int128 int128_1 = (Int128) 0;
          if (!minInclusive.Equals(PartitionKeyInternal.MinimumInclusiveEffectivePartitionKey, StringComparison.Ordinal))
          {
            byte[] byteArray = PartitionKeyInternal.HexStringToByteArray(minInclusive);
            Array.Reverse((Array) byteArray);
            int128_1 = new Int128(byteArray);
          }
          Int128 int128_2 = PartitionKeyInternal.MaxHashV2Value;
          if (!maxExclusive.Equals(PartitionKeyInternal.MaximumExclusiveEffectivePartitionKey, StringComparison.Ordinal))
          {
            byte[] byteArray = PartitionKeyInternal.HexStringToByteArray(maxExclusive);
            Array.Reverse((Array) byteArray);
            int128_2 = new Int128(byteArray);
          }
          if (int128_2 - int128_1 < (Int128) numberOfSubRanges)
            throw new InvalidOperationException("Insufficient range width to produce " + (object) numberOfSubRanges + " equal sub ranges.");
          string[] effectivePartitionKeys2 = new string[numberOfSubRanges - 1];
          for (int index = 1; index < numberOfSubRanges; ++index)
          {
            byte[] bytes = (int128_1 + (Int128) index * ((int128_2 - int128_1) / (Int128) numberOfSubRanges)).Bytes;
            Array.Reverse((Array) bytes);
            effectivePartitionKeys2[index - 1] = PartitionKeyInternal.HexConvert.ToHex(bytes, 0, bytes.Length);
          }
          return effectivePartitionKeys2;
        default:
          throw new InternalServerErrorException("Unexpected PartitionKeyDefinitionVersion");
      }
    }

    public static double GetWidth(
      string minInclusive,
      string maxExclusive,
      PartitionKeyDefinition partitionKeyDefinition)
    {
      if (partitionKeyDefinition.Kind != PartitionKind.Hash)
        throw new InvalidOperationException("Can determine range width only for hash partitioning.");
      switch ((PartitionKeyDefinitionVersion) ((int) partitionKeyDefinition.Version ?? 1))
      {
        case PartitionKeyDefinitionVersion.V1:
          long num = 0;
          long maxValue = (long) uint.MaxValue;
          if (!minInclusive.Equals(PartitionKeyInternal.MinimumInclusiveEffectivePartitionKey, StringComparison.Ordinal))
            num = (long) ((NumberPartitionKeyComponent) PartitionKeyInternal.FromHexEncodedBinaryString(minInclusive).Components[0]).Value;
          if (!maxExclusive.Equals(PartitionKeyInternal.MaximumExclusiveEffectivePartitionKey, StringComparison.Ordinal))
            maxValue = (long) ((NumberPartitionKeyComponent) PartitionKeyInternal.FromHexEncodedBinaryString(maxExclusive).Components[0]).Value;
          return 1.0 * (double) (maxValue - num) / 4294967296.0;
        case PartitionKeyDefinitionVersion.V2:
          UInt128 uint128_1 = (UInt128) 0;
          if (!minInclusive.Equals(PartitionKeyInternal.MinimumInclusiveEffectivePartitionKey, StringComparison.Ordinal))
          {
            byte[] byteArray = PartitionKeyInternal.HexStringToByteArray(minInclusive);
            Array.Reverse((Array) byteArray);
            uint128_1 = UInt128.FromByteArray(byteArray);
          }
          UInt128 uint128_2 = UInt128.MaxValue;
          if (!maxExclusive.Equals(PartitionKeyInternal.MaximumExclusiveEffectivePartitionKey, StringComparison.Ordinal))
          {
            byte[] byteArray = PartitionKeyInternal.HexStringToByteArray(maxExclusive);
            Array.Reverse((Array) byteArray);
            uint128_2 = UInt128.FromByteArray(byteArray);
          }
          return 1.0 * (double) (uint128_2.GetHigh() - uint128_1.GetHigh()) / (double) (UInt128.MaxValue.GetHigh() + 1UL);
        default:
          throw new InternalServerErrorException("Unexpected PartitionKeyDefinitionVersion");
      }
    }

    internal static class HexConvert
    {
      private static readonly ushort[] LookupTable = PartitionKeyInternal.HexConvert.CreateLookupTable();

      private static ushort[] CreateLookupTable()
      {
        ushort[] lookupTable = new ushort[256];
        for (int index = 0; index < 256; ++index)
        {
          string str = index.ToString("X2", (IFormatProvider) CultureInfo.InvariantCulture);
          lookupTable[index] = (ushort) ((uint) str[0] + ((uint) str[1] << 8));
        }
        return lookupTable;
      }

      public static string ToHex(byte[] bytes, int start, int length)
      {
        char[] chArray = new char[length * 2];
        for (int index = 0; index < length; ++index)
        {
          ushort num = PartitionKeyInternal.HexConvert.LookupTable[(int) bytes[index + start]];
          chArray[2 * index] = (char) ((uint) num & (uint) byte.MaxValue);
          chArray[2 * index + 1] = (char) ((uint) num >> 8);
        }
        return new string(chArray);
      }
    }
  }
}
