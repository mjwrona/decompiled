// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.Warning
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [CallOnDeserialization("AfterDeserialize")]
  public class Warning
  {
    internal Guid ownerId;
    internal ChangeType yourChangeType;
    internal int yourItemId;
    internal string yourServerItem;
    private WarningType m_warningType = WarningType.ResourcePendingChangeWarning;
    internal ChangeType ChangeType;
    private string m_user;
    private string m_userDisplayName;
    private string m_parentOrChildPath;
    private string m_workspace;

    [XmlAttribute("wrn")]
    [DefaultValue(WarningType.ResourcePendingChangeWarning)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public WarningType WarningType
    {
      get => this.m_warningType;
      set => this.m_warningType = value;
    }

    [XmlAttribute("chgEx")]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, PropertyName = "ChangeEx")]
    public int ChangeEx
    {
      get => (int) this.ChangeType;
      set => this.ChangeType |= (ChangeType) value;
    }

    [XmlAttribute("chg")]
    [DefaultValue(ChangeType.None)]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, PropertyName = "ChangeType")]
    public ChangeType ChangeTypeOld
    {
      get => PendingChange.GetLegacyChangeType(this.ChangeType);
      set => this.ChangeType |= value;
    }

    [XmlAttribute("user")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string User
    {
      get => this.m_user;
      set => this.m_user = value;
    }

    [XmlAttribute("userdisp")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string UserDisplayName
    {
      get => this.m_userDisplayName;
      set => this.m_userDisplayName = value;
    }

    [XmlAttribute("cpp")]
    [DefaultValue(null)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string ParentOrChildPath
    {
      get => this.m_parentOrChildPath;
      set => this.m_parentOrChildPath = value;
    }

    [XmlAttribute("ws")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string Workspace
    {
      get => this.m_workspace;
      set => this.m_workspace = value;
    }

    internal static void createFailuresFromWarnings(
      VersionControlRequestContext versionControlRequestContext,
      IList warnings,
      IList failures)
    {
      if (warnings.Count <= 0)
        return;
      Hashtable hashtable = new Hashtable();
      foreach (Warning warning in (IEnumerable) warnings)
      {
        Failure failure = (Failure) hashtable[(object) warning.yourItemId];
        if (failure == null)
        {
          failure = new Failure();
          failure.RequestType = ChangeRequest.changeTypeToRequestType(warning.yourChangeType);
          failure.Severity = SeverityType.Warning;
          failure.Message = Resources.Get("WarningPendChanges");
          failure.ServerItem = warning.yourServerItem;
          failure.ItemId = warning.yourItemId;
          hashtable[(object) warning.yourItemId] = (object) failure;
        }
        if (!warning.ownerId.Equals(Guid.Empty))
        {
          string identityName;
          string displayName;
          versionControlRequestContext.VersionControlService.SecurityWrapper.FindIdentityNames(versionControlRequestContext.RequestContext, warning.ownerId, out identityName, out displayName);
          warning.User = identityName;
          warning.UserDisplayName = displayName;
        }
        failure.Warnings.Add(warning);
      }
      foreach (Failure failure in (IEnumerable) hashtable.Values)
        failures.Add((object) failure);
    }
  }
}
