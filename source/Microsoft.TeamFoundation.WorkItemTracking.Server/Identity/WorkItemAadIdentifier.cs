// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Identity.WorkItemAadIdentifier
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Identity
{
  internal class WorkItemAadIdentifier
  {
    private const string c_aadIdStart = "<<";
    private const string c_aadIdEnd = ">>";
    private const string c_userPrefix = "user";
    private const string c_groupPrefix = "group";

    public string FriendlyDisplayName { get; set; }

    public string DisplayName { get; set; }

    public string UniqueName { get; set; }

    public Guid ObjectId { get; set; }

    public bool IsGroup { get; set; }

    public WorkItemAadIdentifier(
      string displayName,
      Guid objectId,
      string uniqueName,
      bool isGroup)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(displayName, nameof (displayName));
      this.DisplayName = displayName;
      this.ObjectId = objectId;
      this.UniqueName = uniqueName;
      this.IsGroup = isGroup;
      string[] strArray = displayName.Split('\\');
      if (strArray.Length == 2)
        this.FriendlyDisplayName = strArray[1];
      else
        this.FriendlyDisplayName = displayName;
    }

    public override string ToString() => string.Format("{0} {1}{2}{3}", (object) this.DisplayName, (object) "<<", (object) this.ObjectId, (object) ">>");

    public static WorkItemAadIdentifier Parse(string name)
    {
      if (string.IsNullOrWhiteSpace(name))
        return (WorkItemAadIdentifier) null;
      name = name.Trim();
      int num1 = name.IndexOf("<<");
      int num2 = name.IndexOf(">>");
      if (num1 == -1 || num2 == -1 || num2 + ">>".Length != name.Length)
        return (WorkItemAadIdentifier) null;
      string displayName = name.Substring(0, num1 - 1).Trim();
      string str1 = name.Substring(num1 + "<<".Length, num2 - num1 - "<<".Length);
      bool isGroup = false;
      string[] strArray1 = str1.Split(':');
      string str2;
      if (strArray1.Length <= 1)
      {
        str2 = str1;
      }
      else
      {
        str2 = strArray1[1];
        if (strArray1[0].Trim().Equals("group", StringComparison.OrdinalIgnoreCase))
          isGroup = true;
      }
      string[] strArray2 = str2.Split('\\');
      string input = strArray2[0];
      string uniqueName = (string) null;
      if (strArray2.Length > 1)
        uniqueName = strArray2[1];
      Guid objectId;
      ref Guid local = ref objectId;
      return Guid.TryParse(input, out local) ? new WorkItemAadIdentifier(displayName, objectId, uniqueName, isGroup) : (WorkItemAadIdentifier) null;
    }
  }
}
