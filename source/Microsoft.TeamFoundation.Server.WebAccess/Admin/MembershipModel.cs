// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.MembershipModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  public class MembershipModel
  {
    private List<string> m_generalErrors;
    private List<string> m_licenceErrors;
    private List<string> m_stakeholderLicenceWarnings;
    private List<string> m_aadErrors;
    private List<IdentityViewModelBase> m_addedIdentities;
    private List<IdentityViewModelBase> m_deletedIdentities;
    private List<IdentityViewModelBase> m_failedAddedIdentities;
    private List<IdentityViewModelBase> m_failedDeletedIdentities;

    public bool EditMembers { get; set; }

    public bool HasErrors
    {
      get
      {
        if (this.m_generalErrors != null && this.m_generalErrors.Count > 0 || this.m_failedAddedIdentities != null && this.m_failedAddedIdentities.Count > 0 || this.m_failedDeletedIdentities != null && this.m_failedDeletedIdentities.Count > 0 || this.m_licenceErrors != null && this.m_licenceErrors.Count > 0)
          return true;
        return this.m_aadErrors != null && this.m_aadErrors.Count > 0;
      }
    }

    public bool HasWarnings => this.m_stakeholderLicenceWarnings != null && this.m_stakeholderLicenceWarnings.Count > 0;

    public List<string> GeneralErrors
    {
      get
      {
        if (this.m_generalErrors == null)
          this.m_generalErrors = new List<string>();
        return this.m_generalErrors;
      }
    }

    public List<IdentityViewModelBase> AddedIdentities
    {
      get
      {
        if (this.m_addedIdentities == null)
          this.m_addedIdentities = new List<IdentityViewModelBase>();
        return this.m_addedIdentities;
      }
    }

    public List<IdentityViewModelBase> DeletedIdentities
    {
      get
      {
        if (this.m_deletedIdentities == null)
          this.m_deletedIdentities = new List<IdentityViewModelBase>();
        return this.m_deletedIdentities;
      }
    }

    public List<IdentityViewModelBase> FailedAddedIdentities
    {
      get
      {
        if (this.m_failedAddedIdentities == null)
          this.m_failedAddedIdentities = new List<IdentityViewModelBase>();
        return this.m_failedAddedIdentities;
      }
    }

    public List<IdentityViewModelBase> FailedDeletedIdentities
    {
      get
      {
        if (this.m_failedDeletedIdentities == null)
          this.m_failedDeletedIdentities = new List<IdentityViewModelBase>();
        return this.m_failedDeletedIdentities;
      }
    }

    public List<string> LicenceErrors => this.m_licenceErrors ?? (this.m_licenceErrors = new List<string>());

    public List<string> StakeholderLicenceWarnings => this.m_stakeholderLicenceWarnings ?? (this.m_stakeholderLicenceWarnings = new List<string>());

    public List<string> AADErrors => this.m_aadErrors ?? (this.m_aadErrors = new List<string>());

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      json["AddedIdentities"] = (object) this.AddedIdentities.Select<IdentityViewModelBase, JsObject>((Func<IdentityViewModelBase, JsObject>) (x => x.ToJson()));
      json["FailedAddedIdentities"] = (object) this.FailedAddedIdentities.Select<IdentityViewModelBase, JsObject>((Func<IdentityViewModelBase, JsObject>) (x => x.ToJson()));
      json["DeletedIdentities"] = (object) this.DeletedIdentities.Select<IdentityViewModelBase, JsObject>((Func<IdentityViewModelBase, JsObject>) (x => x.ToJson()));
      json["FailedDeletedIdentities"] = (object) this.FailedDeletedIdentities.Select<IdentityViewModelBase, JsObject>((Func<IdentityViewModelBase, JsObject>) (x => x.ToJson()));
      json["HasErrors"] = (object) this.HasErrors;
      json["HasWarnings"] = (object) this.HasWarnings;
      json["GeneralErrors"] = (object) this.GeneralErrors;
      json["LicenceErrors"] = (object) this.LicenceErrors;
      json["StakeholderLicenceWarnings"] = (object) this.StakeholderLicenceWarnings;
      json["AADErrors"] = (object) this.AADErrors;
      return json;
    }
  }
}
