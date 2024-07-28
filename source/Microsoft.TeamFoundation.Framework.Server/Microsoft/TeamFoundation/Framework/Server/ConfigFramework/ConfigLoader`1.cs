// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ConfigFramework.ConfigLoader`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.ConfigFramework.Factory;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server.ConfigFramework
{
  internal sealed class ConfigLoader<T> : IConfigLoader<T>
  {
    public static ConfigLoader<T> Default = new ConfigLoader<T>();
    private readonly IMatcherFactory<Guid> _matcherFactory;
    private readonly IRolloutCounterFactory _counterFactory;
    private readonly IExperimentFactory<T> _experimentFactory;
    private readonly IConfigImplementationFactory _configFactory;

    public ConfigLoader(
      IMatcherFactory<Guid> matcherFactory,
      IRolloutCounterFactory counterFactory,
      IExperimentFactory<T> experimentFactory,
      IConfigImplementationFactory configFactory)
    {
      this._matcherFactory = matcherFactory;
      this._counterFactory = counterFactory;
      this._experimentFactory = experimentFactory;
      this._configFactory = configFactory;
    }

    public ConfigLoader()
      : this((IMatcherFactory<Guid>) new MatcherFactory<Guid>(), (IRolloutCounterFactory) new RolloutCounterFactory(), (IExperimentFactory<T>) new ExperimentFactory<T>(), (IConfigImplementationFactory) new ConfigImplementationFactory())
    {
    }

    public IConfigQueryable<T> Load(IVssRequestContext context, T defaultValue, JToken payload)
    {
      List<ExperimentDto<T>> source = payload != null ? ConfigLoader<T>.ParseExperiments(payload) : throw new ArgumentNullException(nameof (payload));
      if (source.Count == 0)
        return this._configFactory.CreateSimpleConfig<T>(defaultValue);
      if (source.Count == 1)
      {
        ExperimentDto<T> expDto = source[0];
        List<IMatcher> matchers = this.ParseMatchers(expDto);
        if (matchers.Count == 0)
          return this._configFactory.CreateSimpleConfig<T>(expDto.Value);
        return this._configFactory.CreateComplexConfig<T>(defaultValue, new List<IExperiment<T>>()
        {
          this._experimentFactory.Create(expDto.Value, matchers)
        });
      }
      List<IExperiment<T>> list = source.Select<ExperimentDto<T>, IExperiment<T>>((Func<ExperimentDto<T>, IExperiment<T>>) (i => this._experimentFactory.Create(i.Value, this.ParseMatchers(i)))).ToList<IExperiment<T>>();
      return this._configFactory.CreateComplexConfig<T>(defaultValue, list);
    }

    private static List<ExperimentDto<T>> ParseExperiments(JToken payload)
    {
      switch (payload.Type)
      {
        case JTokenType.Integer:
        case JTokenType.Float:
        case JTokenType.String:
        case JTokenType.Boolean:
        case JTokenType.Date:
        case JTokenType.Guid:
        case JTokenType.Uri:
        case JTokenType.TimeSpan:
          return new List<ExperimentDto<T>>()
          {
            new ExperimentDto<T>() { Value = payload.ToObject<T>() }
          };
        default:
          return payload.ToObject<List<ExperimentDto<T>>>();
      }
    }

    private List<IMatcher> ParseMatchers(ExperimentDto<T> expDto)
    {
      List<IMatcher> matchers = new List<IMatcher>();
      if (expDto.VSID != null)
        matchers.Add(this.CreateVsidMatcher(expDto.VSID));
      if (expDto.CollectionHostId != null)
        matchers.Add(this.CreateHostIdMatcher(expDto.CollectionHostId));
      if (expDto.TenantId != null)
        matchers.Add(this.CreateTenantIdMatcher(expDto.TenantId));
      if (expDto.Percentage < 100)
        matchers.Add(this.CreateRolloutCounter(expDto.Percentage));
      return matchers;
    }

    private IMatcher CreateVsidMatcher(ExperimentIdDto dto) => this.CreateMatcher(dto, (QueryAccessor<Guid>) ((in Query q) => q.Id));

    private IMatcher CreateHostIdMatcher(ExperimentIdDto dto) => this.CreateMatcher(dto, (QueryAccessor<Guid>) ((in Query q) => q.CollectionHostId));

    private IMatcher CreateTenantIdMatcher(ExperimentIdDto dto) => this.CreateMatcher(dto, (QueryAccessor<Guid>) ((in Query q) => q.TenantId));

    private IMatcher CreateMatcher(ExperimentIdDto dto, QueryAccessor<Guid> accessor) => this._matcherFactory.Create((IEnumerable<Guid>) dto.List, dto.Percentage, accessor, (IEqualityComparer<Guid>) EqualityComparer<Guid>.Default);

    private IMatcher CreateRolloutCounter(int percentage) => this._counterFactory.Create(percentage);
  }
}
