// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.RootCauseEntity
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Microsoft.AzureAd.Icm.Types
{
  [DataContract]
  public class RootCauseEntity
  {
    private static readonly IList<RootCauseBugEntity> EmptyRootCauseBugs = (IList<RootCauseBugEntity>) new List<RootCauseBugEntity>();
    private ICollection<RootCauseBugEntity> rootCauseBugs;

    [Key]
    [DataMember(Name = "Id")]
    public long RootCauseId { get; set; }

    [DataMember]
    public string Category { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public string Title { get; set; }

    [DataMember]
    public DateTime CreatedDate { get; set; }

    [DataMember]
    public DateTime? ModifiedDate { get; set; }

    [DataMember]
    public DateTime? LinkedIncidentDate { get; set; }

    [DataMember]
    public ICollection<RootCauseBugEntity> Bugs
    {
      get => this.rootCauseBugs ?? (this.rootCauseBugs = (ICollection<RootCauseBugEntity>) RootCauseEntity.EmptyRootCauseBugs);
      set => this.rootCauseBugs = value;
    }

    [DataMember(IsRequired = false)]
    [ReadOnly(true)]
    public long OwningTenantId { get; set; }

    public RootCauseEntity Clone() => (RootCauseEntity) this.MemberwiseClone();
  }
}
