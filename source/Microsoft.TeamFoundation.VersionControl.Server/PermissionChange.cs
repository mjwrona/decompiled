// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.PermissionChange
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [CallOnDeserialization("DetermineCorrectIdentityName")]
  public class PermissionChange : SecurityChange
  {
    internal Microsoft.VisualStudio.Services.Identity.Identity m_identity;
    private string m_identityName;
    private string m_displayName;
    private string[] m_allow;
    private string[] m_deny;
    private string[] m_remove;

    [XmlAttribute("ident")]
    public string IdentityName
    {
      get => this.m_identityName;
      set => this.m_identityName = value;
    }

    [XmlAttribute("disp")]
    [ClientProperty(ClientVisibility.Private)]
    public string DisplayName
    {
      get => this.m_displayName;
      set => this.m_displayName = value;
    }

    public string[] Allow
    {
      get => this.m_allow;
      set => this.m_allow = value;
    }

    public string[] Deny
    {
      get => this.m_deny;
      set => this.m_deny = value;
    }

    public string[] Remove
    {
      get => this.m_remove;
      set => this.m_remove = value;
    }

    internal override void Validate(
      VersionControlRequestContext versionControlRequestContext,
      string parameterName)
    {
      base.Validate(versionControlRequestContext, parameterName);
      versionControlRequestContext.Validation.checkIdentity(ref this.m_identityName, "IdentityName", false);
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.CurrentCulture, "PermissionChange(identity={0},display={1},allow={2},deny={3},remove={4})", (object) this.IdentityName, (object) this.DisplayName, (object) this.ArrayToString(this.Allow), (object) this.ArrayToString(this.Deny), (object) this.ArrayToString(this.Remove));

    private string ArrayToString(string[] stringArray)
    {
      if (stringArray == null || stringArray.Length == 0)
        return string.Empty;
      int capacity = 2;
      foreach (string str in stringArray)
        capacity += str.Length + 4;
      StringBuilder stringBuilder = new StringBuilder("{", capacity);
      bool flag = true;
      foreach (string str in stringArray)
      {
        if (!flag)
          stringBuilder.Append(", ");
        stringBuilder.Append(str);
        flag = false;
      }
      stringBuilder.Append("}");
      return stringBuilder.ToString();
    }
  }
}
