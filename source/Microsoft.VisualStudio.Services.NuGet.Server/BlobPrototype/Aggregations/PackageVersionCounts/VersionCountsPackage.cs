// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts.VersionCountsPackage
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts
{
  public sealed class VersionCountsPackage : 
    IMessage<VersionCountsPackage>,
    IMessage,
    IEquatable<VersionCountsPackage>,
    IDeepCloneable<VersionCountsPackage>,
    IBufferMessage
  {
    private static readonly MessageParser<VersionCountsPackage> _parser = new MessageParser<VersionCountsPackage>((Func<VersionCountsPackage>) (() => new VersionCountsPackage()));
    private UnknownFieldSet _unknownFields;
    public const int CompressedDisplayNameFieldNumber = 1;
    private string compressedDisplayName_ = "";
    public const int ViewsFieldNumber = 2;
    private static readonly FieldCodec<VersionCountsPackageView> _repeated_views_codec = FieldCodec.ForMessage<VersionCountsPackageView>(18U, VersionCountsPackageView.Parser);
    private readonly RepeatedField<VersionCountsPackageView> views_ = new RepeatedField<VersionCountsPackageView>();
    public const int LastModifiedFieldNumber = 3;
    private Timestamp lastModified_;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public static MessageParser<VersionCountsPackage> Parser => VersionCountsPackage._parser;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public static MessageDescriptor Descriptor => PackageVersionCountsReflection.Descriptor.MessageTypes[1];

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    MessageDescriptor IMessage.pb\u003A\u003AGoogle\u002EProtobuf\u002EIMessage\u002EDescriptor => VersionCountsPackage.Descriptor;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public VersionCountsPackage()
    {
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public VersionCountsPackage(VersionCountsPackage other)
      : this()
    {
      this.compressedDisplayName_ = other.compressedDisplayName_;
      this.views_ = other.views_.Clone();
      this.lastModified_ = other.lastModified_ != (Timestamp) null ? other.lastModified_.Clone() : (Timestamp) null;
      this._unknownFields = UnknownFieldSet.Clone(other._unknownFields);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public VersionCountsPackage Clone() => new VersionCountsPackage(this);

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public string CompressedDisplayName
    {
      get => this.compressedDisplayName_;
      set => this.compressedDisplayName_ = ProtoPreconditions.CheckNotNull<string>(value, nameof (value));
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public RepeatedField<VersionCountsPackageView> Views => this.views_;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public Timestamp LastModified
    {
      get => this.lastModified_;
      set => this.lastModified_ = value;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public override bool Equals(object other) => this.Equals(other as VersionCountsPackage);

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public bool Equals(VersionCountsPackage other)
    {
      if (other == null)
        return false;
      if (other == this)
        return true;
      return !(this.CompressedDisplayName != other.CompressedDisplayName) && this.views_.Equals(other.views_) && object.Equals((object) this.LastModified, (object) other.LastModified) && object.Equals((object) this._unknownFields, (object) other._unknownFields);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public override int GetHashCode()
    {
      int num = 1;
      if (this.CompressedDisplayName.Length != 0)
        num ^= this.CompressedDisplayName.GetHashCode();
      int hashCode = num ^ this.views_.GetHashCode();
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
      this.views_.WriteTo(ref output, VersionCountsPackage._repeated_views_codec);
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
      int size = num + this.views_.CalculateSize(VersionCountsPackage._repeated_views_codec);
      if (this.lastModified_ != (Timestamp) null)
        size += 1 + CodedOutputStream.ComputeMessageSize((IMessage) this.LastModified);
      if (this._unknownFields != null)
        size += this._unknownFields.CalculateSize();
      return size;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public void MergeFrom(VersionCountsPackage other)
    {
      if (other == null)
        return;
      if (other.CompressedDisplayName.Length != 0)
        this.CompressedDisplayName = other.CompressedDisplayName;
      this.views_.Add((IEnumerable<VersionCountsPackageView>) other.views_);
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
            this.views_.AddEntriesFrom(ref input, VersionCountsPackage._repeated_views_codec);
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
