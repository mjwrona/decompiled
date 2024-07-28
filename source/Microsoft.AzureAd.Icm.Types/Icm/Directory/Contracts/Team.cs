// Decompiled with JetBrains decompiler
// Type: Microsoft.Icm.Directory.Contracts.Team
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using System.Collections.Generic;

namespace Microsoft.Icm.Directory.Contracts
{
  public class Team : ModifiableDocument
  {
    public static readonly string ActiveStatusText = TeamStatus.Active.ToString();
    public static readonly string DeprecatedStatusText = TeamStatus.Deprecated.ToString();
    private ICollection<TeamExternalReference> externalReferences;

    public string Name { get; set; }

    public string Description { get; set; }

    public string Keywords { get; set; }

    public string EmailAddress { get; set; }

    public long OwningServiceId { get; set; }

    public string Status { get; set; }

    public long? InheritsRosterFromTeamId { get; set; }

    public bool IsPrivate { get; set; }

    public bool IsAssignable { get; set; }

    public virtual ICollection<TeamMember> Members { get; set; }

    public virtual ICollection<Contact> Contacts { get; set; }

    public virtual Service OwningService { get; set; }

    public ICollection<TeamExternalReference> ExternalReferences
    {
      get => this.externalReferences ?? (this.externalReferences = (ICollection<TeamExternalReference>) new List<TeamExternalReference>());
      set => this.externalReferences = value;
    }
  }
}
