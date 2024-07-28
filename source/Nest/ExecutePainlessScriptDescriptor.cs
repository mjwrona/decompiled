// Decompiled with JetBrains decompiler
// Type: Nest.ExecutePainlessScriptDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;

namespace Nest
{
  public class ExecutePainlessScriptDescriptor : 
    RequestDescriptorBase<ExecutePainlessScriptDescriptor, ExecutePainlessScriptRequestParameters, IExecutePainlessScriptRequest>,
    IExecutePainlessScriptRequest,
    IRequest<ExecutePainlessScriptRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceExecutePainlessScript;

    string IExecutePainlessScriptRequest.Context { get; set; }

    IPainlessContextSetup IExecutePainlessScriptRequest.ContextSetup { get; set; }

    IInlineScript IExecutePainlessScriptRequest.Script { get; set; }

    public ExecutePainlessScriptDescriptor Script(
      Func<InlineScriptDescriptor, IInlineScript> selector)
    {
      return this.Assign<Func<InlineScriptDescriptor, IInlineScript>>(selector, (Action<IExecutePainlessScriptRequest, Func<InlineScriptDescriptor, IInlineScript>>) ((a, v) => a.Script = v != null ? v(new InlineScriptDescriptor()) : (IInlineScript) null));
    }

    public ExecutePainlessScriptDescriptor ContextSetup(
      Func<PainlessContextSetupDescriptor, IPainlessContextSetup> selector)
    {
      return this.Assign<Func<PainlessContextSetupDescriptor, IPainlessContextSetup>>(selector, (Action<IExecutePainlessScriptRequest, Func<PainlessContextSetupDescriptor, IPainlessContextSetup>>) ((a, v) => a.ContextSetup = v != null ? v(new PainlessContextSetupDescriptor()) : (IPainlessContextSetup) null));
    }

    public ExecutePainlessScriptDescriptor Context(string context) => this.Assign<string>(context, (Action<IExecutePainlessScriptRequest, string>) ((a, v) => a.Context = v));
  }
}
