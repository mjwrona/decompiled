// Decompiled with JetBrains decompiler
// Type: WebGrease.Activities.MinifyJSActivity
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WebGrease.Configuration;
using WebGrease.Extensions;

namespace WebGrease.Activities
{
  internal sealed class MinifyJSActivity
  {
    private readonly IWebGreaseContext context;

    public MinifyJSActivity(IWebGreaseContext context) => this.context = context;

    internal string SourceFile { private get; set; }

    internal string DestinationFile { private get; set; }

    internal string MinifyArgs { private get; set; }

    internal string AnalyzeArgs { private get; set; }

    internal bool ShouldAnalyze { private get; set; }

    internal bool ShouldMinify { private get; set; }

    internal void Execute(ContentItem contentItem = null)
    {
      string destinationDirectory = this.context.Configuration.DestinationDirectory;
      if (contentItem == null && string.IsNullOrWhiteSpace(this.SourceFile))
        throw new ArgumentException("MinifyJSActivity - The source file cannot be null or whitespace.");
      if (string.IsNullOrWhiteSpace(this.DestinationFile))
        throw new ArgumentException("MinifyJSActivity - The destination file cannot be null or whitespace.");
      if (contentItem == null)
        contentItem = ContentItem.FromFile(this.SourceFile, Path.IsPathRooted(this.SourceFile) ? this.SourceFile.MakeRelativeToDirectory(destinationDirectory) : this.SourceFile, (string) null);
      this.Minify(contentItem)?.WriteTo(this.DestinationFile);
    }

    internal ContentItem Minify(ContentItem sourceContentItem)
    {
      ContentItem minifiedJsContentItem = (ContentItem) null;
      this.context.SectionedAction("MinifyJsActivity").MakeCachable(sourceContentItem, (object) new
      {
        ShouldAnalyze = this.ShouldAnalyze,
        ShouldMinify = this.ShouldMinify,
        AnalyzeArgs = this.AnalyzeArgs,
        TreatWarningsAsErrors = this.context.Configuration.Global.TreatWarningsAsErrors
      }).RestoreFromCacheAction((Func<ICacheSection, bool>) (cacheSection =>
      {
        minifiedJsContentItem = cacheSection.GetCachedContentItem("MinifiedJsResult", sourceContentItem.RelativeContentPath, contentPivots: sourceContentItem.ResourcePivotKeys);
        return minifiedJsContentItem != null;
      })).Execute((Func<ICacheSection, bool>) (cacheSection =>
      {
        Minifier minifier = new Minifier()
        {
          FileName = this.SourceFile
        };
        SwitchParser minifierSettings = this.GetMinifierSettings(minifier);
        string content = minifier.MinifyJavaScript(sourceContentItem.Content, minifierSettings.JSSettings);
        this.HandleMinifierErrors(sourceContentItem, minifier.ErrorList);
        if (content != null)
        {
          minifiedJsContentItem = ContentItem.FromContent(content, sourceContentItem.RelativeContentPath, (string) null, sourceContentItem.ResourcePivotKeys == null ? (ResourcePivotKey[]) null : sourceContentItem.ResourcePivotKeys.ToArray<ResourcePivotKey>());
          cacheSection.AddResult(minifiedJsContentItem, "MinifiedJsResult");
        }
        return minifiedJsContentItem != null && !minifier.ErrorList.Any<ContextError>();
      }));
      return minifiedJsContentItem;
    }

    private void HandleMinifierErrors(
      ContentItem contentItem,
      ICollection<ContextError> errorsAndWarnings)
    {
      if (errorsAndWarnings == null || errorsAndWarnings.Count <= 0)
        return;
      bool flag = false;
      string message;
      if (this.context.Log.HasExtendedErrorHandler)
      {
        foreach (ContextError errorsAndWarning in (IEnumerable<ContextError>) errorsAndWarnings)
        {
          string file = this.context.EnsureErrorFileOnDisk(errorsAndWarning.File, contentItem);
          flag = ((flag ? 1 : 0) | (this.context.Log.TreatWarningsAsErrors ? 1 : (errorsAndWarning.IsError ? 1 : 0))) != 0;
          (errorsAndWarning.IsError ? new LogExtendedError(this.context.Log.Error) : new LogExtendedError(this.context.Log.Warning))(errorsAndWarning.Subcategory, errorsAndWarning.ErrorCode, errorsAndWarning.HelpKeyword, file, new int?(errorsAndWarning.StartLine), new int?(errorsAndWarning.StartColumn), new int?(errorsAndWarning.EndLine), new int?(errorsAndWarning.EndColumn), errorsAndWarning.Message);
        }
        message = "Error minifying the JS";
      }
      else
      {
        flag = true;
        StringBuilder stringBuilder = new StringBuilder();
        foreach (ContextError errorsAndWarning in (IEnumerable<ContextError>) errorsAndWarnings)
          stringBuilder.AppendLine(errorsAndWarning.ToString());
        message = stringBuilder.ToString();
      }
      if (flag)
      {
        string file = this.context.EnsureErrorFileOnDisk(this.SourceFile ?? contentItem.RelativeContentPath, contentItem);
        throw new BuildWorkflowException(message, nameof (MinifyJSActivity), "WF000", (string) null, file, 0, 0, 0, 0, (Exception) null);
      }
    }

    private SwitchParser GetMinifierSettings(Minifier minifier)
    {
      CodeSettings scriptSettings;
      if (this.ShouldMinify)
      {
        CodeSettings codeSettings = new CodeSettings();
        codeSettings.TermSemicolons = true;
        scriptSettings = codeSettings;
      }
      else
      {
        CodeSettings codeSettings = new CodeSettings();
        codeSettings.OutputMode = OutputMode.MultipleLines;
        codeSettings.PreserveFunctionNames = true;
        codeSettings.CollapseToLiteral = false;
        codeSettings.LocalRenaming = LocalRenaming.KeepAll;
        codeSettings.ReorderScopeDeclarations = false;
        codeSettings.RemoveFunctionExpressionNames = false;
        codeSettings.RemoveUnneededCode = false;
        codeSettings.StripDebugStatements = false;
        codeSettings.EvalLiteralExpressions = false;
        codeSettings.TermSemicolons = true;
        codeSettings.KillSwitch = -1L;
        scriptSettings = codeSettings;
      }
      string commandLine = this.ShouldAnalyze ? this.AnalyzeArgs + (object) ' ' + this.MinifyArgs : this.MinifyArgs;
      SwitchParser minifierSettings = new SwitchParser(scriptSettings, (CssSettings) null);
      minifierSettings.Parse(commandLine);
      minifier.WarningLevel = minifierSettings.WarningLevel;
      return minifierSettings;
    }
  }
}
