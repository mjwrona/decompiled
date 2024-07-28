// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.BranchProperties
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [CallOnDeserialization("AfterDeserialize")]
  [Serializable]
  public class BranchProperties : IValidatable
  {
    [NonSerialized]
    private ItemIdentifier m_rootItem;
    private string m_description;
    private string m_owner;
    private string m_ownerDisplayName;
    private Guid m_ownerId;
    private List<Mapping> m_mappings;
    [NonSerialized]
    private ItemIdentifier m_parentItem;

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public ItemIdentifier RootItem
    {
      get => this.m_rootItem;
      set => this.m_rootItem = value;
    }

    public string Description
    {
      get => this.m_description;
      set => this.m_description = value;
    }

    [ClientProperty(ClientVisibility.Private)]
    public string Owner
    {
      get => this.m_owner;
      set => this.m_owner = value;
    }

    [ClientProperty(ClientVisibility.Private)]
    public string OwnerDisplayName
    {
      get => this.m_ownerDisplayName;
      set => this.m_ownerDisplayName = value;
    }

    [ClientProperty(ClientVisibility.Private)]
    public string OwnerUniqueName
    {
      get => this.m_owner;
      set
      {
        if (!string.IsNullOrEmpty(this.m_owner))
          return;
        this.m_owner = value;
      }
    }

    internal Guid OwnerId
    {
      get => this.m_ownerId;
      set => this.m_ownerId = value;
    }

    public ItemIdentifier ParentBranch
    {
      get => this.m_parentItem;
      set => this.m_parentItem = value;
    }

    public List<Mapping> BranchMappings
    {
      get => this.m_mappings;
      set => this.m_mappings = value;
    }

    void IValidatable.Validate(
      VersionControlRequestContext versionControlRequestContext,
      string parameterName)
    {
      if (this.RootItem != null)
        this.RootItem.SetValidationOptions(BranchObject.ValidItemOptions, BranchObject.ValidVersionSpecOptions);
      versionControlRequestContext.Validation.check((IValidatable) this.RootItem, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) parameterName, (object) "RootItem"), false);
      if (this.ParentBranch != null)
      {
        this.ParentBranch.SetValidationOptions(BranchObject.ValidItemOptions, BranchObject.ValidVersionSpecOptions);
        versionControlRequestContext.Validation.check((IValidatable) this.ParentBranch, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) parameterName, (object) "ParentBranch"), true);
      }
      Mapping.ValidateSingleRootMappings(this.m_mappings, this.RootItem.Item, versionControlRequestContext);
      versionControlRequestContext.Validation.checkIdentity(ref this.m_owner, "owner", true);
    }
  }
}
