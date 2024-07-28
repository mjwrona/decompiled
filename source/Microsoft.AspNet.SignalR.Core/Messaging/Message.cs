// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.SignalR.Messaging.Message
// Assembly: Microsoft.AspNet.SignalR.Core, Version=2.4.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 095D5FBC-6474-494D-BE26-4EBE2B9AD3D0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.AspNet.SignalR.Core.dll

using System;
using System.IO;
using System.Text;

namespace Microsoft.AspNet.SignalR.Messaging
{
  public class Message
  {
    private static readonly byte[] _zeroByteBuffer = new byte[0];
    private static readonly UTF8Encoding _encoding = new UTF8Encoding();

    public Message() => this.Encoding = (Encoding) Message._encoding;

    public Message(string source, string key, string value)
    {
      if (source == null)
        throw new ArgumentNullException(nameof (source));
      if (key == null)
        throw new ArgumentNullException(nameof (key));
      this.Source = source;
      this.Key = key;
      this.Encoding = (Encoding) Message._encoding;
      this.Value = value == null ? new ArraySegment<byte>(Message._zeroByteBuffer) : new ArraySegment<byte>(this.Encoding.GetBytes(value));
    }

    public Message(string source, string key, ArraySegment<byte> value)
      : this()
    {
      if (source == null)
        throw new ArgumentNullException(nameof (source));
      if (key == null)
        throw new ArgumentNullException(nameof (key));
      this.Source = source;
      this.Key = key;
      this.Value = value;
    }

    public string Source { get; set; }

    public string Key { get; set; }

    public ArraySegment<byte> Value { get; set; }

    public string CommandId { get; set; }

    public bool WaitForAck { get; set; }

    public bool IsAck { get; set; }

    public string Filter { get; set; }

    public Encoding Encoding { get; private set; }

    public ulong MappingId { get; set; }

    public int StreamIndex { get; set; }

    public bool IsCommand => !string.IsNullOrEmpty(this.CommandId);

    public string GetString()
    {
      Encoding encoding = this.Encoding != null ? this.Encoding : throw new NotSupportedException();
      ArraySegment<byte> arraySegment = this.Value;
      byte[] array = arraySegment.Array;
      arraySegment = this.Value;
      int offset = arraySegment.Offset;
      arraySegment = this.Value;
      int count = arraySegment.Count;
      return encoding.GetString(array, offset, count);
    }

    public void WriteTo(Stream stream)
    {
      BinaryWriter binaryWriter = new BinaryWriter(stream);
      binaryWriter.Write(this.Source);
      binaryWriter.Write(this.Key);
      binaryWriter.Write(this.Value.Count);
      ArraySegment<byte> arraySegment = this.Value;
      byte[] array = arraySegment.Array;
      arraySegment = this.Value;
      int offset = arraySegment.Offset;
      arraySegment = this.Value;
      int count = arraySegment.Count;
      binaryWriter.Write(array, offset, count);
      binaryWriter.Write(this.CommandId ?? string.Empty);
      binaryWriter.Write(this.WaitForAck);
      binaryWriter.Write(this.IsAck);
      binaryWriter.Write(this.Filter ?? string.Empty);
    }

    public static Message ReadFrom(Stream stream)
    {
      Message message = new Message();
      BinaryReader binaryReader = new BinaryReader(stream);
      message.Source = binaryReader.ReadString();
      message.Key = binaryReader.ReadString();
      int count = binaryReader.ReadInt32();
      message.Value = new ArraySegment<byte>(binaryReader.ReadBytes(count));
      message.CommandId = binaryReader.ReadString();
      message.WaitForAck = binaryReader.ReadBoolean();
      message.IsAck = binaryReader.ReadBoolean();
      message.Filter = binaryReader.ReadString();
      return message;
    }
  }
}
