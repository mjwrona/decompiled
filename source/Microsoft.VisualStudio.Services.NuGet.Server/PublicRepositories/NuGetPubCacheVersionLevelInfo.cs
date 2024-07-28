// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PublicRepositories.NuGetPubCacheVersionLevelInfo
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Google.Protobuf;
using Google.Protobuf.Reflection;
using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using System;
using System.CodeDom.Compiler;
using System.Diagnostics;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Server.PublicRepositories
{
  public sealed class NuGetPubCacheVersionLevelInfo : 
    IMessage<
    #nullable disable
    NuGetPubCacheVersionLevelInfo>,
    IMessage,
    IEquatable<NuGetPubCacheVersionLevelInfo>,
    IDeepCloneable<NuGetPubCacheVersionLevelInfo>,
    IBufferMessage
  {
    private static readonly MessageParser<NuGetPubCacheVersionLevelInfo> _parser = new MessageParser<NuGetPubCacheVersionLevelInfo>((Func<NuGetPubCacheVersionLevelInfo>) (() => new NuGetPubCacheVersionLevelInfo()));
    private UnknownFieldSet _unknownFields;
    public const int DisplayNameFieldNumber = 1;
    private string displayName_ = "";
    public const int DisplayVersionFieldNumber = 2;
    private string displayVersion_ = "";
    public const int MutableInfoFieldNumber = 3;
    private NuGetPubCacheVersionMutableInfo mutableInfo_;
    public const int NuspecFieldNumber = 4;
    private NuGetPubCacheVersionNuspec nuspec_;
    private 
    #nullable enable
    VssNuGetPackageIdentity? identity;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public static 
    #nullable disable
    MessageParser<NuGetPubCacheVersionLevelInfo> Parser => NuGetPubCacheVersionLevelInfo._parser;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public static MessageDescriptor Descriptor => NugetPublicRepoCacheReflection.Descriptor.MessageTypes[5];

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    MessageDescriptor IMessage.pb\u003A\u003AGoogle\u002EProtobuf\u002EIMessage\u002EDescriptor => NuGetPubCacheVersionLevelInfo.Descriptor;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public NuGetPubCacheVersionLevelInfo()
    {
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public NuGetPubCacheVersionLevelInfo(NuGetPubCacheVersionLevelInfo other)
      : this()
    {
      this.displayName_ = other.displayName_;
      this.displayVersion_ = other.displayVersion_;
      this.mutableInfo_ = other.mutableInfo_ != null ? other.mutableInfo_.Clone() : (NuGetPubCacheVersionMutableInfo) null;
      this.nuspec_ = other.nuspec_ != null ? other.nuspec_.Clone() : (NuGetPubCacheVersionNuspec) null;
      this._unknownFields = UnknownFieldSet.Clone(other._unknownFields);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public NuGetPubCacheVersionLevelInfo Clone() => new NuGetPubCacheVersionLevelInfo(this);

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
    public NuGetPubCacheVersionMutableInfo MutableInfo
    {
      get => this.mutableInfo_;
      set => this.mutableInfo_ = value;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public NuGetPubCacheVersionNuspec Nuspec
    {
      get => this.nuspec_;
      set => this.nuspec_ = value;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public override bool Equals(object other) => this.Equals(other as NuGetPubCacheVersionLevelInfo);

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public bool Equals(NuGetPubCacheVersionLevelInfo other)
    {
      if (other == null)
        return false;
      if (other == this)
        return true;
      return !(this.DisplayName != other.DisplayName) && !(this.DisplayVersion != other.DisplayVersion) && object.Equals((object) this.MutableInfo, (object) other.MutableInfo) && object.Equals((object) this.Nuspec, (object) other.Nuspec) && object.Equals((object) this._unknownFields, (object) other._unknownFields);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public override int GetHashCode()
    {
      int hashCode = 1;
      if (this.DisplayName.Length != 0)
        hashCode ^= this.DisplayName.GetHashCode();
      if (this.DisplayVersion.Length != 0)
        hashCode ^= this.DisplayVersion.GetHashCode();
      if (this.mutableInfo_ != null)
        hashCode ^= this.MutableInfo.GetHashCode();
      if (this.nuspec_ != null)
        hashCode ^= this.Nuspec.GetHashCode();
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
      if (this.mutableInfo_ != null)
      {
        output.WriteRawTag((byte) 26);
        output.WriteMessage((IMessage) this.MutableInfo);
      }
      if (this.nuspec_ != null)
      {
        output.WriteRawTag((byte) 34);
        output.WriteMessage((IMessage) this.Nuspec);
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
      if (this.DisplayName.Length != 0)
        size += 1 + CodedOutputStream.ComputeStringSize(this.DisplayName);
      if (this.DisplayVersion.Length != 0)
        size += 1 + CodedOutputStream.ComputeStringSize(this.DisplayVersion);
      if (this.mutableInfo_ != null)
        size += 1 + CodedOutputStream.ComputeMessageSize((IMessage) this.MutableInfo);
      if (this.nuspec_ != null)
        size += 1 + CodedOutputStream.ComputeMessageSize((IMessage) this.Nuspec);
      if (this._unknownFields != null)
        size += this._unknownFields.CalculateSize();
      return size;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public void MergeFrom(NuGetPubCacheVersionLevelInfo other)
    {
      if (other == null)
        return;
      if (other.DisplayName.Length != 0)
        this.DisplayName = other.DisplayName;
      if (other.DisplayVersion.Length != 0)
        this.DisplayVersion = other.DisplayVersion;
      if (other.mutableInfo_ != null)
      {
        if (this.mutableInfo_ == null)
          this.MutableInfo = new NuGetPubCacheVersionMutableInfo();
        this.MutableInfo.MergeFrom(other.MutableInfo);
      }
      if (other.nuspec_ != null)
      {
        if (this.nuspec_ == null)
          this.Nuspec = new NuGetPubCacheVersionNuspec();
        this.Nuspec.MergeFrom(other.Nuspec);
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
            this.DisplayName = input.ReadString();
            continue;
          case 18:
            this.DisplayVersion = input.ReadString();
            continue;
          case 26:
            if (this.mutableInfo_ == null)
              this.MutableInfo = new NuGetPubCacheVersionMutableInfo();
            input.ReadMessage((IMessage) this.MutableInfo);
            continue;
          case 34:
            if (this.nuspec_ == null)
              this.Nuspec = new NuGetPubCacheVersionNuspec();
            input.ReadMessage((IMessage) this.Nuspec);
            continue;
          default:
            this._unknownFields = UnknownFieldSet.MergeFieldFrom(this._unknownFields, ref input);
            continue;
        }
      }
    }

    public 
    #nullable enable
    VssNuGetPackageIdentity Identity
    {
      get => this.identity ?? (this.identity = new VssNuGetPackageIdentity(this.DisplayName, this.DisplayVersion));
      set
      {
        this.identity = value;
        this.DisplayName = this.identity.Name.DisplayName;
        this.DisplayVersion = this.identity.Version.DisplayVersion;
      }
    }
  }
}
