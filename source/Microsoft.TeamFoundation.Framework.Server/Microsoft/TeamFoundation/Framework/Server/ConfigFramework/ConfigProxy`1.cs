// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ConfigFramework.ConfigProxy`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server.ConfigFramework
{
  internal sealed class ConfigProxy<T> : IConfigQueryable<T>
  {
    private readonly IConfigPrototype<T> _prototype;
    private readonly object _lock = new object();
    private volatile IConfigQueryable<T> _config;

    public ConfigProxy(IConfigPrototype<T> prototype) => this._prototype = prototype;

    public ConfigProxy(string name)
      : this(ConfigPrototype.Create<T>(name))
    {
    }

    public T Query(IVssRequestContext context, in Microsoft.TeamFoundation.Framework.Server.ConfigFramework.Query query) => this.Get(context).Query(context, in query);

    private IConfigQueryable<T> Get(IVssRequestContext context)
    {
      if (this._config != null)
        return this._config;
      lock (this._lock)
      {
        if (this._config != null)
          return this._config;
        IVssRequestContext context1 = context.To(TeamFoundationHostType.Deployment);
        this._config = context1.GetService<IConfigService>().Register<T>(context1, this._prototype);
        return this._config;
      }
    }

    T IConfigQueryable<T>.Query(IVssRequestContext context, in Microsoft.TeamFoundation.Framework.Server.ConfigFramework.Query query) => this.Query(context, in query);
  }
}
