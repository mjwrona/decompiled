// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.Minifier
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Microsoft.Ajax.Utilities
{
  public class Minifier
  {
    private List<ContextError> m_errorList;

    public int WarningLevel { get; set; }

    public string FileName { get; set; }

    public ICollection<ContextError> ErrorList => (ICollection<ContextError>) this.m_errorList;

    public ICollection<string> Errors
    {
      get
      {
        List<string> errors = new List<string>(this.ErrorList.Count);
        foreach (ContextError error in (IEnumerable<ContextError>) this.ErrorList)
          errors.Add(error.ToString());
        return (ICollection<string>) errors;
      }
    }

    public string MinifyJavaScript(string source) => this.MinifyJavaScript(source, new CodeSettings());

    public string MinifyJavaScript(string source, CodeSettings codeSettings)
    {
      string empty = string.Empty;
      this.m_errorList = new List<ContextError>();
      JSParser jsParser = new JSParser();
      jsParser.CompilerError += new EventHandler<ContextErrorEventArgs>(this.OnJavaScriptError);
      try
      {
        bool flag = codeSettings != null && codeSettings.PreprocessOnly;
        StringBuilder sb = new StringBuilder();
        using (StringWriter writer = new StringWriter(sb, (IFormatProvider) CultureInfo.InvariantCulture))
        {
          if (flag)
            jsParser.EchoWriter = (TextWriter) writer;
          Block node = jsParser.Parse(new DocumentContext(source)
          {
            FileContext = this.FileName
          }, codeSettings);
          if (node != null)
          {
            if (!flag)
            {
              if (codeSettings != null && codeSettings.Format == JavaScriptFormat.JSON)
              {
                if (!JSONOutputVisitor.Apply((TextWriter) writer, (AstNode) node, codeSettings))
                  this.m_errorList.Add(new ContextError()
                  {
                    Severity = 0,
                    File = this.FileName,
                    Message = CommonStrings.InvalidJSONOutput
                  });
              }
              else
                OutputVisitor.Apply((TextWriter) writer, (AstNode) node, codeSettings);
            }
          }
        }
        return sb.ToString();
      }
      catch (Exception ex)
      {
        this.m_errorList.Add(new ContextError()
        {
          Severity = 0,
          File = this.FileName,
          Message = ex.Message
        });
        throw;
      }
    }

    public string MinifyStyleSheet(string source) => this.MinifyStyleSheet(source, new CssSettings(), new CodeSettings());

    public string MinifyStyleSheet(string source, CssSettings settings) => this.MinifyStyleSheet(source, settings, new CodeSettings());

    public string MinifyStyleSheet(
      string source,
      CssSettings settings,
      CodeSettings scriptSettings)
    {
      string empty = string.Empty;
      this.m_errorList = new List<ContextError>();
      CssParser cssParser = new CssParser();
      cssParser.FileContext = this.FileName;
      if (settings != null)
        cssParser.Settings = settings;
      if (scriptSettings != null)
        cssParser.JSSettings = scriptSettings;
      cssParser.CssError += new EventHandler<ContextErrorEventArgs>(this.OnCssError);
      try
      {
        return cssParser.Parse(source);
      }
      catch (Exception ex)
      {
        this.m_errorList.Add(new ContextError()
        {
          Severity = 0,
          File = this.FileName,
          Message = ex.Message
        });
        throw;
      }
    }

    private void OnCssError(object sender, ContextErrorEventArgs e)
    {
      ContextError error = e.Error;
      if (error.Severity > this.WarningLevel)
        return;
      this.m_errorList.Add(error);
    }

    private void OnJavaScriptError(object sender, ContextErrorEventArgs e)
    {
      ContextError error = e.Error;
      if (error.Severity > this.WarningLevel)
        return;
      this.m_errorList.Add(error);
    }
  }
}
