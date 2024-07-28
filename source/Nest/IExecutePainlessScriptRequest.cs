// Decompiled with JetBrains decompiler
// Type: Nest.IExecutePainlessScriptRequest
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [MapsApi("scripts_painless_execute.json")]
  [InterfaceDataContract]
  public interface IExecutePainlessScriptRequest : 
    IRequest<ExecutePainlessScriptRequestParameters>,
    IRequest
  {
    [DataMember(Name = "context")]
    string Context { get; set; }

    [DataMember(Name = "context_setup")]
    IPainlessContextSetup ContextSetup { get; set; }

    [DataMember(Name = "script")]
    IInlineScript Script { get; set; }
  }
}
