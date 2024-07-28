// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Common.CommonWebApiResources
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;

namespace Microsoft.AspNet.OData.Common
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class CommonWebApiResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal CommonWebApiResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (CommonWebApiResources.resourceMan == null)
        {
          Assembly assembly = TypeHelper.GetAssembly(typeof (CommonWebApiResources));
          string str = ((IEnumerable<string>) assembly.GetManifestResourceNames()).Where<string>((Func<string, bool>) (s => s.EndsWith("CommonWebApiResources.resources", StringComparison.OrdinalIgnoreCase))).Single<string>();
          CommonWebApiResources.resourceMan = new ResourceManager(str.Substring(0, str.Length - 10), assembly);
        }
        return CommonWebApiResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => CommonWebApiResources.resourceCulture;
      set => CommonWebApiResources.resourceCulture = value;
    }

    internal static string ArgumentInvalidAbsoluteUri => CommonWebApiResources.ResourceManager.GetString(nameof (ArgumentInvalidAbsoluteUri), CommonWebApiResources.resourceCulture);

    internal static string ArgumentInvalidHttpUriScheme => CommonWebApiResources.ResourceManager.GetString(nameof (ArgumentInvalidHttpUriScheme), CommonWebApiResources.resourceCulture);

    internal static string ArgumentMustBeGreaterThanOrEqualTo => CommonWebApiResources.ResourceManager.GetString(nameof (ArgumentMustBeGreaterThanOrEqualTo), CommonWebApiResources.resourceCulture);

    internal static string ArgumentMustBeLessThanOrEqualTo => CommonWebApiResources.ResourceManager.GetString(nameof (ArgumentMustBeLessThanOrEqualTo), CommonWebApiResources.resourceCulture);

    internal static string ArgumentNullOrEmpty => CommonWebApiResources.ResourceManager.GetString(nameof (ArgumentNullOrEmpty), CommonWebApiResources.resourceCulture);

    internal static string ArgumentUriHasQueryOrFragment => CommonWebApiResources.ResourceManager.GetString(nameof (ArgumentUriHasQueryOrFragment), CommonWebApiResources.resourceCulture);

    internal static string InvalidEnumArgument => CommonWebApiResources.ResourceManager.GetString(nameof (InvalidEnumArgument), CommonWebApiResources.resourceCulture);
  }
}
