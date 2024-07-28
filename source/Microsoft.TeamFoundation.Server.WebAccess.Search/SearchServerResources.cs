// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Search.SearchServerResources
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Search, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CC5F1FD-9493-4B23-B40F-49E474A0E625
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Search.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Server.WebAccess.Search
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  public class SearchServerResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal SearchServerResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (SearchServerResources.resourceMan == null)
          SearchServerResources.resourceMan = new ResourceManager("Microsoft.TeamFoundation.Server.WebAccess.Search.SearchServerResources", typeof (SearchServerResources).Assembly);
        return SearchServerResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => SearchServerResources.resourceCulture;
      set => SearchServerResources.resourceCulture = value;
    }

    public static string AccountFaultInErrorMessage => SearchServerResources.ResourceManager.GetString(nameof (AccountFaultInErrorMessage), SearchServerResources.resourceCulture);

    public static string RegionNotSupportedMessage => SearchServerResources.ResourceManager.GetString(nameof (RegionNotSupportedMessage), SearchServerResources.resourceCulture);

    public static string SearchExplorerSplitterLabel => SearchServerResources.ResourceManager.GetString(nameof (SearchExplorerSplitterLabel), SearchServerResources.resourceCulture);
  }
}
