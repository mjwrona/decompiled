// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts.VersionListsPackageVersion
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Newtonsoft.Json;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts
{
  public sealed class VersionListsPackageVersion : 
    IVersionListsPackageVersion,
    IMessage<VersionListsPackageVersion>,
    IMessage,
    IEquatable<VersionListsPackageVersion>,
    IDeepCloneable<VersionListsPackageVersion>,
    IBufferMessage
  {
    private static readonly MessageParser<VersionListsPackageVersion> _parser = new MessageParser<VersionListsPackageVersion>((Func<VersionListsPackageVersion>) (() => new VersionListsPackageVersion()));
    private UnknownFieldSet _unknownFields;
    public const int CompressedDisplayVersionFieldNumber = 1;
    private string compressedDisplayVersion_ = "";
    public const int ViewIndicesFieldNumber = 2;
    private static readonly FieldCodec<int> _repeated_viewIndices_codec = FieldCodec.ForInt32(18U);
    private readonly RepeatedField<int> viewIndices_ = new RepeatedField<int>();
    public const int IsUnlistedFieldNumber = 3;
    private bool isUnlisted_;
    public const int IsDeletedFieldNumber = 4;
    private bool isDeleted_;

    IReadOnlyCollection<Guid> IVersionListsPackageVersion.ViewIds => (IReadOnlyCollection<Guid>) this.ViewIds;

    bool IVersionListsPackageVersion.IsListed => !this.IsUnlisted;

    IVersionListsPackage IVersionListsPackageVersion.Package => (IVersionListsPackage) this.Package;

    public VssNuGetPackageVersion Version => this.PackageIdentity.Version;

    public VssNuGetPackageIdentity PackageIdentity { get; set; }

    public List<Guid> ViewIds { get; set; }

    [JsonIgnore]
    public VersionListsPackage Package { get; set; }

    public static VersionListsPackageVersion CreateNewUnpacked(
      VssNuGetPackageIdentity packageIdentity,
      VersionListsPackage package)
    {
      return new VersionListsPackageVersion()
      {
        PackageIdentity = packageIdentity,
        Package = package,
        ViewIds = new List<Guid>()
      };
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public static MessageParser<VersionListsPackageVersion> Parser => VersionListsPackageVersion._parser;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public static MessageDescriptor Descriptor => PackageVersionCountsReflection.Descriptor.MessageTypes[3];

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    MessageDescriptor IMessage.pb\u003A\u003AGoogle\u002EProtobuf\u002EIMessage\u002EDescriptor => VersionListsPackageVersion.Descriptor;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public VersionListsPackageVersion()
    {
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public VersionListsPackageVersion(VersionListsPackageVersion other)
      : this()
    {
      this.compressedDisplayVersion_ = other.compressedDisplayVersion_;
      this.viewIndices_ = other.viewIndices_.Clone();
      this.isUnlisted_ = other.isUnlisted_;
      this.isDeleted_ = other.isDeleted_;
      this._unknownFields = UnknownFieldSet.Clone(other._unknownFields);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public VersionListsPackageVersion Clone() => new VersionListsPackageVersion(this);

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public string CompressedDisplayVersion
    {
      get => this.compressedDisplayVersion_;
      set => this.compressedDisplayVersion_ = ProtoPreconditions.CheckNotNull<string>(value, nameof (value));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public RepeatedField<int> ViewIndices => this.viewIndices_;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public bool IsUnlisted
    {
      get => this.isUnlisted_;
      set => this.isUnlisted_ = value;
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
    public override bool Equals(object other) => this.Equals(other as VersionListsPackageVersion);

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public bool Equals(VersionListsPackageVersion other)
    {
      if (other == null)
        return false;
      if (other == this)
        return true;
      return !(this.CompressedDisplayVersion != other.CompressedDisplayVersion) && this.viewIndices_.Equals(other.viewIndices_) && this.IsUnlisted == other.IsUnlisted && this.IsDeleted == other.IsDeleted && object.Equals((object) this._unknownFields, (object) other._unknownFields);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public override int GetHashCode()
    {
      int num = 1;
      if (this.CompressedDisplayVersion.Length != 0)
        num ^= this.CompressedDisplayVersion.GetHashCode();
      int hashCode = num ^ this.viewIndices_.GetHashCode();
      if (this.IsUnlisted)
        hashCode ^= this.IsUnlisted.GetHashCode();
      if (this.IsDeleted)
        hashCode ^= this.IsDeleted.GetHashCode();
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
      this.viewIndices_.WriteTo(ref output, VersionListsPackageVersion._repeated_viewIndices_codec);
      if (this.IsUnlisted)
      {
        output.WriteRawTag((byte) 24);
        output.WriteBool(this.IsUnlisted);
      }
      if (this.IsDeleted)
      {
        output.WriteRawTag((byte) 32);
        output.WriteBool(this.IsDeleted);
      }
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
      int size = num + this.viewIndices_.CalculateSize(VersionListsPackageVersion._repeated_viewIndices_codec);
      if (this.IsUnlisted)
        size += 2;
      if (this.IsDeleted)
        size += 2;
      if (this._unknownFields != null)
        size += this._unknownFields.CalculateSize();
      return size;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public void MergeFrom(VersionListsPackageVersion other)
    {
      if (other == null)
        return;
      if (other.CompressedDisplayVersion.Length != 0)
        this.CompressedDisplayVersion = other.CompressedDisplayVersion;
      this.viewIndices_.Add((IEnumerable<int>) other.viewIndices_);
      if (other.IsUnlisted)
        this.IsUnlisted = other.IsUnlisted;
      if (other.IsDeleted)
        this.IsDeleted = other.IsDeleted;
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
          case 18:
            this.viewIndices_.AddEntriesFrom(ref input, VersionListsPackageVersion._repeated_viewIndices_codec);
            continue;
          case 24:
            this.IsUnlisted = input.ReadBool();
            continue;
          case 32:
            this.IsDeleted = input.ReadBool();
            continue;
          default:
            this._unknownFields = UnknownFieldSet.MergeFieldFrom(this._unknownFields, ref input);
            continue;
        }
      }
    }
  }
}
