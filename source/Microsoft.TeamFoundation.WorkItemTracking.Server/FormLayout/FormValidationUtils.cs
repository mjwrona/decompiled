// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.FormValidationUtils
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout
{
  public static class FormValidationUtils
  {
    private const int c_maxLabelLength = 128;
    private static Regex s_labelRegex = new Regex("[&]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static HashSet<string> s_validSections = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      string.Format("Section{0}", (object) 1),
      string.Format("Section{0}", (object) 2),
      string.Format("Section{0}", (object) 3)
    };

    public static void CheckLabel(string label)
    {
      FormValidationUtils.CheckLabelLength(label);
      FormValidationUtils.CheckLabelForInvalidCharacters(label);
    }

    public static string MakeLabelValid(string label)
    {
      if (label == null)
        return (string) null;
      label = FormValidationUtils.s_labelRegex.Replace(label, "");
      label = label.Trim();
      if (label.Length > 128)
        label = label.Substring(0, 128);
      return label;
    }

    public static void CheckLabelLength(string label)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(label, nameof (label));
      if (label.Length > 128)
        throw new FormLayoutLabelExceededLimitException(label, 128);
    }

    public static void CheckLabelForInvalidCharacters(string label)
    {
      if (FormValidationUtils.s_labelRegex.IsMatch(label))
        throw new FormLayoutLabelInvalidException(label);
    }

    public static void CheckRestrictedSections(string sectionId)
    {
      if (!FormValidationUtils.s_validSections.Contains(sectionId))
        throw new FormLayoutSectionInvalidException(sectionId);
    }
  }
}
