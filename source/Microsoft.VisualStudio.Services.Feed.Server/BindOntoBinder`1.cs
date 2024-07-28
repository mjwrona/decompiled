// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.BindOntoBinder`1
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class BindOntoBinder<T> : ObjectBinder<T> where T : new()
  {
    private readonly IBindOnto<T> binderOnto;

    public BindOntoBinder(IBindOnto<T> binderOnto) => this.binderOnto = binderOnto;

    protected override T Bind()
    {
      T obj = new T();
      this.binderOnto.BindOnto(this.Reader, obj);
      return obj;
    }
  }
}
