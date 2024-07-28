// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PublicRepositories.NuGetPubCachePackageDeprecation
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server.PublicRepositories
{
  public sealed class NuGetPubCachePackageDeprecation : 
    IMessage<
    #nullable disable
    NuGetPubCachePackageDeprecation>,
    IMessage,
    IEquatable<NuGetPubCachePackageDeprecation>,
    IDeepCloneable<NuGetPubCachePackageDeprecation>,
    IBufferMessage
  {
    private static readonly MessageParser<NuGetPubCachePackageDeprecation> _parser = new MessageParser<NuGetPubCachePackageDeprecation>((Func<NuGetPubCachePackageDeprecation>) (() => new NuGetPubCachePackageDeprecation()));
    private UnknownFieldSet _unknownFields;
    public const int ReasonsFieldNumber = 1;
    private static readonly FieldCodec<string> _repeated_reasons_codec = FieldCodec.ForString(10U);
    private readonly RepeatedField<string> reasons_ = new RepeatedField<string>();
    public const int MessageFieldNumber = 2;
    private static readonly string MessageDefaultValue = "";
    private string message_;
    public const int AlternatePackageFieldNumber = 3;
    private NuGetPubCacheAlternatePackage alternatePackage_;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public static MessageParser<NuGetPubCachePackageDeprecation> Parser => NuGetPubCachePackageDeprecation._parser;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public static MessageDescriptor Descriptor => NugetPublicRepoCacheReflection.Descriptor.MessageTypes[1];

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    MessageDescriptor IMessage.pb\u003A\u003AGoogle\u002EProtobuf\u002EIMessage\u002EDescriptor => NuGetPubCachePackageDeprecation.Descriptor;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public NuGetPubCachePackageDeprecation()
    {
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public NuGetPubCachePackageDeprecation(NuGetPubCachePackageDeprecation other)
      : this()
    {
      this.reasons_ = other.reasons_.Clone();
      this.message_ = other.message_;
      this.alternatePackage_ = other.alternatePackage_ != null ? other.alternatePackage_.Clone() : (NuGetPubCacheAlternatePackage) null;
      this._unknownFields = UnknownFieldSet.Clone(other._unknownFields);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public NuGetPubCachePackageDeprecation Clone() => new NuGetPubCachePackageDeprecation(this);

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public RepeatedField<string> Reasons => this.reasons_;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public string Message
    {
      get => this.message_ ?? NuGetPubCachePackageDeprecation.MessageDefaultValue;
      set => this.message_ = ProtoPreconditions.CheckNotNull<string>(value, nameof (value));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public bool HasMessage => this.message_ != null;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public void ClearMessage() => this.message_ = (string) null;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public NuGetPubCacheAlternatePackage AlternatePackage
    {
      get => this.alternatePackage_;
      set => this.alternatePackage_ = value;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public override bool Equals(object other) => this.Equals(other as NuGetPubCachePackageDeprecation);

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public bool Equals(NuGetPubCachePackageDeprecation other)
    {
      if (other == null)
        return false;
      if (other == this)
        return true;
      return this.reasons_.Equals(other.reasons_) && !(this.Message != other.Message) && object.Equals((object) this.AlternatePackage, (object) other.AlternatePackage) && object.Equals((object) this._unknownFields, (object) other._unknownFields);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public override int GetHashCode()
    {
      int hashCode = 1 ^ this.reasons_.GetHashCode();
      if (this.HasMessage)
        hashCode ^= this.Message.GetHashCode();
      if (this.alternatePackage_ != null)
        hashCode ^= this.AlternatePackage.GetHashCode();
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
      this.reasons_.WriteTo(ref output, NuGetPubCachePackageDeprecation._repeated_reasons_codec);
      if (this.HasMessage)
      {
        output.WriteRawTag((byte) 18);
        output.WriteString(this.Message);
      }
      if (this.alternatePackage_ != null)
      {
        output.WriteRawTag((byte) 26);
        output.WriteMessage((IMessage) this.AlternatePackage);
      }
      if (this._unknownFields == null)
        return;
      this._unknownFields.WriteTo(ref output);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public int CalculateSize()
    {
      int size = 0 + this.reasons_.CalculateSize(NuGetPubCachePackageDeprecation._repeated_reasons_codec);
      if (this.HasMessage)
        size += 1 + CodedOutputStream.ComputeStringSize(this.Message);
      if (this.alternatePackage_ != null)
        size += 1 + CodedOutputStream.ComputeMessageSize((IMessage) this.AlternatePackage);
      if (this._unknownFields != null)
        size += this._unknownFields.CalculateSize();
      return size;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public void MergeFrom(NuGetPubCachePackageDeprecation other)
    {
      if (other == null)
        return;
      this.reasons_.Add((IEnumerable<string>) other.reasons_);
      if (other.HasMessage)
        this.Message = other.Message;
      if (other.alternatePackage_ != null)
      {
        if (this.alternatePackage_ == null)
          this.AlternatePackage = new NuGetPubCacheAlternatePackage();
        this.AlternatePackage.MergeFrom(other.AlternatePackage);
      }
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
            this.reasons_.AddEntriesFrom(ref input, NuGetPubCachePackageDeprecation._repeated_reasons_codec);
            continue;
          case 18:
            this.Message = input.ReadString();
            continue;
          case 26:
            if (this.alternatePackage_ == null)
              this.AlternatePackage = new NuGetPubCacheAlternatePackage();
            input.ReadMessage((IMessage) this.AlternatePackage);
            continue;
          default:
            this._unknownFields = UnknownFieldSet.MergeFieldFrom(this._unknownFields, ref input);
            continue;
        }
      }
    }

    public 
    #nullable enable
    string? NullableMessage
    {
      get => !this.HasMessage ? (string) null : this.Message;
      set
      {
        if (value == null)
          this.ClearMessage();
        else
          this.Message = value;
      }
    }
  }
}
