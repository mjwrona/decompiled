// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.VersionListWithSize.VersionListWithSizePackageVersion
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Newtonsoft.Json;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.VersionListWithSize
{
  public sealed class VersionListWithSizePackageVersion : 
    IMessage<VersionListWithSizePackageVersion>,
    IMessage,
    IEquatable<VersionListWithSizePackageVersion>,
    IDeepCloneable<VersionListWithSizePackageVersion>,
    IBufferMessage,
    IVersionListWithSizePackageVersion
  {
    private static readonly MessageParser<VersionListWithSizePackageVersion> _parser = new MessageParser<VersionListWithSizePackageVersion>((Func<VersionListWithSizePackageVersion>) (() => new VersionListWithSizePackageVersion()));
    private UnknownFieldSet _unknownFields;
    public const int CompressedDisplayVersionFieldNumber = 1;
    private string compressedDisplayVersion_ = "";
    public const int IsDeletedFieldNumber = 2;
    private bool isDeleted_;
    public const int PackageFilesFieldNumber = 3;
    private static readonly FieldCodec<VersionListWithSizePackageVersionFile> _repeated_packageFiles_codec = FieldCodec.ForMessage<VersionListWithSizePackageVersionFile>(26U, VersionListWithSizePackageVersionFile.Parser);
    private readonly RepeatedField<VersionListWithSizePackageVersionFile> packageFiles_ = new RepeatedField<VersionListWithSizePackageVersionFile>();

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public static MessageParser<VersionListWithSizePackageVersion> Parser => VersionListWithSizePackageVersion._parser;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public static MessageDescriptor Descriptor => VersionListWithSizeReflection.Descriptor.MessageTypes[1];

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    MessageDescriptor IMessage.pb\u003A\u003AGoogle\u002EProtobuf\u002EIMessage\u002EDescriptor => VersionListWithSizePackageVersion.Descriptor;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public VersionListWithSizePackageVersion()
    {
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public VersionListWithSizePackageVersion(VersionListWithSizePackageVersion other)
      : this()
    {
      this.compressedDisplayVersion_ = other.compressedDisplayVersion_;
      this.isDeleted_ = other.isDeleted_;
      this.packageFiles_ = other.packageFiles_.Clone();
      this._unknownFields = UnknownFieldSet.Clone(other._unknownFields);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public VersionListWithSizePackageVersion Clone() => new VersionListWithSizePackageVersion(this);

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public string CompressedDisplayVersion
    {
      get => this.compressedDisplayVersion_;
      set => this.compressedDisplayVersion_ = ProtoPreconditions.CheckNotNull<string>(value, nameof (value));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public bool IsDeleted
    {
      get => this.isDeleted_;
      set => this.isDeleted_ = value;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public RepeatedField<VersionListWithSizePackageVersionFile> PackageFiles => this.packageFiles_;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public override bool Equals(object other) => this.Equals(other as VersionListWithSizePackageVersion);

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public bool Equals(VersionListWithSizePackageVersion other)
    {
      if (other == null)
        return false;
      if (other == this)
        return true;
      return !(this.CompressedDisplayVersion != other.CompressedDisplayVersion) && this.IsDeleted == other.IsDeleted && this.packageFiles_.Equals(other.packageFiles_) && object.Equals((object) this._unknownFields, (object) other._unknownFields);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public override int GetHashCode()
    {
      int num = 1;
      if (this.CompressedDisplayVersion.Length != 0)
        num ^= this.CompressedDisplayVersion.GetHashCode();
      if (this.IsDeleted)
        num ^= this.IsDeleted.GetHashCode();
      int hashCode = num ^ this.packageFiles_.GetHashCode();
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
      if (this.CompressedDisplayVersion.Length != 0)
      {
        output.WriteRawTag((byte) 10);
        output.WriteString(this.CompressedDisplayVersion);
      }
      if (this.IsDeleted)
      {
        output.WriteRawTag((byte) 16);
        output.WriteBool(this.IsDeleted);
      }
      this.packageFiles_.WriteTo(ref output, VersionListWithSizePackageVersion._repeated_packageFiles_codec);
      if (this._unknownFields == null)
        return;
      this._unknownFields.WriteTo(ref output);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public int CalculateSize()
    {
      int num = 0;
      if (this.CompressedDisplayVersion.Length != 0)
        num += 1 + CodedOutputStream.ComputeStringSize(this.CompressedDisplayVersion);
      if (this.IsDeleted)
        num += 2;
      int size = num + this.packageFiles_.CalculateSize(VersionListWithSizePackageVersion._repeated_packageFiles_codec);
      if (this._unknownFields != null)
        size += this._unknownFields.CalculateSize();
      return size;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public void MergeFrom(VersionListWithSizePackageVersion other)
    {
      if (other == null)
        return;
      if (other.CompressedDisplayVersion.Length != 0)
        this.CompressedDisplayVersion = other.CompressedDisplayVersion;
      if (other.IsDeleted)
        this.IsDeleted = other.IsDeleted;
      this.packageFiles_.Add((IEnumerable<VersionListWithSizePackageVersionFile>) other.packageFiles_);
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
            this.CompressedDisplayVersion = input.ReadString();
            continue;
          case 16:
            this.IsDeleted = input.ReadBool();
            continue;
          case 26:
            this.packageFiles_.AddEntriesFrom(ref input, VersionListWithSizePackageVersion._repeated_packageFiles_codec);
            continue;
          default:
            this._unknownFields = UnknownFieldSet.MergeFieldFrom(this._unknownFields, ref input);
            continue;
        }
      }
    }

    public string DisplayVersion { get; set; }

    IVersionListWithSizePackage IVersionListWithSizePackageVersion.Package => (IVersionListWithSizePackage) this.Package;

    [JsonIgnore]
    public VersionListWithSizePackage Package { get; set; }

    IReadOnlyList<IVersionListWithSizePackageVersionFile> IVersionListWithSizePackageVersion.PackageFiles => (IReadOnlyList<IVersionListWithSizePackageVersionFile>) this.PackageFiles;

    public static VersionListWithSizePackageVersion CreateNewUnpacked(
      IPackageIdentity packageIdentity,
      VersionListWithSizePackage package)
    {
      return new VersionListWithSizePackageVersion()
      {
        DisplayVersion = packageIdentity.Version.DisplayVersion,
        Package = package
      };
    }
  }
}
