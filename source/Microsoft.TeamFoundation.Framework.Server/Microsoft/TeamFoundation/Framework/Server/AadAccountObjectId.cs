// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AadAccountObjectId
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Text.RegularExpressions;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class AadAccountObjectId
  {
    private static readonly Regex s_annotatedObjectIdRegEx = new Regex("^(.*)\\(([0-9a-f-]{36})\\)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    public string AccountName { get; set; }

    public string ObjectId { get; set; }

    public static AadAccountObjectId FromAnnotatedObjectIdString(string input)
    {
      Match match = AadAccountObjectId.s_annotatedObjectIdRegEx.Match(input);
      if (!match.Success)
        throw new ArgumentException("Aad User ObjectId '" + input + "' is not formatted correctly. Supported formats are annotated ObjectId (\"billg@microsoft.com(18abc4b6-fa6b-471e-90e9-b0dd6b9269f7)\").");
      return new AadAccountObjectId()
      {
        AccountName = match.Groups[1].Value,
        ObjectId = match.Groups[2].Value
      };
    }

    public override string ToString() => this.AccountName + "(" + this.ObjectId + ")";
  }
}
