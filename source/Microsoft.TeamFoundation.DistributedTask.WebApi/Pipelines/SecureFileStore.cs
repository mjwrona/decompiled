// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.SecureFileStore
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class SecureFileStore : ISecureFileStore
  {
    private readonly Dictionary<Guid, SecureFile> m_resourcesById = new Dictionary<Guid, SecureFile>();
    private readonly Dictionary<string, List<SecureFile>> m_resourcesByName = new Dictionary<string, List<SecureFile>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private readonly bool m_lazyLoadFiles;

    public SecureFileStore(
      IList<SecureFile> files,
      ISecureFileResolver resolver = null,
      bool lazyLoadFiles = false)
    {
      this.Resolver = resolver;
      this.m_lazyLoadFiles = lazyLoadFiles;
      this.Add(files != null ? files.ToArray<SecureFile>() : (SecureFile[]) null);
    }

    public ISecureFileResolver Resolver { get; }

    public IList<SecureFileReference> GetAuthorizedReferences()
    {
      if (!this.m_lazyLoadFiles)
        return (IList<SecureFileReference>) this.m_resourcesById.Values.Select<SecureFile, SecureFileReference>((Func<SecureFile, SecureFileReference>) (x => new SecureFileReference()
        {
          Id = x.Id
        })).ToList<SecureFileReference>();
      return this.Resolver == null ? (IList<SecureFileReference>) new List<SecureFileReference>() : this.Resolver.GetAuthorizedReferences();
    }

    public void Authorize(SecureFileReference reference) => this.Resolver?.Authorize(reference);

    public SecureFile Get(SecureFileReference reference)
    {
      if (reference == null)
        return (SecureFile) null;
      Guid id = reference.Id;
      string literal = reference.Name?.Literal;
      if (id == Guid.Empty && string.IsNullOrEmpty(literal))
        return (SecureFile) null;
      SecureFile secureFile = (SecureFile) null;
      if (id != Guid.Empty)
      {
        if (this.m_resourcesById.TryGetValue(id, out secureFile))
          return secureFile;
      }
      else
      {
        List<SecureFile> secureFileList;
        if (!string.IsNullOrEmpty(literal) && this.m_resourcesByName.TryGetValue(literal, out secureFileList))
          return secureFileList.Count <= 1 ? secureFileList[0] : throw new AmbiguousResourceSpecificationException(PipelineStrings.AmbiguousServiceEndpointSpecification((object) id));
      }
      ISecureFileResolver resolver = this.Resolver;
      secureFile = resolver != null ? resolver.Resolve(reference) : (SecureFile) null;
      if (secureFile != null)
        this.Add(secureFile);
      return secureFile;
    }

    private void Add(params SecureFile[] resources)
    {
      if (resources == null || resources.Length == 0)
        return;
      foreach (SecureFile resource in resources)
      {
        if (!this.m_resourcesById.TryGetValue(resource.Id, out SecureFile _))
        {
          this.m_resourcesById.Add(resource.Id, resource);
          List<SecureFile> secureFileList;
          if (!this.m_resourcesByName.TryGetValue(resource.Name, out secureFileList))
          {
            secureFileList = new List<SecureFile>();
            this.m_resourcesByName.Add(resource.Name, secureFileList);
          }
          secureFileList.Add(resource);
        }
      }
    }
  }
}
