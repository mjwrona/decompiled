// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.RntbdTokenStream`1
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Cosmos.Rntbd;
using Microsoft.Azure.Documents.Collections;
using System;
using System.Buffers;
using System.Collections.Generic;

namespace Microsoft.Azure.Documents
{
  internal abstract class RntbdTokenStream<T> where T : Enum
  {
    private static Dictionary<ushort, int> TokenPositionMap;
    internal RntbdToken[] tokens;
    private ArrayPool<byte> arrayPool = ArrayPool<byte>.Create();
    private List<byte[]> borrowedBytes = new List<byte[]>();

    protected void SetTokens(RntbdToken[] t)
    {
      this.tokens = t;
      if (RntbdTokenStream<T>.TokenPositionMap != null)
        return;
      RntbdTokenStream<T>.TokenPositionMap = RntbdTokenStream<T>.GetTokenPositions(t);
    }

    public byte[] GetBytes(int length)
    {
      byte[] bytes = this.arrayPool.Rent(length);
      this.borrowedBytes.Add(bytes);
      return bytes;
    }

    public void Reset()
    {
      for (int index = 0; index < this.tokens.Length; ++index)
      {
        this.tokens[index].isPresent = false;
        this.tokens[index].value.valueBytes = new ReadOnlyMemory<byte>();
      }
      foreach (byte[] borrowedByte in this.borrowedBytes)
        this.arrayPool.Return(borrowedByte);
      this.borrowedBytes.Clear();
    }

    private static Dictionary<ushort, int> GetTokenPositions(RntbdToken[] t)
    {
      Dictionary<ushort, int> tokenPositions = new Dictionary<ushort, int>(t.Length);
      for (int index = 0; index < t.Length; ++index)
        tokenPositions[t[index].GetTokenIdentifier()] = index;
      return tokenPositions;
    }

    public int CalculateLength()
    {
      int length = 0;
      foreach (RntbdToken token in this.tokens)
      {
        if (token.isPresent)
        {
          int num = length + 1 + 2;
          switch (token.GetTokenType())
          {
            case RntbdTokenTypes.Byte:
              length = num + 1;
              continue;
            case RntbdTokenTypes.UShort:
              length = num + 2;
              continue;
            case RntbdTokenTypes.ULong:
            case RntbdTokenTypes.Long:
              length = num + 4;
              continue;
            case RntbdTokenTypes.ULongLong:
            case RntbdTokenTypes.LongLong:
              length = num + 8;
              continue;
            case RntbdTokenTypes.Guid:
              length = num + 16;
              continue;
            case RntbdTokenTypes.SmallString:
            case RntbdTokenTypes.SmallBytes:
              length = num + 1 + token.value.valueBytes.Length;
              continue;
            case RntbdTokenTypes.String:
            case RntbdTokenTypes.Bytes:
              length = num + 2 + token.value.valueBytes.Length;
              continue;
            case RntbdTokenTypes.ULongString:
            case RntbdTokenTypes.ULongBytes:
              length = num + 4 + token.value.valueBytes.Length;
              continue;
            case RntbdTokenTypes.Float:
              length = num + 4;
              continue;
            case RntbdTokenTypes.Double:
              length = num + 8;
              continue;
            default:
              throw new BadRequestException();
          }
        }
      }
      return length;
    }

    public void SerializeToBinaryWriter(ref BytesSerializer writer, out int tokensLength)
    {
      tokensLength = 0;
      foreach (RntbdToken token in this.tokens)
      {
        int num = 0;
        ref BytesSerializer local1 = ref writer;
        ref int local2 = ref num;
        token.SerializeToBinaryWriter(ref local1, out local2);
        tokensLength += num;
      }
    }

    public void ParseFrom(ref BytesDeserializer reader)
    {
      while (reader.Position < reader.Length)
      {
        ushort num = reader.ReadUInt16();
        RntbdTokenTypes type = (RntbdTokenTypes) reader.ReadByte();
        int index;
        RntbdToken rntbdToken = !RntbdTokenStream<T>.TokenPositionMap.TryGetValue(num, out index) ? new RntbdToken(false, type, num) : this.tokens[index];
        if (rntbdToken.isPresent)
        {
          DefaultTrace.TraceError("Duplicate token with identifier {0} type {1} found in RNTBD token stream", (object) rntbdToken.GetTokenIdentifier(), (object) rntbdToken.GetTokenType());
          throw new InternalServerErrorException(RMResources.InternalServerError, this.GetValidationFailureHeader());
        }
        switch (rntbdToken.GetTokenType())
        {
          case RntbdTokenTypes.Byte:
            rntbdToken.value.valueByte = reader.ReadByte();
            break;
          case RntbdTokenTypes.UShort:
            rntbdToken.value.valueUShort = reader.ReadUInt16();
            break;
          case RntbdTokenTypes.ULong:
            rntbdToken.value.valueULong = reader.ReadUInt32();
            break;
          case RntbdTokenTypes.Long:
            rntbdToken.value.valueLong = reader.ReadInt32();
            break;
          case RntbdTokenTypes.ULongLong:
            rntbdToken.value.valueULongLong = reader.ReadUInt64();
            break;
          case RntbdTokenTypes.LongLong:
            rntbdToken.value.valueLongLong = reader.ReadInt64();
            break;
          case RntbdTokenTypes.Guid:
            rntbdToken.value.valueGuid = reader.ReadGuid();
            break;
          case RntbdTokenTypes.SmallString:
          case RntbdTokenTypes.SmallBytes:
            byte length1 = reader.ReadByte();
            rntbdToken.value.valueBytes = reader.ReadBytes((int) length1);
            break;
          case RntbdTokenTypes.String:
          case RntbdTokenTypes.Bytes:
            ushort length2 = reader.ReadUInt16();
            rntbdToken.value.valueBytes = reader.ReadBytes((int) length2);
            break;
          case RntbdTokenTypes.ULongString:
          case RntbdTokenTypes.ULongBytes:
            uint length3 = reader.ReadUInt32();
            rntbdToken.value.valueBytes = reader.ReadBytes((int) length3);
            break;
          case RntbdTokenTypes.Float:
            rntbdToken.value.valueFloat = reader.ReadSingle();
            break;
          case RntbdTokenTypes.Double:
            rntbdToken.value.valueDouble = reader.ReadDouble();
            break;
          default:
            DefaultTrace.TraceError("Unrecognized token type {0} with identifier {1} found in RNTBD token stream", (object) rntbdToken.GetTokenType(), (object) rntbdToken.GetTokenIdentifier());
            throw new InternalServerErrorException(RMResources.InternalServerError, this.GetValidationFailureHeader());
        }
        rntbdToken.isPresent = true;
      }
      foreach (RntbdToken token in this.tokens)
      {
        if (!token.isPresent && token.IsRequired())
        {
          DefaultTrace.TraceError("Required token with identifier {0} not found in RNTBD token stream", (object) token.GetTokenIdentifier());
          throw new InternalServerErrorException(RMResources.InternalServerError, this.GetValidationFailureHeader());
        }
      }
    }

    private INameValueCollection GetValidationFailureHeader()
    {
      DictionaryNameValueCollection validationFailureHeader = new DictionaryNameValueCollection();
      validationFailureHeader.Add("x-ms-request-validation-failure", "1");
      return (INameValueCollection) validationFailureHeader;
    }
  }
}
