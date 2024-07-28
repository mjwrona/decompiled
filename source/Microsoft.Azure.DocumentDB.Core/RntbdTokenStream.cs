// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.RntbdTokenStream
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using Microsoft.Azure.Documents.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.Azure.Documents
{
  internal abstract class RntbdTokenStream
  {
    internal RntbdToken[] tokens;
    private Dictionary<ushort, RntbdToken> tokenMap;

    protected void SetTokens(RntbdToken[] t)
    {
      this.tokens = t;
      this.tokenMap = ((IEnumerable<RntbdToken>) t).ToDictionary<RntbdToken, ushort, RntbdToken>((Func<RntbdToken, ushort>) (token => token.GetTokenIdentifier()), (Func<RntbdToken, RntbdToken>) (token => token));
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
              length = num + 12;
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

    public void SerializeToBinaryWriter(BinaryWriter writer, out int tokensLength)
    {
      tokensLength = 0;
      foreach (RntbdToken token in this.tokens)
      {
        int num = 0;
        BinaryWriter writer1 = writer;
        ref int local = ref num;
        token.SerializeToBinaryWriter(writer1, out local);
        tokensLength += num;
      }
    }

    public void ParseFrom(BinaryReader reader)
    {
      while (reader.BaseStream.Position < reader.BaseStream.Length)
      {
        ushort num = reader.ReadUInt16();
        RntbdTokenTypes type = (RntbdTokenTypes) reader.ReadByte();
        RntbdToken rntbdToken;
        if (!this.tokenMap.TryGetValue(num, out rntbdToken))
          rntbdToken = new RntbdToken(false, type, num);
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
            rntbdToken.value.valueGuid = new Guid(reader.ReadBytes(16));
            break;
          case RntbdTokenTypes.SmallString:
          case RntbdTokenTypes.SmallBytes:
            byte count1 = reader.ReadByte();
            rntbdToken.value.valueBytes = reader.ReadBytes((int) count1);
            break;
          case RntbdTokenTypes.String:
          case RntbdTokenTypes.Bytes:
            ushort count2 = reader.ReadUInt16();
            rntbdToken.value.valueBytes = reader.ReadBytes((int) count2);
            break;
          case RntbdTokenTypes.ULongString:
          case RntbdTokenTypes.ULongBytes:
            uint count3 = reader.ReadUInt32();
            rntbdToken.value.valueBytes = reader.ReadBytes((int) count3);
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
