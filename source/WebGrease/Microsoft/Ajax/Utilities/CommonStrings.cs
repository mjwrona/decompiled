// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.CommonStrings
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.Ajax.Utilities
{
  [DebuggerNonUserCode]
  [CompilerGenerated]
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  internal class CommonStrings
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal CommonStrings()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (object.ReferenceEquals((object) CommonStrings.resourceMan, (object) null))
          CommonStrings.resourceMan = new ResourceManager("WebGrease.Ajax.Utilities.CommonStrings", typeof (CommonStrings).Assembly);
        return CommonStrings.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => CommonStrings.resourceCulture;
      set => CommonStrings.resourceCulture = value;
    }

    internal static string ContextSeparator => CommonStrings.ResourceManager.GetString(nameof (ContextSeparator), CommonStrings.resourceCulture);

    internal static string FallbackEncodingFailed => CommonStrings.ResourceManager.GetString(nameof (FallbackEncodingFailed), CommonStrings.resourceCulture);

    internal static string InvalidJSONOutput => CommonStrings.ResourceManager.GetString(nameof (InvalidJSONOutput), CommonStrings.resourceCulture);

    internal static string Severity0 => CommonStrings.ResourceManager.GetString(nameof (Severity0), CommonStrings.resourceCulture);

    internal static string Severity1 => CommonStrings.ResourceManager.GetString(nameof (Severity1), CommonStrings.resourceCulture);

    internal static string Severity2 => CommonStrings.ResourceManager.GetString(nameof (Severity2), CommonStrings.resourceCulture);

    internal static string Severity3 => CommonStrings.ResourceManager.GetString(nameof (Severity3), CommonStrings.resourceCulture);

    internal static string Severity4 => CommonStrings.ResourceManager.GetString(nameof (Severity4), CommonStrings.resourceCulture);

    internal static string SeverityUnknown => CommonStrings.ResourceManager.GetString(nameof (SeverityUnknown), CommonStrings.resourceCulture);
  }
}
