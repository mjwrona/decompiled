// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Navigation.UserContext
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

namespace Microsoft.TeamFoundation.Server.WebAccess.Navigation
{
  public class UserContext
  {
    public bool IsAcsAccount { get; set; }

    public string DisplayName { get; set; }

    public string MailAddress { get; set; }

    public string AccountName { get; set; }

    public string FormattedAccountName { get; set; }

    public string Domain { get; set; }

    public string IdentityImage { get; set; }
  }
}
