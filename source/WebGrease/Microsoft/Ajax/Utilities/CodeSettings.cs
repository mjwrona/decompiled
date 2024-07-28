// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.CodeSettings
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.Collections.Generic;
using System.Text;

namespace Microsoft.Ajax.Utilities
{
  public class CodeSettings : CommonSettings
  {
    private bool m_minify;
    private Dictionary<string, string> m_identifierReplacementMap;
    private HashSet<string> m_noRenameSet;
    private HashSet<string> m_knownGlobals;
    private HashSet<string> m_debugLookups;

    public CodeSettings()
    {
      this.MinifyCode = true;
      this.EvalTreatment = EvalTreatment.Ignore;
      this.InlineSafeStrings = true;
      this.MacSafariQuirks = true;
      this.PreserveImportantComments = true;
      this.QuoteObjectLiteralProperties = false;
      this.StrictMode = false;
      this.StripDebugStatements = true;
      this.ManualRenamesProperties = true;
      this.OutputMode = OutputMode.SingleLine;
      this.m_knownGlobals = new HashSet<string>();
      this.m_debugLookups = new HashSet<string>();
      this.m_noRenameSet = new HashSet<string>((IEnumerable<string>) new string[1]
      {
        "$super"
      });
      this.m_identifierReplacementMap = new Dictionary<string, string>();
    }

    public CodeSettings Clone()
    {
      CodeSettings codeSettings1 = new CodeSettings();
      codeSettings1.m_minify = this.m_minify;
      codeSettings1.AllowEmbeddedAspNetBlocks = this.AllowEmbeddedAspNetBlocks;
      codeSettings1.AlwaysEscapeNonAscii = this.AlwaysEscapeNonAscii;
      codeSettings1.CollapseToLiteral = this.CollapseToLiteral;
      codeSettings1.ConstStatementsMozilla = this.ConstStatementsMozilla;
      codeSettings1.DebugLookupList = this.DebugLookupList;
      codeSettings1.EvalLiteralExpressions = this.EvalLiteralExpressions;
      codeSettings1.EvalTreatment = this.EvalTreatment;
      codeSettings1.Format = this.Format;
      codeSettings1.IgnoreConditionalCompilation = this.IgnoreConditionalCompilation;
      codeSettings1.IgnoreAllErrors = this.IgnoreAllErrors;
      codeSettings1.IgnoreErrorList = this.IgnoreErrorList;
      codeSettings1.IgnorePreprocessorDefines = this.IgnorePreprocessorDefines;
      codeSettings1.IndentSize = this.IndentSize;
      codeSettings1.InlineSafeStrings = this.InlineSafeStrings;
      codeSettings1.KillSwitch = this.KillSwitch;
      codeSettings1.KnownGlobalNamesList = this.KnownGlobalNamesList;
      codeSettings1.LineBreakThreshold = this.LineBreakThreshold;
      codeSettings1.LocalRenaming = this.LocalRenaming;
      codeSettings1.MacSafariQuirks = this.MacSafariQuirks;
      codeSettings1.ManualRenamesProperties = this.ManualRenamesProperties;
      codeSettings1.NoAutoRenameList = this.NoAutoRenameList;
      codeSettings1.OutputMode = this.OutputMode;
      codeSettings1.PreprocessOnly = this.PreprocessOnly;
      codeSettings1.PreprocessorDefineList = this.PreprocessorDefineList;
      codeSettings1.PreserveFunctionNames = this.PreserveFunctionNames;
      codeSettings1.PreserveImportantComments = this.PreserveImportantComments;
      codeSettings1.QuoteObjectLiteralProperties = this.QuoteObjectLiteralProperties;
      codeSettings1.RemoveFunctionExpressionNames = this.RemoveFunctionExpressionNames;
      codeSettings1.RemoveUnneededCode = this.RemoveUnneededCode;
      codeSettings1.RenamePairs = this.RenamePairs;
      codeSettings1.ReorderScopeDeclarations = this.ReorderScopeDeclarations;
      codeSettings1.SourceMode = this.SourceMode;
      codeSettings1.StrictMode = this.StrictMode;
      codeSettings1.StripDebugStatements = this.StripDebugStatements;
      codeSettings1.TermSemicolons = this.TermSemicolons;
      codeSettings1.BlocksStartOnSameLine = this.BlocksStartOnSameLine;
      codeSettings1.ErrorIfNotInlineSafe = this.ErrorIfNotInlineSafe;
      codeSettings1.SymbolsMap = this.SymbolsMap;
      CodeSettings codeSettings2 = codeSettings1;
      codeSettings2.AddResourceStrings((IEnumerable<Microsoft.Ajax.Utilities.ResourceStrings>) this.ResourceStrings);
      foreach (KeyValuePair<string, string> replacementToken in (IEnumerable<KeyValuePair<string, string>>) this.ReplacementTokens)
        codeSettings2.ReplacementTokens.Add(replacementToken);
      foreach (KeyValuePair<string, string> replacementFallback in (IEnumerable<KeyValuePair<string, string>>) this.ReplacementFallbacks)
        codeSettings2.ReplacementTokens.Add(replacementFallback);
      return codeSettings2;
    }

