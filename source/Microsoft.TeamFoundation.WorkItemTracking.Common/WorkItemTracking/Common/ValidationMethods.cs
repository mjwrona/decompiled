// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.ValidationMethods
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct ValidationMethods
  {
    public static bool IsValidLinkTypeName(string name) => ValidationMethods.IsValidFieldName(name);

    public static bool IsValidLinkTypeReferenceName(string name) => ValidationMethods.IsValidReferenceFieldName(name);

    public static bool IsValidLinkTypeReferenceNameForImport(string name) => ValidationMethods.IsValidReferenceFieldNameForImport(name);

    public static bool IsValidWorkItemTypeCategoryName(string name) => ValidationMethods.IsValidFieldName(name);

    public static bool IsValidWorkItemTypeCategoryReferenceName(string name) => ValidationMethods.IsValidReferenceFieldName(name);

    public static bool IsValidFieldName(string name) => Regex.IsMatch(name, "^[^\\.\\[\\]]+$") && !Regex.IsMatch(name, "^\\s") && !Regex.IsMatch(name, "\\s$") && !Regex.IsMatch(name, "\\s{2,}") && name.Length <= 128;

    public static bool IsValidReferenceFieldName(string name) => Regex.IsMatch(name, "^[a-zA-Z_][a-zA-Z0-9_]*(\\.[a-zA-Z0-9_]+)+$") && !Regex.IsMatch(name, "--") && name.Length <= 70;

    public static bool IsValidReferenceFieldNameForImport(string name) => ValidationMethods.IsValidReferenceFieldName(name) && !ValidationMethods.IsSystemReferenceName(name);

    public static bool IsSystemReferenceName(string name) => name.StartsWith("system.", StringComparison.OrdinalIgnoreCase);
  }
}
