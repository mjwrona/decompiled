// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ReleaseManagement2.Resources.Resource
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.ReleaseManagement2, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B374520F-138F-4DCB-BCF6-50FBC8C65346
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.ReleaseManagement2.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Server.WebAccess.ReleaseManagement2.Resources
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "15.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class Resource
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
    internal Resource()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (Resource.resourceMan == null)
          Resource.resourceMan = new ResourceManager("Microsoft.TeamFoundation.Server.WebAccess.ReleaseManagement2.Resources.Resource", typeof (Resource).Assembly);
        return Resource.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => Resource.resourceCulture;
      set => Resource.resourceCulture = value;
    }

    internal static string InvalidAuthenticationParameter => Resource.ResourceManager.GetString(nameof (InvalidAuthenticationParameter), Resource.resourceCulture);

    internal static string InvalidTenantId => Resource.ResourceManager.GetString(nameof (InvalidTenantId), Resource.resourceCulture);

    internal static string InvalidVstsAccessTokenKey => Resource.ResourceManager.GetString(nameof (InvalidVstsAccessTokenKey), Resource.resourceCulture);

    internal static string InvalidVstsProjectId => Resource.ResourceManager.GetString(nameof (InvalidVstsProjectId), Resource.resourceCulture);

    internal static string UnexpectedCompleteCallbackPayload => Resource.ResourceManager.GetString(nameof (UnexpectedCompleteCallbackPayload), Resource.resourceCulture);
  }
}
