// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Routing.StringPartitionKeyComponent
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace Microsoft.Azure.Documents.Routing
{
  internal sealed class StringPartitionKeyComponent : IPartitionKeyComponent
  {
    public const int MaxStringChars = 100;
    public const int MaxStringBytesToAppend = 100;
    private readonly string value;
    private readonly byte[] utf8Value;

    public StringPartitionKeyComponent(string value)
    {
      this.value = value != null ? value : throw new ArgumentNullException(nameof (value));
      this.utf8Value = Encoding.UTF8.GetBytes(value);
    }

    public void JsonEncode(JsonWriter writer) => writer.WriteValue(this.value);

    public object ToObject() => (object) this.value;

    public int CompareTo(IPartitionKeyComponent other) => other is StringPartitionKeyComponent partitionKeyComponent ? string.CompareOrdinal(this.value, partitionKeyComponent.value) : throw new ArgumentException(nameof (other));

    public int GetTypeOrdinal() => 8;

    public override int GetHashCode() => this.value.GetHashCode();

    public IPartitionKeyComponent Truncate() => this.value.Length > 100 ? (IPartitionKeyComponent) new StringPartitionKeyComponent(this.value.Substring(0, 100)) : (IPartitionKeyComponent) this;

    public void WriteForHashing(BinaryWriter writer)
    {
      writer.Write((byte) 8);
      writer.Write(this.utf8Value);
      writer.Write((byte) 0);
    }

    public void WriteForHashingV2(BinaryWriter writer)
    {
      writer.Write((byte) 8);
      writer.Write(this.utf8Value);
      writer.Write(byte.MaxValue);
    }

    public void WriteForBinaryEncoding(BinaryWriter binaryWriter)
    {
      binaryWriter.Write((byte) 8);
      bool flag = this.utf8Value.Length <= 100;
      for (int index = 0; index < (flag ? this.utf8Value.Length : 101); ++index)
      {
        byte num = this.utf8Value[index];
        if (num < byte.MaxValue)
          ++num;
        binaryWriter.Write(num);
      }
      if (!flag)
        return;
      binaryWriter.Write((byte) 0);
    }

    public static IPartitionKeyComponent FromHexEncodedBinaryString(
      byte[] byteString,
      ref int offset)
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index <= 100; ++index)
      {
        byte num = byteString[offset++];
        if (num != (byte) 0)
        {
          char ch = (char) ((uint) num - 1U);
          stringBuilder.Append(ch);
        }
        else
          break;
      }
      return (IPartitionKeyComponent) new StringPartitionKeyComponent(stringBuilder.ToString());
    }
  }
}
