// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.VersionListWithSize.VersionListWithSizePackageVersionFile
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;
using Newtonsoft.Json;
using System;
using System.CodeDom.Compiler;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.VersionListWithSize
{
  public sealed class VersionListWithSizePackageVersionFile : 
    IMessage<VersionListWithSizePackageVersionFile>,
    IMessage,
    IEquatable<VersionListWithSizePackageVersionFile>,
    IDeepCloneable<VersionListWithSizePackageVersionFile>,
    IBufferMessage,
    IVersionListWithSizePackageVersionFile
  {
    private static readonly MessageParser<VersionListWithSizePackageVersionFile> _parser = new MessageParser<VersionListWithSizePackageVersionFile>((Func<VersionListWithSizePackageVersionFile>) (() => new VersionListWithSizePackageVersionFile()));
    private UnknownFieldSet _unknownFields;
    public const int CompressedFileNameFieldNumber = 1;
    private string compressedFileName_ = "";
    public const int SizeInBytesFieldNumber = 2;
    private double sizeInBytes_;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public static MessageParser<VersionListWithSizePackageVersionFile> Parser => VersionListWithSizePackageVersionFile._parser;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public static MessageDescriptor Descriptor => VersionListWithSizeReflection.Descriptor.MessageTypes[0];

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    MessageDescriptor IMessage.pb\u003A\u003AGoogle\u002EProtobuf\u002EIMessage\u002EDescriptor => VersionListWithSizePackageVersionFile.Descriptor;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public VersionListWithSizePackageVersionFile()
    {
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public VersionListWithSizePackageVersionFile(VersionListWithSizePackageVersionFile other)
      : this()
    {
      this.compressedFileName_ = other.compressedFileName_;
      this.sizeInBytes_ = other.sizeInBytes_;
      this._unknownFields = UnknownFieldSet.Clone(other._unknownFields);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public VersionListWithSizePackageVersionFile Clone() => new VersionListWithSizePackageVersionFile(this);

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public string CompressedFileName
    {
      get => this.compressedFileName_;
      set => this.compressedFileName_ = ProtoPreconditions.CheckNotNull<string>(value, nameof (value));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public double SizeInBytes
    {
      get => this.sizeInBytes_;
      set => this.sizeInBytes_ = value;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public override bool Equals(object other) => this.Equals(other as VersionListWithSizePackageVersionFile);

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public bool Equals(VersionListWithSizePackageVersionFile other)
    {
      if (other == null)
        return false;
      if (other == this)
        return true;
      return !(this.CompressedFileName != other.CompressedFileName) && ProtobufEqualityComparers.BitwiseDoubleEqualityComparer.Equals(this.SizeInBytes, other.SizeInBytes) && object.Equals((object) this._unknownFields, (object) other._unknownFields);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public override int GetHashCode()
    {
      int hashCode = 1;
      if (this.CompressedFileName.Length != 0)
        hashCode ^= this.CompressedFileName.GetHashCode();
      if (this.SizeInBytes != 0.0)
        hashCode ^= ProtobufEqualityComparers.BitwiseDoubleEqualityComparer.GetHashCode(this.SizeInBytes);
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
      if (this.CompressedFileName.Length != 0)
      {
        output.WriteRawTag((byte) 10);
        output.WriteString(this.CompressedFileName);
      }
      if (this.SizeInBytes != 0.0)
      {
        output.WriteRawTag((byte) 17);
        output.WriteDouble(this.SizeInBytes);
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
      if (this.CompressedFileName.Length != 0)
        size += 1 + CodedOutputStream.ComputeStringSize(this.CompressedFileName);
      if (this.SizeInBytes != 0.0)
        size += 9;
      if (this._unknownFields != null)
        size += this._unknownFields.CalculateSize();
      return size;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public void MergeFrom(VersionListWithSizePackageVersionFile other)
    {
      if (other == null)
        return;
      if (other.CompressedFileName.Length != 0)
        this.CompressedFileName = other.CompressedFileName;
      if (other.SizeInBytes != 0.0)
        this.SizeInBytes = other.SizeInBytes;
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
            this.CompressedFileName = input.ReadString();
            continue;
          case 17:
            this.SizeInBytes = input.ReadDouble();
            continue;
          default:
            this._unknownFields = UnknownFieldSet.MergeFieldFrom(this._unknownFields, ref input);
            continue;
        }
      }
    }

    public string FileName { get; set; }

    IVersionListWithSizePackageVersion IVersionListWithSizePackageVersionFile.Version => (IVersionListWithSizePackageVersion) this.Version;

    [JsonIgnore]
    public VersionListWithSizePackageVersion Version { get; set; }

    public static VersionListWithSizePackageVersionFile CreateNewUnpacked(
      string fileName,
      VersionListWithSizePackageVersion version)
    {
      return new VersionListWithSizePackageVersionFile()
      {
        FileName = fileName,
        Version = version
      };
    }
  }
}
