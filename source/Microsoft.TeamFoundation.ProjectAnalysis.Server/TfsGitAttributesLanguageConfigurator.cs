// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ProjectAnalysis.Server.TfsGitAttributesLanguageConfigurator
// Assembly: Microsoft.TeamFoundation.ProjectAnalysis.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 076482BC-74A4-4A35-9427-1E61C33D1FA6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ProjectAnalysis.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.IO;

namespace Microsoft.TeamFoundation.ProjectAnalysis.Server
{
  internal class TfsGitAttributesLanguageConfigurator : TfsLanguageConfiguratorBase
  {
    private readonly string[] m_supportedLangaugeAttributes = new string[2]
    {
      "project-discovery-language=",
      "linguist-language="
    };
    private readonly string[] m_supportedPathExclusionAttributes = new string[2]
    {
      "project-discovery-exclude-path",
      "linguist-vendored"
    };
    private const string c_layer = "TfsGitAttributesLanguageConfigurator";

    public TfsGitAttributesLanguageConfigurator(
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
      foreach (string attribute in attributes)
      {
        int num = line.IndexOf(attribute);
        if (num > 0)
        {
          if (config != null)
            return false;
          config = new string[2]
          {
            line.Substring(0, num - 1).Trim(),
            line.Substring(num + attribute.Length).Trim()
          };
        }
      }
      return config != null;
    }

    protected override string[] LanguageNameAttributes => this.m_supportedLangaugeAttributes;

    protected override string[] PathExclusionAttributes => this.m_supportedPathExclusionAttributes;
  }
}
