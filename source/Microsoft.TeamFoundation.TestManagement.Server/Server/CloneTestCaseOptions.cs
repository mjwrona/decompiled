// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CloneTestCaseOptions
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Public)]
  public class CloneTestCaseOptions
  {
    private List<NameValuePair> m_overrideFieldDetails;
    private Dictionary<string, string> m_resolvedFieldDetails;

    public string RelatedLinkComment { get; set; }

    public bool IncludeLinks { get; set; }

    public bool IncludeAttachments { get; set; }

    public List<NameValuePair> OverrideFieldDetails
    {
      get
      {
        if (this.m_overrideFieldDetails == null)
          this.m_overrideFieldDetails = new List<NameValuePair>();
        return this.m_overrideFieldDetails;
      }
      set => this.m_overrideFieldDetails = value;
    }

    [XmlIgnore]
    internal Dictionary<string, string> ResolvedFieldDetails
    {
      get
      {
        if (this.m_resolvedFieldDetails == null)
          this.m_resolvedFieldDetails = new Dictionary<string, string>();
        return this.m_resolvedFieldDetails;
      }
      set => this.m_resolvedFieldDetails = value;
    }

    public static CloneTestCaseOptions ConvertToCloneTestCaseOptions(
      Microsoft.TeamFoundation.TestManagement.WebApi.CloneTestCaseOptions cloneOptions,
      Dictionary<string, string> overrideFieldDetails)
    {
      if (cloneOptions == null)
        return (CloneTestCaseOptions) null;
      CloneTestCaseOptions cloneTestCaseOptions = new CloneTestCaseOptions()
      {
        RelatedLinkComment = cloneOptions.RelatedLinkComment,
        IncludeLinks = cloneOptions.IncludeLinks,
        IncludeAttachments = cloneOptions.IncludeAttachments,
        OverrideFieldDetails = new List<NameValuePair>()
      };
      if (overrideFieldDetails != null)
      {
        foreach (KeyValuePair<string, string> overrideFieldDetail in overrideFieldDetails)
          cloneTestCaseOptions.OverrideFieldDetails.Add(new NameValuePair(overrideFieldDetail.Key, overrideFieldDetail.Value));
      }
      return cloneTestCaseOptions;
    }
  }
}
