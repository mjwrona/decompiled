// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Common.XamlNamespaceComparer
// Assembly: Microsoft.TeamFoundation.Build.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AD9C54FA-787C-49B8-AA73-C4A6EF8CE391
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build.Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.TeamFoundation.Build.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class XamlNamespaceComparer : IEqualityComparer<string>
  {
    public static bool Equals(string left, string right)
    {
      XamlNamespaceComparer.XamlNamespace xamlNamespace1 = XamlNamespaceComparer.XamlNamespace.TryParse(left);
      XamlNamespaceComparer.XamlNamespace xamlNamespace2 = XamlNamespaceComparer.XamlNamespace.TryParse(right);
      if (xamlNamespace1 == null || xamlNamespace2 == null)
        return string.Equals(left, right, StringComparison.OrdinalIgnoreCase);
      StringComparer ordinal = StringComparer.Ordinal;
      return ordinal.Equals(xamlNamespace1.AssemblyName, xamlNamespace2.AssemblyName) && ordinal.Equals(xamlNamespace1.ClrNamespace, xamlNamespace2.ClrNamespace);
    }

    bool IEqualityComparer<string>.Equals(string left, string right) => XamlNamespaceComparer.Equals(left, right);

    int IEqualityComparer<string>.GetHashCode(string obj)
    {
      XamlNamespaceComparer.XamlNamespace xamlNamespace = XamlNamespaceComparer.XamlNamespace.TryParse(obj);
      return xamlNamespace == null ? 0 : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}|{1}", (object) xamlNamespace.AssemblyName, (object) xamlNamespace.ClrNamespace).GetHashCode();
    }

    private sealed class XamlNamespace
    {
      private const string ClrNamespaceKey = "clr-namespace";
      private const string AssemblyNameKey = "assembly";

      private static bool TryParseNamespace(
        string ns,
        out string assemblyName,
        out string clrNamespace)
      {
        assemblyName = (string) null;
        clrNamespace = (string) null;
        if (ns != null)
        {
          string[] strArray1 = ns.Split(new char[1]{ ';' }, StringSplitOptions.RemoveEmptyEntries);
          StringComparer ordinalIgnoreCase = StringComparer.OrdinalIgnoreCase;
          foreach (string str in strArray1)
          {
            string[] strArray2 = str.Trim().Split(':');
            if (strArray2.Length == 2)
            {
              string x = strArray2[0].Trim();
              if (ordinalIgnoreCase.Equals(x, "clr-namespace"))
                clrNamespace = strArray2[1].Trim();
              else if (ordinalIgnoreCase.Equals(x, "assembly"))
                assemblyName = strArray2[1].Trim();
            }
          }
        }
        return clrNamespace != null;
      }

      public static XamlNamespaceComparer.XamlNamespace TryParse(string ns)
      {
        string assemblyName;
        string clrNamespace;
        if (!XamlNamespaceComparer.XamlNamespace.TryParseNamespace(ns, out assemblyName, out clrNamespace))
          return (XamlNamespaceComparer.XamlNamespace) null;
        return new XamlNamespaceComparer.XamlNamespace()
        {
          AssemblyName = assemblyName,
          ClrNamespace = clrNamespace
        };
      }

      public string ClrNamespace { get; private set; }

      public string AssemblyName { get; private set; }
    }
  }
}
