// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Common.ConstraintRegexConstants
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Common
{
  public class ConstraintRegexConstants
  {
    public const string NaturalNumberRegex = "\\0*[1-9][0-9]*";
    public const string OptionalNaturalNumberRegex = "(\\0*[1-9][0-9]*)*";
  }
}
