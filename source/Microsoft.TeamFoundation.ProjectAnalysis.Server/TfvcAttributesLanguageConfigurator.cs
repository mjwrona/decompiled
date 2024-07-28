// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ProjectAnalysis.Server.TfvcAttributesLanguageConfigurator
// Assembly: Microsoft.TeamFoundation.ProjectAnalysis.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 076482BC-74A4-4A35-9427-1E61C33D1FA6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ProjectAnalysis.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.IO;

namespace Microsoft.TeamFoundation.ProjectAnalysis.Server
{
  internal class TfvcAttributesLanguageConfigurator : TfsLanguageConfiguratorBase
  {
    public const string c_attributeSeparator = ":";
    private readonly string[] m_supportedLangaugeAttributes = new string[1]
    {
      "project-discovery-language="
    };
    private readonly string[] m_supportedPathExclusionAttributes = new string[1]
    {
      "project-discovery-exclude-path"
    };

    public TfvcAttributesLanguageConfigurator(
      IVssRequestContext requestContext,
      Stream content,
      bool caseSensitivity = false)
      : base(requestContext, content, caseSensitivity)
    {
    }

    protected override bool TryParseLine(string line, string[] attributes, out string[] config)
    {
      ArgumentUtility.CheckForNull<string>(line, nameof (line));
      ArgumentUtility.CheckForNull<string[]>(attributes, nameof (attributes));
      config = (string[]) null;
      line = line.Trim();
      if (attributes.Length != 1)
        return false;
      string attribute = attributes[0];
      if (line.IndexOf(attribute) > 0)
      {
        int length = line.IndexOf(":");
        if (length > 0)
        {
          string str1 = line.Substring(0, length).Trim();
          string str2 = line.Substring(length + 1).Trim();
          char[] chArray = new char[1]{ '|' };
          foreach (string str3 in str2.Split(chArray))
          {
            if (str3.StartsWith(attribute))
            {
              string str4 = str3.Substring(attribute.Length);
              config = new string[2]{ str1, str4 };
            }
          }
        }
      }
      return config != null;
    }

    protected override string[] LanguageNameAttributes => this.m_supportedLangaugeAttributes;

    protected override string[] PathExclusionAttributes => this.m_supportedPathExclusionAttributes;
  }
}
