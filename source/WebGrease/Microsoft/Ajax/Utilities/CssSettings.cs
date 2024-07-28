// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.CssSettings
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.Collections.Generic;

namespace Microsoft.Ajax.Utilities
{
  public class CssSettings : CommonSettings
  {
    public CssSettings()
    {
      this.ColorNames = CssColor.Strict;
      this.CommentMode = CssComment.Important;
      this.MinifyExpressions = true;
      this.CssType = CssType.FullStyleSheet;
    }

    public CssSettings Clone()
    {
      CssSettings cssSettings1 = new CssSettings();
      cssSettings1.AllowEmbeddedAspNetBlocks = this.AllowEmbeddedAspNetBlocks;
      cssSettings1.ColorNames = this.ColorNames;
      cssSettings1.CommentMode = this.CommentMode;
      cssSettings1.IgnoreAllErrors = this.IgnoreAllErrors;
      cssSettings1.IgnoreErrorList = this.IgnoreErrorList;
      cssSettings1.IndentSize = this.IndentSize;
      cssSettings1.KillSwitch = this.KillSwitch;
      cssSettings1.LineBreakThreshold = this.LineBreakThreshold;
      cssSettings1.MinifyExpressions = this.MinifyExpressions;
      cssSettings1.OutputMode = this.OutputMode;
      cssSettings1.PreprocessorDefineList = this.PreprocessorDefineList;
      cssSettings1.TermSemicolons = this.TermSemicolons;
      cssSettings1.CssType = this.CssType;
      cssSettings1.BlocksStartOnSameLine = this.BlocksStartOnSameLine;
      CssSettings cssSettings2 = cssSettings1;
      cssSettings2.AddResourceStrings((IEnumerable<Microsoft.Ajax.Utilities.ResourceStrings>) this.ResourceStrings);
      foreach (KeyValuePair<string, string> replacementToken in (IEnumerable<KeyValuePair<string, string>>) this.ReplacementTokens)
        cssSettings2.ReplacementTokens.Add(replacementToken);
      foreach (KeyValuePair<string, string> replacementFallback in (IEnumerable<KeyValuePair<string, string>>) this.ReplacementFallbacks)
        cssSettings2.ReplacementTokens.Add(replacementFallback);
      return cssSettings2;
    }

    public CssColor ColorNames { get; set; }

    public CssComment CommentMode { get; set; }

    public bool MinifyExpressions { get; set; }

    public CssType CssType { get; set; }
  }
}
