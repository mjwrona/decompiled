// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Templates.Resources.GitTemplateResources
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Git.Server.Templates.Resources
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class GitTemplateResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal GitTemplateResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (GitTemplateResources.resourceMan == null)
          GitTemplateResources.resourceMan = new ResourceManager("Microsoft.TeamFoundation.Git.Server.Templates.Resources.GitTemplateResources", typeof (GitTemplateResources).Assembly);
        return GitTemplateResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => GitTemplateResources.resourceCulture;
      set => GitTemplateResources.resourceCulture = value;
    }

    internal static string readme => GitTemplateResources.ResourceManager.GetString(nameof (readme), GitTemplateResources.resourceCulture);
  }
}
