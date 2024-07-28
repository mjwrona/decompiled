// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PublicRepositories.NuGetPubCacheVersionNuspec
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Google.Protobuf;
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;
using System;
using System.CodeDom.Compiler;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.NuGet.Server.PublicRepositories
{
  public sealed class NuGetPubCacheVersionNuspec : 
    IMessage<NuGetPubCacheVersionNuspec>,
    IMessage,
    IEquatable<NuGetPubCacheVersionNuspec>,
    IDeepCloneable<NuGetPubCacheVersionNuspec>,
    IBufferMessage
  {
    private static readonly MessageParser<NuGetPubCacheVersionNuspec> _parser = new MessageParser<NuGetPubCacheVersionNuspec>((Func<NuGetPubCacheVersionNuspec>) (() => new NuGetPubCacheVersionNuspec()));
    private UnknownFieldSet _unknownFields;
    public const int FetchDateFieldNumber = 1;
    private Timestamp fetchDate_;
    public const int DeflatedBytesFieldNumber = 2;
    private ByteString deflatedBytes_ = ByteString.Empty;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public static MessageParser<NuGetPubCacheVersionNuspec> Parser => NuGetPubCacheVersionNuspec._parser;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public static MessageDescriptor Descriptor => NugetPublicRepoCacheReflection.Descriptor.MessageTypes[4];

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    MessageDescriptor IMessage.pb\u003A\u003AGoogle\u002EProtobuf\u002EIMessage\u002EDescriptor => NuGetPubCacheVersionNuspec.Descriptor;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public NuGetPubCacheVersionNuspec()
    {
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public NuGetPubCacheVersionNuspec(NuGetPubCacheVersionNuspec other)
      : this()
    {
      this.fetchDate_ = other.fetchDate_ != (Timestamp) null ? other.fetchDate_.Clone() : (Timestamp) null;
      this.deflatedBytes_ = other.deflatedBytes_;
      this._unknownFields = UnknownFieldSet.Clone(other._unknownFields);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public NuGetPubCacheVersionNuspec Clone() => new NuGetPubCacheVersionNuspec(this);

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public Timestamp FetchDate
    {
      get => this.fetchDate_;
      set => this.fetchDate_ = value;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public ByteString DeflatedBytes
    {
      get => this.deflatedBytes_;
      set => this.deflatedBytes_ = ProtoPreconditions.CheckNotNull<ByteString>(value, nameof (value));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public override bool Equals(object other) => this.Equals(other as NuGetPubCacheVersionNuspec);

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public bool Equals(NuGetPubCacheVersionNuspec other)
    {
      if (other == null)
        return false;
      if (other == this)
        return true;
      return object.Equals((object) this.FetchDate, (object) other.FetchDate) && !(this.DeflatedBytes != other.DeflatedBytes) && object.Equals((object) this._unknownFields, (object) other._unknownFields);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public override int GetHashCode()
    {
      int hashCode = 1;
      if (this.fetchDate_ != (Timestamp) null)
        hashCode ^= this.FetchDate.GetHashCode();
      if (this.DeflatedBytes.Length != 0)
        hashCode ^= this.DeflatedBytes.GetHashCode();
      if (this._unknownFields != null)
        hashCode ^= this._unknownFields.GetHashCode();
      return hashCode;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public override string ToString() => JsonFormatter.ToDiagnosticString((IMessage) this);

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public void WriteTo(CodedOutputStream output) => output.WriteRawMessage((IMessage) this);

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    void IBufferMessage.pb\u003A\u003AGoogle\u002EProtobuf\u002EIBufferMessage\u002EInternalWriteTo(
      ref WriteContext output)
    {
      if (this.fetchDate_ != (Timestamp) null)
      {
        output.WriteRawTag((byte) 10);
        output.WriteMessage((IMessage) this.FetchDate);
      }
      if (this.DeflatedBytes.Length != 0)
      {
        output.WriteRawTag((byte) 18);
        output.WriteBytes(this.DeflatedBytes);
      }
      if (this._unknownFields == null)
        return;
      this._unknownFields.WriteTo(ref output);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public int CalculateSize()
    {
      int size = 0;
      if (this.fetchDate_ != (Timestamp) null)
        size += 1 + CodedOutputStream.ComputeMessageSize((IMessage) this.FetchDate);
      if (this.DeflatedBytes.Length != 0)
        size += 1 + CodedOutputStream.ComputeBytesSize(this.DeflatedBytes);
      if (this._unknownFields != null)
        size += this._unknownFields.CalculateSize();
      return size;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public void MergeFrom(NuGetPubCacheVersionNuspec other)
    {
      if (other == null)
        return;
      if (other.fetchDate_ != (Timestamp) null)
      {
        if (this.fetchDate_ == (Timestamp) null)
          this.FetchDate = new Timestamp();
        this.FetchDate.MergeFrom(other.FetchDate);
      }
      if (other.DeflatedBytes.Length != 0)
        this.DeflatedBytes = other.DeflatedBytes;
      this._unknownFields = UnknownFieldSet.MergeFrom(this._unknownFields, other._unknownFields);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public void MergeFrom(CodedInputStream input) => input.ReadRawMessage((IMessage) this);

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    void IBufferMessage.pb\u003A\u003AGoogle\u002EProtobuf\u002EIBufferMessage\u002EInternalMergeFrom(
      ref ParseContext input)
    {
      uint num;
      while ((num = input.ReadTag()) != 0U)
      {
        switch (num)
        {
          case 10:
            if (this.fetchDate_ == (Timestamp) null)
              this.FetchDate = new Timestamp();
            input.ReadMessage((IMessage) this.FetchDate);
            continue;
          case 18:
            this.DeflatedBytes = input.ReadBytes();
            continue;
          default:
            this._unknownFields = UnknownFieldSet.MergeFieldFrom(this._unknownFields, ref input);
            continue;
        }
      }
    }
  }
}
