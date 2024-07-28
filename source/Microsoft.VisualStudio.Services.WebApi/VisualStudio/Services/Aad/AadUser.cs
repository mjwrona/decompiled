// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Aad.AadUser
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Aad
{
  [DataContract]
  public class AadUser : AadObject
  {
    [DataMember]
    private bool accountEnabled;
    [DataMember]
    private string mail;
    [DataMember]
    private IEnumerable<string> otherMails;
    [DataMember]
    private string mailNickname;
    [DataMember]
    private string userPrincipalName;
    [DataMember]
    private string signInAddress;
    [DataMember]
    private bool hasThumbnailPhoto;
    [DataMember]
    private string jobTitle;
    [DataMember]
    private string department;
    [DataMember]
    private string physicalDeliveryOfficeName;
    [DataMember]
    private AadUser manager;
    [DataMember]
    private IEnumerable<AadUser> directReports;
    [DataMember]
    private string userType;
    [DataMember]
    private string userState;
    [DataMember]
    private string surname;
    [DataMember]
    private string onPremisesSecurityIdentifier;
    [DataMember]
    private string immutableId;
    [DataMember]
    private string telephoneNumber;
    [DataMember]
    private string country;
    [DataMember]
    private string usageLocation;

    protected AadUser()
    {
    }

    private AadUser(
      Guid objectId,
      string displayName,
      bool accountEnabled,
      string mail,
      IEnumerable<string> otherMails,
      string userPrincipalName,
      string signInAddress,
      bool hasThumbnailPhoto,
      string jobTitle,
      string department,
      string physicalDeliveryOfficeName,
      string mailNickname,
      AadUser manager,
      IEnumerable<AadUser> directReports,
      string userType,
      string userState,
      string surname,
      string onPremisesSecurityIdentifier,
      string immutableId,
      string telephoneNumber,
      string country,
      string usageLocation)
      : base(objectId, displayName)
    {
      this.accountEnabled = accountEnabled;
      this.mail = mail;
      this.otherMails = otherMails;
      this.mailNickname = mailNickname;
      this.userPrincipalName = userPrincipalName;
      this.signInAddress = signInAddress;
      this.hasThumbnailPhoto = hasThumbnailPhoto;
      this.jobTitle = jobTitle;
      this.department = department;
      this.physicalDeliveryOfficeName = physicalDeliveryOfficeName;
      this.manager = manager;
      this.directReports = directReports;
      this.userType = userType;
      this.userState = userState;
      this.surname = surname;
      this.onPremisesSecurityIdentifier = onPremisesSecurityIdentifier;
      this.immutableId = immutableId;
      this.telephoneNumber = telephoneNumber;
      this.country = country;
      this.usageLocation = usageLocation;
    }

    public bool AccountEnabled
    {
      get => this.accountEnabled;
      set => this.accountEnabled = value;
    }

    public string Mail
    {
      get => this.mail;
      set => this.mail = value;
    }

    public IEnumerable<string> OtherMails
    {
      get => this.otherMails;
      set => this.otherMails = value;
    }

    public string MailNickname => this.mailNickname;

    public string UserPrincipalName
    {
      get => this.userPrincipalName;
      set => this.userPrincipalName = value;
    }

    public string SignInAddress => this.signInAddress;

    public bool HasThumbnailPhoto => this.hasThumbnailPhoto;

    public string JobTitle => this.jobTitle;

    public string Department => this.department;

    public string PhysicalDeliveryOfficeName => this.physicalDeliveryOfficeName;

    public AadUser Manager => this.manager;

    public IEnumerable<AadUser> DirectReports => this.directReports;

    public string UserType => this.userType;

    public string UserState => this.userState;

    public string Surname => this.surname;

    public string OnPremisesSecurityIdentifier => this.onPremisesSecurityIdentifier;

    public string ImmutableId => this.immutableId;

    public string TelephoneNumber
    {
      get => this.telephoneNumber;
      set => this.telephoneNumber = value;
    }

    public string Country
    {
      get => this.country;
      set => this.country = value;
    }

    public string UsageLocation
    {
      get => this.usageLocation;
      set => this.usageLocation = value;
    }

    public class Factory
    {
      public AadUser Create()
      {
        IEnumerable<string> strings = this.OtherMails;
        if (strings != null)
          strings = (IEnumerable<string>) strings.ToArray<string>();
        IEnumerable<AadUser> aadUsers = this.DirectReports;
        if (aadUsers != null)
          aadUsers = (IEnumerable<AadUser>) aadUsers.ToArray<AadUser>();
        return new AadUser(this.ObjectId, this.DisplayName, this.AccountEnabled, this.Mail, strings, this.UserPrincipalName, this.SignInAddress, this.HasThumbnailPhoto, this.JobTitle, this.Department, this.PhysicalDeliveryOfficeName, this.MailNickname, this.Manager, aadUsers, this.UserType, this.UserState, this.Surname, this.OnPremisesSecurityIdentifier, this.ImmutableId, this.TelephoneNumber, this.Country, this.UsageLocation);
      }

      public Guid ObjectId { get; set; }

      public string DisplayName { get; set; }

      public bool AccountEnabled { get; set; }

      public string Mail { get; set; }

      public IEnumerable<string> OtherMails { get; set; }

      public string MailNickname { get; set; }

      public string UserPrincipalName { get; set; }

      public string SignInAddress { get; set; }

      public bool HasThumbnailPhoto { get; set; }

      public string JobTitle { get; set; }

      public string Department { get; set; }

      public string PhysicalDeliveryOfficeName { get; set; }

      public AadUser Manager { get; set; }

      public IEnumerable<AadUser> DirectReports { get; set; }

      public string UserType { get; set; }

      public string UserState { get; set; }

      public string Surname { get; set; }

      public string OnPremisesSecurityIdentifier { get; set; }

      public string ImmutableId { get; set; }

      public string TelephoneNumber { get; set; }

      public string Country { get; set; }

      public string UsageLocation { get; set; }
    }
  }
}
