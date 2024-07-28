// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.CommonSettings
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Ajax.Utilities
{
  public class CommonSettings
  {
    private int m_indentLevel;

    protected CommonSettings()
    {
      this.IndentSize = 4;
      this.OutputMode = OutputMode.SingleLine;
      this.TermSemicolons = false;
      this.KillSwitch = 0L;
      this.LineBreakThreshold = 2147482647;
      this.AllowEmbeddedAspNetBlocks = false;
      this.IgnoreErrorCollection = (ICollection<string>) new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.PreprocessorValues = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.ResourceStrings = (IList<Microsoft.Ajax.Utilities.ResourceStrings>) new List<Microsoft.Ajax.Utilities.ResourceStrings>();
      this.ReplacementTokens = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.ReplacementFallbacks = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public bool AllowEmbeddedAspNetBlocks { get; set; }

    public BlockStart BlocksStartOnSameLine { get; set; }

    public bool IgnoreAllErrors { get; set; }

    public int IndentSize { get; set; }

    public int LineBreakThreshold { get; set; }

    public OutputMode OutputMode { get; set; }

    public bool TermSemicolons { get; set; }

    public long KillSwitch { get; set; }

    public string LineTerminator => this.OutputMode != OutputMode.MultipleLines ? "\n" : "\r\n";

    internal void Indent() => ++this.m_indentLevel;

    internal void Unindent()
    {
      if (this.m_indentLevel <= 0)
        return;
      --this.m_indentLevel;
    }

    internal string TabSpaces => new string(' ', this.m_indentLevel * this.IndentSize);

    public ICollection<string> IgnoreErrorCollection { get; private set; }

    public int SetIgnoreErrors(IEnumerable<string> ignoreErrors)
    {
      this.IgnoreErrorCollection.Clear();
      if (ignoreErrors != null)
      {
        foreach (string ignoreError in ignoreErrors)
          this.IgnoreErrorCollection.Add(ignoreError.Trim());
      }
      return this.IgnoreErrorCollection.Count;
    }

    public string IgnoreErrorList
    {
      get
      {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (string ignoreError in (IEnumerable<string>) this.IgnoreErrorCollection)
        {
          if (stringBuilder.Length > 0)
            stringBuilder.Append(',');
          stringBuilder.Append(ignoreError);
        }
        return stringBuilder.ToString();
      }
      set
      {
        if (!string.IsNullOrEmpty(value))
        {
          string str1 = value;
          char[] chArray = new char[1]{ ',' };
          foreach (string str2 in str1.Split(chArray))
            this.IgnoreErrorCollection.Add(str2);
        }
        else
          this.IgnoreErrorCollection.Clear();
      }
    }

    public IDictionary<string, string> PreprocessorValues { get; private set; }

    public int SetPreprocessorDefines(params string[] definedNames)
    {
      this.PreprocessorValues.Clear();
      if (definedNames != null && definedNames.Length > 0)
      {
        foreach (string definedName in definedNames)
        {
          int length = definedName.IndexOf('=');
          string str = length >= 0 ? definedName.Substring(0, length).Trim() : definedName.Trim();
          if (JSScanner.IsValidIdentifier(str))
            this.PreprocessorValues.Add(str, length < 0 ? string.Empty : definedName.Substring(length + 1));
        }
      }
      return this.PreprocessorValues.Count;
    }

    public int SetPreprocessorValues(IDictionary<string, string> defines)
    {
      this.PreprocessorValues.Clear();
      if (defines != null && defines.Count > 0)
      {
        foreach (KeyValuePair<string, string> define in (IEnumerable<KeyValuePair<string, string>>) defines)
        {
          if (JSScanner.IsValidIdentifier(define.Key))
            this.PreprocessorValues.Add(define.Key, define.Value);
        }
      }
      return this.PreprocessorValues.Count;
    }

    public string PreprocessorDefineList
    {
      get
      {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (KeyValuePair<string, string> preprocessorValue in (IEnumerable<KeyValuePair<string, string>>) this.PreprocessorValues)
        {
          if (stringBuilder.Length > 0)
            stringBuilder.Append(',');
          stringBuilder.Append(preprocessorValue.Key);
          if (!string.IsNullOrEmpty(preprocessorValue.Value))
          {
            stringBuilder.Append('=');
            stringBuilder.Append(preprocessorValue.Value);
          }
        }
        return stringBuilder.ToString();
      }
      set
      {
        if (!string.IsNullOrEmpty(value))
          this.SetPreprocessorDefines(value.Split(','));
        else
          this.PreprocessorValues.Clear();
      }
    }

    public IList<Microsoft.Ajax.Utilities.ResourceStrings> ResourceStrings { get; private set; }

    public void AddResourceStrings(Microsoft.Ajax.Utilities.ResourceStrings resourceStrings) => this.ResourceStrings.Add(resourceStrings);

    public void AddResourceStrings(IEnumerable<Microsoft.Ajax.Utilities.ResourceStrings> collection)
    {
      if (collection == null)
        return;
      foreach (Microsoft.Ajax.Utilities.ResourceStrings resourceStrings in collection)
        this.ResourceStrings.Add(resourceStrings);
    }

    public void ClearResourceStrings() => this.ResourceStrings.Clear();

    public void RemoveResourceStrings(Microsoft.Ajax.Utilities.ResourceStrings resourceStrings) => this.ResourceStrings.Remove(resourceStrings);

    public IDictionary<string, string> ReplacementTokens { get; private set; }

    public IDictionary<string, string> ReplacementFallbacks { get; private set; }

    public void ReplacementTokensApplyDefaults(IDictionary<string, string> otherSet)
    {
      if (otherSet == null)
        return;
      foreach (KeyValuePair<string, string> other in (IEnumerable<KeyValuePair<string, string>>) otherSet)
      {
        if (!this.ReplacementTokens.ContainsKey(other.Key))
          this.ReplacementTokens.Add(other);
      }
    }

    public void ReplacementTokensApplyOverrides(IDictionary<string, string> otherSet)
    {
      if (otherSet == null)
        return;
      foreach (KeyValuePair<string, string> other in (IEnumerable<KeyValuePair<string, string>>) otherSet)
      {
        if (!this.ReplacementTokens.ContainsKey(other.Key))
          this.ReplacementTokens.Add(other);
        else
          this.ReplacementTokens[other.Key] = other.Value;
      }
    }
  }
}
