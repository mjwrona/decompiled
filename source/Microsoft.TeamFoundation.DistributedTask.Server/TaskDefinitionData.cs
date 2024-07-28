// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.TaskDefinitionData
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal class TaskDefinitionData
  {
    private TaskDefinition m_definition;

    internal TaskDefinitionData()
    {
    }

    internal TaskDefinitionData(IVssRequestContext requestContext)
      : this(requestContext, (TaskDefinition) null)
    {
    }

    internal TaskDefinitionData(IVssRequestContext requestContext, TaskDefinition definition)
    {
      if (requestContext != null)
        this.SetHostType(requestContext);
      if (definition == null)
        return;
      this.ContributionIdentifier = definition.ContributionIdentifier;
      this.ContributionVersion = definition.ContributionVersion;
      this.Id = definition.Id;
      this.DisplayName = definition.FriendlyName;
      this.Name = definition.Name;
      this.Version = definition.Version;
      this.MetadataDocument = JsonUtility.Serialize((object) definition);
    }

    internal Guid Id { get; set; }

    internal string Name { get; set; }

    internal string DisplayName { get; set; }

    internal TaskVersion Version { get; set; }

    internal long? ContainerId { get; set; }

    internal string FilePath { get; set; }

    internal byte[] MetadataDocument { get; set; }

    internal TeamFoundationHostType HostType { get; private set; }

    internal string IconPath { get; set; }

    internal string ContributionIdentifier { get; set; }

    internal string ContributionVersion { get; set; }

    internal bool Disabled { get; set; }

    internal bool ContributionInstallComplete { get; set; }

    internal DateTime ContributionUpdatedOn { get; set; }

    internal TaskDefinition GetDefinition() => this.EnsureAndGetDefinition();

    internal bool IsVisible(string[] visiblities)
    {
      if (visiblities != null && visiblities.Length != 0)
      {
        IList<string> visibility = this.EnsureAndGetDefinition().Visibility;
        string[] array = visibility != null ? visibility.Where<string>((Func<string, bool>) (x => !string.Equals(x, "Preview", StringComparison.OrdinalIgnoreCase))).ToArray<string>() : (string[]) null;
        if (array != null && array.Length != 0)
          return ((IEnumerable<string>) array).Intersect<string>((IEnumerable<string>) visiblities, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).Any<string>();
      }
      return true;
    }

    internal bool IsDeployment => (this.HostType & TeamFoundationHostType.Deployment) == TeamFoundationHostType.Deployment;

    private TaskDefinition EnsureAndGetDefinition()
    {
      if (this.m_definition == null)
      {
        TaskDefinition taskDefinition = JsonUtility.Deserialize<TaskDefinition>(this.MetadataDocument);
        taskDefinition.ContentsUploaded = this.ContainerId.HasValue;
        taskDefinition.ServerOwned = this.IsDeployment;
        Interlocked.CompareExchange<TaskDefinition>(ref this.m_definition, taskDefinition, (TaskDefinition) null);
      }
      return this.m_definition;
    }

    internal void SetHostType(IVssRequestContext requestContext) => this.HostType = requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) ? TeamFoundationHostType.Deployment : requestContext.ServiceHost.HostType;
  }
}
