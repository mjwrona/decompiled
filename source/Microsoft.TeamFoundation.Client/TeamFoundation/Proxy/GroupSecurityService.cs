// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Proxy.GroupSecurityService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Server;
using System;
using System.Collections.Generic;
using System.Web.Services.Protocols;

namespace Microsoft.TeamFoundation.Proxy
{
  [Obsolete("IGroupSecurityService is obsolete.  Please use the IIdentityManagementService or ISecurityService instead.", false)]
  internal class GroupSecurityService : IGroupSecurityService
  {
    private Microsoft.TeamFoundation.Server.GroupSecurityService proxy;
    private IdentityManagementWebService m_proxyV3;
    private ICommonStructureService m_structureService;
    private bool m_serverIsV3;

    internal GroupSecurityService(TfsTeamProjectCollection tfs, string url)
    {
      this.proxy = new Microsoft.TeamFoundation.Server.GroupSecurityService(tfs, url);
      this.m_proxyV3 = new IdentityManagementWebService((TfsConnection) tfs);
      this.m_structureService = tfs.GetService<ICommonStructureService>();
      if (tfs.GetService<IRegistration>().GetRegistrationEntries("Framework").Length != 0)
        this.m_serverIsV3 = true;
      else
        this.m_serverIsV3 = false;
    }

