// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitTemplatesProvider
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;

namespace Microsoft.TeamFoundation.Git.Server
{
  public sealed class GitTemplatesProvider
  {
    private const string c_templateResourcePrefix = "Template.";
    private Func<List<string>> m_allTemplateListFinder;

    public GitTemplatesProvider() => this.m_allTemplateListFinder = new Func<List<string>>(this.GetAllTemplateResources);

    internal GitTemplatesProvider(Func<List<string>> templateListFinder) => this.m_allTemplateListFinder = templateListFinder;

    public IList<GitTemplate> GetTemplatesOfType(string type = null)
    {
      List<string> stringList = this.m_allTemplateListFinder();
      IList<GitTemplate> templatesOfType = (IList<GitTemplate>) new List<GitTemplate>();
      foreach (string str1 in stringList)
      {
        string str2 = str1.Remove(0, "Template.".Length);
        string strB = str2.Substring(0, str2.IndexOf("."));
        if (type == null || string.Compare(type, strB, StringComparison.OrdinalIgnoreCase) == 0)
          templatesOfType.Add(new GitTemplate()
          {
            Name = str2.Remove(0, strB.Length + 1),
            Type = strB
          });
      }
      return templatesOfType;
    }

    public static string GetTemplateContent(string name, string type)
    {
      string templateContent = GitTemplateResources.Get(GitTemplatesProvider.GetResourceNameFromTemplate(name, type));
      if (templateContent != null)
        return templateContent;
      string name1 = "Template." + type + "." + name;
      using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name1))
        return manifestResourceStream != null ? new StreamReader(manifestResourceStream).ReadToEnd() : throw new ArgumentException("fullName", Microsoft.TeamFoundation.Git.Server.Resources.Get("ErrorGitTemplateNotFound"));
    }

    private List<string> GetAllTemplateResources()
    {
      List<string> list = ((IEnumerable<string>) Assembly.GetExecutingAssembly().GetManifestResourceNames()).ToList<string>();
      list.RemoveAll((Predicate<string>) (item => !item.StartsWith("Template.")));
      ResourceSet resourceSet = GitTemplateResources.Manager.GetResourceSet(CultureInfo.CurrentUICulture, true, false);
      if (resourceSet != null)
      {
        foreach (DictionaryEntry dictionaryEntry in resourceSet)
          list.Add(GitTemplatesProvider.GetTemplateResourceNameFromKey(dictionaryEntry.Key.ToString()));
      }
      return list;
    }

    private static string GetTemplateResourceNameFromKey(string key) => "Template." + key.Replace('_', '.');

    private static string GetResourceNameFromTemplate(string name, string type) => type + "_" + name.Replace('.', '_');
  }
}
