// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Web.UserManagement.AADUserModel
// Assembly: Microsoft.VisualStudio.Services.Web.UserManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7BDE82F4-5081-4A92-A83F-EE78FF05B171
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Web.UserManagement.dll

using Microsoft.TeamFoundation.Server.WebAccess;
using Microsoft.VisualStudio.Services.Aad;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Web.UserManagement
{
  internal class AADUserModel
  {
    internal static JsObject ToJsonFromAADUser(
      AadUser aadUser,
      string userImage = null,
      bool hasAadThumbnailPhoto = false)
    {
      JsObject jsonFromAadUser = new JsObject();
      if (aadUser == null)
      {
        jsonFromAadUser["error"] = (object) "User not found";
        return jsonFromAadUser;
      }
      jsonFromAadUser["ObjectId"] = (object) (aadUser.ObjectId.ToString() ?? string.Empty);
      jsonFromAadUser["DisplayName"] = (object) (aadUser.DisplayName ?? string.Empty);
      jsonFromAadUser["AccountEnabled"] = (object) aadUser.AccountEnabled;
      jsonFromAadUser["Mail"] = (object) (aadUser.Mail ?? string.Empty);
      jsonFromAadUser["OtherMails"] = (object) aadUser.OtherMails.ToList<string>();
      jsonFromAadUser["MailNickname"] = (object) (aadUser.MailNickname ?? string.Empty);
      jsonFromAadUser["UserPrincipalName"] = (object) (aadUser.UserPrincipalName ?? string.Empty);
      jsonFromAadUser["SignInAddress"] = (object) (aadUser.SignInAddress ?? string.Empty);
      jsonFromAadUser["ThumbnailPhoto"] = (object) (userImage ?? string.Empty);
      jsonFromAadUser["HasAadThumbnailPhoto"] = (object) hasAadThumbnailPhoto;
      jsonFromAadUser["JobTitle"] = (object) (aadUser.JobTitle ?? string.Empty);
      jsonFromAadUser["Department"] = (object) (aadUser.Department ?? string.Empty);
      jsonFromAadUser["PhysicalDeliveryOfficeName"] = (object) (aadUser.PhysicalDeliveryOfficeName ?? string.Empty);
      if (aadUser.Manager != null)
        jsonFromAadUser["Manager"] = (object) AADUserModel.ToJsonFromAADUser(aadUser.Manager);
      else
        jsonFromAadUser["Manager"] = (object) string.Empty;
      jsonFromAadUser["UserType"] = (object) (aadUser.UserType ?? string.Empty);
      return jsonFromAadUser;
    }
  }
}
