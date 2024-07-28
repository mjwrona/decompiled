// Decompiled with JetBrains decompiler
// Type: Nest.PathHierarchyTokenizerDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class PathHierarchyTokenizerDescriptor : 
    TokenizerDescriptorBase<PathHierarchyTokenizerDescriptor, IPathHierarchyTokenizer>,
    IPathHierarchyTokenizer,
    ITokenizer
  {
    protected override string Type => "path_hierarchy";

    int? IPathHierarchyTokenizer.BufferSize { get; set; }

    char? IPathHierarchyTokenizer.Delimiter { get; set; }

    char? IPathHierarchyTokenizer.Replacement { get; set; }

    bool? IPathHierarchyTokenizer.Reverse { get; set; }

    int? IPathHierarchyTokenizer.Skip { get; set; }

    public PathHierarchyTokenizerDescriptor BufferSize(int? bufferSize) => this.Assign<int?>(bufferSize, (Action<IPathHierarchyTokenizer, int?>) ((a, v) => a.BufferSize = v));

    public PathHierarchyTokenizerDescriptor Skip(int? skip) => this.Assign<int?>(skip, (Action<IPathHierarchyTokenizer, int?>) ((a, v) => a.Skip = v));

    public PathHierarchyTokenizerDescriptor Reverse(bool? reverse = true) => this.Assign<bool?>(reverse, (Action<IPathHierarchyTokenizer, bool?>) ((a, v) => a.Reverse = v));

    public PathHierarchyTokenizerDescriptor Delimiter(char? delimiter) => this.Assign<char?>(delimiter, (Action<IPathHierarchyTokenizer, char?>) ((a, v) => a.Delimiter = v));

    public PathHierarchyTokenizerDescriptor Replacement(char? replacement) => this.Assign<char?>(replacement, (Action<IPathHierarchyTokenizer, char?>) ((a, v) => a.Replacement = v));
  }
}
