// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Models.SettingsErrorModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Models
{
  public class SettingsErrorModel
  {
    public SettingsErrorModel() => this.ExceptionMessages = (IEnumerable<string>) Array.Empty<string>();

    public string ErrorText { get; set; }

    public IEnumerable<string> ExceptionMessages { get; set; }

    public string LinkTarget { get; private set; }

    public string LinkText { get; private set; }

    public string SecondaryLinkTarget { get; private set; }

    public string SecondaryLinkText { get; private set; }

    internal void SetLink(string linkTarget, string linkText)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(linkTarget, "link");
      ArgumentUtility.CheckStringForNullOrEmpty(linkText, nameof (linkText));
      this.LinkTarget = linkTarget;
      this.LinkText = linkText;
    }

    internal void SetSecondaryLink(string linkTarget, string linkText)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(linkTarget, "link");
      ArgumentUtility.CheckStringForNullOrEmpty(linkText, nameof (linkText));
      this.SecondaryLinkTarget = linkTarget;
      this.SecondaryLinkText = linkText;
    }
  }
}