    public Identity ReadIdentity(
      SearchFactor factor,
      string factorValue,
      QueryMembership queryMembership)
    {
      try
      {
        SecurityValidation.CheckFactorAndValue(factor, ref factorValue, nameof (factor), nameof (factorValue));
        SecurityValidation.CheckQueryMembership(queryMembership, nameof (queryMembership));
        return this.proxy.ReadIdentity(factor, factorValue, queryMembership);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public Identity[] ReadIdentities(
      SearchFactor factor,
      string[] factorValue,
      QueryMembership queryMembership)
    {
      try
      {
        SecurityValidation.CheckFactorAndValueArray(factor, ref factorValue, nameof (factor), nameof (factorValue));
        SecurityValidation.CheckQueryMembership(queryMembership, nameof (queryMembership));
        Identity[] identityArray = new Identity[factorValue.Length];
        if (this.m_serverIsV3 && factorValue.Length > 1 && factor == SearchFactor.Sid)
        {
          IdentityDescriptor[] descriptors = new IdentityDescriptor[factorValue.Length];
          for (int index = 0; index < factorValue.Length; ++index)
            descriptors[index] = IdentityHelper.CreateDescriptorFromSid(factorValue[index]);
          TeamFoundationIdentity[] foundationIdentityArray = this.m_proxyV3.ReadIdentitiesByDescriptor(descriptors, (int) queryMembership, 0, 1, (IEnumerable<string>) null, 0);
          for (int index = 0; index < foundationIdentityArray.Length; ++index)
          {
            if (foundationIdentityArray[index] != null)
              foundationIdentityArray[index].InitializeFromWebService();
            identityArray[index] = this.Convert(foundationIdentityArray[index]);
          }
        }
        else
        {
          for (int index = 0; index < factorValue.Length; ++index)
            identityArray[index] = this.proxy.ReadIdentity(factor, factorValue[index], queryMembership);
        }
        return identityArray;
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public bool IsIdentityCached(SearchFactor factor, string factorValue)
    {
      try
      {
        SecurityValidation.CheckFactorAndValue(factor, ref factorValue, nameof (factor), nameof (factorValue));
        return this.proxy.IsIdentityCached(factor, factorValue);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public string GetChangedIdentities(int sequence_id)
    {
      try
      {
        SecurityValidation.CheckSequenceId(sequence_id, nameof (sequence_id));
        return this.proxy.GetChangedIdentities(sequence_id);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public Identity ReadIdentityFromSource(SearchFactor factor, string factorValue)
    {
      try
      {
        SecurityValidation.CheckFactorAndValue(factor, ref factorValue, nameof (factor), nameof (factorValue));
        return this.proxy.ReadIdentityFromSource(factor, factorValue);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public string CreateApplicationGroup(
      string projectUri,
      string groupName,
      string groupDescription)
    {
      try
      {
        SecurityValidation.CheckProjectUri(ref projectUri, true, nameof (projectUri));
        SecurityValidation.CheckGroupName(ref groupName, nameof (groupName));
        SecurityValidation.CheckGroupDescription(ref groupDescription, nameof (groupDescription));
        return this.proxy.CreateApplicationGroup(projectUri, groupName, groupDescription);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public Identity[] ListApplicationGroups(string projectUri)
    {
      try
      {
        SecurityValidation.CheckProjectUri(ref projectUri, true, nameof (projectUri));
        return this.proxy.ListApplicationGroups(projectUri);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public void UpdateApplicationGroup(
      string groupSid,
      ApplicationGroupProperty property,
      string newValue)
    {
      try
      {
        SecurityValidation.CheckSid(ref groupSid, nameof (groupSid));
        SecurityValidation.CheckApplicationGroupPropertyAndValue(property, ref newValue, nameof (property), nameof (newValue));
        this.proxy.UpdateApplicationGroup(groupSid, property, newValue);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public void DeleteApplicationGroup(string groupSid)
    {
      try
      {
        SecurityValidation.CheckSid(ref groupSid, nameof (groupSid));
        this.proxy.DeleteApplicationGroup(groupSid);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public void AddMemberToApplicationGroup(string groupSid, string identitySid)
    {
      try
      {
        SecurityValidation.CheckSid(ref groupSid, nameof (groupSid));
        SecurityValidation.CheckSid(ref identitySid, nameof (identitySid));
        this.proxy.AddMemberToApplicationGroup(groupSid, identitySid);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public void RemoveMemberFromApplicationGroup(string groupSid, string identitySid)
    {
      try
      {
        SecurityValidation.CheckSid(ref groupSid, nameof (groupSid));
        SecurityValidation.CheckSid(ref identitySid, nameof (identitySid));
        this.proxy.RemoveMemberFromApplicationGroup(groupSid, identitySid);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public bool IsMember(string groupSid, string identitySid)
    {
      try
      {
        SecurityValidation.CheckSid(ref groupSid, nameof (groupSid));
        SecurityValidation.CheckSid(ref identitySid, nameof (identitySid));
        return this.proxy.IsMember(groupSid, identitySid);
      }
      catch (SoapException ex)
      {
        throw SoapExceptionUtilities.ConvertToStronglyTypedException(ex);
      }
    }

    public Identity Convert(TeamFoundationIdentity identity)
    {
      if (identity == null)
        return (Identity) null;
      Identity identity1 = new Identity()
      {
        Type = Identity.GetType(identity.Descriptor.IdentityType, identity.IsContainer),
        SpecialType = (ApplicationGroupSpecialType) IdentityHelper.GetGroupSpecialType(identity),
        Sid = identity.Descriptor.Identifier,
        TeamFoundationId = identity.TeamFoundationId,
        Description = identity.GetAttribute("Description", string.Empty),
        Domain = identity.GetAttribute("Domain", string.Empty),
        AccountName = identity.GetAttribute("Account", string.Empty),
        DistinguishedName = identity.GetAttribute("DN", string.Empty),
        MailAddress = identity.GetAttribute("Mail", string.Empty),
        Deleted = !identity.IsActive,
        SecurityGroup = !string.IsNullOrEmpty(identity.GetAttribute("SecurityGroup", (string) null))
      };
      identity1.DisplayName = identity1.Type != IdentityType.ApplicationGroup ? identity.DisplayName : identity1.AccountName;
      if (identity.Members != null)
      {
        identity1.Members = new string[identity.Members.Length];
        int num = 0;
        foreach (IdentityDescriptor member in identity.Members)
          identity1.Members[num++] = member.Identifier;
      }
      else
        identity1.Members = Array.Empty<string>();
      if (identity.MemberOf != null)
      {
        identity1.MemberOf = new string[identity.MemberOf.Length];
        int num = 0;
        foreach (IdentityDescriptor identityDescriptor in identity.MemberOf)
          identity1.MemberOf[num++] = identityDescriptor.Identifier;
      }
      else
        identity1.MemberOf = Array.Empty<string>();
      return identity1;
    }

    public TeamFoundationIdentity Convert(Identity identity)
    {
      if (identity == null)
        return (TeamFoundationIdentity) null;
      IdentityDescriptor descriptorFromSid = IdentityHelper.CreateDescriptorFromSid(identity.Sid);
      IdentityDescriptor[] members = (IdentityDescriptor[]) null;
      if (identity.Members != null)
      {
        members = new IdentityDescriptor[identity.Members.Length];
        for (int index = 0; index < identity.Members.Length; ++index)
          members[index] = IdentityHelper.CreateDescriptorFromSid(identity.Members[index]);
      }
      IdentityDescriptor[] memberOf = (IdentityDescriptor[]) null;
      if (identity.MemberOf != null)
      {
        memberOf = new IdentityDescriptor[identity.MemberOf.Length];
        for (int index = 0; index < identity.MemberOf.Length; ++index)
          memberOf[index] = IdentityHelper.CreateDescriptorFromSid(identity.MemberOf[index]);
      }
      TeamFoundationIdentity foundationIdentity = new TeamFoundationIdentity(descriptorFromSid, identity.DisplayName, !identity.Deleted, members, memberOf);
      if (identity.Type == IdentityType.ApplicationGroup || identity.Type == IdentityType.WindowsGroup)
        foundationIdentity.SetAttribute("SchemaClassName", "Group");
      else
        foundationIdentity.SetAttribute("SchemaClassName", "User");
      foundationIdentity.SetAttribute("Description", identity.Description);
      foundationIdentity.SetAttribute("Domain", identity.Domain);
      if (identity.Type == IdentityType.ApplicationGroup)
      {
        foundationIdentity.SetAttribute("Account", identity.DisplayName);
        if (string.IsNullOrEmpty(identity.Domain))
        {
          foundationIdentity.SetAttribute("GlobalScope", "GlobalScope");
        }
        else
        {
          string projectName = this.GetProjectName(identity.Domain);
          foundationIdentity.SetAttribute("ScopeName", projectName);
          foundationIdentity.DisplayName = "[" + projectName + "]\\" + identity.DisplayName;
        }
      }
      else
        foundationIdentity.SetAttribute("Account", identity.AccountName);
      foundationIdentity.SetAttribute("DN", identity.DistinguishedName);
      foundationIdentity.SetAttribute("Mail", identity.MailAddress);
      foundationIdentity.SetAttribute("SecurityGroup", "SecurityGroup");
      foundationIdentity.SetAttribute("SpecialType", identity.SpecialType.ToString());
      return foundationIdentity;
    }

    private string GetProjectName(string scopeId)
    {
      string projectName = scopeId;
      if (this.m_structureService != null)
      {
        try
        {
          ProjectInfo project = this.m_structureService.GetProject(scopeId);
          projectName = project == null ? string.Empty : project.Name;
        }
        catch (Exception ex)
        {
        }
      }
      return projectName;
    }
  }
}