    public bool AddRenamePair(string sourceName, string newName)
    {
      bool flag = false;
      if (JSScanner.IsValidIdentifier(sourceName) && JSScanner.IsValidIdentifier(newName))
      {
        if (this.m_identifierReplacementMap.ContainsKey(sourceName))
          this.m_identifierReplacementMap[sourceName] = newName;
        else
          this.m_identifierReplacementMap.Add(sourceName, newName);
        flag = true;
      }
      return flag;
    }

    public void ClearRenamePairs() => this.m_identifierReplacementMap.Clear();

    public bool HasRenamePairs => this.m_identifierReplacementMap.Count > 0;

    public string GetNewName(string sourceName)
    {
      string newName;
      if (!this.m_identifierReplacementMap.TryGetValue(sourceName, out newName))
        newName = (string) null;
      return newName;
    }

    public string RenamePairs
    {
      get
      {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (KeyValuePair<string, string> identifierReplacement in this.m_identifierReplacementMap)
        {
          if (stringBuilder.Length > 0)
            stringBuilder.Append(',');
          stringBuilder.Append(identifierReplacement.Key);
          stringBuilder.Append('=');
          stringBuilder.Append(identifierReplacement.Value);
        }
        return stringBuilder.ToString();
      }
      set
      {
        if (!string.IsNullOrEmpty(value))
        {
          string str1 = value;
          char[] chArray1 = new char[2]{ ',', ';' };
          foreach (string str2 in str1.Split(chArray1))
          {
            char[] chArray2 = new char[1]{ '=' };
            string[] strArray = str2.Split(chArray2);
            if (strArray.Length == 2)
              this.AddRenamePair(strArray[0].Trim(), strArray[1].Trim());
          }
        }
        else
          this.m_identifierReplacementMap.Clear();
      }
    }

    public IEnumerable<string> NoAutoRenameCollection => (IEnumerable<string>) this.m_noRenameSet;

    public int SetNoAutoRenames(IEnumerable<string> noRenameNames)
    {
      this.m_noRenameSet.Clear();
      if (noRenameNames != null)
      {
        foreach (string noRenameName in noRenameNames)
          this.AddNoAutoRename(noRenameName);
      }
      return this.m_noRenameSet.Count;
    }

    public bool AddNoAutoRename(string noRename)
    {
      if (!JSScanner.IsValidIdentifier(noRename))
        return false;
      this.m_noRenameSet.Add(noRename);
      return true;
    }

    public string NoAutoRenameList
    {
      get
      {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (string noRename in this.m_noRenameSet)
        {
          if (stringBuilder.Length > 0)
            stringBuilder.Append(',');
          stringBuilder.Append(noRename);
        }
        return stringBuilder.ToString();
      }
      set
      {
        if (!string.IsNullOrEmpty(value))
        {
          string str = value;
          char[] chArray = new char[2]{ ',', ';' };
          foreach (string noRename in str.Split(chArray))
            this.AddNoAutoRename(noRename);
        }
        else
          this.m_noRenameSet.Clear();
      }
    }

    public IEnumerable<string> KnownGlobalCollection => (IEnumerable<string>) this.m_knownGlobals;

    public int SetKnownGlobalIdentifiers(IEnumerable<string> globalArray)
    {
      this.m_knownGlobals.Clear();
      if (globalArray != null)
      {
        foreach (string global in globalArray)
          this.AddKnownGlobal(global);
      }
      return this.m_knownGlobals.Count;
    }

    public bool AddKnownGlobal(string identifier)
    {
      if (!JSScanner.IsValidIdentifier(identifier))
        return false;
      this.m_knownGlobals.Add(identifier);
      return true;
    }

