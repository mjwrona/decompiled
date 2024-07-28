// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories.PubCacheProtobufDocumentProcessor`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Google.Protobuf;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.DocumentProvider;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories
{
  public class PubCacheProtobufDocumentProcessor<TDoc> : 
    IAggregationDocumentProcessor<TDoc>,
    IEmptyDocumentProvider<TDoc>
    where TDoc : IMessage<TDoc>
  {
    public PubCacheProtobufDocumentProcessor(MessageParser<TDoc> parser)
    {
      // ISSUE: reference to a compiler-generated field
      this.\u003Cparser\u003EP = parser;
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }

    public TDoc GetEmptyDocument() => this.\u003Cparser\u003EP.ParseFrom(ByteString.Empty);

    public TDoc Deserialize(byte[] buffer)
    {
      int start = PubCacheProtobufDocumentFormattingHelper.ValidateHeader(buffer);
      // ISSUE: reference to a compiler-generated field
      return this.\u003Cparser\u003EP.ParseFrom((ReadOnlySpan<byte>) buffer.AsSpan<byte>(start));
    }

    public byte[] Serialize(TDoc doc)
    {
      byte[] numArray = new byte[PubCacheProtobufDocumentFormattingHelper.CalculateBufferSize((IMessage) doc)];
      int start = PubCacheProtobufDocumentFormattingHelper.WriteHeader(numArray);
      MessageExtensions.WriteTo((IMessage) doc, numArray.AsSpan<byte>(start));
      return numArray;
    }

    public void NotifySaved(TDoc doc)
    {
    }
  }
}
