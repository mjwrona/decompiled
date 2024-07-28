// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts.VersionCountsPackageView
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Google.Protobuf;
using Google.Protobuf.Reflection;
using System;
using System.CodeDom.Compiler;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.PackageVersionCounts
{
  public sealed class VersionCountsPackageView : 
    IMessage<VersionCountsPackageView>,
    IMessage,
    IEquatable<VersionCountsPackageView>,
    IDeepCloneable<VersionCountsPackageView>,
    IBufferMessage
  {
    private static readonly MessageParser<VersionCountsPackageView> _parser = new MessageParser<VersionCountsPackageView>((Func<VersionCountsPackageView>) (() => new VersionCountsPackageView()));
    private UnknownFieldSet _unknownFields;
    public const int ViewIndexFieldNumber = 1;
    private int viewIndex_;
    public const int CountFieldNumber = 2;
    private int count_;
    public const int FlagsFieldNumber = 3;
    private LatestVersionFlags flags_;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public static MessageParser<VersionCountsPackageView> Parser => VersionCountsPackageView._parser;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public static MessageDescriptor Descriptor => PackageVersionCountsReflection.Descriptor.MessageTypes[0];

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    MessageDescriptor IMessage.pb\u003A\u003AGoogle\u002EProtobuf\u002EIMessage\u002EDescriptor => VersionCountsPackageView.Descriptor;

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public VersionCountsPackageView()
    {
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public VersionCountsPackageView(VersionCountsPackageView other)
      : this()
    {
      this.viewIndex_ = other.viewIndex_;
      this.count_ = other.count_;
      this.flags_ = other.flags_;
      this._unknownFields = UnknownFieldSet.Clone(other._unknownFields);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public VersionCountsPackageView Clone() => new VersionCountsPackageView(this);

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public int ViewIndex
    {
      get => this.viewIndex_;
      set => this.viewIndex_ = value;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public int Count
    {
      get => this.count_;
      set => this.count_ = value;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public LatestVersionFlags Flags
    {
      get => this.flags_;
      set => this.flags_ = value;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public override bool Equals(object other) => this.Equals(other as VersionCountsPackageView);

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public bool Equals(VersionCountsPackageView other)
    {
      if (other == null)
        return false;
      if (other == this)
        return true;
      return this.ViewIndex == other.ViewIndex && this.Count == other.Count && this.Flags == other.Flags && object.Equals((object) this._unknownFields, (object) other._unknownFields);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public override int GetHashCode()
    {
      int hashCode1 = 1;
      int num1;
      if (this.ViewIndex != 0)
      {
        int num2 = hashCode1;
        num1 = this.ViewIndex;
        int hashCode2 = num1.GetHashCode();
        hashCode1 = num2 ^ hashCode2;
      }
      if (this.Count != 0)
      {
        int num3 = hashCode1;
        num1 = this.Count;
        int hashCode3 = num1.GetHashCode();
        hashCode1 = num3 ^ hashCode3;
      }
      if (this.Flags != LatestVersionFlags.None)
        hashCode1 ^= this.Flags.GetHashCode();
      if (this._unknownFields != null)
        hashCode1 ^= this._unknownFields.GetHashCode();
      return hashCode1;
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
      if (this.ViewIndex != 0)
      {
        output.WriteRawTag((byte) 8);
        output.WriteInt32(this.ViewIndex);
      }
      if (this.Count != 0)
      {
        output.WriteRawTag((byte) 16);
        output.WriteInt32(this.Count);
      }
      if (this.Flags != LatestVersionFlags.None)
      {
        output.WriteRawTag((byte) 24);
        output.WriteEnum((int) this.Flags);
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
      if (this.ViewIndex != 0)
        size += 1 + CodedOutputStream.ComputeInt32Size(this.ViewIndex);
      if (this.Count != 0)
        size += 1 + CodedOutputStream.ComputeInt32Size(this.Count);
      if (this.Flags != LatestVersionFlags.None)
        size += 1 + CodedOutputStream.ComputeEnumSize((int) this.Flags);
      if (this._unknownFields != null)
        size += this._unknownFields.CalculateSize();
      return size;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("protoc", null)]
    public void MergeFrom(VersionCountsPackageView other)
    {
      if (other == null)
        return;
      if (other.ViewIndex != 0)
        this.ViewIndex = other.ViewIndex;
      if (other.Count != 0)
        this.Count = other.Count;
      if (other.Flags != LatestVersionFlags.None)
        this.Flags = other.Flags;
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
            this.ViewIndex = input.ReadInt32();
            continue;
          case 16:
            this.Count = input.ReadInt32();
            continue;
          case 24:
            this.Flags = (LatestVersionFlags) input.ReadEnum();
            continue;
          default:
            this._unknownFields = UnknownFieldSet.MergeFieldFrom(this._unknownFields, ref input);
            continue;
        }
      }
    }
  }
}
