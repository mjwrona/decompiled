// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.PublicRepository.PyPiPubCacheVersionLevelInfo
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;


#nullable enable
namespace Microsoft.VisualStudio.Services.PyPi.Server.PublicRepository
{
  public sealed class PyPiPubCacheVersionLevelInfo : 
    IMessage<
    #nullable disable
    PyPiPubCacheVersionLevelInfo>,
    IMessage,
    IEquatable<PyPiPubCacheVersionLevelInfo>,
    IDeepCloneable<PyPiPubCacheVersionLevelInfo>,
    IBufferMessage
  {
    private 
    #nullable enable
    PyPiPackageIdentity? identity;
    private static readonly 
    #nullable disable
    MessageParser<PyPiPubCacheVersionLevelInfo> _parser = new MessageParser<PyPiPubCacheVersionLevelInfo>((Func<PyPiPubCacheVersionLevelInfo>) (() => new PyPiPubCacheVersionLevelInfo()));
    private UnknownFieldSet _unknownFields;
    public const int DisplayNameFieldNumber = 1;
    private string displayName_ = "";
    public const int DisplayVersionFieldNumber = 2;
    private string displayVersion_ = "";
    public const int FilesFieldNumber = 3;
    private static readonly FieldCodec<PyPiPubCachePackageVersionFileLevelInfo> _repeated_files_codec = FieldCodec.ForMessage<PyPiPubCachePackageVersionFileLevelInfo>(26U, PyPiPubCachePackageVersionFileLevelInfo.Parser);
    private readonly RepeatedField<PyPiPubCachePackageVersionFileLevelInfo> files_ = new RepeatedField<PyPiPubCachePackageVersionFileLevelInfo>();

    public 
    #nullable enable
    PyPiPackageIdentity Identity
    {
      get => this.identity ?? (this.identity = PyPiIdentityResolver.Instance.ResolvePackageIdentity(this.DisplayName, this.DisplayVersion));
      set
      {
        this.identity = value;
        this.DisplayName = this.identity.Name.DisplayName;
        this.DisplayVersion = this.identity.Version.DisplayVersion;
      }
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public static 
    #nullable disable
    MessageParser<PyPiPubCacheVersionLevelInfo> Parser => PyPiPubCacheVersionLevelInfo._parser;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public static MessageDescriptor Descriptor => PyPiPublicRepoCacheReflection.Descriptor.MessageTypes[1];

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    MessageDescriptor IMessage.pb\u003A\u003AGoogle\u002EProtobuf\u002EIMessage\u002EDescriptor => PyPiPubCacheVersionLevelInfo.Descriptor;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public PyPiPubCacheVersionLevelInfo()
    {
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public PyPiPubCacheVersionLevelInfo(PyPiPubCacheVersionLevelInfo other)
      : this()
    {
      this.displayName_ = other.displayName_;
      this.displayVersion_ = other.displayVersion_;
      this.files_ = other.files_.Clone();
      this._unknownFields = UnknownFieldSet.Clone(other._unknownFields);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public PyPiPubCacheVersionLevelInfo Clone() => new PyPiPubCacheVersionLevelInfo(this);

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public string DisplayName
    {
      get => this.displayName_;
      set => this.displayName_ = ProtoPreconditions.CheckNotNull<string>(value, nameof (value));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public string DisplayVersion
    {
      get => this.displayVersion_;
      set => this.displayVersion_ = ProtoPreconditions.CheckNotNull<string>(value, nameof (value));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public RepeatedField<PyPiPubCachePackageVersionFileLevelInfo> Files => this.files_;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public override bool Equals(object other) => this.Equals(other as PyPiPubCacheVersionLevelInfo);

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public bool Equals(PyPiPubCacheVersionLevelInfo other)
    {
      if (other == null)
        return false;
      if (other == this)
        return true;
      return !(this.DisplayName != other.DisplayName) && !(this.DisplayVersion != other.DisplayVersion) && this.files_.Equals(other.files_) && object.Equals((object) this._unknownFields, (object) other._unknownFields);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public override int GetHashCode()
    {
      int num = 1;
      if (this.DisplayName.Length != 0)
        num ^= this.DisplayName.GetHashCode();
      if (this.DisplayVersion.Length != 0)
        num ^= this.DisplayVersion.GetHashCode();
      int hashCode = num ^ this.files_.GetHashCode();
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
      if (this.DisplayName.Length != 0)
      {
        output.WriteRawTag((byte) 10);
        output.WriteString(this.DisplayName);
      }
      if (this.DisplayVersion.Length != 0)
      {
        output.WriteRawTag((byte) 18);
        output.WriteString(this.DisplayVersion);
      }
      this.files_.WriteTo(ref output, PyPiPubCacheVersionLevelInfo._repeated_files_codec);
      if (this._unknownFields == null)
        return;
      this._unknownFields.WriteTo(ref output);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public int CalculateSize()
    {
      int num = 0;
      if (this.DisplayName.Length != 0)
        num += 1 + CodedOutputStream.ComputeStringSize(this.DisplayName);
      if (this.DisplayVersion.Length != 0)
        num += 1 + CodedOutputStream.ComputeStringSize(this.DisplayVersion);
      int size = num + this.files_.CalculateSize(PyPiPubCacheVersionLevelInfo._repeated_files_codec);
      if (this._unknownFields != null)
        size += this._unknownFields.CalculateSize();
      return size;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public void MergeFrom(PyPiPubCacheVersionLevelInfo other)
    {
      if (other == null)
        return;
      if (other.DisplayName.Length != 0)
        this.DisplayName = other.DisplayName;
      if (other.DisplayVersion.Length != 0)
        this.DisplayVersion = other.DisplayVersion;
      this.files_.Add((IEnumerable<PyPiPubCachePackageVersionFileLevelInfo>) other.files_);
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
            this.DisplayName = input.ReadString();
            continue;
          case 18:
            this.DisplayVersion = input.ReadString();
            continue;
          case 26:
            this.files_.AddEntriesFrom(ref input, PyPiPubCacheVersionLevelInfo._repeated_files_codec);
            continue;
          default:
            this._unknownFields = UnknownFieldSet.MergeFieldFrom(this._unknownFields, ref input);
            continue;
        }
      }
    }
  }
}
