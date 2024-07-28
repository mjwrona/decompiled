// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ReparentingStatusModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class ReparentingStatusModel
  {
    public string Title { get; private set; }

    public string CollectionName { get; private set; }

    public string OrganizationName { get; private set; }

    public bool IsJoiningOrganization { get; private set; }

    public ReparentingStatusModel(
      string title,
      string collectionName,
      string organizationName,
      bool isJoiningOrganization)
    {
      this.Title = title;
      this.CollectionName = collectionName;
      this.OrganizationName = organizationName;
      this.IsJoiningOrganization = isJoiningOrganization;
    }
  }
}
