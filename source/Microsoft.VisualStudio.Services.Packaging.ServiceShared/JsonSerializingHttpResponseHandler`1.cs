// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.JsonSerializingHttpResponseHandler`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.WebApi;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class JsonSerializingHttpResponseHandler<TObject> : 
    IAsyncHandler<TObject, HttpResponseMessage>,
    IHaveInputType<TObject>,
    IHaveOutputType<HttpResponseMessage>
  {
    public Task<HttpResponseMessage> Handle(TObject request) => Task.FromResult<HttpResponseMessage>(new HttpResponseMessage()
    {
      Content = (HttpContent) new ObjectContent<TObject>(request, (MediaTypeFormatter) new VssJsonMediaTypeFormatter())
    });
  }
}
