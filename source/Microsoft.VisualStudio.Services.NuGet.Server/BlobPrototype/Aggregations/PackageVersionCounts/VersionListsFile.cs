// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts.VersionListsFile
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts
{
  [ProtobufRoot]
  public sealed class VersionListsFile : 
    IVersionCountsImplementationMetrics,
    IMessage<VersionListsFile>,
    IMessage,
    IEquatable<VersionListsFile>,
    IDeepCloneable<VersionListsFile>,
    IBufferMessage
  {
    private bool fileLevelNeedsSave;
    private static readonly MessageParser<VersionListsFile> _parser = new MessageParser<VersionListsFile>((Func<VersionListsFile>) (() => new VersionListsFile()));
    private UnknownFieldSet _unknownFields;
    public const int KnownViewsFieldNumber = 1;
    private static readonly FieldCodec<ByteString> _repeated_knownViews_codec = FieldCodec.ForBytes(10U);
    private readonly RepeatedField<ByteString> knownViews_ = new RepeatedField<ByteString>();
    public const int PackagesFieldNumber = 2;
    private static readonly FieldCodec<VersionListsPackage> _repeated_packages_codec = FieldCodec.ForMessage<VersionListsPackage>(18U, VersionListsPackage.Parser);
    private readonly RepeatedField<VersionListsPackage> packages_ = new RepeatedField<VersionListsPackage>();
    public const int LastModifiedFieldNumber = 3;
    private Timestamp lastModified_;

    public bool NeedsSave => this.fileLevelNeedsSave || this.Packages.Any<VersionListsPackage>((Func<VersionListsPackage, bool>) (x => x.NeedsSave));

    public int PackagesUnpacked { get; private set; }

    public int PackagesPacked { get; private set; }

    public int NumPackagesNeedingUnpack => this.Packages.Count<VersionListsPackage>((Func<VersionListsPackage, bool>) (x => x.VersionsNeedUnpack));

    public int NumPackagesNeedingRepack => this.Packages.Count<VersionListsPackage>((Func<VersionListsPackage, bool>) (x => x.VersionsNeedRepack));

    public int NumPackagesNeedingSave => this.Packages.Count<VersionListsPackage>((Func<VersionListsPackage, bool>) (x => x.NeedsSave));

    public int NumPackages => this.Packages.Count;

    public int NumTotalVersions => this.Packages.Sum<VersionListsPackage>((Func<VersionListsPackage, int>) (x => x.Versions.Count));

    public void NotifyContainedPackagePacked() => ++this.PackagesPacked;

    public void NotifyContainedPackageUnpacked() => ++this.PackagesUnpacked;

    public static VersionListsFile CreateNewUnpacked() => new VersionListsFile()
    {
      ViewIds = new List<Guid>() { Guid.Empty }
    };

    public List<Guid> ViewIds { get; set; }

    public DateTime LastModifiedAsDateTime
    {
      get
      {
        Timestamp timestamp = this.LastModified;
        if ((object) timestamp == null)
          timestamp = ProtobufConstants.ZeroTimestamp;
        return timestamp.ToDateTime();
      }
      set => this.LastModified = Timestamp.FromDateTime(value);
    }

    public void NotifyModified(DateTime modTime)
    {
      if (modTime > this.LastModifiedAsDateTime)
        this.LastModifiedAsDateTime = modTime;
      this.fileLevelNeedsSave = true;
    }

    public void NotifySaved()
    {
      foreach (VersionListsPackage package in this.Packages)
        package.NotifySaved();
      this.fileLevelNeedsSave = false;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public static MessageParser<VersionListsFile> Parser => VersionListsFile._parser;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public static MessageDescriptor Descriptor => PackageVersionCountsReflection.Descriptor.MessageTypes[5];

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    MessageDescriptor IMessage.pb\u003A\u003AGoogle\u002EProtobuf\u002EIMessage\u002EDescriptor => VersionListsFile.Descriptor;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public VersionListsFile()
    {
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public VersionListsFile(VersionListsFile other)
      : this()
    {
      this.knownViews_ = other.knownViews_.Clone();
      this.packages_ = other.packages_.Clone();
      this.lastModified_ = other.lastModified_ != (Timestamp) null ? other.lastModified_.Clone() : (Timestamp) null;
      this._unknownFields = UnknownFieldSet.Clone(other._unknownFields);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public VersionListsFile Clone() => new VersionListsFile(this);

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public RepeatedField<ByteString> KnownViews => this.knownViews_;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public RepeatedField<VersionListsPackage> Packages => this.packages_;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public Timestamp LastModified
    {
      get => this.lastModified_;
      set => this.lastModified_ = value;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public override bool Equals(object other) => this.Equals(other as VersionListsFile);

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public bool Equals(VersionListsFile other)
    {
      if (other == null)
        return false;
      if (other == this)
        return true;
      return this.knownViews_.Equals(other.knownViews_) && this.packages_.Equals(other.packages_) && object.Equals((object) this.LastModified, (object) other.LastModified) && object.Equals((object) this._unknownFields, (object) other._unknownFields);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public override int GetHashCode()
    {
      int hashCode = 1 ^ this.knownViews_.GetHashCode() ^ this.packages_.GetHashCode();
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
      this.knownViews_.WriteTo(ref output, VersionListsFile._repeated_knownViews_codec);
      this.packages_.WriteTo(ref output, VersionListsFile._repeated_packages_codec);
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
      int size = 0 + this.knownViews_.CalculateSize(VersionListsFile._repeated_knownViews_codec) + this.packages_.CalculateSize(VersionListsFile._repeated_packages_codec);
      if (this.lastModified_ != (Timestamp) null)
        size += 1 + CodedOutputStream.ComputeMessageSize((IMessage) this.LastModified);
      if (this._unknownFields != null)
        size += this._unknownFields.CalculateSize();
      return size;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public void MergeFrom(VersionListsFile other)
    {
      if (other == null)
        return;
      this.knownViews_.Add((IEnumerable<ByteString>) other.knownViews_);
      this.packages_.Add((IEnumerable<VersionListsPackage>) other.packages_);
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
            this.knownViews_.AddEntriesFrom(ref input, VersionListsFile._repeated_knownViews_codec);
            continue;
          case 18:
            this.packages_.AddEntriesFrom(ref input, VersionListsFile._repeated_packages_codec);
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
