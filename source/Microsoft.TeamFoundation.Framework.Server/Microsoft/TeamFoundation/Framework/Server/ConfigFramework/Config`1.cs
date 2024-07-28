// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ConfigFramework.Config`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.TeamFoundation.Framework.Server.ConfigFramework
{
  internal sealed class Config<T> : IConfig<T>, IConfigQueryable<T>, IConfigReloadable
  {
    private readonly IConfigPrototype<T> _prototype;
    private string _serializedConfig;
    private readonly string _serializedDefault;
    private volatile IConfigQueryable<T> _config;

    public Config(IConfigPrototype<T> prototype)
      : this(prototype, (IConfigQueryable<T>) new SimpleConfig<T>(prototype.Default))
    {
    }

    public Config(IConfigPrototype<T> prototype, IConfigQueryable<T> config)
    {
      this._prototype = prototype;
      this._serializedConfig = "";
      this._serializedDefault = JsonConvert.SerializeObject((object) prototype.Default);
      this._config = config;
    }

    public T Query(IVssRequestContext context, in Microsoft.TeamFoundation.Framework.Server.ConfigFramework.Query query) => this._config.Query(context, in query);

    public void Reload(IVssRequestContext context, string config)
    {
      this._config = this._prototype.Loader.Load(context, this._prototype.Default, JToken.Parse(config));
      this._serializedConfig = config;
    }

    public string GetConfigPayload() => this._serializedConfig;

    public string GetConfigDefault() => this._serializedDefault;

    T IConfigQueryable<T>.Query(IVssRequestContext context, in Microsoft.TeamFoundation.Framework.Server.ConfigFramework.Query query) => this.Query(context, in query);
  }
}
