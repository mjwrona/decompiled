// Decompiled with JetBrains decompiler
// Type: Nest.TypeMapping
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class TypeMapping : ITypeMapping
  {
    [Obsolete("The _all field is no longer supported in Elasticsearch 7.x and will be removed in the next major release. The value will not be sent in a request. An _all like field can be achieved using copy_to")]
    public IAllField AllField { get; set; }

    public bool? DateDetection { get; set; }

    public Union<bool, DynamicMapping> Dynamic { get; set; }

    public IEnumerable<string> DynamicDateFormats { get; set; }

    public IDynamicTemplateContainer DynamicTemplates { get; set; }

    public IFieldNamesField FieldNamesField { get; set; }

    [Obsolete("Configuration for the _index field is no longer supported in Elasticsearch 7.x and will be removed in the next major release.")]
    public IIndexField IndexField { get; set; }

    public IDictionary<string, object> Meta { get; set; }

    public bool? NumericDetection { get; set; }

    public IProperties Properties { get; set; }

    public IRoutingField RoutingField { get; set; }

    public IRuntimeFields RuntimeFields { get; set; }

    public ISizeField SizeField { get; set; }

    public ISourceField SourceField { get; set; }
  }
}
