// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.IRepoPermissionsManager
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

namespace Microsoft.TeamFoundation.Git.Server
{
  public interface IRepoPermissionsManager
  {
    void CheckWrite(bool considerAnyBranches);

    void CheckWrite(string refName = null);

    bool CanCreateBranch(string refName);

    bool CanCreateTag(string refName);

    void CheckEditPolicies(string refName = null);

    bool HasEditPolicies(string refName = null);

    void CheckPullRequestContribute();

    bool HasPullRequestContribute();

    bool HasRead();

    bool HasCreateBranch(string refName = null, bool considerAnyBranches = false);

    bool HasViewAdvSecAlert();

    bool HasDismissAdvSecAlert();

    bool HasManageAdvSec();

    bool HasViewAdvSecEnablement();
  }
}
