// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.JobData.CustomRepositoryIndexerJobData
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common.Utils;
using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.JobData
{
  public class CustomRepositoryIndexerJobData
  {
    public string ProjectName { get; set; }

    public string RepositoryName { get; set; }

    public string BranchName { get; set; }

    public CustomRepositoryIndexerJobData(
      string projectName,
      string repositoryName,
      string branchName)
    {
      this.ProjectName = projectName;
      this.RepositoryName = repositoryName;
      this.BranchName = branchName;
    }

    public CustomRepositoryIndexerJobData(XmlNode jobData)
    {
      this.ProjectName = jobData.GetAttributeValue(nameof (ProjectName));
      this.RepositoryName = jobData.GetAttributeValue(nameof (RepositoryName));
      this.BranchName = jobData.GetAttributeValue(nameof (BranchName));
    }

    [SuppressMessage("Microsoft.Security.Xml", "CA3053", Justification = "PR build is reporting this issue even though it is fixed")]
    public XmlNode ToXml()
    {
      XmlDocument xmlDoc = new XmlDocument();
      xmlDoc.XmlResolver = (XmlResolver) null;
      XmlNode element = (XmlNode) xmlDoc.CreateElement("IndexJob");
      xmlDoc.AddAttribute(element, "ProjectName", this.ProjectName);
      xmlDoc.AddAttribute(element, "RepositoryName", this.RepositoryName);
      xmlDoc.AddAttribute(element, "BranchName", this.BranchName);
      return element;
    }
  }
}
