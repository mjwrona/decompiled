// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CloneOptions
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClassVisibility(ClientVisibility.Public)]
  public class CloneOptions
  {
    private List<NameValuePair> m_overrideFieldDetails;
    private Dictionary<string, string> m_resolvedFieldDetails;

    public string RelatedLinkComment { get; set; }

    public string OverrideFieldName { get; set; }

    public string OverrideFieldValue { get; set; }

    public bool CopyAllSuites { get; set; }

    public bool CopyAncestorHierarchy { get; set; }

    public string DestinationWorkItemType { get; set; }

    [XmlIgnore]
    internal int ResolvedFieldId { get; set; }

    [XmlIgnore]
    internal string ResolvedFieldValue { get; set; }

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

    public bool CloneRequirements { get; set; }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (this.OverrideFieldDetails != null)
      {
        foreach (NameValuePair overrideFieldDetail in this.OverrideFieldDetails)
        {
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "OverrideFieldName={0},", (object) (overrideFieldDetail.Name ?? "null"));
          stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "OverrideFieldValue={0},", (object) (overrideFieldDetail.Value ?? "null"));
        }
      }
      else
      {
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "OverrideFieldName={0},", (object) (this.OverrideFieldName ?? "null"));
        stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "OverrideFieldValue={0},", (object) (this.OverrideFieldValue ?? "null"));
      }
      stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, "RelatedLinkComment={0},", (object) (this.RelatedLinkComment ?? "null"));
      return stringBuilder.ToString();
    }

    public static CloneOptions ConvertToCloneOptions(Microsoft.TeamFoundation.TestManagement.WebApi.CloneOptions cloneOptions)
    {
      if (cloneOptions == null)
        return (CloneOptions) null;
      CloneOptions cloneOptions1 = new CloneOptions()
      {
        CloneRequirements = cloneOptions.CloneRequirements,
        CopyAllSuites = cloneOptions.CopyAllSuites,
        CopyAncestorHierarchy = cloneOptions.CopyAncestorHierarchy,
        DestinationWorkItemType = cloneOptions.DestinationWorkItemType,
        RelatedLinkComment = cloneOptions.RelatedLinkComment,
        OverrideFieldDetails = new List<NameValuePair>()
      };
      if (cloneOptions.OverrideParameters != null)
      {
        foreach (KeyValuePair<string, string> overrideParameter in cloneOptions.OverrideParameters)
          cloneOptions1.OverrideFieldDetails.Add(new NameValuePair(overrideParameter.Key, overrideParameter.Value));
      }
      return cloneOptions1;
    }
  }
}
