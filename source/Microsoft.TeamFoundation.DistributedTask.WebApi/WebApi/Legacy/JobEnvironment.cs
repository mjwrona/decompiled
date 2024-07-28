// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.Legacy.JobEnvironment
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi.Legacy
{
  [DataContract]
  [JsonConverter(typeof (LegacyJsonConverter<JobEnvironment>))]
  public sealed class JobEnvironment : ICloneable
  {
    private List<MaskHint> m_maskHints;
    private List<JobEndpoint> m_endpoints;
    private IDictionary<string, string> m_variables;
    private IDictionary<string, object> m_secrets;
    private IDictionary<Guid, JobOption> m_jobOptions;
    [DataMember(Name = "Endpoints", Order = 2, EmitDefaultValue = false)]
    private List<JobEndpoint> m_serializedEndpoints;
    [DataMember(Name = "Options", Order = 6, EmitDefaultValue = false)]
    private IDictionary<Guid, JobOption> m_serializedJobOptions;
    [DataMember(Name = "Mask", Order = 7, EmitDefaultValue = false)]
    private List<MaskHint> m_serializedMaskHints;
    [DataMember(Name = "Variables", Order = 5, EmitDefaultValue = false)]
    private IDictionary<string, string> m_serializedVariables;
    private static readonly Lazy<Regex> s_variableReferenceRegex = new Lazy<Regex>((Func<Regex>) (() => new Regex("\\$\\(([^)]+)\\)", RegexOptions.Compiled | RegexOptions.Singleline)), true);

    public JobEnvironment()
    {
    }

    public JobEnvironment(PlanEnvironment environment)
    {
      ArgumentUtility.CheckForNull<PlanEnvironment>(environment, nameof (environment));
      if (environment.MaskHints.Count > 0)
        this.m_maskHints = new List<MaskHint>(environment.MaskHints.Select<MaskHint, MaskHint>((Func<MaskHint, MaskHint>) (x => x.Clone())));
      if (environment.Options.Count > 0)
        this.m_jobOptions = (IDictionary<Guid, JobOption>) environment.Options.ToDictionary<KeyValuePair<Guid, JobOption>, Guid, JobOption>((Func<KeyValuePair<Guid, JobOption>, Guid>) (x => x.Key), (Func<KeyValuePair<Guid, JobOption>, JobOption>) (x => x.Value.Clone()));
      if (environment.Variables.Count <= 0)
        return;
      this.m_variables = (IDictionary<string, string>) new Dictionary<string, string>(environment.Variables, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    private JobEnvironment(JobEnvironment environmentToClone)
    {
      if (environmentToClone.m_maskHints != null)
        this.m_maskHints = environmentToClone.m_maskHints.Select<MaskHint, MaskHint>((Func<MaskHint, MaskHint>) (x => x.Clone())).ToList<MaskHint>();
      if (environmentToClone.m_variables != null)
        this.m_variables = (IDictionary<string, string>) new Dictionary<string, string>(environmentToClone.m_variables, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      if (environmentToClone.m_endpoints != null)
        this.m_endpoints = environmentToClone.m_endpoints.Select<JobEndpoint, JobEndpoint>((Func<JobEndpoint, JobEndpoint>) (x => x.Clone())).ToList<JobEndpoint>();
      if (environmentToClone.m_jobOptions == null)
        return;
      this.m_jobOptions = (IDictionary<Guid, JobOption>) environmentToClone.m_jobOptions.ToDictionary<KeyValuePair<Guid, JobOption>, Guid, JobOption>((Func<KeyValuePair<Guid, JobOption>, Guid>) (x => x.Key), (Func<KeyValuePair<Guid, JobOption>, JobOption>) (x => x.Value.Clone()));
    }

    public List<MaskHint> MaskHints
    {
      get
      {
        if (this.m_maskHints == null)
          this.m_maskHints = new List<MaskHint>();
        return this.m_maskHints;
      }
    }

    public IDictionary<string, string> Variables
    {
      get
      {
        if (this.m_variables == null)
          this.m_variables = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_variables;
      }
    }

    public List<JobEndpoint> Endpoints
    {
      get
      {
        if (this.m_endpoints == null)
          this.m_endpoints = new List<JobEndpoint>();
        return this.m_endpoints;
      }
    }

    [Obsolete]
    public IDictionary<string, object> Secrets
    {
      get
      {
        if (this.m_secrets == null)
          this.m_secrets = (IDictionary<string, object>) new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_secrets;
      }
    }

    public IDictionary<Guid, JobOption> Options
    {
      get
      {
        if (this.m_jobOptions == null)
          this.m_jobOptions = (IDictionary<Guid, JobOption>) new Dictionary<Guid, JobOption>();
        return this.m_jobOptions;
      }
    }

    object ICloneable.Clone() => (object) this.Clone();

    public JobEnvironment Clone() => new JobEnvironment(this);

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
      Microsoft.TeamFoundation.DistributedTask.WebApi.SerializationHelper.Copy<MaskHint>(ref this.m_serializedMaskHints, ref this.m_maskHints, true);
      Microsoft.TeamFoundation.DistributedTask.WebApi.SerializationHelper.Copy<string, string>(ref this.m_serializedVariables, ref this.m_variables, true);
      Microsoft.TeamFoundation.DistributedTask.WebApi.SerializationHelper.Copy<JobEndpoint>(ref this.m_serializedEndpoints, ref this.m_endpoints, true);
      Microsoft.TeamFoundation.DistributedTask.WebApi.SerializationHelper.Copy<Guid, JobOption>(ref this.m_serializedJobOptions, ref this.m_jobOptions, true);
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      Microsoft.TeamFoundation.DistributedTask.WebApi.SerializationHelper.Copy<MaskHint>(ref this.m_maskHints, ref this.m_serializedMaskHints);
      Microsoft.TeamFoundation.DistributedTask.WebApi.SerializationHelper.Copy<string, string>(ref this.m_variables, ref this.m_serializedVariables);
      Microsoft.TeamFoundation.DistributedTask.WebApi.SerializationHelper.Copy<JobEndpoint>(ref this.m_endpoints, ref this.m_serializedEndpoints);
      Microsoft.TeamFoundation.DistributedTask.WebApi.SerializationHelper.Copy<Guid, JobOption>(ref this.m_jobOptions, ref this.m_serializedJobOptions);
    }

    [System.Runtime.Serialization.OnSerialized]
    private void OnSerialized(StreamingContext context)
    {
      this.m_serializedMaskHints = (List<MaskHint>) null;
      this.m_serializedVariables = (IDictionary<string, string>) null;
      this.m_serializedEndpoints = (List<JobEndpoint>) null;
      this.m_serializedJobOptions = (IDictionary<Guid, JobOption>) null;
    }
  }
}
