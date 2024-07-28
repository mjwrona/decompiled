// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PublicRepositories.NuGetPubCacheVersionMutableInfo
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
  public sealed class NuGetPubCacheVersionMutableInfo : 
    IMessage<
    #nullable disable
    NuGetPubCacheVersionMutableInfo>,
    IMessage,
    IEquatable<NuGetPubCacheVersionMutableInfo>,
    IDeepCloneable<NuGetPubCacheVersionMutableInfo>,
    IBufferMessage,
    IHaveGenerationCursor<
    #nullable enable
    NuGetCatalogCursor>
  {
    private static readonly 
    #nullable disable
    MessageParser<NuGetPubCacheVersionMutableInfo> _parser = new MessageParser<NuGetPubCacheVersionMutableInfo>((Func<NuGetPubCacheVersionMutableInfo>) (() => new NuGetPubCacheVersionMutableInfo()));
    private UnknownFieldSet _unknownFields;
    public const int FetchDateFieldNumber = 1;
    private Timestamp fetchDate_;
    public const int ListedFieldNumber = 2;
    private bool listed_;
    public const int PublishDateFieldNumber = 3;
    private Timestamp publishDate_;
    public const int DeprecationFieldNumber = 4;
    private NuGetPubCachePackageDeprecation deprecation_;
    public const int VulnerabilitiesFieldNumber = 5;
    private static readonly FieldCodec<NuGetPubCacheVulnerability> _repeated_vulnerabilities_codec = FieldCodec.ForMessage<NuGetPubCacheVulnerability>(42U, NuGetPubCacheVulnerability.Parser);
    private readonly RepeatedField<NuGetPubCacheVulnerability> vulnerabilities_ = new RepeatedField<NuGetPubCacheVulnerability>();
    public const int CommitTimestampFieldNumber = 6;
    private Timestamp commitTimestamp_;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public static MessageParser<NuGetPubCacheVersionMutableInfo> Parser => NuGetPubCacheVersionMutableInfo._parser;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public static MessageDescriptor Descriptor => NugetPublicRepoCacheReflection.Descriptor.MessageTypes[3];

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    MessageDescriptor IMessage.pb\u003A\u003AGoogle\u002EProtobuf\u002EIMessage\u002EDescriptor => NuGetPubCacheVersionMutableInfo.Descriptor;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public NuGetPubCacheVersionMutableInfo()
    {
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public NuGetPubCacheVersionMutableInfo(NuGetPubCacheVersionMutableInfo other)
      : this()
    {
      this.fetchDate_ = other.fetchDate_ != (Timestamp) null ? other.fetchDate_.Clone() : (Timestamp) null;
      this.listed_ = other.listed_;
      this.publishDate_ = other.publishDate_ != (Timestamp) null ? other.publishDate_.Clone() : (Timestamp) null;
      this.deprecation_ = other.deprecation_ != null ? other.deprecation_.Clone() : (NuGetPubCachePackageDeprecation) null;
      this.vulnerabilities_ = other.vulnerabilities_.Clone();
      this.commitTimestamp_ = other.commitTimestamp_ != (Timestamp) null ? other.commitTimestamp_.Clone() : (Timestamp) null;
      this._unknownFields = UnknownFieldSet.Clone(other._unknownFields);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public NuGetPubCacheVersionMutableInfo Clone() => new NuGetPubCacheVersionMutableInfo(this);

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public Timestamp FetchDate
    {
      get => this.fetchDate_;
      set => this.fetchDate_ = value;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public bool Listed
    {
      get => this.listed_;
      set => this.listed_ = value;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public Timestamp PublishDate
    {
      get => this.publishDate_;
      set => this.publishDate_ = value;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public NuGetPubCachePackageDeprecation Deprecation
    {
      get => this.deprecation_;
      set => this.deprecation_ = value;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public RepeatedField<NuGetPubCacheVulnerability> Vulnerabilities => this.vulnerabilities_;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public Timestamp CommitTimestamp
    {
      get => this.commitTimestamp_;
      set => this.commitTimestamp_ = value;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public override bool Equals(object other) => this.Equals(other as NuGetPubCacheVersionMutableInfo);

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public bool Equals(NuGetPubCacheVersionMutableInfo other)
    {
      if (other == null)
        return false;
      if (other == this)
        return true;
      return object.Equals((object) this.FetchDate, (object) other.FetchDate) && this.Listed == other.Listed && object.Equals((object) this.PublishDate, (object) other.PublishDate) && object.Equals((object) this.Deprecation, (object) other.Deprecation) && this.vulnerabilities_.Equals(other.vulnerabilities_) && object.Equals((object) this.CommitTimestamp, (object) other.CommitTimestamp) && object.Equals((object) this._unknownFields, (object) other._unknownFields);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public override int GetHashCode()
    {
      int num = 1;
      if (this.fetchDate_ != (Timestamp) null)
        num ^= this.FetchDate.GetHashCode();
      if (this.Listed)
        num ^= this.Listed.GetHashCode();
      if (this.publishDate_ != (Timestamp) null)
        num ^= this.PublishDate.GetHashCode();
      if (this.deprecation_ != null)
        num ^= this.Deprecation.GetHashCode();
      int hashCode = num ^ this.vulnerabilities_.GetHashCode();
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
      if (this.fetchDate_ != (Timestamp) null)
      {
        output.WriteRawTag((byte) 10);
        output.WriteMessage((IMessage) this.FetchDate);
      }
      if (this.Listed)
      {
        output.WriteRawTag((byte) 16);
        output.WriteBool(this.Listed);
      }
      if (this.publishDate_ != (Timestamp) null)
      {
        output.WriteRawTag((byte) 26);
        output.WriteMessage((IMessage) this.PublishDate);
      }
      if (this.deprecation_ != null)
      {
        output.WriteRawTag((byte) 34);
        output.WriteMessage((IMessage) this.Deprecation);
      }
      this.vulnerabilities_.WriteTo(ref output, NuGetPubCacheVersionMutableInfo._repeated_vulnerabilities_codec);
      if (this.commitTimestamp_ != (Timestamp) null)
      {
        output.WriteRawTag((byte) 50);
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
      if (this.fetchDate_ != (Timestamp) null)
        num += 1 + CodedOutputStream.ComputeMessageSize((IMessage) this.FetchDate);
      if (this.Listed)
        num += 2;
      if (this.publishDate_ != (Timestamp) null)
        num += 1 + CodedOutputStream.ComputeMessageSize((IMessage) this.PublishDate);
      if (this.deprecation_ != null)
        num += 1 + CodedOutputStream.ComputeMessageSize((IMessage) this.Deprecation);
      int size = num + this.vulnerabilities_.CalculateSize(NuGetPubCacheVersionMutableInfo._repeated_vulnerabilities_codec);
      if (this.commitTimestamp_ != (Timestamp) null)
        size += 1 + CodedOutputStream.ComputeMessageSize((IMessage) this.CommitTimestamp);
      if (this._unknownFields != null)
        size += this._unknownFields.CalculateSize();
      return size;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public void MergeFrom(NuGetPubCacheVersionMutableInfo other)
    {
      if (other == null)
        return;
      if (other.fetchDate_ != (Timestamp) null)
      {
        if (this.fetchDate_ == (Timestamp) null)
          this.FetchDate = new Timestamp();
        this.FetchDate.MergeFrom(other.FetchDate);
      }
      if (other.Listed)
        this.Listed = other.Listed;
      if (other.publishDate_ != (Timestamp) null)
      {
        if (this.publishDate_ == (Timestamp) null)
          this.PublishDate = new Timestamp();
        this.PublishDate.MergeFrom(other.PublishDate);
      }
      if (other.deprecation_ != null)
      {
        if (this.deprecation_ == null)
          this.Deprecation = new NuGetPubCachePackageDeprecation();
        this.Deprecation.MergeFrom(other.Deprecation);
      }
      this.vulnerabilities_.Add((IEnumerable<NuGetPubCacheVulnerability>) other.vulnerabilities_);
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
            if (this.fetchDate_ == (Timestamp) null)
              this.FetchDate = new Timestamp();
            input.ReadMessage((IMessage) this.FetchDate);
            continue;
          case 16:
            this.Listed = input.ReadBool();
            continue;
          case 26:
            if (this.publishDate_ == (Timestamp) null)
              this.PublishDate = new Timestamp();
            input.ReadMessage((IMessage) this.PublishDate);
            continue;
          case 34:
            if (this.deprecation_ == null)
              this.Deprecation = new NuGetPubCachePackageDeprecation();
            input.ReadMessage((IMessage) this.Deprecation);
            continue;
          case 42:
            this.vulnerabilities_.AddEntriesFrom(ref input, NuGetPubCacheVersionMutableInfo._repeated_vulnerabilities_codec);
            continue;
          case 50:
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
  }
}
