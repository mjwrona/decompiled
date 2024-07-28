// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.WebService.Client.OnCallManagerClient
// Assembly: Microsoft.AzureAd.Icm.WebService.Client, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 85C23930-39A1-49EE-A03A-507264E2FE4B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.WebService.Client.dll

using Microsoft.AzureAd.Icm.Types;
using Microsoft.AzureAd.Icm.Types.OnCall;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.AzureAd.Icm.WebService.Client
{
  public class OnCallManagerClient : ClientBase<IOnCallManager>, IOnCallManager
  {
    public OnCallManagerClient()
    {
    }

    public OnCallManagerClient(string endpointConfigurationName)
      : base(endpointConfigurationName)
    {
    }

    public OnCallManagerClient(string endpointConfigurationName, string remoteAddress)
      : base(endpointConfigurationName, remoteAddress)
    {
    }

    public OnCallManagerClient(string endpointConfigurationName, EndpointAddress remoteAddress)
      : base(endpointConfigurationName, remoteAddress)
    {
    }

    public OnCallManagerClient(Binding binding, EndpointAddress remoteAddress)
      : base(binding, remoteAddress)
    {
    }

    public ContactInformation GetContact(string alias) => this.Channel.GetContact(alias);

    public ICollection<ContactInformation> GetContacts(string publicTeamId) => this.Channel.GetContacts(publicTeamId);

    public ContactUploadResult UploadContact(ContactInformation contact, bool allowUpdate) => this.Channel.UploadContact(contact, allowUpdate);

    public ICollection<ContactUploadResult> UploadContacts(
      ICollection<ContactInformation> contactList,
      bool allowUpdate)
    {
      return this.Channel.UploadContacts(contactList, allowUpdate);
    }

    public TeamMembershipUploadResult UploadTeamMembership(
      TeamMembership membership,
      Guid publicTenantId,
      TeamMembershipOperation operation)
    {
      return this.Channel.UploadTeamMembership(membership, publicTenantId, operation);
    }

    public ICollection<TeamMembershipUploadResult> UploadTeamMemberships(
      ICollection<TeamMembership> membershipList,
      Guid publicTenantId,
      TeamMembershipOperation operation)
    {
      return this.Channel.UploadTeamMemberships(membershipList, publicTenantId, operation);
    }

    public TeamMembership GetTeamMembership(string alias) => this.Channel.GetTeamMembership(alias);

    public PagedResult<TeamRotation> EnumOnCallContacts(
      TeamRotationCondition rotationCondition,
      PageToken token)
    {
      return this.Channel.EnumOnCallContacts(rotationCondition, token);
    }
  }
}
