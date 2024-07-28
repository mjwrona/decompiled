// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.VersionListWithSize.VersionListWithSizeFile
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.Reflection;
using Google.Protobuf.WellKnownTypes;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.VersionListWithSize
{
  [ProtobufRoot]
  public sealed class VersionListWithSizeFile : 
    IMessage<VersionListWithSizeFile>,
    IMessage,
    IEquatable<VersionListWithSizeFile>,
    IDeepCloneable<VersionListWithSizeFile>,
    IBufferMessage,
    IVersionCountsImplementationMetrics
  {
    private static readonly MessageParser<VersionListWithSizeFile> _parser = new MessageParser<VersionListWithSizeFile>((Func<VersionListWithSizeFile>) (() => new VersionListWithSizeFile()));
    private UnknownFieldSet _unknownFields;
    public const int PackagesFieldNumber = 1;
    private static readonly FieldCodec<VersionListWithSizePackage> _repeated_packages_codec = FieldCodec.ForMessage<VersionListWithSizePackage>(10U, VersionListWithSizePackage.Parser);
    private readonly RepeatedField<VersionListWithSizePackage> packages_ = new RepeatedField<VersionListWithSizePackage>();
    public const int LastModifiedFieldNumber = 2;
    private Timestamp lastModified_;
    private bool fileLevelNeedsSave;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public static MessageParser<VersionListWithSizeFile> Parser => VersionListWithSizeFile._parser;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public static MessageDescriptor Descriptor => VersionListWithSizeReflection.Descriptor.MessageTypes[3];

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    MessageDescriptor IMessage.pb\u003A\u003AGoogle\u002EProtobuf\u002EIMessage\u002EDescriptor => VersionListWithSizeFile.Descriptor;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public VersionListWithSizeFile()
    {
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public VersionListWithSizeFile(VersionListWithSizeFile other)
      : this()
    {
      this.packages_ = other.packages_.Clone();
      this.lastModified_ = other.lastModified_ != (Timestamp) null ? other.lastModified_.Clone() : (Timestamp) null;
      this._unknownFields = UnknownFieldSet.Clone(other._unknownFields);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public VersionListWithSizeFile Clone() => new VersionListWithSizeFile(this);

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public RepeatedField<VersionListWithSizePackage> Packages => this.packages_;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public Timestamp LastModified
    {
      get => this.lastModified_;
      set => this.lastModified_ = value;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public override bool Equals(object other) => this.Equals(other as VersionListWithSizeFile);

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public bool Equals(VersionListWithSizeFile other)
    {
      if (other == null)
        return false;
      if (other == this)
        return true;
      return this.packages_.Equals(other.packages_) && object.Equals((object) this.LastModified, (object) other.LastModified) && object.Equals((object) this._unknownFields, (object) other._unknownFields);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public override int GetHashCode()
    {
      int hashCode = 1 ^ this.packages_.GetHashCode();
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
      this.packages_.WriteTo(ref output, VersionListWithSizeFile._repeated_packages_codec);
      if (this.lastModified_ != (Timestamp) null)
      {
        output.WriteRawTag((byte) 18);
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
      int size = 0 + this.packages_.CalculateSize(VersionListWithSizeFile._repeated_packages_codec);
      if (this.lastModified_ != (Timestamp) null)
        size += 1 + CodedOutputStream.ComputeMessageSize((IMessage) this.LastModified);
      if (this._unknownFields != null)
        size += this._unknownFields.CalculateSize();
      return size;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public void MergeFrom(VersionListWithSizeFile other)
    {
      if (other == null)
        return;
      this.packages_.Add((IEnumerable<VersionListWithSizePackage>) other.packages_);
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
            this.packages_.AddEntriesFrom(ref input, VersionListWithSizeFile._repeated_packages_codec);
            continue;
          case 18:
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

    public bool NeedsSave => this.fileLevelNeedsSave || this.Packages.Any<VersionListWithSizePackage>((Func<VersionListWithSizePackage, bool>) (x => x.NeedsSave));

    public int PackagesUnpacked { get; private set; }

    public int PackagesPacked { get; private set; }

    public int NumPackagesNeedingUnpack => this.Packages.Count<VersionListWithSizePackage>((Func<VersionListWithSizePackage, bool>) (x => x.VersionsNeedUnpack));

    public int NumPackagesNeedingRepack => this.Packages.Count<VersionListWithSizePackage>((Func<VersionListWithSizePackage, bool>) (x => x.VersionsNeedRepack));

    public int NumPackagesNeedingSave => this.Packages.Count<VersionListWithSizePackage>((Func<VersionListWithSizePackage, bool>) (x => x.NeedsSave));

    public int NumPackages => this.Packages.Count;

    public int NumTotalVersions => this.Packages.Sum<VersionListWithSizePackage>((Func<VersionListWithSizePackage, int>) (x => x.Versions.Count));

    public void NotifyContainedPackagePacked() => ++this.PackagesPacked;

    public void NotifyContainedPackageUnpacked() => ++this.PackagesUnpacked;

    public static VersionListWithSizeFile CreateNewUnpacked() => new VersionListWithSizeFile();

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
      foreach (VersionListWithSizePackage package in this.Packages)
        package.NotifySaved();
      this.fileLevelNeedsSave = false;
    }
  }
}
