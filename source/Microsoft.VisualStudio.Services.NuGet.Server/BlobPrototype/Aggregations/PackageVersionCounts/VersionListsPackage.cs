// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts.VersionListsPackage
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts
{
  public sealed class VersionListsPackage : 
    IVersionListsPackage,
    ILazyVersionListsPackage,
    IPackageNameEntry<VssNuGetPackageName>,
    IMessage<VersionListsPackage>,
    IMessage,
    IEquatable<VersionListsPackage>,
    IDeepCloneable<VersionListsPackage>,
    IBufferMessage
  {
    private static readonly MessageParser<VersionListsPackage> _parser = new MessageParser<VersionListsPackage>((Func<VersionListsPackage>) (() => new VersionListsPackage()));
    private UnknownFieldSet _unknownFields;
    public const int CompressedDisplayNameFieldNumber = 1;
    private string compressedDisplayName_ = "";
    public const int VersionsFieldNumber = 2;
    private static readonly FieldCodec<VersionListsPackageVersion> _repeated_versions_codec = FieldCodec.ForMessage<VersionListsPackageVersion>(18U, VersionListsPackageVersion.Parser);
    private readonly RepeatedField<VersionListsPackageVersion> versions_ = new RepeatedField<VersionListsPackageVersion>();
    public const int LastModifiedFieldNumber = 3;
    private Timestamp lastModified_;

    public bool VersionsNeedUnpack { get; private set; } = true;

    public bool VersionsNeedRepack { get; private set; }

    public bool NeedsSave { get; private set; }

    public void NotifyVersionsUnpacked()
    {
      this.VersionsNeedUnpack = false;
      this.VersionListsFile.NotifyContainedPackageUnpacked();
    }

    public void NotifyVersionsRepacked()
    {
      this.VersionsNeedRepack = false;
      this.VersionListsFile.NotifyContainedPackagePacked();
    }

    public void NotifySaved() => this.NeedsSave = false;

    public void NotifyModified(DateTime modTime)
    {
      if (modTime > this.LastModifiedAsDateTime)
        this.LastModifiedAsDateTime = modTime;
      this.VersionsNeedRepack = true;
      this.NeedsSave = true;
    }

    IReadOnlyList<IVersionListsPackageVersion> IVersionListsPackage.Versions => (IReadOnlyList<IVersionListsPackageVersion>) this.Versions;

    DateTime IVersionListsPackage.LastModified => this.LastModifiedAsDateTime;

    public VssNuGetPackageName Name { get; set; }

    DateTime IPackageNameEntry<VssNuGetPackageName>.LastUpdatedDateTime => this.LastModifiedAsDateTime;

    IVersionListsPackage ILazyVersionListsPackage.Get()
    {
      VersionListsFileUnpacker.EnsurePackageUnpacked(this.VersionListsFile, this);
      return (IVersionListsPackage) this;
    }

    public VersionListsFile VersionListsFile { get; set; }

    public DateTime LastModifiedAsDateTime
    {
      get
      {
        Timestamp timestamp = this.LastModified;
        if ((object) timestamp == null)
          timestamp = ProtobufConstants.ZeroTimestamp;
        return timestamp.ToDateTime();
      }
      private set => this.LastModified = Timestamp.FromDateTime(value);
    }

    public static VersionListsPackage CreateNewUnpacked(
      VssNuGetPackageName name,
      VersionListsFile wrapped,
      DateTime lastModified)
    {
      return new VersionListsPackage()
      {
        Name = name,
        VersionListsFile = wrapped,
        LastModifiedAsDateTime = lastModified,
        NeedsSave = true,
        VersionsNeedUnpack = false,
        VersionsNeedRepack = true
      };
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public static MessageParser<VersionListsPackage> Parser => VersionListsPackage._parser;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public static MessageDescriptor Descriptor => PackageVersionCountsReflection.Descriptor.MessageTypes[4];

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    MessageDescriptor IMessage.pb\u003A\u003AGoogle\u002EProtobuf\u002EIMessage\u002EDescriptor => VersionListsPackage.Descriptor;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public VersionListsPackage()
    {
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public VersionListsPackage(VersionListsPackage other)
      : this()
    {
      this.compressedDisplayName_ = other.compressedDisplayName_;
      this.versions_ = other.versions_.Clone();
      this.lastModified_ = other.lastModified_ != (Timestamp) null ? other.lastModified_.Clone() : (Timestamp) null;
      this._unknownFields = UnknownFieldSet.Clone(other._unknownFields);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public VersionListsPackage Clone() => new VersionListsPackage(this);

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public string CompressedDisplayName
    {
      get => this.compressedDisplayName_;
      set => this.compressedDisplayName_ = ProtoPreconditions.CheckNotNull<string>(value, nameof (value));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public RepeatedField<VersionListsPackageVersion> Versions => this.versions_;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public Timestamp LastModified
    {
      get => this.lastModified_;
      set => this.lastModified_ = value;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public override bool Equals(object other) => this.Equals(other as VersionListsPackage);

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public bool Equals(VersionListsPackage other)
    {
      if (other == null)
        return false;
      if (other == this)
        return true;
      return !(this.CompressedDisplayName != other.CompressedDisplayName) && this.versions_.Equals(other.versions_) && object.Equals((object) this.LastModified, (object) other.LastModified) && object.Equals((object) this._unknownFields, (object) other._unknownFields);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public override int GetHashCode()
    {
      int num = 1;
      if (this.CompressedDisplayName.Length != 0)
        num ^= this.CompressedDisplayName.GetHashCode();
      int hashCode = num ^ this.versions_.GetHashCode();
      if (this.lastModified_ != (Timestamp) null)
        hashCode ^= this.LastModified.GetHashCode();
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
      if (this.CompressedDisplayName.Length != 0)
      {
        output.WriteRawTag((byte) 10);
        output.WriteString(this.CompressedDisplayName);
      }
      this.versions_.WriteTo(ref output, VersionListsPackage._repeated_versions_codec);
      if (this.lastModified_ != (Timestamp) null)
      {
        output.WriteRawTag((byte) 26);
        output.WriteMessage((IMessage) this.LastModified);
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
      if (this.CompressedDisplayName.Length != 0)
        num += 1 + CodedOutputStream.ComputeStringSize(this.CompressedDisplayName);
      int size = num + this.versions_.CalculateSize(VersionListsPackage._repeated_versions_codec);
      if (this.lastModified_ != (Timestamp) null)
        size += 1 + CodedOutputStream.ComputeMessageSize((IMessage) this.LastModified);
      if (this._unknownFields != null)
        size += this._unknownFields.CalculateSize();
      return size;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public void MergeFrom(VersionListsPackage other)
    {
      if (other == null)
        return;
      if (other.CompressedDisplayName.Length != 0)
        this.CompressedDisplayName = other.CompressedDisplayName;
      this.versions_.Add((IEnumerable<VersionListsPackageVersion>) other.versions_);
      if (other.lastModified_ != (Timestamp) null)
      {
        if (this.lastModified_ == (Timestamp) null)
          this.LastModified = new Timestamp();
        this.LastModified.MergeFrom(other.LastModified);
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
            this.CompressedDisplayName = input.ReadString();
            continue;
          case 18:
            this.versions_.AddEntriesFrom(ref input, VersionListsPackage._repeated_versions_codec);
            continue;
          case 26:
            if (this.lastModified_ == (Timestamp) null)
              this.LastModified = new Timestamp();
            input.ReadMessage((IMessage) this.LastModified);
            continue;
          default:
            this._unknownFields = UnknownFieldSet.MergeFieldFrom(this._unknownFields, ref input);
            continue;
        }
      }
    }
  }
}
