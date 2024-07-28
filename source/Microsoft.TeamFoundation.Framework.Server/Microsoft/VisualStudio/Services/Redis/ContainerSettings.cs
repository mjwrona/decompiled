// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.ContainerSettings
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Redis
{
  public class ContainerSettings
  {
    private static readonly ContainerSettings s_defaultSettings = new ContainerSettings()
    {
      KeySerializer = (IKeySerializer) new TypeConverterSerializer(),
      ValueSerializer = (IValueSerializer) new JsonTextSerializer(),
      KeyExpiry = new TimeSpan?(TimeSpan.FromDays(1.0))
    };
    private const string s_noThrowFeatureFlag = "VisualStudio.FrameworkService.RedisCache.NoThrowMode";

    public IKeySerializer KeySerializer { get; set; }

    public IValueSerializer ValueSerializer { get; set; }

    public TimeSpan? KeyExpiry { get; set; }

    public string CiAreaName { get; set; }

    public bool? NoThrowMode { get; set; }

    public bool? AllowBatching { get; set; }

    internal static ContainerSettings UserOrDefault(
      IVssRequestContext requestContext,
      Guid namespaceId,
      ContainerSettings settings,
      RedisConfiguration redisConfiguration)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IVssRegistryService service = vssRequestContext.GetService<IVssRegistryService>();
      TimeSpan? nullable1 = (TimeSpan?) settings?.KeyExpiry;
      if (nullable1.HasValue)
      {
        TimeSpan? nullable2 = nullable1;
        TimeSpan maxExpiry = RedisConfiguration.MaxExpiry;
        if ((nullable2.HasValue ? (nullable2.GetValueOrDefault() > maxExpiry ? 1 : 0) : 0) == 0)
          goto label_3;
      }
      nullable1 = new TimeSpan?(service.GetValue<TimeSpan>(vssRequestContext, (RegistryQuery) redisConfiguration.Keys.KeyExpiry(namespaceId), ContainerSettings.s_defaultSettings.KeyExpiry.Value));
label_3:
      bool? noThrowMode = (bool?) settings?.NoThrowMode;
      RegistryQuery registryQuery;
      if (!noThrowMode.HasValue)
      {
        ref bool? local1 = ref noThrowMode;
        IVssRegistryService registryService = service;
        IVssRequestContext requestContext1 = vssRequestContext;
        registryQuery = (RegistryQuery) redisConfiguration.Keys.NoThrowMode();
        ref RegistryQuery local2 = ref registryQuery;
        int num = registryService.GetValue<bool>(requestContext1, in local2, true) ? 1 : 0;
        local1 = new bool?(num != 0);
      }
      bool? allowBatching = (bool?) settings?.AllowBatching;
      if (!allowBatching.HasValue)
      {
        ref bool? local3 = ref allowBatching;
        IVssRegistryService registryService = service;
        IVssRequestContext requestContext2 = vssRequestContext;
        registryQuery = (RegistryQuery) redisConfiguration.Keys.AllowBatching();
        ref RegistryQuery local4 = ref registryQuery;
        int num = registryService.GetValue<bool>(requestContext2, in local4, false) ? 1 : 0;
        local3 = new bool?(num != 0);
      }
      return new ContainerSettings()
      {
        KeyExpiry = nullable1,
        KeySerializer = settings?.KeySerializer ?? ContainerSettings.s_defaultSettings.KeySerializer,
        ValueSerializer = settings?.ValueSerializer ?? ContainerSettings.s_defaultSettings.ValueSerializer,
        CiAreaName = settings?.CiAreaName ?? namespaceId.ToString(),
        NoThrowMode = noThrowMode,
        AllowBatching = allowBatching
      };
    }

    public ContainerSettings Clone() => (ContainerSettings) this.MemberwiseClone();
  }
}
