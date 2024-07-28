// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PublicRepositories.NuGetPubCacheAlternatePackage
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Google.Protobuf;
using Google.Protobuf.Reflection;
using System;
using System.CodeDom.Compiler;
using System.Diagnostics;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server.PublicRepositories
{
  public sealed class NuGetPubCacheAlternatePackage : 
    IMessage<
    #nullable disable
    NuGetPubCacheAlternatePackage>,
    IMessage,
    IEquatable<NuGetPubCacheAlternatePackage>,
    IDeepCloneable<NuGetPubCacheAlternatePackage>,
    IBufferMessage
  {
    private static readonly MessageParser<NuGetPubCacheAlternatePackage> _parser = new MessageParser<NuGetPubCacheAlternatePackage>((Func<NuGetPubCacheAlternatePackage>) (() => new NuGetPubCacheAlternatePackage()));
    private UnknownFieldSet _unknownFields;
    public const int IdFieldNumber = 1;
    private string id_ = "";
    public const int RangeFieldNumber = 2;
    private static readonly string RangeDefaultValue = "";
    private string range_;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public static MessageParser<NuGetPubCacheAlternatePackage> Parser => NuGetPubCacheAlternatePackage._parser;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public static MessageDescriptor Descriptor => NugetPublicRepoCacheReflection.Descriptor.MessageTypes[0];

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    MessageDescriptor IMessage.pb\u003A\u003AGoogle\u002EProtobuf\u002EIMessage\u002EDescriptor => NuGetPubCacheAlternatePackage.Descriptor;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public NuGetPubCacheAlternatePackage()
    {
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public NuGetPubCacheAlternatePackage(NuGetPubCacheAlternatePackage other)
      : this()
    {
      this.id_ = other.id_;
      this.range_ = other.range_;
      this._unknownFields = UnknownFieldSet.Clone(other._unknownFields);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public NuGetPubCacheAlternatePackage Clone() => new NuGetPubCacheAlternatePackage(this);

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public string Id
    {
      get => this.id_;
      set => this.id_ = ProtoPreconditions.CheckNotNull<string>(value, nameof (value));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public string Range
    {
      get => this.range_ ?? NuGetPubCacheAlternatePackage.RangeDefaultValue;
      set => this.range_ = ProtoPreconditions.CheckNotNull<string>(value, nameof (value));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public bool HasRange => this.range_ != null;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public void ClearRange() => this.range_ = (string) null;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public override bool Equals(object other) => this.Equals(other as NuGetPubCacheAlternatePackage);

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public bool Equals(NuGetPubCacheAlternatePackage other)
    {
      if (other == null)
        return false;
      if (other == this)
        return true;
      return !(this.Id != other.Id) && !(this.Range != other.Range) && object.Equals((object) this._unknownFields, (object) other._unknownFields);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public override int GetHashCode()
    {
      int hashCode = 1;
      if (this.Id.Length != 0)
        hashCode ^= this.Id.GetHashCode();
      if (this.HasRange)
        hashCode ^= this.Range.GetHashCode();
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
      if (this.Id.Length != 0)
      {
        output.WriteRawTag((byte) 10);
        output.WriteString(this.Id);
      }
      if (this.HasRange)
      {
        output.WriteRawTag((byte) 18);
        output.WriteString(this.Range);
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
      if (this.Id.Length != 0)
        size += 1 + CodedOutputStream.ComputeStringSize(this.Id);
      if (this.HasRange)
        size += 1 + CodedOutputStream.ComputeStringSize(this.Range);
      if (this._unknownFields != null)
        size += this._unknownFields.CalculateSize();
      return size;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public void MergeFrom(NuGetPubCacheAlternatePackage other)
    {
      if (other == null)
        return;
      if (other.Id.Length != 0)
        this.Id = other.Id;
      if (other.HasRange)
        this.Range = other.Range;
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
            this.Id = input.ReadString();
            continue;
          case 18:
            this.Range = input.ReadString();
            continue;
          default:
            this._unknownFields = UnknownFieldSet.MergeFieldFrom(this._unknownFields, ref input);
            continue;
        }
      }
    }

    public 
    #nullable enable
    string? NullableRange
    {
      get => !this.HasRange ? (string) null : this.Range;
      set
      {
        if (value == null)
          this.ClearRange();
        else
          this.Range = value;
      }
    }
  }
}
