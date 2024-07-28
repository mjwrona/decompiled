// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ConnectedService.Server.ConnectedServiceProviderResources
// Assembly: Microsoft.TeamFoundation.ConnectedService.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5BEB400C-7A81-4197-B897-D0116BC50257
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.ConnectedService.Server.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.ConnectedService.Server
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class ConnectedServiceProviderResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal ConnectedServiceProviderResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (ConnectedServiceProviderResources.resourceMan == null)
          ConnectedServiceProviderResources.resourceMan = new ResourceManager("Microsoft.TeamFoundation.ConnectedService.Server.ConnectedServiceProviderResources", typeof (ConnectedServiceProviderResources).Assembly);
        return ConnectedServiceProviderResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => ConnectedServiceProviderResources.resourceCulture;
      set => ConnectedServiceProviderResources.resourceCulture = value;
    }

    internal static string Error_DuplicateProviderIdentifierFormat => ConnectedServiceProviderResources.ResourceManager.GetString(nameof (Error_DuplicateProviderIdentifierFormat), ConnectedServiceProviderResources.resourceCulture);

    internal static string Error_FailedToStartProviderFormat => ConnectedServiceProviderResources.ResourceManager.GetString(nameof (Error_FailedToStartProviderFormat), ConnectedServiceProviderResources.resourceCulture);

    internal static string Error_GeneratingRedirectUrl => ConnectedServiceProviderResources.ResourceManager.GetString(nameof (Error_GeneratingRedirectUrl), ConnectedServiceProviderResources.resourceCulture);

    internal static string Error_ProviderNotFoundFormat => ConnectedServiceProviderResources.ResourceManager.GetString(nameof (Error_ProviderNotFoundFormat), ConnectedServiceProviderResources.resourceCulture);
  }
}
