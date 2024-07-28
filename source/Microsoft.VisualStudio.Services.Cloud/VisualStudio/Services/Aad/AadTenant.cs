// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.AadTenant
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Aad
{
  [DataContract]
  public class AadTenant : AadObject
  {
    [DataMember]
    private IEnumerable<AadDomain> verifiedDomains;
    [DataMember]
    private string countryLetterCode;
    [DataMember]
    private bool? dirSyncEnabled;
    [DataMember]
    private DateTime? companyLastDirSyncTime;

    internal AadTenant()
    {
    }

    private AadTenant(Guid objectId, string displayName, IEnumerable<AadDomain> verifiedDomains)
      : base(objectId, displayName)
    {
      this.verifiedDomains = verifiedDomains;
    }

    public IEnumerable<AadDomain> VerifiedDomains
    {
      get => this.verifiedDomains;
      set => this.verifiedDomains = value;
    }

    public string CountryLetterCode
    {
      get => this.countryLetterCode;
      set => this.countryLetterCode = value;
    }

    public bool? DirSyncEnabled
    {
      get => this.dirSyncEnabled;
      set => this.dirSyncEnabled = value;
    }

    public DateTime? CompanyLastDirSyncTime
    {
      get => this.companyLastDirSyncTime;
      set => this.companyLastDirSyncTime = value;
    }

    public class Factory
    {
      public AadTenant Create()
      {
        IEnumerable<AadDomain> aadDomains = this.VerifiedDomains;
        if (aadDomains != null)
          aadDomains = (IEnumerable<AadDomain>) aadDomains.ToArray<AadDomain>();
        return new AadTenant(this.ObjectId, this.DisplayName, aadDomains)
        {
          CountryLetterCode = this.CountryLetterCode,
          DirSyncEnabled = this.DirSyncEnabled,
          CompanyLastDirSyncTime = this.CompanyLastDirSyncTime
        };
      }

      public Guid ObjectId { get; set; }

      public string DisplayName { get; set; }

      public IEnumerable<AadDomain> VerifiedDomains { get; set; }

      public string CountryLetterCode { get; set; }

      public bool? DirSyncEnabled { get; set; }

      public DateTime? CompanyLastDirSyncTime { get; set; }
    }
  }
}
