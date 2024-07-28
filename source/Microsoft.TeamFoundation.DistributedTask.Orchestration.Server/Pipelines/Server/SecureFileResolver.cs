// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Server.SecureFileResolver
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Server
{
  internal sealed class SecureFileResolver : ISecureFileResolver
  {
    private readonly Guid m_projectId;
    private readonly IVssRequestContext m_requestContext;
    private readonly bool m_allowImplicitAuthorization;
    private HashSet<SecureFileReference> m_authorizedReferences = new HashSet<SecureFileReference>((IEqualityComparer<SecureFileReference>) new PipelineResources.FileComparer());

    public SecureFileResolver(
      IVssRequestContext requestContext,
      Guid projectId,
      bool allowImplicitAuthorization = false)
    {
      this.m_projectId = projectId;
      this.m_requestContext = requestContext;
      this.m_allowImplicitAuthorization = allowImplicitAuthorization;
    }

    public IList<SecureFile> Resolve(
      ICollection<SecureFileReference> references,
      ResourceActionFilter actionFilter = ResourceActionFilter.Use)
    {
      List<SecureFile> list = SecureFileResolver.Resolve(this.m_requestContext.Elevate(), this.m_projectId, references).ToList<SecureFile>();
      Dictionary<Guid, SecureFile> dictionary;
      if (this.m_allowImplicitAuthorization)
      {
        dictionary = SecureFileResolver.Resolve(this.m_requestContext, this.m_projectId, references, actionFilter).ToDictionary<SecureFile, Guid>((Func<SecureFile, Guid>) (f => f.Id));
        foreach (KeyValuePair<Guid, SecureFile> keyValuePair in dictionary)
        {
          SecureFileReference reference = new SecureFileReference();
          reference.Id = keyValuePair.Value.Id;
          reference.Name = (ExpressionValue<string>) keyValuePair.Value.Name;
          this.Authorize(reference);
        }
      }
      else
        dictionary = new Dictionary<Guid, SecureFile>();
      foreach (SecureFileReference reference in (IEnumerable<SecureFileReference>) references)
        SecureFileResolver.ResolveFileId(reference, list);
      foreach (SecureFileReference authorizedReference in this.m_authorizedReferences)
        SecureFileResolver.ResolveFileId(authorizedReference, list);
      foreach (SecureFileReference reference1 in (IEnumerable<SecureFileReference>) references)
      {
        SecureFileReference reference = reference1;
        if (!dictionary.ContainsKey(reference.Id) && this.m_authorizedReferences.Any<SecureFileReference>((Func<SecureFileReference, bool>) (x => x.Id == reference.Id)))
        {
          SecureFile secureFile = list.Find((Predicate<SecureFile>) (f => f.Id == reference.Id));
          if (secureFile != null)
            dictionary.Add(secureFile.Id, secureFile);
        }
      }
      return (IList<SecureFile>) dictionary.Values.ToList<SecureFile>();
    }

    public IList<SecureFileReference> GetAuthorizedReferences() => (IList<SecureFileReference>) this.m_authorizedReferences.ToList<SecureFileReference>();

    public void Authorize(SecureFileReference reference) => this.m_authorizedReferences.Add(reference);

    internal static IList<SecureFile> Resolve(
      IVssRequestContext requestContext,
      Guid projectId,
      ICollection<SecureFileReference> references,
      ResourceActionFilter actionFilter = ResourceActionFilter.Use)
    {
      SecureFileActionFilter fileActionFilter = SecureFileResolver.ParseToSecureFileActionFilter(actionFilter);
      List<SecureFile> secureFileList = new List<SecureFile>();
      if (references != null && references.Count > 0)
      {
        ISecureFileService service = requestContext.GetService<ISecureFileService>();
        List<Guid> list1 = references.Where<SecureFileReference>((Func<SecureFileReference, bool>) (x => x.Id != Guid.Empty)).Select<SecureFileReference, Guid>((Func<SecureFileReference, Guid>) (x => x.Id)).ToList<Guid>();
        if (list1.Count > 0)
        {
          IList<SecureFile> secureFiles = service.GetSecureFiles(requestContext, projectId, (IEnumerable<Guid>) list1, actionFilter: fileActionFilter);
          secureFileList.AddRange((IEnumerable<SecureFile>) secureFiles);
        }
        List<string> list2 = references.Where<SecureFileReference>((Func<SecureFileReference, bool>) (x => x.Id == Guid.Empty && !string.IsNullOrEmpty(x.Name?.Literal))).Select<SecureFileReference, string>((Func<SecureFileReference, string>) (x => x.Name.Literal)).ToList<string>();
        if (list2.Count > 0)
        {
          IList<SecureFile> secureFiles = service.GetSecureFiles(requestContext, projectId, (IEnumerable<string>) list2, actionFilter: fileActionFilter);
          secureFileList.AddRange((IEnumerable<SecureFile>) secureFiles);
        }
      }
      return (IList<SecureFile>) secureFileList;
    }

    private static void ResolveFileId(SecureFileReference reference, List<SecureFile> resolvedFiles)
    {
      if (!(reference.Id == Guid.Empty))
        return;
      SecureFile secureFile = resolvedFiles.Find((Predicate<SecureFile>) (f => f.Name.Equals(reference.Name.Literal, StringComparison.OrdinalIgnoreCase)));
      if (secureFile == null)
        return;
      reference.Id = secureFile.Id;
    }

    private static SecureFileActionFilter ParseToSecureFileActionFilter(
      ResourceActionFilter actionFilter)
    {
      SecureFileActionFilter fileActionFilter = SecureFileActionFilter.Use;
      switch (actionFilter)
      {
        case ResourceActionFilter.None:
          fileActionFilter = SecureFileActionFilter.None;
          break;
        case ResourceActionFilter.Manage:
          fileActionFilter = SecureFileActionFilter.Manage;
          break;
        case ResourceActionFilter.Use:
          fileActionFilter = SecureFileActionFilter.Use;
          break;
      }
      return fileActionFilter;
    }
  }
}
