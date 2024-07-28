// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.RntbdToken
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System.IO;

namespace Microsoft.Azure.Documents
{
  internal sealed class RntbdToken
  {
    private ushort identifier;
    private RntbdTokenTypes type;
    private bool isRequired;
    public bool isPresent;
    public RntbdTokenValue value;

    public RntbdToken(bool isRequired, RntbdTokenTypes type, ushort identifier)
    {
      this.isRequired = isRequired;
      this.isPresent = false;
      this.type = type;
      this.identifier = identifier;
      this.value = new RntbdTokenValue();
    }

    public RntbdTokenTypes GetTokenType() => this.type;

    public ushort GetTokenIdentifier() => this.identifier;

    public bool IsRequired() => this.isRequired;

    public void SerializeToBinaryWriter(BinaryWriter writer, out int written)
    {
      if (!this.isPresent && this.isRequired)
        throw new BadRequestException();
      if (this.isPresent)
      {
        writer.Write(this.identifier);
        writer.Write((byte) this.type);
        switch (this.type)
        {
          case RntbdTokenTypes.Byte:
            writer.Write(this.value.valueByte);
            written = 4;
            break;
          case RntbdTokenTypes.UShort:
            writer.Write(this.value.valueUShort);
            written = 5;
            break;
          case RntbdTokenTypes.ULong:
            writer.Write(this.value.valueULong);
            written = 7;
            break;
          case RntbdTokenTypes.Long:
            writer.Write(this.value.valueLong);
            written = 7;
            break;
          case RntbdTokenTypes.ULongLong:
            writer.Write(this.value.valueULongLong);
            written = 11;
            break;
          case RntbdTokenTypes.LongLong:
            writer.Write(this.value.valueLongLong);
            written = 11;
            break;
          case RntbdTokenTypes.Guid:
            byte[] byteArray = this.value.valueGuid.ToByteArray();
            writer.Write(byteArray);
            written = 3 + byteArray.Length;
            break;
          case RntbdTokenTypes.SmallString:
          case RntbdTokenTypes.SmallBytes:
            if (this.value.valueBytes.Length > (int) byte.MaxValue)
              throw new RequestEntityTooLargeException();
            writer.Write((byte) this.value.valueBytes.Length);
            writer.Write(this.value.valueBytes);
            written = 4 + this.value.valueBytes.Length;
            break;
          case RntbdTokenTypes.String:
          case RntbdTokenTypes.Bytes:
            if (this.value.valueBytes.Length > (int) ushort.MaxValue)
              throw new RequestEntityTooLargeException();
            writer.Write((ushort) this.value.valueBytes.Length);
            writer.Write(this.value.valueBytes);
            written = 5 + this.value.valueBytes.Length;
            break;
          case RntbdTokenTypes.ULongString:
          case RntbdTokenTypes.ULongBytes:
            writer.Write((uint) this.value.valueBytes.Length);
            writer.Write(this.value.valueBytes);
            written = 7 + this.value.valueBytes.Length;
            break;
          case RntbdTokenTypes.Float:
            writer.Write(this.value.valueFloat);
            written = 7;
            break;
          case RntbdTokenTypes.Double:
            writer.Write(this.value.valueDouble);
            written = 11;
            break;
          default:
            throw new BadRequestException();
        }
      }
      else
        written = 0;
    }
  }
}
