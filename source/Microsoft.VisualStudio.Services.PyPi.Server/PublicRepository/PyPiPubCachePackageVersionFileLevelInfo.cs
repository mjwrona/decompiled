// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.PublicRepository.PyPiPubCachePackageVersionFileLevelInfo
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;


#nullable enable
namespace Microsoft.VisualStudio.Services.PyPi.Server.PublicRepository
{
  public sealed class PyPiPubCachePackageVersionFileLevelInfo : 
    IMessage<
    #nullable disable
    PyPiPubCachePackageVersionFileLevelInfo>,
    IMessage,
    IEquatable<PyPiPubCachePackageVersionFileLevelInfo>,
    IDeepCloneable<PyPiPubCachePackageVersionFileLevelInfo>,
    IBufferMessage
  {
    private static readonly MessageParser<PyPiPubCachePackageVersionFileLevelInfo> _parser = new MessageParser<PyPiPubCachePackageVersionFileLevelInfo>((Func<PyPiPubCachePackageVersionFileLevelInfo>) (() => new PyPiPubCachePackageVersionFileLevelInfo()));
    private UnknownFieldSet _unknownFields;
    public const int FilenameFieldNumber = 1;
    private string filename_ = "";
    public const int UrlFieldNumber = 2;
    private string url_ = "";
    public const int HashesFieldNumber = 3;
    private static readonly MapField<string, ByteString>.Codec _map_hashes_codec = new MapField<string, ByteString>.Codec(FieldCodec.ForString(10U, ""), FieldCodec.ForBytes(18U, ByteString.Empty), 26U);
    private readonly MapField<string, ByteString> hashes_ = new MapField<string, ByteString>();
    public const int RequiresPythonFieldNumber = 4;
    private static readonly FieldCodec<string> _single_requiresPython_codec = FieldCodec.ForClassWrapper<string>(34U);
    private string requiresPython_;
    public const int CoreMetadataStateFieldNumber = 5;
    private PyPiPubCachePackageVersionFileLevelInfo.Types.CoreMetadataState coreMetadataState_;
    public const int GpgSigFieldNumber = 6;
    private static readonly FieldCodec<bool?> _single_gpgSig_codec = FieldCodec.ForStructWrapper<bool>(50U);
    private bool? gpgSig_;
    public const int YankStateFieldNumber = 7;
    private PyPiPubCachePackageVersionFileLevelInfo.Types.YankState yankState_;
    public const int UploadTimeFieldNumber = 8;
    private Timestamp uploadTime_;
    public const int SizeFieldNumber = 9;
    private ulong size_;
    public const int DistTypeFieldNumber = 10;
    private static readonly FieldCodec<string> _single_distType_codec = FieldCodec.ForClassWrapper<string>(82U);
    private string distType_;

    public static 
    #nullable enable
    PyPiPubCachePackageVersionFileLevelInfo.Types.YankState? BuildYankState(
      bool? isYanked,
      string? yankedReason)
    {
      if (!isYanked.HasValue)
        return (PyPiPubCachePackageVersionFileLevelInfo.Types.YankState) null;
      PyPiPubCachePackageVersionFileLevelInfo.Types.YankState yankState = new PyPiPubCachePackageVersionFileLevelInfo.Types.YankState();
      yankState.Yanked = isYanked.Value;
      bool? nullable = isYanked;
      bool flag = true;
      yankState.YankedReason = !(nullable.GetValueOrDefault() == flag & nullable.HasValue) || yankedReason == null ? string.Empty : yankedReason;
      return yankState;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public static 
    #nullable disable
    MessageParser<PyPiPubCachePackageVersionFileLevelInfo> Parser => PyPiPubCachePackageVersionFileLevelInfo._parser;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public static MessageDescriptor Descriptor => PyPiPublicRepoCacheReflection.Descriptor.MessageTypes[0];

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    MessageDescriptor IMessage.pb\u003A\u003AGoogle\u002EProtobuf\u002EIMessage\u002EDescriptor => PyPiPubCachePackageVersionFileLevelInfo.Descriptor;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public PyPiPubCachePackageVersionFileLevelInfo()
    {
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public PyPiPubCachePackageVersionFileLevelInfo(PyPiPubCachePackageVersionFileLevelInfo other)
      : this()
    {
      this.filename_ = other.filename_;
      this.url_ = other.url_;
      this.hashes_ = other.hashes_.Clone();
      this.RequiresPython = other.RequiresPython;
      this.coreMetadataState_ = other.coreMetadataState_ != null ? other.coreMetadataState_.Clone() : (PyPiPubCachePackageVersionFileLevelInfo.Types.CoreMetadataState) null;
      this.GpgSig = other.GpgSig;
      this.yankState_ = other.yankState_ != null ? other.yankState_.Clone() : (PyPiPubCachePackageVersionFileLevelInfo.Types.YankState) null;
      this.uploadTime_ = other.uploadTime_ != (Timestamp) null ? other.uploadTime_.Clone() : (Timestamp) null;
      this.size_ = other.size_;
      this.DistType = other.DistType;
      this._unknownFields = UnknownFieldSet.Clone(other._unknownFields);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public PyPiPubCachePackageVersionFileLevelInfo Clone() => new PyPiPubCachePackageVersionFileLevelInfo(this);

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public string Filename
    {
      get => this.filename_;
      set => this.filename_ = ProtoPreconditions.CheckNotNull<string>(value, nameof (value));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public string Url
    {
      get => this.url_;
      set => this.url_ = ProtoPreconditions.CheckNotNull<string>(value, nameof (value));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public MapField<string, ByteString> Hashes => this.hashes_;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public string RequiresPython
    {
      get => this.requiresPython_;
      set => this.requiresPython_ = value;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public PyPiPubCachePackageVersionFileLevelInfo.Types.CoreMetadataState CoreMetadataState
    {
      get => this.coreMetadataState_;
      set => this.coreMetadataState_ = value;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public bool? GpgSig
    {
      get => this.gpgSig_;
      set => this.gpgSig_ = value;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public PyPiPubCachePackageVersionFileLevelInfo.Types.YankState YankState
    {
      get => this.yankState_;
      set => this.yankState_ = value;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public Timestamp UploadTime
    {
      get => this.uploadTime_;
      set => this.uploadTime_ = value;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public ulong Size
    {
      get => this.size_;
      set => this.size_ = value;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public string DistType
    {
      get => this.distType_;
      set => this.distType_ = value;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public override bool Equals(object other) => this.Equals(other as PyPiPubCachePackageVersionFileLevelInfo);

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public bool Equals(PyPiPubCachePackageVersionFileLevelInfo other)
    {
      if (other == null)
        return false;
      if (other == this)
        return true;
      if (this.Filename != other.Filename || this.Url != other.Url || !this.Hashes.Equals(other.Hashes) || this.RequiresPython != other.RequiresPython || !object.Equals((object) this.CoreMetadataState, (object) other.CoreMetadataState))
        return false;
      bool? gpgSig1 = this.GpgSig;
      bool? gpgSig2 = other.GpgSig;
      return gpgSig1.GetValueOrDefault() == gpgSig2.GetValueOrDefault() & gpgSig1.HasValue == gpgSig2.HasValue && object.Equals((object) this.YankState, (object) other.YankState) && object.Equals((object) this.UploadTime, (object) other.UploadTime) && (long) this.Size == (long) other.Size && !(this.DistType != other.DistType) && object.Equals((object) this._unknownFields, (object) other._unknownFields);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public override int GetHashCode()
    {
      int num = 1;
      if (this.Filename.Length != 0)
        num ^= this.Filename.GetHashCode();
      if (this.Url.Length != 0)
        num ^= this.Url.GetHashCode();
      int hashCode = num ^ this.Hashes.GetHashCode();
      if (this.requiresPython_ != null)
        hashCode ^= this.RequiresPython.GetHashCode();
      if (this.coreMetadataState_ != null)
        hashCode ^= this.CoreMetadataState.GetHashCode();
      if (this.gpgSig_.HasValue)
        hashCode ^= this.GpgSig.GetHashCode();
      if (this.yankState_ != null)
        hashCode ^= this.YankState.GetHashCode();
      if (this.uploadTime_ != (Timestamp) null)
        hashCode ^= this.UploadTime.GetHashCode();
      if (this.Size != 0UL)
        hashCode ^= this.Size.GetHashCode();
      if (this.distType_ != null)
        hashCode ^= this.DistType.GetHashCode();
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
      if (this.Filename.Length != 0)
      {
        output.WriteRawTag((byte) 10);
        output.WriteString(this.Filename);
      }
      if (this.Url.Length != 0)
      {
        output.WriteRawTag((byte) 18);
        output.WriteString(this.Url);
      }
      this.hashes_.WriteTo(ref output, PyPiPubCachePackageVersionFileLevelInfo._map_hashes_codec);
      if (this.requiresPython_ != null)
        PyPiPubCachePackageVersionFileLevelInfo._single_requiresPython_codec.WriteTagAndValue(ref output, this.RequiresPython);
      if (this.coreMetadataState_ != null)
      {
        output.WriteRawTag((byte) 42);
        output.WriteMessage((IMessage) this.CoreMetadataState);
      }
      if (this.gpgSig_.HasValue)
        PyPiPubCachePackageVersionFileLevelInfo._single_gpgSig_codec.WriteTagAndValue(ref output, this.GpgSig);
      if (this.yankState_ != null)
      {
        output.WriteRawTag((byte) 58);
        output.WriteMessage((IMessage) this.YankState);
      }
      if (this.uploadTime_ != (Timestamp) null)
      {
        output.WriteRawTag((byte) 66);
        output.WriteMessage((IMessage) this.UploadTime);
      }
      if (this.Size != 0UL)
      {
        output.WriteRawTag((byte) 72);
        output.WriteUInt64(this.Size);
      }
      if (this.distType_ != null)
        PyPiPubCachePackageVersionFileLevelInfo._single_distType_codec.WriteTagAndValue(ref output, this.DistType);
      if (this._unknownFields == null)
        return;
      this._unknownFields.WriteTo(ref output);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public int CalculateSize()
    {
      int num = 0;
      if (this.Filename.Length != 0)
        num += 1 + CodedOutputStream.ComputeStringSize(this.Filename);
      if (this.Url.Length != 0)
        num += 1 + CodedOutputStream.ComputeStringSize(this.Url);
      int size = num + this.hashes_.CalculateSize(PyPiPubCachePackageVersionFileLevelInfo._map_hashes_codec);
      if (this.requiresPython_ != null)
        size += PyPiPubCachePackageVersionFileLevelInfo._single_requiresPython_codec.CalculateSizeWithTag(this.RequiresPython);
      if (this.coreMetadataState_ != null)
        size += 1 + CodedOutputStream.ComputeMessageSize((IMessage) this.CoreMetadataState);
      if (this.gpgSig_.HasValue)
        size += PyPiPubCachePackageVersionFileLevelInfo._single_gpgSig_codec.CalculateSizeWithTag(this.GpgSig);
      if (this.yankState_ != null)
        size += 1 + CodedOutputStream.ComputeMessageSize((IMessage) this.YankState);
      if (this.uploadTime_ != (Timestamp) null)
        size += 1 + CodedOutputStream.ComputeMessageSize((IMessage) this.UploadTime);
      if (this.Size != 0UL)
        size += 1 + CodedOutputStream.ComputeUInt64Size(this.Size);
      if (this.distType_ != null)
        size += PyPiPubCachePackageVersionFileLevelInfo._single_distType_codec.CalculateSizeWithTag(this.DistType);
      if (this._unknownFields != null)
        size += this._unknownFields.CalculateSize();
      return size;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public void MergeFrom(PyPiPubCachePackageVersionFileLevelInfo other)
    {
      if (other == null)
        return;
      if (other.Filename.Length != 0)
        this.Filename = other.Filename;
      if (other.Url.Length != 0)
        this.Url = other.Url;
      this.hashes_.MergeFrom((IDictionary<string, ByteString>) other.hashes_);
      if (other.requiresPython_ != null && (this.requiresPython_ == null || other.RequiresPython != ""))
        this.RequiresPython = other.RequiresPython;
      if (other.coreMetadataState_ != null)
      {
        if (this.coreMetadataState_ == null)
          this.CoreMetadataState = new PyPiPubCachePackageVersionFileLevelInfo.Types.CoreMetadataState();
        this.CoreMetadataState.MergeFrom(other.CoreMetadataState);
      }
      if (other.gpgSig_.HasValue)
      {
        if (this.gpgSig_.HasValue)
        {
          bool? gpgSig = other.GpgSig;
          bool flag = false;
          if (gpgSig.GetValueOrDefault() == flag & gpgSig.HasValue)
            goto label_15;
        }
        this.GpgSig = other.GpgSig;
      }
label_15:
      if (other.yankState_ != null)
      {
        if (this.yankState_ == null)
          this.YankState = new PyPiPubCachePackageVersionFileLevelInfo.Types.YankState();
        this.YankState.MergeFrom(other.YankState);
      }
      if (other.uploadTime_ != (Timestamp) null)
      {
        if (this.uploadTime_ == (Timestamp) null)
          this.UploadTime = new Timestamp();
        this.UploadTime.MergeFrom(other.UploadTime);
      }
      if (other.Size != 0UL)
        this.Size = other.Size;
      if (other.distType_ != null && (this.distType_ == null || other.DistType != ""))
        this.DistType = other.DistType;
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
            this.Filename = input.ReadString();
            continue;
          case 18:
            this.Url = input.ReadString();
            continue;
          case 26:
            this.hashes_.AddEntriesFrom(ref input, PyPiPubCachePackageVersionFileLevelInfo._map_hashes_codec);
            continue;
          case 34:
            string str1 = PyPiPubCachePackageVersionFileLevelInfo._single_requiresPython_codec.Read(ref input);
            if (this.requiresPython_ == null || str1 != "")
            {
              this.RequiresPython = str1;
              continue;
            }
            continue;
          case 42:
            if (this.coreMetadataState_ == null)
              this.CoreMetadataState = new PyPiPubCachePackageVersionFileLevelInfo.Types.CoreMetadataState();
            input.ReadMessage((IMessage) this.CoreMetadataState);
            continue;
          case 50:
            bool? nullable1 = PyPiPubCachePackageVersionFileLevelInfo._single_gpgSig_codec.Read(ref input);
            if (this.gpgSig_.HasValue)
            {
              bool? nullable2 = nullable1;
              bool flag = false;
              if (nullable2.GetValueOrDefault() == flag & nullable2.HasValue)
                continue;
            }
            this.GpgSig = nullable1;
            continue;
          case 58:
            if (this.yankState_ == null)
              this.YankState = new PyPiPubCachePackageVersionFileLevelInfo.Types.YankState();
            input.ReadMessage((IMessage) this.YankState);
            continue;
          case 66:
            if (this.uploadTime_ == (Timestamp) null)
              this.UploadTime = new Timestamp();
            input.ReadMessage((IMessage) this.UploadTime);
            continue;
          case 72:
            this.Size = input.ReadUInt64();
            continue;
          case 82:
            string str2 = PyPiPubCachePackageVersionFileLevelInfo._single_distType_codec.Read(ref input);
            if (this.distType_ == null || str2 != "")
            {
              this.DistType = str2;
              continue;
            }
            continue;
          default:
            this._unknownFields = UnknownFieldSet.MergeFieldFrom(this._unknownFields, ref input);
            continue;
        }
      }
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public static class Types
    {
      public sealed class CoreMetadataState : 
        IMessage<PyPiPubCachePackageVersionFileLevelInfo.Types.CoreMetadataState>,
        IMessage,
        IEquatable<PyPiPubCachePackageVersionFileLevelInfo.Types.CoreMetadataState>,
        IDeepCloneable<PyPiPubCachePackageVersionFileLevelInfo.Types.CoreMetadataState>,
        IBufferMessage
      {
        private static readonly MessageParser<PyPiPubCachePackageVersionFileLevelInfo.Types.CoreMetadataState> _parser = new MessageParser<PyPiPubCachePackageVersionFileLevelInfo.Types.CoreMetadataState>((Func<PyPiPubCachePackageVersionFileLevelInfo.Types.CoreMetadataState>) (() => new PyPiPubCachePackageVersionFileLevelInfo.Types.CoreMetadataState()));
        private UnknownFieldSet _unknownFields;
        public const int HasCoreMetadataFieldNumber = 1;
        private bool hasCoreMetadata_;
        public const int HashesFieldNumber = 2;
        private static readonly MapField<string, ByteString>.Codec _map_hashes_codec = new MapField<string, ByteString>.Codec(FieldCodec.ForString(10U, ""), FieldCodec.ForBytes(18U, ByteString.Empty), 18U);
        private readonly MapField<string, ByteString> hashes_ = new MapField<string, ByteString>();

        [DebuggerNonUserCode]
        [GeneratedCode("protoc", null)]
        public static MessageParser<PyPiPubCachePackageVersionFileLevelInfo.Types.CoreMetadataState> Parser => PyPiPubCachePackageVersionFileLevelInfo.Types.CoreMetadataState._parser;

        [DebuggerNonUserCode]
        [GeneratedCode("protoc", null)]
        public static MessageDescriptor Descriptor => PyPiPubCachePackageVersionFileLevelInfo.Descriptor.NestedTypes[1];

        [DebuggerNonUserCode]
        [GeneratedCode("protoc", null)]
        MessageDescriptor IMessage.pb\u003A\u003AGoogle\u002EProtobuf\u002EIMessage\u002EDescriptor => PyPiPubCachePackageVersionFileLevelInfo.Types.CoreMetadataState.Descriptor;

        [DebuggerNonUserCode]
        [GeneratedCode("protoc", null)]
        public CoreMetadataState()
        {
        }

        [DebuggerNonUserCode]
        [GeneratedCode("protoc", null)]
        public CoreMetadataState(
          PyPiPubCachePackageVersionFileLevelInfo.Types.CoreMetadataState other)
          : this()
        {
          this.hasCoreMetadata_ = other.hasCoreMetadata_;
          this.hashes_ = other.hashes_.Clone();
          this._unknownFields = UnknownFieldSet.Clone(other._unknownFields);
        }

        [DebuggerNonUserCode]
        [GeneratedCode("protoc", null)]
        public PyPiPubCachePackageVersionFileLevelInfo.Types.CoreMetadataState Clone() => new PyPiPubCachePackageVersionFileLevelInfo.Types.CoreMetadataState(this);

        [DebuggerNonUserCode]
        [GeneratedCode("protoc", null)]
        public bool HasCoreMetadata
        {
          get => this.hasCoreMetadata_;
          set => this.hasCoreMetadata_ = value;
        }

        [DebuggerNonUserCode]
        [GeneratedCode("protoc", null)]
        public MapField<string, ByteString> Hashes => this.hashes_;

        [DebuggerNonUserCode]
        [GeneratedCode("protoc", null)]
        public override bool Equals(object other) => this.Equals(other as PyPiPubCachePackageVersionFileLevelInfo.Types.CoreMetadataState);

        [DebuggerNonUserCode]
        [GeneratedCode("protoc", null)]
        public bool Equals(
          PyPiPubCachePackageVersionFileLevelInfo.Types.CoreMetadataState other)
        {
          if (other == null)
            return false;
          if (other == this)
            return true;
          return this.HasCoreMetadata == other.HasCoreMetadata && this.Hashes.Equals(other.Hashes) && object.Equals((object) this._unknownFields, (object) other._unknownFields);
        }

        [DebuggerNonUserCode]
        [GeneratedCode("protoc", null)]
        public override int GetHashCode()
        {
          int num = 1;
          if (this.HasCoreMetadata)
            num ^= this.HasCoreMetadata.GetHashCode();
          int hashCode = num ^ this.Hashes.GetHashCode();
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
          if (this.HasCoreMetadata)
          {
            output.WriteRawTag((byte) 8);
            output.WriteBool(this.HasCoreMetadata);
          }
          this.hashes_.WriteTo(ref output, PyPiPubCachePackageVersionFileLevelInfo.Types.CoreMetadataState._map_hashes_codec);
          if (this._unknownFields == null)
            return;
          this._unknownFields.WriteTo(ref output);
        }

        [DebuggerNonUserCode]
        [GeneratedCode("protoc", null)]
        public int CalculateSize()
        {
          int num = 0;
          if (this.HasCoreMetadata)
            num += 2;
          int size = num + this.hashes_.CalculateSize(PyPiPubCachePackageVersionFileLevelInfo.Types.CoreMetadataState._map_hashes_codec);
          if (this._unknownFields != null)
            size += this._unknownFields.CalculateSize();
          return size;
        }

        [DebuggerNonUserCode]
        [GeneratedCode("protoc", null)]
        public void MergeFrom(
          PyPiPubCachePackageVersionFileLevelInfo.Types.CoreMetadataState other)
        {
          if (other == null)
            return;
          if (other.HasCoreMetadata)
            this.HasCoreMetadata = other.HasCoreMetadata;
          this.hashes_.MergeFrom((IDictionary<string, ByteString>) other.hashes_);
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
              case 8:
                this.HasCoreMetadata = input.ReadBool();
                continue;
              case 18:
                this.hashes_.AddEntriesFrom(ref input, PyPiPubCachePackageVersionFileLevelInfo.Types.CoreMetadataState._map_hashes_codec);
                continue;
              default:
                this._unknownFields = UnknownFieldSet.MergeFieldFrom(this._unknownFields, ref input);
                continue;
            }
          }
        }
      }

      public sealed class YankState : 
        IMessage<PyPiPubCachePackageVersionFileLevelInfo.Types.YankState>,
        IMessage,
        IEquatable<PyPiPubCachePackageVersionFileLevelInfo.Types.YankState>,
        IDeepCloneable<PyPiPubCachePackageVersionFileLevelInfo.Types.YankState>,
        IBufferMessage
      {
        private static readonly MessageParser<PyPiPubCachePackageVersionFileLevelInfo.Types.YankState> _parser = new MessageParser<PyPiPubCachePackageVersionFileLevelInfo.Types.YankState>((Func<PyPiPubCachePackageVersionFileLevelInfo.Types.YankState>) (() => new PyPiPubCachePackageVersionFileLevelInfo.Types.YankState()));
        private UnknownFieldSet _unknownFields;
        public const int YankedFieldNumber = 1;
        private bool yanked_;
        public const int YankedReasonFieldNumber = 2;
        private string yankedReason_ = "";

        [DebuggerNonUserCode]
        [GeneratedCode("protoc", null)]
        public static MessageParser<PyPiPubCachePackageVersionFileLevelInfo.Types.YankState> Parser => PyPiPubCachePackageVersionFileLevelInfo.Types.YankState._parser;

        [DebuggerNonUserCode]
        [GeneratedCode("protoc", null)]
        public static MessageDescriptor Descriptor => PyPiPubCachePackageVersionFileLevelInfo.Descriptor.NestedTypes[2];

        [DebuggerNonUserCode]
        [GeneratedCode("protoc", null)]
        MessageDescriptor IMessage.pb\u003A\u003AGoogle\u002EProtobuf\u002EIMessage\u002EDescriptor => PyPiPubCachePackageVersionFileLevelInfo.Types.YankState.Descriptor;

        [DebuggerNonUserCode]
        [GeneratedCode("protoc", null)]
        public YankState()
        {
        }

        [DebuggerNonUserCode]
        [GeneratedCode("protoc", null)]
        public YankState(
          PyPiPubCachePackageVersionFileLevelInfo.Types.YankState other)
          : this()
        {
          this.yanked_ = other.yanked_;
          this.yankedReason_ = other.yankedReason_;
          this._unknownFields = UnknownFieldSet.Clone(other._unknownFields);
        }

        [DebuggerNonUserCode]
        [GeneratedCode("protoc", null)]
        public PyPiPubCachePackageVersionFileLevelInfo.Types.YankState Clone() => new PyPiPubCachePackageVersionFileLevelInfo.Types.YankState(this);

        [DebuggerNonUserCode]
        [GeneratedCode("protoc", null)]
        public bool Yanked
        {
          get => this.yanked_;
          set => this.yanked_ = value;
        }

        [DebuggerNonUserCode]
        [GeneratedCode("protoc", null)]
        public string YankedReason
        {
          get => this.yankedReason_;
          set => this.yankedReason_ = ProtoPreconditions.CheckNotNull<string>(value, nameof (value));
        }

        [DebuggerNonUserCode]
        [GeneratedCode("protoc", null)]
        public override bool Equals(object other) => this.Equals(other as PyPiPubCachePackageVersionFileLevelInfo.Types.YankState);

        [DebuggerNonUserCode]
        [GeneratedCode("protoc", null)]
        public bool Equals(
          PyPiPubCachePackageVersionFileLevelInfo.Types.YankState other)
        {
          if (other == null)
            return false;
          if (other == this)
            return true;
          return this.Yanked == other.Yanked && !(this.YankedReason != other.YankedReason) && object.Equals((object) this._unknownFields, (object) other._unknownFields);
        }

        [DebuggerNonUserCode]
        [GeneratedCode("protoc", null)]
        public override int GetHashCode()
        {
          int hashCode = 1;
          if (this.Yanked)
            hashCode ^= this.Yanked.GetHashCode();
          if (this.YankedReason.Length != 0)
            hashCode ^= this.YankedReason.GetHashCode();
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
          if (this.Yanked)
          {
            output.WriteRawTag((byte) 8);
            output.WriteBool(this.Yanked);
          }
          if (this.YankedReason.Length != 0)
          {
            output.WriteRawTag((byte) 18);
            output.WriteString(this.YankedReason);
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
          if (this.Yanked)
            size += 2;
          if (this.YankedReason.Length != 0)
            size += 1 + CodedOutputStream.ComputeStringSize(this.YankedReason);
          if (this._unknownFields != null)
            size += this._unknownFields.CalculateSize();
          return size;
        }

        [DebuggerNonUserCode]
        [GeneratedCode("protoc", null)]
        public void MergeFrom(
          PyPiPubCachePackageVersionFileLevelInfo.Types.YankState other)
        {
          if (other == null)
            return;
          if (other.Yanked)
            this.Yanked = other.Yanked;
          if (other.YankedReason.Length != 0)
            this.YankedReason = other.YankedReason;
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
              case 8:
                this.Yanked = input.ReadBool();
                continue;
              case 18:
                this.YankedReason = input.ReadString();
                continue;
              default:
                this._unknownFields = UnknownFieldSet.MergeFieldFrom(this._unknownFields, ref input);
                continue;
            }
          }
        }
      }
    }
  }
}
