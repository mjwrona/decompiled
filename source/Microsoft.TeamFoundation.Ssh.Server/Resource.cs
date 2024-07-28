// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.Resource
// Assembly: Microsoft.TeamFoundation.Ssh.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9CD799E8-FCEB-494E-B317-B5A120D16BCC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Ssh.Server
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class Resource
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Resource()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (Resource.resourceMan == null)
          Resource.resourceMan = new ResourceManager("Microsoft.TeamFoundation.Ssh.Server.Resource", typeof (Resource).Assembly);
        return Resource.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => Resource.resourceCulture;
      set => Resource.resourceCulture = value;
    }

    internal static string SSHServerConfigDbSettingInvalid => Resource.ResourceManager.GetString(nameof (SSHServerConfigDbSettingInvalid), Resource.resourceCulture);
  }
}
