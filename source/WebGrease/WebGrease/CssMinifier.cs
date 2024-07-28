// Decompiled with JetBrains decompiler
// Type: WebGrease.CssMinifier
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Linq;
using WebGrease.Activities;
using WebGrease.Css;

namespace WebGrease
{
  public class CssMinifier
  {
    public CssMinifier(IWebGreaseContext context)
    {
      this.CssActivity = new MinifyCssActivity(context)
      {
        ShouldMinify = true,
        ShouldOptimize = true,
        ShouldValidateForLowerCase = false,
        ShouldExcludeProperties = false,
        ShouldAssembleBackgroundImages = false
      };
      this.ShouldMinify = true;
      this.Errors = new List<string>();
    }

    public List<string> Errors { get; private set; }

    public bool ShouldMinify { get; set; }

    private MinifyCssActivity CssActivity { get; set; }

    public string Minify(string cssContent)
    {
      this.CssActivity.ShouldMinify = this.ShouldMinify;
      MinifyCssResult minifyCssResult = (MinifyCssResult) null;
      Exception exception = (Exception) null;
      try
      {
        minifyCssResult = this.CssActivity.Process(ContentItem.FromContent(cssContent));
      }
      catch (Exception ex)
      {
        exception = ex;
      }
      if (exception != null && exception is AggregateException aggEx)
        this.Errors.AddRange(aggEx.DedupeCSSErrors());
      return minifyCssResult == null || minifyCssResult.Css == null || !minifyCssResult.Css.Any<ContentItem>() ? (string) null : minifyCssResult.Css.FirstOrDefault<ContentItem>().Content;
    }
  }
}
