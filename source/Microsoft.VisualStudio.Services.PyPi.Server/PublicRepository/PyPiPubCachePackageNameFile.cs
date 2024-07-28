// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.PublicRepository.PyPiPubCachePackageNameFile
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

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
namespace Microsoft.VisualStudio.Services.PyPi.Server.PublicRepository
{
  [PublicRepoCacheDocumentRoot]
  public sealed class PyPiPubCachePackageNameFile : 
    IVersionedDocument,
    IHaveGenerationCursor<PyPiChangelogCursor>,
    IHaveIsDefaultInitialized,
    IMessage<
    #nullable disable
    PyPiPubCachePackageNameFile>,
    IMessage,
    IEquatable<PyPiPubCachePackageNameFile>,
    IDeepCloneable<PyPiPubCachePackageNameFile>,
    IBufferMessage
  {
    private static readonly MessageParser<PyPiPubCachePackageNameFile> _parser = new MessageParser<PyPiPubCachePackageNameFile>((Func<PyPiPubCachePackageNameFile>) (() => new PyPiPubCachePackageNameFile()));
    private UnknownFieldSet _unknownFields;
    public const int ModifiedDateFieldNumber = 1;
    private Timestamp modifiedDate_;
    public const int DisplayNameFieldNumber = 2;
    private string displayName_ = "";
    public const int VersionsFieldNumber = 3;
    private static readonly FieldCodec<PyPiPubCacheVersionLevelInfo> _repeated_versions_codec = FieldCodec.ForMessage<PyPiPubCacheVersionLevelInfo>(26U, PyPiPubCacheVersionLevelInfo.Parser);
    private readonly RepeatedField<PyPiPubCacheVersionLevelInfo> versions_ = new RepeatedField<PyPiPubCacheVersionLevelInfo>();
    public const int DocumentVersionFieldNumber = 4;
    private long documentVersion_;
    public const int LastSerialFieldNumber = 5;
    private static readonly FieldCodec<ulong?> _single_lastSerial_codec = FieldCodec.ForStructWrapper<ulong>(42U);
    private ulong? lastSerial_;

    public 
    #nullable enable
    PyPiChangelogCursor? GenerationCursorPosition
    {
      get => !this.LastSerial.HasValue ? (PyPiChangelogCursor) null : new PyPiChangelogCursor(this.LastSerial.Value);
      set => this.LastSerial = value?.SinceSerial;
    }

    public bool IsDefaultInitialized()
    {
      Timestamp modifiedDate = this.ModifiedDate;
      return (object) modifiedDate != null && modifiedDate.Seconds > 0L;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public static 
    #nullable disable
    MessageParser<PyPiPubCachePackageNameFile> Parser => PyPiPubCachePackageNameFile._parser;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public static MessageDescriptor Descriptor => PyPiPublicRepoCacheReflection.Descriptor.MessageTypes[2];

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    MessageDescriptor IMessage.pb\u003A\u003AGoogle\u002EProtobuf\u002EIMessage\u002EDescriptor => PyPiPubCachePackageNameFile.Descriptor;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public PyPiPubCachePackageNameFile()
    {
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public PyPiPubCachePackageNameFile(PyPiPubCachePackageNameFile other)
      : this()
    {
      this.modifiedDate_ = other.modifiedDate_ != (Timestamp) null ? other.modifiedDate_.Clone() : (Timestamp) null;
      this.displayName_ = other.displayName_;
      this.versions_ = other.versions_.Clone();
      this.documentVersion_ = other.documentVersion_;
      this.LastSerial = other.LastSerial;
      this._unknownFields = UnknownFieldSet.Clone(other._unknownFields);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public PyPiPubCachePackageNameFile Clone() => new PyPiPubCachePackageNameFile(this);

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
    public RepeatedField<PyPiPubCacheVersionLevelInfo> Versions => this.versions_;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public long DocumentVersion
    {
      get => this.documentVersion_;
      set => this.documentVersion_ = value;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public ulong? LastSerial
    {
      get => this.lastSerial_;
      set => this.lastSerial_ = value;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public override bool Equals(object other) => this.Equals(other as PyPiPubCachePackageNameFile);

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public bool Equals(PyPiPubCachePackageNameFile other)
    {
      if (other == null)
        return false;
      if (other == this)
        return true;
      if (!object.Equals((object) this.ModifiedDate, (object) other.ModifiedDate) || this.DisplayName != other.DisplayName || !this.versions_.Equals(other.versions_) || this.DocumentVersion != other.DocumentVersion)
        return false;
      ulong? lastSerial1 = this.LastSerial;
      ulong? lastSerial2 = other.LastSerial;
      return (long) lastSerial1.GetValueOrDefault() == (long) lastSerial2.GetValueOrDefault() & lastSerial1.HasValue == lastSerial2.HasValue && object.Equals((object) this._unknownFields, (object) other._unknownFields);
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
      if (this.lastSerial_.HasValue)
        hashCode ^= this.LastSerial.GetHashCode();
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
      this.versions_.WriteTo(ref output, PyPiPubCachePackageNameFile._repeated_versions_codec);
      if (this.DocumentVersion != 0L)
      {
        output.WriteRawTag((byte) 32);
        output.WriteInt64(this.DocumentVersion);
      }
      if (this.lastSerial_.HasValue)
        PyPiPubCachePackageNameFile._single_lastSerial_codec.WriteTagAndValue(ref output, this.LastSerial);
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
      int size = num + this.versions_.CalculateSize(PyPiPubCachePackageNameFile._repeated_versions_codec);
      if (this.DocumentVersion != 0L)
        size += 1 + CodedOutputStream.ComputeInt64Size(this.DocumentVersion);
      if (this.lastSerial_.HasValue)
        size += PyPiPubCachePackageNameFile._single_lastSerial_codec.CalculateSizeWithTag(this.LastSerial);
      if (this._unknownFields != null)
        size += this._unknownFields.CalculateSize();
      return size;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public void MergeFrom(PyPiPubCachePackageNameFile other)
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
      this.versions_.Add((IEnumerable<PyPiPubCacheVersionLevelInfo>) other.versions_);
      if (other.DocumentVersion != 0L)
        this.DocumentVersion = other.DocumentVersion;
      if (other.lastSerial_.HasValue)
      {
        if (this.lastSerial_.HasValue)
        {
          ulong? lastSerial = other.LastSerial;
          ulong num = 0;
          if ((long) lastSerial.GetValueOrDefault() == (long) num & lastSerial.HasValue)
            goto label_13;
        }
        this.LastSerial = other.LastSerial;
      }
label_13:
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
      uint num1;
      while ((num1 = input.ReadTag()) != 0U)
      {
        switch (num1)
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
            this.versions_.AddEntriesFrom(ref input, PyPiPubCachePackageNameFile._repeated_versions_codec);
            continue;
          case 32:
            this.DocumentVersion = input.ReadInt64();
            continue;
          case 42:
            ulong? nullable1 = PyPiPubCachePackageNameFile._single_lastSerial_codec.Read(ref input);
            if (this.lastSerial_.HasValue)
            {
              ulong? nullable2 = nullable1;
              ulong num2 = 0;
              if ((long) nullable2.GetValueOrDefault() == (long) num2 & nullable2.HasValue)
                continue;
            }
            this.LastSerial = nullable1;
            continue;
          default:
            this._unknownFields = UnknownFieldSet.MergeFieldFrom(this._unknownFields, ref input);
            continue;
        }
      }
    }
  }
}
