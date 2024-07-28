// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuth2.ScopeTemplateEntry
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Identity;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server.OAuth2
{
  public class ScopeTemplateEntry
  {
    public readonly string Identifier;
    public readonly ACETemplate[] Templates;

    internal ScopeTemplateEntry(string identifier, ACETemplate[] templates)
    {
      this.Identifier = identifier;
      this.Templates = templates;
    }

    public ScopeTemplateEntry(string identifier, string templates)
    {
      this.Identifier = identifier;
      this.Templates = JsonUtilities.Deserialize<ACETemplate[]>(templates);
    }

    public IReadOnlyList<IdentityDescriptor> GenerateScopeSubjects(string[] variables)
    {
      List<IdentityDescriptor> scopeSubjects = new List<IdentityDescriptor>();
      foreach (ACETemplate template in this.Templates)
      {
        string token = string.Format(template.Token, (object[]) variables);
        scopeSubjects.Add(SecurityServiceHelpers.CreateStatelessAceDescriptor(template.NamespaceId, token, template.Allow, template.Deny));
      }
      return (IReadOnlyList<IdentityDescriptor>) scopeSubjects;
    }
  }
}
