// Decompiled with JetBrains decompiler
// Type: Validation.Strings
// Assembly: Validation, Version=2.2.0.0, Culture=neutral, PublicKeyToken=2fc06f0d701809a7
// MVID: B008DAAB-8462-4DA1-958C-4C90CA316797
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Validation.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Validation
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class Strings
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal Strings()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (Strings.resourceMan == null)
          Strings.resourceMan = new ResourceManager("Validation.Strings", typeof (Strings).GetTypeInfo().Assembly);
        return Strings.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => Strings.resourceCulture;
      set => Strings.resourceCulture = value;
    }

    internal static string Argument_EmptyArray => Strings.ResourceManager.GetString(nameof (Argument_EmptyArray), Strings.resourceCulture);

    internal static string Argument_EmptyString => Strings.ResourceManager.GetString(nameof (Argument_EmptyString), Strings.resourceCulture);

    internal static string Argument_NullElement => Strings.ResourceManager.GetString(nameof (Argument_NullElement), Strings.resourceCulture);

    internal static string Argument_Whitespace => Strings.ResourceManager.GetString(nameof (Argument_Whitespace), Strings.resourceCulture);

    internal static string InternalExceptionMessage => Strings.ResourceManager.GetString(nameof (InternalExceptionMessage), Strings.resourceCulture);

    internal static string ServiceMissing => Strings.ResourceManager.GetString(nameof (ServiceMissing), Strings.resourceCulture);
  }
}
