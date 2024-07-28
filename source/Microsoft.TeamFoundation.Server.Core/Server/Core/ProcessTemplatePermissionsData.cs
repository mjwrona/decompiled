// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProcessTemplatePermissionsData
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Server.Core
{
  public abstract class ProcessTemplatePermissionsData
  {
    private static readonly char[] s_permissionSplitChars = new char[5]
    {
      ',',
      ' ',
      '\t',
      '\r',
      '\n'
    };

    [XmlAttribute("allow")]
    public string Allow { get; set; }

    [XmlAttribute("deny")]
    public string Deny { get; set; }

    [XmlAttribute("identity")]
    public string Identity { get; set; }

    public ProcessTemplatePermissionsData.ResolvedPermissionsData Resolve(
      IDictionary<string, string> tokens)
    {
      return new ProcessTemplatePermissionsData.ResolvedPermissionsData()
      {
        Identity = ProcessTemplatePermissionsData.ResolveIdentity(this.Identity, tokens),
        Allow = this.Allow != null ? this.Allow.Split(ProcessTemplatePermissionsData.s_permissionSplitChars, StringSplitOptions.RemoveEmptyEntries) : (string[]) null,
        Deny = this.Deny != null ? this.Deny.Split(ProcessTemplatePermissionsData.s_permissionSplitChars, StringSplitOptions.RemoveEmptyEntries) : (string[]) null
      };
    }

    internal static string ResolveIdentity(string identityName, IDictionary<string, string> tokens)
    {
      identityName = identityName.Replace("[$$PROJECTNAME$$]\\", "[$$PROJECTURI$$]\\");
      foreach (KeyValuePair<string, string> token in (IEnumerable<KeyValuePair<string, string>>) tokens)
        identityName = identityName.Replace(token.Key, token.Value);
      return identityName;
    }

    public class ResolvedPermissionsData
    {
      public string[] Allow { get; set; }

      public string[] Deny { get; set; }

      public string Identity { get; set; }
    }
  }
}
