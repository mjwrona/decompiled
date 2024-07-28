// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.EmailNotification.AccountCreationEmailEventData
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.EmailNotification
{
  [DataContract]
  public class AccountCreationEmailEventData
  {
    [DataMember]
    public string OrganizationUrl { get; set; }

    [DataMember]
    public string SignInAddress { get; set; }

    [DataMember]
    public string PreferredEmail { get; set; }

    [DataMember]
    public bool ContactWithOffers { get; set; }

    [DataMember]
    public string UpdatePreferencesLink { get; set; }

    [DataMember]
    public string ReviewProfileDetailsLink { get; set; }

    [DataMember]
    public string SupportLink { get; set; }

    [DataMember]
    public string HeaderAccountText { get; set; }

    [DataMember]
    public string StartProjectLink { get; set; }

    [DataMember]
    public string OpenInVsLink { get; set; }

    [DataMember]
    public string AddUsersLink { get; set; }

    [DataMember]
    public string EmailType { get; set; }
  }
}
