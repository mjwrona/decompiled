// Decompiled with JetBrains decompiler
// Type: WebGrease.TemporaryOverrides
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using WebGrease.Configuration;
using WebGrease.Extensions;

namespace WebGrease
{
  public class TemporaryOverrides
  {
    private readonly IDictionary<string, List<string>> resourcePivots = (IDictionary<string, List<string>>) new Dictionary<string, List<string>>();
    private readonly List<string> outputs = new List<string>();
    private readonly List<string> outputExtensions = new List<string>();
    private string uniqueKey;

    public bool SkipAll { get; set; }

    public string UniqueKey => this.uniqueKey;

    public static TemporaryOverrides Load(string overrideFile)
    {
      TemporaryOverrides temporaryOverrides = new TemporaryOverrides();
      temporaryOverrides.LoadFromFile(overrideFile);
      temporaryOverrides.uniqueKey = temporaryOverrides.ToJson(true);
      return !temporaryOverrides.resourcePivots.Any<KeyValuePair<string, List<string>>>((Func<KeyValuePair<string, List<string>>, bool>) (rp => rp.Value.Any<string>())) && !temporaryOverrides.outputs.Any<string>() && !temporaryOverrides.SkipAll ? (TemporaryOverrides) null : temporaryOverrides;
    }

    public bool ShouldIgnore(ContentItem contentItem) => contentItem != null && this.ShouldIgnore(contentItem.ResourcePivotKeys);

    public bool ShouldIgnore(IEnumerable<ResourcePivotKey> resourcePivotKeys) => resourcePivotKeys != null && resourcePivotKeys.Any<ResourcePivotKey>() && resourcePivotKeys.GroupBy<ResourcePivotKey, string>((Func<ResourcePivotKey, string>) (rpk => rpk.GroupKey)).Any<IGrouping<string, ResourcePivotKey>>((Func<IGrouping<string, ResourcePivotKey>, bool>) (rpk => rpk.All<ResourcePivotKey>(new Func<ResourcePivotKey, bool>(this.ShouldIgnore))));

    public bool ShouldIgnore(IFileSet fileSet)
    {
      if (fileSet == null || string.IsNullOrWhiteSpace(fileSet.Output))
        return false;
      return this.ShouldIgnoreOutputs(fileSet) || this.ShouldIgnoreOutputExtensions(fileSet);
    }

    private bool ShouldIgnore(ResourcePivotKey resourcePivotKey) => this.resourcePivots.ContainsKey(resourcePivotKey.GroupKey) && this.resourcePivots[resourcePivotKey.GroupKey].Any<string>() && this.resourcePivots[resourcePivotKey.GroupKey].All<string>((Func<string, bool>) (pivotToIgnore => resourcePivotKey.Key.IndexOf(pivotToIgnore, StringComparison.OrdinalIgnoreCase) == -1));

    private static IEnumerable<string> GetItems(string items)
    {
      if (items == null)
        return (IEnumerable<string>) new string[0];
      return (IEnumerable<string>) items.Split(new char[1]
      {
        ';'
      }, StringSplitOptions.RemoveEmptyEntries);
    }

    private static IEnumerable<string> GetElementItems(
      IEnumerable<XElement> elements,
      string elementName)
    {
      return TemporaryOverrides.GetItems(elements.Elements<XElement>((XName) elementName).Select<XElement, string>((Func<XElement, string>) (e => (string) e)).FirstOrDefault<string>()).Where<string>((Func<string, bool>) (i => !i.IsNullOrWhitespace())).Select<string, string>((Func<string, string>) (i => i.Trim()));
    }

    private bool ShouldIgnoreOutputs(IFileSet fileSet) => this.outputs.Any<string>() && !this.outputs.Any<string>((Func<string, bool>) (output =>
    {
      if (fileSet.Output.IndexOf(output, StringComparison.OrdinalIgnoreCase) < 0)
        return false;
      return output.IndexOf(".", StringComparison.OrdinalIgnoreCase) == -1 || fileSet.Output.Count<char>((Func<char, bool>) (o => o == '.')) > 1;
    }));

    private bool ShouldIgnoreOutputExtensions(IFileSet fileSet) => this.outputExtensions.Any<string>() && !this.outputExtensions.Any<string>((Func<string, bool>) (outputExtension => fileSet.Output.EndsWith(outputExtension, StringComparison.OrdinalIgnoreCase)));

    private void LoadFromFile(string overrideFile)
    {
      if (!File.Exists(overrideFile))
        return;
      try
      {
        IEnumerable<XElement> xelements = XDocument.Load(overrideFile).Elements((XName) "Overrides");
        bool? nullable = xelements.Attributes((XName) "SkipAll").Select<XAttribute, bool?>((Func<XAttribute, bool?>) (a => (bool?) a)).FirstOrDefault<bool?>();
        this.SkipAll = nullable.GetValueOrDefault() && nullable.HasValue;
        this.resourcePivots.Add("locales", TemporaryOverrides.GetElementItems(xelements, "Locales").ToList<string>());
        this.resourcePivots.Add("themes", TemporaryOverrides.GetElementItems(xelements, "Themes").ToList<string>());
        this.resourcePivots.Add("dpi", TemporaryOverrides.GetElementItems(xelements, "Dpi").ToList<string>());
        this.outputs.AddRange(TemporaryOverrides.GetElementItems(xelements, "Outputs"));
        this.outputExtensions.AddRange(TemporaryOverrides.GetElementItems(xelements, "OutputExtensions"));
        foreach (XElement element in xelements.Elements<XElement>((XName) "ResourcePivot"))
          this.resourcePivots.Add((string) element.Attribute((XName) "key"), ((string) element).SafeSplitSemiColonSeperatedValue().ToList<string>());
      }
      catch (Exception ex)
      {
        throw new ConfigurationErrorsException(ResourceStrings.OverrideFileLoadErrorMessage.InvariantFormat((object) overrideFile), ex);
      }
    }
  }
}
