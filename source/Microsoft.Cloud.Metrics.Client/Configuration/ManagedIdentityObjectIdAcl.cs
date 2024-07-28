// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Configuration.ManagedIdentityObjectIdAcl
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.Cloud.Metrics.Client.Configuration
{
  public sealed class ManagedIdentityObjectIdAcl : IPermissionV2
  {
    public ManagedIdentityObjectIdAcl(
      ManagedIdentityByObjectId managedIdentityByObjectId,
      RoleConfiguration roleConfiguration,
      string description)
    {
      this.ManagedIdentityByObjectId = managedIdentityByObjectId != null ? managedIdentityByObjectId : throw new ArgumentNullException(nameof (managedIdentityByObjectId));
      this.RoleConfiguration = roleConfiguration;
      this.Description = description;
    }

    public ManagedIdentityByObjectId ManagedIdentityByObjectId { get; }

    public string Identity => this.ManagedIdentityByObjectId.Identity;

    public RoleConfiguration RoleConfiguration { get; set; }

    [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Description { get; }
  }
}
