// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Common.WorkItemTypeMetadata
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Common
{
  internal static class WorkItemTypeMetadata
  {
    public static readonly char[] IllegalNameChars = new char[28]
    {
      '\'',
      '.',
      ',',
      ';',
      '~',
      ':',
      '/',
      '\\',
      '*',
      '|',
      '?',
      '"',
      '&',
      '%',
      '$',
      '!',
      '+',
      '=',
      '(',
      ')',
      '[',
      ']',
      '{',
      '}',
      '<',
      '>',
      '-',
      '์'
    };
    public static readonly char[] IllegalStateNameChars = new char[23]
    {
      '\'',
      ',',
      ';',
      '~',
      ':',
      '\\',
      '*',
      '|',
      '?',
      '"',
      '&',
      '%',
      '$',
      '!',
      '+',
      '=',
      '[',
      ']',
      '{',
      '}',
      '<',
      '>',
      '์'
    };
    public const int NameLength = 128;
    public const int ReferenceNameLength = 260;
    public const int DescriptionLength = 256;
    public const int WorkItemRuleDefaultStringValueLength = 255;
  }
}
