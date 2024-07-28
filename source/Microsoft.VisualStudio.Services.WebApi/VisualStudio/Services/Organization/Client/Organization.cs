// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.Client.Organization
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Organization.Client
{
  [DataContract]
  public sealed class Organization
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid Id { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public OrganizationType Type { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public OrganizationStatus Status { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string PreferredRegion { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string PreferredGeography { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public bool IsActivated { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public ICollection<Collection> Collections { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime DateCreated { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DateTime LastUpdated { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid CreatorId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid TenantId { get; internal set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Collection PrimaryCollection { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public PropertiesCollection Properties { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IDictionary<string, object> Data { get; set; }
  }
}
