// Decompiled with JetBrains decompiler
// Type: WebGrease.Configuration.CssMinificationConfig
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Xml.Linq;
using WebGrease.Extensions;

namespace WebGrease.Configuration
{
  internal class CssMinificationConfig : INamedConfig
  {
    public CssMinificationConfig()
    {
      this.ShouldMinify = true;
      this.ForbiddenSelectors = (IEnumerable<string>) new string[0];
      this.RemoveSelectors = (IEnumerable<string>) new string[0];
      this.NonMergeSelectors = (IEnumerable<string>) new string[0];
    }

    public CssMinificationConfig(XElement element)
      : this()
    {
      this.Name = (string) element.Attribute((XName) "config") ?? string.Empty;
      foreach (XElement descendant in element.Descendants())
      {
        string str1 = descendant.Name.ToString();
        string str2 = descendant.Value;
        switch (str1)
        {
          case "MergeMediaQueries":
            this.ShouldMergeMediaQueries = str2.TryParseBool();
            continue;
          case "Optimize":
            this.ShouldOptimize = str2.TryParseBool();
            continue;
          case "Minify":
            this.ShouldMinify = str2.TryParseBool();
            continue;
          case "ValidateLowerCase":
          case "ValidateForLowerCase":
            this.ShouldValidateLowerCase = str2.TryParseBool();
            continue;
          case "ExcludeProperties":
            this.ShouldExcludeProperties = str2.TryParseBool();
            continue;
          case "ProhibitedSelectors":
            string[] strArray1;
            if (!str2.IsNullOrWhitespace())
              strArray1 = str2.Split(new char[1]{ ';' }, StringSplitOptions.RemoveEmptyEntries);
            else
              strArray1 = new string[0];
            this.ForbiddenSelectors = (IEnumerable<string>) strArray1;
            continue;
          case nameof (RemoveSelectors):
            string[] strArray2;
            if (!str2.IsNullOrWhitespace())
              strArray2 = str2.Split(new char[1]{ ';' }, StringSplitOptions.RemoveEmptyEntries);
            else
              strArray2 = new string[0];
            this.RemoveSelectors = (IEnumerable<string>) strArray2;
            continue;
          case nameof (NonMergeSelectors):
            string[] strArray3;
            if (!str2.IsNullOrWhitespace())
              strArray3 = str2.Split(new char[1]{ ';' }, StringSplitOptions.RemoveEmptyEntries);
            else
              strArray3 = new string[0];
            this.NonMergeSelectors = (IEnumerable<string>) strArray3;
            continue;
          case "PreventOrderBasedConflict":
            this.ShouldPreventOrderBasedConflict = str2.TryParseBool();
            continue;
          case "MergeBasedOnCommonDeclarations":
            this.ShouldMergeBasedOnCommonDeclarations = str2.TryParseBool();
            continue;
          default:
            continue;
        }
      }
    }

    public string Name { get; set; }

    internal bool ShouldMinify { get; set; }

    internal bool ShouldOptimize { get; set; }

    internal bool ShouldMergeMediaQueries { get; set; }

    internal bool ShouldValidateLowerCase { get; set; }

    internal bool ShouldExcludeProperties { get; set; }

    internal bool ShouldPreventOrderBasedConflict { get; set; }

    internal bool ShouldMergeBasedOnCommonDeclarations { get; set; }

    internal IEnumerable<string> ForbiddenSelectors { get; set; }

    internal IEnumerable<string> RemoveSelectors { get; set; }

    internal IEnumerable<string> NonMergeSelectors { get; set; }
  }
}
