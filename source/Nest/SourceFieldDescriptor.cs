// Decompiled with JetBrains decompiler
// Type: Nest.SourceFieldDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class SourceFieldDescriptor : 
    DescriptorBase<SourceFieldDescriptor, ISourceField>,
    ISourceField,
    IFieldMapping
  {
    bool? ISourceField.Compress { get; set; }

    string ISourceField.CompressThreshold { get; set; }

    bool? ISourceField.Enabled { get; set; }

    IEnumerable<string> ISourceField.Excludes { get; set; }

    IEnumerable<string> ISourceField.Includes { get; set; }

    public SourceFieldDescriptor Enabled(bool? enabled = true) => this.Assign<bool?>(enabled, (Action<ISourceField, bool?>) ((a, v) => a.Enabled = v));

    public SourceFieldDescriptor Compress(bool? compress = true) => this.Assign<bool?>(compress, (Action<ISourceField, bool?>) ((a, v) => a.Compress = v));

    public SourceFieldDescriptor CompressionThreshold(string compressionThreshold) => this.Assign<string>(compressionThreshold, (Action<ISourceField, string>) ((a, v) =>
    {
      a.Compress = new bool?(true);
      a.CompressThreshold = v;
    }));

    public SourceFieldDescriptor Includes(IEnumerable<string> includes) => this.Assign<IEnumerable<string>>(includes, (Action<ISourceField, IEnumerable<string>>) ((a, v) => a.Includes = v));

    public SourceFieldDescriptor Excludes(IEnumerable<string> excludes) => this.Assign<IEnumerable<string>>(excludes, (Action<ISourceField, IEnumerable<string>>) ((a, v) => a.Excludes = v));
  }
}
