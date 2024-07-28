// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.ProjectDomainInfo
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using Microsoft.VisualStudio.Services.Content.Common.MultiDomainExceptions;
using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  [Serializable]
  public class ProjectDomainInfo : MultiDomainInfo
  {
    [JsonConverter(typeof (DomainIdJsonConverter))]
    public IDomainId PhysicalDomainId { get; private set; }

    public ProjectDomainInfo.AssociatedProject Project { get; private set; }

    public string PartitionName { get; private set; }

    public ProjectDomainInfo(
      IDomainId projectDomainId,
      PhysicalDomainInfo associatedPhysicalDomain,
      ProjectDomainInfo.AssociatedProject project,
      string partitionKey)
      : base(projectDomainId, false, associatedPhysicalDomain.Region, associatedPhysicalDomain.RedundancyType)
    {
      this.PhysicalDomainId = associatedPhysicalDomain.DomainId;
      this.Project = project ?? throw new InvalidProjectException(Resources.ProjectDomainProjectIsInvalid((object) associatedPhysicalDomain.DomainId));
      this.PartitionName = this.IsPartitionKeyValid(partitionKey) ? partitionKey : throw new InvalidPartitionKeyException(Resources.InvalidPartitionKey());
    }

    private bool IsPartitionKeyValid(string partitionKey)
    {
      if (string.IsNullOrEmpty(partitionKey) || partitionKey.Length < 3 || partitionKey.Length > 63 || !this.IsAlphaNumeric(partitionKey))
        return false;
      char ch1 = partitionKey[0];
      for (int index = 1; index < partitionKey.Length; ++index)
      {
        char ch2 = partitionKey[index];
        if ((int) ch1 == (int) ch2 && ch1 == '-')
          return false;
        ch1 = ch2;
      }
      return true;
    }

    private bool IsAlphaNumeric(string str) => !new Regex("[^a-zA-Z0-9]").IsMatch(str);

    [Serializable]
    public class AssociatedProject
    {
      public Guid Id { get; set; }

      public string AzureCompatibleId { get; set; }

      public string Name { get; set; }

      [JsonConstructor]
      public AssociatedProject()
      {
      }
    }
  }
}
