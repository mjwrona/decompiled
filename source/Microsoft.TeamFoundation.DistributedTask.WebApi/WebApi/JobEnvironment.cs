// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.JobEnvironment
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public sealed class JobEnvironment : ICloneable
  {
    private List<MaskHint> m_maskHints;
    private List<ServiceEndpoint> m_endpoints;
    private List<SecureFile> m_secureFiles;
    private IDictionary<Guid, JobOption> m_options;
    private IDictionary<string, string> m_variables;
    [DataMember(Name = "Endpoints", EmitDefaultValue = false)]
    private List<ServiceEndpoint> m_serializedEndpoints;
    [DataMember(Name = "SecureFiles", EmitDefaultValue = false)]
    private List<SecureFile> m_serializedSecureFiles;
    [DataMember(Name = "Options", EmitDefaultValue = false)]
    private IDictionary<Guid, JobOption> m_serializedOptions;
    [DataMember(Name = "Mask", EmitDefaultValue = false)]
    private List<MaskHint> m_serializedMaskHints;
    [DataMember(Name = "Variables", EmitDefaultValue = false)]
    private IDictionary<string, string> m_serializedVariables;

    public JobEnvironment()
    {
    }

    public JobEnvironment(
      IDictionary<string, VariableValue> variables,
      List<MaskHint> maskhints,
      JobResources resources)
    {
      if (resources != null)
      {
        this.Endpoints.AddRange(resources.Endpoints.Where<ServiceEndpoint>((Func<ServiceEndpoint, bool>) (x => !string.Equals(x.Name, "SystemVssConnection", StringComparison.OrdinalIgnoreCase))));
        this.SystemConnection = resources.Endpoints.FirstOrDefault<ServiceEndpoint>((Func<ServiceEndpoint, bool>) (x => string.Equals(x.Name, "SystemVssConnection", StringComparison.OrdinalIgnoreCase)));
        this.SecureFiles.AddRange((IEnumerable<SecureFile>) resources.SecureFiles);
      }
      if (maskhints != null)
        this.MaskHints.AddRange((IEnumerable<MaskHint>) maskhints);
      if (variables == null)
        return;
      foreach (KeyValuePair<string, VariableValue> variable in (IEnumerable<KeyValuePair<string, VariableValue>>) variables)
      {
        this.Variables[variable.Key] = variable.Value?.Value;
        VariableValue variableValue = variable.Value;
        if ((variableValue != null ? (variableValue.IsSecret ? 1 : 0) : 0) != 0)
          this.MaskHints.Add(new MaskHint()
          {
            Type = MaskType.Variable,
            Value = variable.Key
          });
      }
    }

    public void Extract(
      Dictionary<string, VariableValue> variables,
      HashSet<MaskHint> maskhints,
      JobResources jobResources)
    {
      HashSet<string> secretVariables = new HashSet<string>(this.MaskHints.Where<MaskHint>((Func<MaskHint, bool>) (t => t.Type == MaskType.Variable)).Select<MaskHint, string>((Func<MaskHint, string>) (v => v.Value)), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (KeyValuePair<string, string> variable in (IEnumerable<KeyValuePair<string, string>>) this.Variables)
        variables[variable.Key] = new VariableValue(variable.Value, secretVariables.Contains(variable.Key));
      maskhints.AddRange<MaskHint, HashSet<MaskHint>>(this.MaskHints.Where<MaskHint>((Func<MaskHint, bool>) (x => x.Type != MaskType.Variable || !secretVariables.Contains(x.Value))).Select<MaskHint, MaskHint>((Func<MaskHint, MaskHint>) (x => x.Clone())));
      jobResources.SecureFiles.AddRange(this.SecureFiles.Select<SecureFile, SecureFile>((Func<SecureFile, SecureFile>) (x => x.Clone())));
      jobResources.Endpoints.AddRange(this.Endpoints.Select<ServiceEndpoint, ServiceEndpoint>((Func<ServiceEndpoint, ServiceEndpoint>) (x => x.Clone())));
      if (this.SystemConnection == null)
        return;
      jobResources.Endpoints.Add(this.SystemConnection.Clone());
    }

    public JobEnvironment(PlanEnvironment environment)
    {
      ArgumentUtility.CheckForNull<PlanEnvironment>(environment, nameof (environment));
      if (environment.MaskHints.Count > 0)
        this.m_maskHints = new List<MaskHint>(environment.MaskHints.Select<MaskHint, MaskHint>((Func<MaskHint, MaskHint>) (x => x.Clone())));
      if (environment.Options.Count > 0)
        this.m_options = (IDictionary<Guid, JobOption>) environment.Options.ToDictionary<KeyValuePair<Guid, JobOption>, Guid, JobOption>((Func<KeyValuePair<Guid, JobOption>, Guid>) (x => x.Key), (Func<KeyValuePair<Guid, JobOption>, JobOption>) (x => x.Value.Clone()));
      if (environment.Variables.Count <= 0)
        return;
      this.m_variables = (IDictionary<string, string>) new Dictionary<string, string>(environment.Variables, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    private JobEnvironment(JobEnvironment environmentToClone)
    {
      if (environmentToClone.SystemConnection != null)
        this.SystemConnection = environmentToClone.SystemConnection.Clone();
      if (environmentToClone.m_maskHints != null)
        this.m_maskHints = environmentToClone.m_maskHints.Select<MaskHint, MaskHint>((Func<MaskHint, MaskHint>) (x => x.Clone())).ToList<MaskHint>();
      if (environmentToClone.m_endpoints != null)
        this.m_endpoints = environmentToClone.m_endpoints.Select<ServiceEndpoint, ServiceEndpoint>((Func<ServiceEndpoint, ServiceEndpoint>) (x => x.Clone())).ToList<ServiceEndpoint>();
      if (environmentToClone.m_secureFiles != null)
        this.m_secureFiles = environmentToClone.m_secureFiles.Select<SecureFile, SecureFile>((Func<SecureFile, SecureFile>) (x => x.Clone())).ToList<SecureFile>();
      if (environmentToClone.m_options != null)
        this.m_options = (IDictionary<Guid, JobOption>) environmentToClone.m_options.ToDictionary<KeyValuePair<Guid, JobOption>, Guid, JobOption>((Func<KeyValuePair<Guid, JobOption>, Guid>) (x => x.Key), (Func<KeyValuePair<Guid, JobOption>, JobOption>) (x => x.Value.Clone()));
      if (environmentToClone.m_variables == null)
        return;
      this.m_variables = (IDictionary<string, string>) new Dictionary<string, string>(environmentToClone.m_variables, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    [DataMember(EmitDefaultValue = false)]
    public ServiceEndpoint SystemConnection { get; set; }

    public List<MaskHint> MaskHints
    {
      get
      {
        if (this.m_maskHints == null)
          this.m_maskHints = new List<MaskHint>();
        return this.m_maskHints;
      }
    }

    public List<ServiceEndpoint> Endpoints
    {
      get
      {
        if (this.m_endpoints == null)
          this.m_endpoints = new List<ServiceEndpoint>();
        return this.m_endpoints;
      }
    }

    public List<SecureFile> SecureFiles
    {
      get
      {
        if (this.m_secureFiles == null)
          this.m_secureFiles = new List<SecureFile>();
        return this.m_secureFiles;
      }
    }

    public IDictionary<Guid, JobOption> Options
    {
      get
      {
        if (this.m_options == null)
          this.m_options = (IDictionary<Guid, JobOption>) new Dictionary<Guid, JobOption>();
        return this.m_options;
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

    object ICloneable.Clone() => (object) this.Clone();

    public JobEnvironment Clone() => new JobEnvironment(this);

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
      if (this.m_serializedMaskHints != null && this.m_serializedMaskHints.Count > 0)
        this.m_maskHints = new List<MaskHint>(this.m_serializedMaskHints.Distinct<MaskHint>());
      this.m_serializedMaskHints = (List<MaskHint>) null;
      SerializationHelper.Copy<string, string>(ref this.m_serializedVariables, ref this.m_variables, true);
      SerializationHelper.Copy<ServiceEndpoint>(ref this.m_serializedEndpoints, ref this.m_endpoints, true);
      SerializationHelper.Copy<SecureFile>(ref this.m_serializedSecureFiles, ref this.m_secureFiles, true);
      SerializationHelper.Copy<Guid, JobOption>(ref this.m_serializedOptions, ref this.m_options, true);
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      if (this.m_maskHints != null && this.m_maskHints.Count > 0)
        this.m_serializedMaskHints = new List<MaskHint>(this.m_maskHints.Distinct<MaskHint>());
      SerializationHelper.Copy<string, string>(ref this.m_variables, ref this.m_serializedVariables);
      SerializationHelper.Copy<ServiceEndpoint>(ref this.m_endpoints, ref this.m_serializedEndpoints);
      SerializationHelper.Copy<SecureFile>(ref this.m_secureFiles, ref this.m_serializedSecureFiles);
      SerializationHelper.Copy<Guid, JobOption>(ref this.m_options, ref this.m_serializedOptions);
    }

    [System.Runtime.Serialization.OnSerialized]
    private void OnSerialized(StreamingContext context)
    {
      this.m_serializedMaskHints = (List<MaskHint>) null;
      this.m_serializedVariables = (IDictionary<string, string>) null;
      this.m_serializedEndpoints = (List<ServiceEndpoint>) null;
      this.m_serializedSecureFiles = (List<SecureFile>) null;
      this.m_serializedOptions = (IDictionary<Guid, JobOption>) null;
    }
  }
}
