// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ContributionIdentifier
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4FCC2C3-B106-43A6-A409-E4BF8CFC545C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.WebApi
{
  public class ContributionIdentifier
  {
    private string m_contributionId;
    private string m_publisherName;
    private string m_extensionName;
    private string m_contributionName;

    public ContributionIdentifier(string contributionId)
    {
      this.m_contributionId = contributionId;
      int length = this.m_contributionId.IndexOf('.');
      int num = this.m_contributionId.IndexOf('.', length + 1);
      this.m_publisherName = this.m_contributionId.Substring(0, length);
      this.m_extensionName = this.m_contributionId.Substring(length + 1, num - length - 1);
      this.m_contributionName = this.m_contributionId.Substring(num + 1);
    }

    public string PublisherName => this.m_publisherName;

    public string ExtensionName => this.m_extensionName;

    public string RelativeId => this.m_contributionName;

    public override string ToString() => this.m_contributionId;

    public static bool TryParse(
      string identifier,
      out ContributionIdentifier contributionIdentifier)
    {
      contributionIdentifier = (ContributionIdentifier) null;
      if (string.IsNullOrEmpty(identifier))
        return false;
      try
      {
        contributionIdentifier = new ContributionIdentifier(identifier);
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }
  }
}
