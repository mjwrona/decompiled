// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.RepositoryResource
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class RepositoryResource : Resource
  {
    public RepositoryResource()
    {
    }

    private RepositoryResource(RepositoryResource referenceToCopy)
      : base((Resource) referenceToCopy)
    {
    }

    public string Id
    {
      get => this.Properties.Get<string>(RepositoryPropertyNames.Id);
      set => this.Properties.Set<string>(RepositoryPropertyNames.Id, value);
    }

    public string Name
    {
      get => this.Properties.Get<string>(RepositoryPropertyNames.Name);
      set => this.Properties.Set<string>(RepositoryPropertyNames.Name, value);
    }

    public string Ref
    {
      get => this.Properties.Get<string>(RepositoryPropertyNames.Ref);
      set => this.Properties.Set<string>(RepositoryPropertyNames.Ref, value);
    }

    public string Type
    {
      get => this.Properties.Get<string>(RepositoryPropertyNames.Type);
      set => this.Properties.Set<string>(RepositoryPropertyNames.Type, value);
    }

    public Uri Url
    {
      get => this.Properties.Get<Uri>(RepositoryPropertyNames.Url);
      set => this.Properties.Set<Uri>(RepositoryPropertyNames.Url, value);
    }

    public string Version
    {
      get => this.Properties.Get<string>(RepositoryPropertyNames.Version);
      set => this.Properties.Set<string>(RepositoryPropertyNames.Version, value);
    }

    public bool IsRoot
    {
      get => this.Properties.Get<bool>(RepositoryPropertyNames.IsRoot);
      set => this.Properties.Set<bool>(RepositoryPropertyNames.IsRoot, value);
    }

    public ContinuousIntegrationTrigger Trigger
    {
      get => this.Properties.Get<ContinuousIntegrationTrigger>(RepositoryPropertyNames.Trigger);
      set => this.Properties.Set<ContinuousIntegrationTrigger>(RepositoryPropertyNames.Trigger, value);
    }

    public PullRequestTrigger PR
    {
      get => this.Properties.Get<PullRequestTrigger>(RepositoryPropertyNames.PR);
      set => this.Properties.Set<PullRequestTrigger>(RepositoryPropertyNames.PR, value);
    }

    public RepositoryResource Clone() => new RepositoryResource(this);
  }
}
