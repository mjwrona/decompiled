// Decompiled with JetBrains decompiler
// Type: Nest.IConnectionSettingsValues
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Nest
{
  public interface IConnectionSettingsValues : IConnectionConfigurationValues, IDisposable
  {
    Func<string, string> DefaultFieldNameInferrer { get; }

    string DefaultIndex { get; }

    FluentDictionary<Type, string> DefaultIndices { get; }

    FluentDictionary<Type, string> DefaultRelationNames { get; }

    FluentDictionary<Type, string> IdProperties { get; }

    Inferrer Inferrer { get; }

    IPropertyMappingProvider PropertyMappingProvider { get; }

    FluentDictionary<MemberInfo, IPropertyMapping> PropertyMappings { get; }

    FluentDictionary<Type, string> RouteProperties { get; }

    HashSet<Type> DisableIdInference { get; }

    bool DefaultDisableIdInference { get; }

    IElasticsearchSerializer SourceSerializer { get; }
  }
}
