// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.Web.TcmResources
// Assembly: Microsoft.VisualStudio.Services.Tcm.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 78439952-D4CA-4096-B285-270E3917D35C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Web.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Tcm.Web
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  public class TcmResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal TcmResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static ResourceManager ResourceManager
    {
      get
      {
        if (TcmResources.resourceMan == null)
          TcmResources.resourceMan = new ResourceManager("Microsoft.VisualStudio.Services.Tcm.Web.TcmResources", typeof (TcmResources).Assembly);
        return TcmResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static CultureInfo Culture
    {
      get => TcmResources.resourceCulture;
      set => TcmResources.resourceCulture = value;
    }
  }
}
