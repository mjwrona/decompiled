// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Integration.Server.TeamFoundationGroupSecurityService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Integration.Server
{
  [Obsolete("TeamFoundationGroupSecurityService is obsolete.  Please use the TeamFoundationIdentityService or TeamFoundationSecurityService instead.", false)]
  public class TeamFoundationGroupSecurityService : IVssFrameworkService
  {
    internal static readonly string s_gssPrefix = "GSS://";
    internal static readonly string s_adWinNTToken = "WinNT://";
    internal static readonly string s_adLDAPToken = "LDAP://";
    internal static readonly string s_adLocalToken = "LOCAL://";
    private TeamFoundationIdentityService m_identityService;

    internal TeamFoundationGroupSecurityService()
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.m_identityService = systemRequestContext.GetService<TeamFoundationIdentityService>();

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    internal string GetChangedIdentities(IVssRequestContext requestContext, int sequenceId)
    {
      using (CommandGetIdentityChanges getIdentityChanges = new CommandGetIdentityChanges(requestContext))
      {
        getIdentityChanges.Execute(sequenceId);
        StringBuilder output = new StringBuilder();
        using (XmlWriter xmlWriter1 = XmlWriter.Create(output))
        {
          xmlWriter1.WriteStartDocument();
          xmlWriter1.WriteStartElement("IdentityChanges");
          XmlWriter xmlWriter2 = xmlWriter1;
          int index = getIdentityChanges.LastSequenceId;
          string str1 = index.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          xmlWriter2.WriteAttributeString("MaxSequence", str1);
          xmlWriter1.WriteAttributeString("fMore", "0");
          xmlWriter1.WriteStartElement("Identities");
          foreach (Microsoft.TeamFoundation.Integration.Server.Identity identity in getIdentityChanges.Identities)
          {
            xmlWriter1.WriteStartElement("Identity");
            xmlWriter1.WriteAttributeString("SID", identity.Sid);
            xmlWriter1.WriteAttributeString("AccountName", identity.AccountName);
            xmlWriter1.WriteAttributeString("DisplayName", identity.DisplayName);
            xmlWriter1.WriteAttributeString("DistinguishedName", identity.DistinguishedName);
            xmlWriter1.WriteAttributeString("Domain", identity.Domain);
            xmlWriter1.WriteAttributeString("MailAddress", identity.MailAddress);
            xmlWriter1.WriteAttributeString("SpecialType", identity.SpecialType.ToString());
            xmlWriter1.WriteAttributeString("Type", identity.Type.ToString());
            xmlWriter1.WriteAttributeString("Deleted", identity.Deleted.ToString());
            if ((identity.Type == IdentityType.WindowsGroup || identity.Type == IdentityType.ApplicationGroup) && identity.Members != null && identity.Members.Length != 0 && !identity.Deleted)
            {
              xmlWriter1.WriteStartElement("Members");
              string[] members = identity.Members;
              for (index = 0; index < members.Length; ++index)
              {
                string str2 = members[index];
                xmlWriter1.WriteStartElement("Member");
                xmlWriter1.WriteAttributeString("SID", str2);
                xmlWriter1.WriteEndElement();
              }
              xmlWriter1.WriteEndElement();
            }
            xmlWriter1.WriteEndElement();
          }
          xmlWriter1.WriteEndElement();
          xmlWriter1.WriteEndElement();
          xmlWriter1.WriteEndDocument();
          xmlWriter1.Close();
          return output.ToString();
        }
      }
    }

    public TeamFoundationDataReader GetIdentityChanges(
      IVssRequestContext requestContext,
      int sequenceId)
    {
      CommandGetIdentityChanges disposableObject = (CommandGetIdentityChanges) null;
      try
      {
        disposableObject = new CommandGetIdentityChanges(requestContext);
        disposableObject.Execute(sequenceId);
        return new TeamFoundationDataReader((IDisposable) disposableObject, new object[2]
        {
          (object) disposableObject.LastSequenceId,
          (object) disposableObject.Identities
        });
      }
      catch (Exception ex)
      {
        disposableObject?.Dispose();
        throw;
      }
    }

    internal IdentityDescriptor GetSidBySearchFactor(
      IVssRequestContext requestContext,
      SearchFactor sf,
      string sfValue,
      ReadIdentityOptions readOptions)
    {
      IdentityDescriptor sidBySearchFactor = (IdentityDescriptor) null;
      switch (sf)
      {
        case SearchFactor.Sid:
          if (!string.IsNullOrEmpty(sfValue))
          {
            TeamFoundationIdentity foundationIdentity = this.m_identityService.ReadIdentity(requestContext, IdentitySearchFactor.Identifier, sfValue, MembershipQuery.None, readOptions, (IEnumerable<string>) null);
            if (foundationIdentity != null)
            {
              sidBySearchFactor = foundationIdentity.Descriptor;
              break;
            }
            break;
          }
          break;
        case SearchFactor.AccountName:
          TeamFoundationIdentity foundationIdentity1 = this.m_identityService.ReadIdentity(requestContext, IdentitySearchFactor.AccountName, sfValue, MembershipQuery.None, readOptions, (IEnumerable<string>) null);
          if (foundationIdentity1 != null)
          {
            sidBySearchFactor = foundationIdentity1.Descriptor;
            break;
          }
          break;
        case SearchFactor.DistinguishedName:
          try
          {
            if (!string.IsNullOrEmpty(sfValue))
            {
              if (sfValue.StartsWith(TeamFoundationGroupSecurityService.s_gssPrefix, StringComparison.OrdinalIgnoreCase))
              {
                string str = sfValue.Substring(TeamFoundationGroupSecurityService.s_gssPrefix.Length);
                if (str.StartsWith("CN=S-1-9-", StringComparison.OrdinalIgnoreCase))
                {
                  int num1 = str.IndexOf('=');
                  int num2 = str.IndexOf(',');
                  if (num1 >= 0)
                  {
                    if (num2 >= 0)
                    {
                      sidBySearchFactor = IdentityHelper.CreateWindowsDescriptor(str.Substring(num1 + 1, num2 - (num1 + 1)));
                      break;
                    }
                    break;
                  }
                  break;
                }
                break;
              }
              DirectoryEntry directoryEntry;
              if (sfValue.StartsWith(TeamFoundationGroupSecurityService.s_adWinNTToken, StringComparison.OrdinalIgnoreCase))
                directoryEntry = new DirectoryEntry(TeamFoundationGroupSecurityService.s_adWinNTToken + sfValue.Substring(TeamFoundationGroupSecurityService.s_adWinNTToken.Length));
              else if (sfValue.StartsWith(TeamFoundationGroupSecurityService.s_adLocalToken, StringComparison.OrdinalIgnoreCase))
              {
                directoryEntry = new DirectoryEntry(TeamFoundationGroupSecurityService.s_adWinNTToken + "./" + sfValue.Substring(TeamFoundationGroupSecurityService.s_adLocalToken.Length));
              }
              else
              {
                directoryEntry = new DirectoryEntry(!sfValue.StartsWith(TeamFoundationGroupSecurityService.s_adLDAPToken, StringComparison.OrdinalIgnoreCase) ? TeamFoundationGroupSecurityService.s_adLDAPToken + sfValue : TeamFoundationGroupSecurityService.s_adLDAPToken + sfValue.Substring(TeamFoundationGroupSecurityService.s_adLDAPToken.Length));
                if (directoryEntry != null && directoryEntry.SchemaClassName == "contact" && directoryEntry.Properties["msDS-SourceObjectDN"] != null)
                {
                  string str = directoryEntry.Properties["msDS-SourceObjectDN"].Value.ToString();
                  directoryEntry = new DirectoryEntry(TeamFoundationGroupSecurityService.s_adLDAPToken + str.Replace("/", "\\/"));
                }
              }
              if (directoryEntry != null)
              {
                if (directoryEntry.Properties["objectSid"] != null)
                {
                  IdentityHelper.CreateWindowsDescriptor(TeamFoundationGroupSecurityService.ExtractSid(directoryEntry.Properties["objectSid"].Value));
                  break;
                }
                break;
              }
              break;
            }
            break;
          }
          catch (COMException ex)
          {
            TeamFoundationTrace.Info(Microsoft.Azure.Boards.CssNodes.ServerResources.GSS_DIRECTORYSERVICESCOMEXCEPTION((object) sf.ToString(), (object) sfValue.ToString()));
            break;
          }
        case SearchFactor.AdministrativeApplicationGroup:
          sidBySearchFactor = IdentityHelper.CreateTeamFoundationDescriptor(this.m_identityService.GetProjectAdminSid(requestContext, sfValue));
          break;
        case SearchFactor.ServiceApplicationGroup:
          sidBySearchFactor = GroupWellKnownIdentityDescriptors.ServiceUsersGroup;
          break;
        case SearchFactor.EveryoneApplicationGroup:
          sidBySearchFactor = GroupWellKnownIdentityDescriptors.EveryoneGroup;
          break;
      }
      return sidBySearchFactor;
    }

    internal static SecurityIdentifier ExtractSid(object objectSid) => objectSid is byte[] binaryForm ? new SecurityIdentifier(binaryForm, 0) : throw new ActiveDirectoryAccessException(Microsoft.Azure.Boards.CssNodes.ServerResources.GSS_ACTIVE_DIRECTORY_EXTRACT_SID_EXCEPTION());
  }
}
