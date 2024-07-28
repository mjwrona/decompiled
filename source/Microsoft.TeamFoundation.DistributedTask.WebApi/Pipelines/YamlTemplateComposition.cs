// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.YamlTemplateComposition
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  public sealed class YamlTemplateComposition
  {
    private const int m_rootId = 1;
    private int m_nextId = 1;
    private Dictionary<string, YamlTemplateReference> m_fileReferences = new Dictionary<string, YamlTemplateReference>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public YamlTemplateComposition(
      IList<YamlTemplateReference> yamlTemplateReferences = null)
    {
      if (yamlTemplateReferences == null || yamlTemplateReferences.Count == 0)
        return;
      HashSet<int> intSet = new HashSet<int>();
      foreach (YamlTemplateReference templateReference in (IEnumerable<YamlTemplateReference>) yamlTemplateReferences)
      {
        if (templateReference?.Location == null)
          throw new ArgumentException("The template location must not be null.");
        if (intSet.Contains(templateReference.Id))
          throw new ArgumentException(string.Format("The template id '{0}' is duplicated.", (object) templateReference.Id));
        if (templateReference.Id >= this.m_nextId)
          this.m_nextId = templateReference.Id + 1;
        this.m_fileReferences[templateReference.Location.ToString()] = templateReference;
        intSet.Add(templateReference.Id);
      }
    }

    public IList<YamlTemplateReference> Files => (IList<YamlTemplateReference>) this.m_fileReferences.Values.ToList<YamlTemplateReference>();

    internal int NextId => this.m_nextId;

    public YamlTemplateReference AddRootYamlFile(YamlTemplateLocation location)
    {
      if (this.m_nextId != 1)
        throw new ArgumentException(string.Format("Attempting to add a root file with non-root Id {0}", (object) this.m_nextId));
      return this.AddTemplate(location, (YamlTemplateLocation) null, false);
    }

    public YamlTemplateReference AddTemplate(
      YamlTemplateLocation location,
      YamlTemplateLocation fromLocation,
      bool isExtendedBy)
    {
      ArgumentUtility.CheckForNull<YamlTemplateLocation>(location, nameof (location));
      YamlTemplateReference templateReference1 = this.EnsureTemplateReference(location);
      if (fromLocation != null)
      {
        YamlTemplateReference templateReference2 = this.EnsureTemplateReference(fromLocation);
        if (isExtendedBy)
          templateReference1.ExtendedBy.Add(templateReference2.Id);
        else
          templateReference1.IncludedBy.Add(templateReference2.Id);
      }
      return templateReference1;
    }

    public bool Extends(string repositoryAlias, string path)
    {
      for (YamlTemplateReference result = this.m_fileReferences.Values.Where<YamlTemplateReference>((Func<YamlTemplateReference, bool>) (x => 1 == x.ExtendedBy.FirstOrDefault<int>())).FirstOrDefault<YamlTemplateReference>(); result != null; result = this.m_fileReferences.Values.Where<YamlTemplateReference>((Func<YamlTemplateReference, bool>) (x => result.Id == x.ExtendedBy.FirstOrDefault<int>())).FirstOrDefault<YamlTemplateReference>())
      {
        if (string.Equals(result.Location.RepositoryAlias, repositoryAlias, StringComparison.Ordinal) && this.Match(result.Location.Path, path))
          return true;
      }
      return false;
    }

    private bool Match(string str, string pattern) => Wildcard.IsWildcard(pattern) ? Wildcard.Match(str, pattern, StringComparison.Ordinal) : str.Equals(pattern, StringComparison.Ordinal);

    private YamlTemplateReference EnsureTemplateReference(YamlTemplateLocation location)
    {
      YamlTemplateReference templateReference;
      if (!this.m_fileReferences.TryGetValue(location.ToString(), out templateReference))
      {
        templateReference = new YamlTemplateReference(this.m_nextId, location);
        ++this.m_nextId;
        this.m_fileReferences.Add(templateReference.Location.ToString(), templateReference);
      }
      return templateReference;
    }
  }
}
