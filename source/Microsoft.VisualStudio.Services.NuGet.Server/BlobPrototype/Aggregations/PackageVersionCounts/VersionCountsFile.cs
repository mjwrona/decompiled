// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts.VersionCountsFile
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

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts
{
  public sealed class VersionCountsFile : 
    IMessage<VersionCountsFile>,
    IMessage,
    IEquatable<VersionCountsFile>,
    IDeepCloneable<VersionCountsFile>,
    IBufferMessage
  {
    private static readonly MessageParser<VersionCountsFile> _parser = new MessageParser<VersionCountsFile>((Func<VersionCountsFile>) (() => new VersionCountsFile()));
    private UnknownFieldSet _unknownFields;
    public const int KnownViewsFieldNumber = 1;
    private static readonly FieldCodec<ByteString> _repeated_knownViews_codec = FieldCodec.ForBytes(10U);
    private readonly RepeatedField<ByteString> knownViews_ = new RepeatedField<ByteString>();
    public const int PackagesFieldNumber = 2;
    private static readonly FieldCodec<VersionCountsPackage> _repeated_packages_codec = FieldCodec.ForMessage<VersionCountsPackage>(18U, VersionCountsPackage.Parser);
    private readonly RepeatedField<VersionCountsPackage> packages_ = new RepeatedField<VersionCountsPackage>();

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public static MessageParser<VersionCountsFile> Parser => VersionCountsFile._parser;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public static MessageDescriptor Descriptor => PackageVersionCountsReflection.Descriptor.MessageTypes[2];

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    MessageDescriptor IMessage.pb\u003A\u003AGoogle\u002EProtobuf\u002EIMessage\u002EDescriptor => VersionCountsFile.Descriptor;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public VersionCountsFile()
    {
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public VersionCountsFile(VersionCountsFile other)
      : this()
    {
      this.knownViews_ = other.knownViews_.Clone();
      this.packages_ = other.packages_.Clone();
      this._unknownFields = UnknownFieldSet.Clone(other._unknownFields);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public VersionCountsFile Clone() => new VersionCountsFile(this);

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public RepeatedField<ByteString> KnownViews => this.knownViews_;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public RepeatedField<VersionCountsPackage> Packages => this.packages_;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public override bool Equals(object other) => this.Equals(other as VersionCountsFile);

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public bool Equals(VersionCountsFile other)
    {
      if (other == null)
        return false;
      if (other == this)
        return true;
      return this.knownViews_.Equals(other.knownViews_) && this.packages_.Equals(other.packages_) && object.Equals((object) this._unknownFields, (object) other._unknownFields);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public override int GetHashCode()
    {
      int hashCode = 1 ^ this.knownViews_.GetHashCode() ^ this.packages_.GetHashCode();
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
      this.knownViews_.WriteTo(ref output, VersionCountsFile._repeated_knownViews_codec);
      this.packages_.WriteTo(ref output, VersionCountsFile._repeated_packages_codec);
      if (this._unknownFields == null)
        return;
      this._unknownFields.WriteTo(ref output);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public int CalculateSize()
    {
      int size = 0 + this.knownViews_.CalculateSize(VersionCountsFile._repeated_knownViews_codec) + this.packages_.CalculateSize(VersionCountsFile._repeated_packages_codec);
      if (this._unknownFields != null)
        size += this._unknownFields.CalculateSize();
      return size;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public void MergeFrom(VersionCountsFile other)
    {
      if (other == null)
        return;
      this.knownViews_.Add((IEnumerable<ByteString>) other.knownViews_);
      this.packages_.Add((IEnumerable<VersionCountsPackage>) other.packages_);
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
            this.knownViews_.AddEntriesFrom(ref input, VersionCountsFile._repeated_knownViews_codec);
            continue;
          case 18:
            this.packages_.AddEntriesFrom(ref input, VersionCountsFile._repeated_packages_codec);
            continue;
          default:
            this._unknownFields = UnknownFieldSet.MergeFieldFrom(this._unknownFields, ref input);
            continue;
        }
      }
    }
  }
}
