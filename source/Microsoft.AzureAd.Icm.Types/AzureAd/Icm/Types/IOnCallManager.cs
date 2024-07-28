// Decompiled with JetBrains decompiler
// Type: Microsoft.AzureAd.Icm.Types.IOnCallManager
// Assembly: Microsoft.AzureAd.Icm.Types, Version=2.1.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 6A852DFE-F17D-49CB-9E7D-8AB8112703DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.AzureAd.Icm.Types.dll

using Microsoft.AzureAd.Icm.Types.OnCall;
using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace Microsoft.AzureAd.Icm.Types
{
  [ServiceContract]
  public interface IOnCallManager
  {
    [OperationContract]
    [FaultContract(typeof (IcmFault))]
    ContactInformation GetContact(string alias);

    [OperationContract]
    [FaultContract(typeof (IcmFault))]
    ICollection<ContactInformation> GetContacts(string publicTeamId);

    [OperationContract]
    [FaultContract(typeof (IcmFault))]
    ContactUploadResult UploadContact(ContactInformation contact, bool allowUpdate);

    [OperationContract]
    [FaultContract(typeof (IcmFault))]
    ICollection<ContactUploadResult> UploadContacts(
      ICollection<ContactInformation> contactList,
      bool allowUpdate);

    [OperationContract]
    [FaultContract(typeof (IcmFault))]
    TeamMembershipUploadResult UploadTeamMembership(
      TeamMembership membership,
      Guid publicTenantId,
      TeamMembershipOperation operation);

    [OperationContract]
    [FaultContract(typeof (IcmFault))]
    ICollection<TeamMembershipUploadResult> UploadTeamMemberships(
      ICollection<TeamMembership> membershipList,
      Guid publicTenantId,
      TeamMembershipOperation operation);

    [OperationContract]
    [FaultContract(typeof (IcmFault))]
    TeamMembership GetTeamMembership(string alias);

    [OperationContract]
    [FaultContract(typeof (IcmFault))]
    PagedResult<TeamRotation> EnumOnCallContacts(
      TeamRotationCondition rotationCondition,
      PageToken token);
  }
}