    public string KnownGlobalNamesList
    {
      get
      {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (string knownGlobal in this.m_knownGlobals)
        {
          if (stringBuilder.Length > 0)
            stringBuilder.Append(',');
          stringBuilder.Append(knownGlobal);
        }
        return stringBuilder.ToString();
      }
      set
      {
        if (!string.IsNullOrEmpty(value))
        {
          string str = value;
          char[] chArray = new char[2]{ ',', ';' };
          foreach (string identifier in str.Split(chArray))
            this.AddKnownGlobal(identifier);
        }
        else
          this.m_knownGlobals.Clear();
      }
    }

    public IEnumerable<string> DebugLookupCollection => (IEnumerable<string>) this.m_debugLookups;

    public int SetDebugNamespaces(IEnumerable<string> debugLookups)
    {
      this.m_debugLookups.Clear();
      if (debugLookups != null)
      {
        foreach (string debugLookup in debugLookups)
          this.AddDebugLookup(debugLookup);
      }
      return this.m_debugLookups.Count;
    }

    public bool AddDebugLookup(string debugNamespace)
    {
      if (string.IsNullOrEmpty(debugNamespace))
        return false;
      if (debugNamespace.IndexOf('.') > 0)
      {
        string str = debugNamespace;
        char[] chArray = new char[1]{ '.' };
        foreach (string name in str.Split(chArray))
        {
          if (!JSScanner.IsValidIdentifier(name))
            return false;
        }
      }
      else if (!JSScanner.IsValidIdentifier(debugNamespace))
        return false;
      this.m_debugLookups.Add(debugNamespace);
      return true;
    }

    public string DebugLookupList
    {
      get
      {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (string debugLookup in this.m_debugLookups)
        {
          if (stringBuilder.Length > 0)
            stringBuilder.Append(',');
          stringBuilder.Append(debugLookup);
        }
        return stringBuilder.ToString();
      }
      set
      {
        this.m_debugLookups.Clear();
        if (string.IsNullOrEmpty(value))
          return;
        string str = value;
        char[] chArray = new char[2]{ ',', ';' };
        foreach (string debugNamespace in str.Split(chArray))
          this.AddDebugLookup(debugNamespace);
      }
    }

    public bool AlwaysEscapeNonAscii { get; set; }

    public bool CollapseToLiteral { get; set; }

    public bool ConstStatementsMozilla { get; set; }

    public bool ErrorIfNotInlineSafe { get; set; }

    public bool EvalLiteralExpressions { get; set; }

    public EvalTreatment EvalTreatment { get; set; }

    public JavaScriptFormat Format { get; set; }

    public bool IgnoreConditionalCompilation { get; set; }

    public bool IgnorePreprocessorDefines { get; set; }

    public bool InlineSafeStrings { get; set; }

    public LocalRenaming LocalRenaming { get; set; }

    public bool MacSafariQuirks { get; set; }

    public bool MinifyCode
    {
      get => this.m_minify;
      set
      {
        this.m_minify = value;
        this.CollapseToLiteral = this.m_minify;
        this.EvalLiteralExpressions = this.m_minify;
        this.RemoveFunctionExpressionNames = this.m_minify;
        this.RemoveUnneededCode = this.m_minify;
        this.ReorderScopeDeclarations = this.m_minify;
        this.PreserveFunctionNames = !this.m_minify;
        this.PreserveImportantComments = !this.m_minify;
        this.LocalRenaming = this.m_minify ? LocalRenaming.CrunchAll : LocalRenaming.KeepAll;
        this.KillSwitch = this.m_minify ? 0L : -2L;
      }
    }

    public bool ManualRenamesProperties { get; set; }

    public bool PreprocessOnly { get; set; }

    public bool PreserveFunctionNames { get; set; }

    public bool PreserveImportantComments { get; set; }

    public bool QuoteObjectLiteralProperties { get; set; }

    public bool ReorderScopeDeclarations { get; set; }

    public bool RemoveFunctionExpressionNames { get; set; }

    public bool RemoveUnneededCode { get; set; }

    public ScriptVersion ScriptVersion { get; set; }

    public JavaScriptSourceMode SourceMode { get; set; }

    public bool StrictMode { get; set; }

    public bool StripDebugStatements { get; set; }

    public ISourceMap SymbolsMap { get; set; }

    public bool IsModificationAllowed(TreeModifications modification) => ((TreeModifications) this.KillSwitch & modification) == TreeModifications.None;
  }
}
