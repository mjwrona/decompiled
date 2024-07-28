// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestManagementErrorModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public class TestManagementErrorModel
  {
    public string ErrorText { get; set; }

    public string LinkTarget { get; private set; }

    public string LinkText { get; private set; }

    internal void SetLink(string linkTarget, string linkText)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(linkTarget, "link");
      ArgumentUtility.CheckStringForNullOrEmpty(linkText, nameof (linkText));
      this.LinkTarget = linkTarget;
      this.LinkText = linkText;
    }
  }
}
