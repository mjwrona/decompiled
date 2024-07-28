// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PublicRepositories.NuGetPubCachePackageNameFile
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server.PublicRepositories
{
  [PublicRepoCacheDocumentRoot]
  public sealed class NuGetPubCachePackageNameFile : 
    IMessage<
    #nullable disable
    NuGetPubCachePackageNameFile>,
    IMessage,
    IEquatable<NuGetPubCachePackageNameFile>,
    IDeepCloneable<NuGetPubCachePackageNameFile>,
    IBufferMessage,
    IVersionedDocument,
    IHaveGenerationCursor<
    #nullable enable
    NuGetCatalogCursor>,
    IHaveIsDefaultInitialized
  {
    private static readonly 
    #nullable disable
    MessageParser<NuGetPubCachePackageNameFile> _parser = new MessageParser<NuGetPubCachePackageNameFile>((Func<NuGetPubCachePackageNameFile>) (() => new NuGetPubCachePackageNameFile()));
    private UnknownFieldSet _unknownFields;
    public const int ModifiedDateFieldNumber = 1;
    private Timestamp modifiedDate_;
    public const int DisplayNameFieldNumber = 2;
    private string displayName_ = "";
    public const int VersionsFieldNumber = 3;
    private static readonly FieldCodec<NuGetPubCacheVersionLevelInfo> _repeated_versions_codec = FieldCodec.ForMessage<NuGetPubCacheVersionLevelInfo>(26U, NuGetPubCacheVersionLevelInfo.Parser);
    private readonly RepeatedField<NuGetPubCacheVersionLevelInfo> versions_ = new RepeatedField<NuGetPubCacheVersionLevelInfo>();
    public const int DocumentVersionFieldNumber = 4;
    private long documentVersion_;
    public const int CommitTimestampFieldNumber = 5;
    private Timestamp commitTimestamp_;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public static MessageParser<NuGetPubCachePackageNameFile> Parser => NuGetPubCachePackageNameFile._parser;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public static MessageDescriptor Descriptor => NugetPublicRepoCacheReflection.Descriptor.MessageTypes[6];

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    MessageDescriptor IMessage.pb\u003A\u003AGoogle\u002EProtobuf\u002EIMessage\u002EDescriptor => NuGetPubCachePackageNameFile.Descriptor;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public NuGetPubCachePackageNameFile()
    {
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public NuGetPubCachePackageNameFile(NuGetPubCachePackageNameFile other)
      : this()
    {
      this.modifiedDate_ = other.modifiedDate_ != (Timestamp) null ? other.modifiedDate_.Clone() : (Timestamp) null;
      this.displayName_ = other.displayName_;
      this.versions_ = other.versions_.Clone();
      this.documentVersion_ = other.documentVersion_;
      this.commitTimestamp_ = other.commitTimestamp_ != (Timestamp) null ? other.commitTimestamp_.Clone() : (Timestamp) null;
      this._unknownFields = UnknownFieldSet.Clone(other._unknownFields);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public NuGetPubCachePackageNameFile Clone() => new NuGetPubCachePackageNameFile(this);

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public Timestamp ModifiedDate
    {
      get => this.modifiedDate_;
      set => this.modifiedDate_ = value;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public string DisplayName
    {
      get => this.displayName_;
      set => this.displayName_ = ProtoPreconditions.CheckNotNull<string>(value, nameof (value));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public RepeatedField<NuGetPubCacheVersionLevelInfo> Versions => this.versions_;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public long DocumentVersion
    {
      get => this.documentVersion_;
      set => this.documentVersion_ = value;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public Timestamp CommitTimestamp
    {
      get => this.commitTimestamp_;
      set => this.commitTimestamp_ = value;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public override bool Equals(object other) => this.Equals(other as NuGetPubCachePackageNameFile);

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public bool Equals(NuGetPubCachePackageNameFile other)
    {
      if (other == null)
        return false;
      if (other == this)
        return true;
      return object.Equals((object) this.ModifiedDate, (object) other.ModifiedDate) && !(this.DisplayName != other.DisplayName) && this.versions_.Equals(other.versions_) && this.DocumentVersion == other.DocumentVersion && object.Equals((object) this.CommitTimestamp, (object) other.CommitTimestamp) && object.Equals((object) this._unknownFields, (object) other._unknownFields);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public override int GetHashCode()
    {
      int num = 1;
      if (this.modifiedDate_ != (Timestamp) null)
        num ^= this.ModifiedDate.GetHashCode();
      if (this.DisplayName.Length != 0)
        num ^= this.DisplayName.GetHashCode();
      int hashCode = num ^ this.versions_.GetHashCode();
      if (this.DocumentVersion != 0L)
        hashCode ^= this.DocumentVersion.GetHashCode();
      if (this.commitTimestamp_ != (Timestamp) null)
        hashCode ^= this.CommitTimestamp.GetHashCode();
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
      if (this.modifiedDate_ != (Timestamp) null)
      {
        output.WriteRawTag((byte) 10);
        output.WriteMessage((IMessage) this.ModifiedDate);
      }
      if (this.DisplayName.Length != 0)
      {
        output.WriteRawTag((byte) 18);
        output.WriteString(this.DisplayName);
      }
      this.versions_.WriteTo(ref output, NuGetPubCachePackageNameFile._repeated_versions_codec);
      if (this.DocumentVersion != 0L)
      {
        output.WriteRawTag((byte) 32);
        output.WriteInt64(this.DocumentVersion);
      }
      if (this.commitTimestamp_ != (Timestamp) null)
      {
        output.WriteRawTag((byte) 42);
        output.WriteMessage((IMessage) this.CommitTimestamp);
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
      if (this.modifiedDate_ != (Timestamp) null)
        num += 1 + CodedOutputStream.ComputeMessageSize((IMessage) this.ModifiedDate);
      if (this.DisplayName.Length != 0)
        num += 1 + CodedOutputStream.ComputeStringSize(this.DisplayName);
      int size = num + this.versions_.CalculateSize(NuGetPubCachePackageNameFile._repeated_versions_codec);
      if (this.DocumentVersion != 0L)
        size += 1 + CodedOutputStream.ComputeInt64Size(this.DocumentVersion);
      if (this.commitTimestamp_ != (Timestamp) null)
        size += 1 + CodedOutputStream.ComputeMessageSize((IMessage) this.CommitTimestamp);
      if (this._unknownFields != null)
        size += this._unknownFields.CalculateSize();
      return size;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public void MergeFrom(NuGetPubCachePackageNameFile other)
    {
      if (other == null)
        return;
      if (other.modifiedDate_ != (Timestamp) null)
      {
        if (this.modifiedDate_ == (Timestamp) null)
          this.ModifiedDate = new Timestamp();
        this.ModifiedDate.MergeFrom(other.ModifiedDate);
      }
      if (other.DisplayName.Length != 0)
        this.DisplayName = other.DisplayName;
      this.versions_.Add((IEnumerable<NuGetPubCacheVersionLevelInfo>) other.versions_);
      if (other.DocumentVersion != 0L)
        this.DocumentVersion = other.DocumentVersion;
      if (other.commitTimestamp_ != (Timestamp) null)
      {
        if (this.commitTimestamp_ == (Timestamp) null)
          this.CommitTimestamp = new Timestamp();
        this.CommitTimestamp.MergeFrom(other.CommitTimestamp);
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
            if (this.modifiedDate_ == (Timestamp) null)
              this.ModifiedDate = new Timestamp();
            input.ReadMessage((IMessage) this.ModifiedDate);
            continue;
          case 18:
            this.DisplayName = input.ReadString();
            continue;
          case 26:
            this.versions_.AddEntriesFrom(ref input, NuGetPubCachePackageNameFile._repeated_versions_codec);
            continue;
          case 32:
            this.DocumentVersion = input.ReadInt64();
            continue;
          case 42:
            if (this.commitTimestamp_ == (Timestamp) null)
              this.CommitTimestamp = new Timestamp();
            input.ReadMessage((IMessage) this.CommitTimestamp);
            continue;
          default:
            this._unknownFields = UnknownFieldSet.MergeFieldFrom(this._unknownFields, ref input);
            continue;
        }
      }
    }

    public 
    #nullable enable
    NuGetCatalogCursor? GenerationCursorPosition
    {
      get => (object) this.CommitTimestamp == null ? (NuGetCatalogCursor) null : new NuGetCatalogCursor(this.CommitTimestamp.ToDateTimeOffset());
      set => this.CommitTimestamp = (object) value != null ? Timestamp.FromDateTimeOffset(value.Value) : (Timestamp) null;
    }

    public bool IsDefaultInitialized()
    {
      Timestamp modifiedDate = this.ModifiedDate;
      return (object) modifiedDate != null && modifiedDate.Seconds > 0L;
    }
  }
}
