// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.Telemetry.DirectoryEntityInvitationMethod
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.VisualStudio.Services.Directories.Telemetry
{
  public static class DirectoryEntityInvitationMethod
  {
    public const string IdentityHelper = "IdentityHelper";
    public const string IdentityImporter = "IdentityImporter";
    public const string WorkItemAadIdentityCreator = "WorkItemAadIdentityCreator";
    public const string MailIdentityHelper = "MailIdentityHelper";
    public const string MaterializeMentionedUser = "MaterializeMentionedUser";
    public const string GetOrCreateBindPendingIdentity = "GetOrCreateBindPendingIdentity";
    public const string CollectionReparenting = "CollectionReparenting";
    public const string MaterializeMemberCmdlet = "MaterializeMemberCmdlet";
    public const string MaterializeBindPendingUsersCmdlet = "MaterializeBindPendingUsersCmdlet";
    public const string GraphUpdate = "Graph.Update";
    public const string GraphCreate = "Graph.Create";
    public const string IdentityRightsController = "IdentityRightsController";
    public const string ProcessReviewersList = "ProcessReviewersList";

    public static string QualifyInvitationMethod(
      string invitationMethod,
      string invitationMethodCaller)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(invitationMethod, nameof (invitationMethod));
      return string.IsNullOrWhiteSpace(invitationMethodCaller) ? invitationMethod : invitationMethod + "@" + invitationMethodCaller;
    }
  }
}
