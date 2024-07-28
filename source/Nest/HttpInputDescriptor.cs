// Decompiled with JetBrains decompiler
// Type: Nest.HttpInputDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class HttpInputDescriptor : 
    DescriptorBase<HttpInputDescriptor, IHttpInput>,
    IHttpInput,
    IInput
  {
    IEnumerable<string> IHttpInput.Extract { get; set; }

    IHttpInputRequest IHttpInput.Request { get; set; }

    Nest.ResponseContentType? IHttpInput.ResponseContentType { get; set; }

    public HttpInputDescriptor Request(
      Func<HttpInputRequestDescriptor, IHttpInputRequest> httpRequestSelector)
    {
      return this.Assign<IHttpInputRequest>(httpRequestSelector(new HttpInputRequestDescriptor()), (Action<IHttpInput, IHttpInputRequest>) ((a, v) => a.Request = v));
    }

    public HttpInputDescriptor Extract(IEnumerable<string> extract) => this.Assign<IEnumerable<string>>(extract, (Action<IHttpInput, IEnumerable<string>>) ((a, v) => a.Extract = v));

    public HttpInputDescriptor Extract(params string[] extract) => this.Assign<string[]>(extract, (Action<IHttpInput, string[]>) ((a, v) => a.Extract = (IEnumerable<string>) v));

    public HttpInputDescriptor ResponseContentType(Nest.ResponseContentType? responseContentType) => this.Assign<Nest.ResponseContentType?>(responseContentType, (Action<IHttpInput, Nest.ResponseContentType?>) ((a, v) => a.ResponseContentType = v));
  }
}
