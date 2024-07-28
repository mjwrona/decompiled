// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.VersionListWithSize.VersionListWithSizePackage
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.VersionListWithSize
{
  public sealed class VersionListWithSizePackage : 
    IMessage<VersionListWithSizePackage>,
    IMessage,
    IEquatable<VersionListWithSizePackage>,
    IDeepCloneable<VersionListWithSizePackage>,
    IBufferMessage,
    IVersionListWithSizePackage,
    ILazyVersionListWithSizePackage
  {
    private static readonly MessageParser<VersionListWithSizePackage> _parser = new MessageParser<VersionListWithSizePackage>((Func<VersionListWithSizePackage>) (() => new VersionListWithSizePackage()));
    private UnknownFieldSet _unknownFields;
    public const int CompressedDisplayNameFieldNumber = 1;
    private string compressedDisplayName_ = "";
    public const int VersionsFieldNumber = 2;
    private static readonly FieldCodec<VersionListWithSizePackageVersion> _repeated_versions_codec = FieldCodec.ForMessage<VersionListWithSizePackageVersion>(18U, VersionListWithSizePackageVersion.Parser);
    private readonly RepeatedField<VersionListWithSizePackageVersion> versions_ = new RepeatedField<VersionListWithSizePackageVersion>();
    public const int LastModifiedFieldNumber = 3;
    private Timestamp lastModified_;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public static MessageParser<VersionListWithSizePackage> Parser => VersionListWithSizePackage._parser;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public static MessageDescriptor Descriptor => VersionListWithSizeReflection.Descriptor.MessageTypes[2];

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    MessageDescriptor IMessage.pb\u003A\u003AGoogle\u002EProtobuf\u002EIMessage\u002EDescriptor => VersionListWithSizePackage.Descriptor;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public VersionListWithSizePackage()
    {
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public VersionListWithSizePackage(VersionListWithSizePackage other)
      : this()
    {
      this.compressedDisplayName_ = other.compressedDisplayName_;
      this.versions_ = other.versions_.Clone();
      this.lastModified_ = other.lastModified_ != (Timestamp) null ? other.lastModified_.Clone() : (Timestamp) null;
      this._unknownFields = UnknownFieldSet.Clone(other._unknownFields);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public VersionListWithSizePackage Clone() => new VersionListWithSizePackage(this);

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public string CompressedDisplayName
    {
      get => this.compressedDisplayName_;
      set => this.compressedDisplayName_ = ProtoPreconditions.CheckNotNull<string>(value, nameof (value));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public RepeatedField<VersionListWithSizePackageVersion> Versions => this.versions_;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public Timestamp LastModified
    {
      get => this.lastModified_;
      set => this.lastModified_ = value;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public override bool Equals(object other) => this.Equals(other as VersionListWithSizePackage);

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public bool Equals(VersionListWithSizePackage other)
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
      this.versions_.WriteTo(ref output, VersionListWithSizePackage._repeated_versions_codec);
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
      int size = num + this.versions_.CalculateSize(VersionListWithSizePackage._repeated_versions_codec);
      if (this.lastModified_ != (Timestamp) null)
        size += 1 + CodedOutputStream.ComputeMessageSize((IMessage) this.LastModified);
      if (this._unknownFields != null)
        size += this._unknownFields.CalculateSize();
      return size;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public void MergeFrom(VersionListWithSizePackage other)
    {
      if (other == null)
        return;
      if (other.CompressedDisplayName.Length != 0)
        this.CompressedDisplayName = other.CompressedDisplayName;
      this.versions_.Add((IEnumerable<VersionListWithSizePackageVersion>) other.versions_);
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
            this.versions_.AddEntriesFrom(ref input, VersionListWithSizePackage._repeated_versions_codec);
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

    public bool VersionsNeedUnpack { get; private set; } = true;

    public bool VersionsNeedRepack { get; private set; }

    public bool NeedsSave { get; private set; }

    public void NotifyVersionsUnpacked()
    {
      this.VersionsNeedUnpack = false;
      this.VersionListWithSizeFile.NotifyContainedPackageUnpacked();
    }

    public void NotifyVersionsRepacked()
    {
      this.VersionsNeedRepack = false;
      this.VersionListWithSizeFile.NotifyContainedPackagePacked();
    }

    public void NotifySaved() => this.NeedsSave = false;

    public void NotifyModified(DateTime modTime)
    {
      if (modTime > this.LastModifiedAsDateTime)
        this.LastModifiedAsDateTime = modTime;
      this.VersionsNeedRepack = true;
      this.NeedsSave = true;
    }

    public string DisplayName { get; set; }

    DateTime IVersionListWithSizePackage.LastModified => this.LastModifiedAsDateTime;

    IReadOnlyList<IVersionListWithSizePackageVersion> IVersionListWithSizePackage.Versions => (IReadOnlyList<IVersionListWithSizePackageVersion>) this.Versions;

    IVersionListWithSizePackage ILazyVersionListWithSizePackage.Get()
    {
      VersionListWithSizeFileUnpacker.EnsurePackageUnpacked(this);
      return (IVersionListWithSizePackage) this;
    }

    public VersionListWithSizeFile VersionListWithSizeFile { get; set; }

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

    public static VersionListWithSizePackage CreateNewUnpacked(
      string displayName,
      VersionListWithSizeFile wrapped,
      DateTime lastModified)
    {
      return new VersionListWithSizePackage()
      {
        DisplayName = displayName,
        VersionListWithSizeFile = wrapped,
        LastModifiedAsDateTime = lastModified,
        NeedsSave = true,
        VersionsNeedUnpack = false,
        VersionsNeedRepack = true
      };
    }
  }
}
