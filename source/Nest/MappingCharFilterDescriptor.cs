// Decompiled with JetBrains decompiler
// Type: Nest.MappingCharFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class MappingCharFilterDescriptor : 
    CharFilterDescriptorBase<MappingCharFilterDescriptor, IMappingCharFilter>,
    IMappingCharFilter,
    ICharFilter
  {
    protected override string Type => "mapping";

    IEnumerable<string> IMappingCharFilter.Mappings { get; set; }

    string IMappingCharFilter.MappingsPath { get; set; }

    public MappingCharFilterDescriptor Mappings(params string[] mappings) => this.Assign<string[]>(mappings, (Action<IMappingCharFilter, string[]>) ((a, v) => a.Mappings = (IEnumerable<string>) v));

    public MappingCharFilterDescriptor Mappings(IEnumerable<string> mappings) => this.Assign<IEnumerable<string>>(mappings, (Action<IMappingCharFilter, IEnumerable<string>>) ((a, v) => a.Mappings = v));

    public MappingCharFilterDescriptor MappingsPath(string path) => this.Assign<string>(path, (Action<IMappingCharFilter, string>) ((a, v) => a.MappingsPath = v));
  }
}
