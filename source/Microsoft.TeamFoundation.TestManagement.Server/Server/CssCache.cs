// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CssCache
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Server;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class CssCache : ICssCache
  {
    private Dictionary<string, IdAndString> m_pathToUri;
    private Dictionary<string, IdAndString> m_uriToPath;
    private readonly object m_lock;
    private CssCacheType m_cacheType;

    internal CssCache()
    {
    }

    public CssCache(
      TestManagementRequestContext context,
      Dictionary<string, IdAndString> pathToUri,
      CssCacheType cacheType)
    {
      context.IfNullThenTraceAndDebugFail("BusinessLayer", (object) pathToUri, nameof (pathToUri));
      this.m_lock = new object();
      this.m_pathToUri = pathToUri;
      this.m_uriToPath = new Dictionary<string, IdAndString>(this.m_pathToUri.Count);
      this.m_cacheType = cacheType;
      foreach (KeyValuePair<string, IdAndString> keyValuePair in this.m_pathToUri)
      {
        TestManagementRequestContext context1 = context;
        Dictionary<string, IdAndString>.KeyCollection keys = this.m_uriToPath.Keys;
        IdAndString idAndString1 = keyValuePair.Value;
        string str = idAndString1.String;
        int num = !keys.Contains<string>(str) ? 1 : 0;
        context1.TraceAndDebugAssert("BusinessLayer", num != 0, "!m_uriToPath.Keys.Contains(pair.Value.String)");
        Dictionary<string, IdAndString> uriToPath = this.m_uriToPath;
        idAndString1 = keyValuePair.Value;
        string key1 = idAndString1.String;
        string key2 = keyValuePair.Key;
        idAndString1 = keyValuePair.Value;
        int id = idAndString1.Id;
        IdAndString idAndString2 = new IdAndString(key2, id);
        uriToPath.Add(key1, idAndString2);
      }
    }

    public virtual TcmCommonStructureNodeInfo GetCssNodeAndThrow(
      TestManagementRequestContext context,
      string path)
    {
      try
      {
        context.TraceEnter("Framework", "CssCache.GetCssNodeAndThrow");
        TcmCommonStructureNodeInfo cssNode = this.GetCssNode(context, path);
        if (cssNode != null && this.GetIdAndPath(context, cssNode.Uri).Equals(IdAndString.Empty))
          throw new InvalidStructurePathException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidStructurePath, (object) path));
        return cssNode;
      }
      finally
      {
        context.TraceLeave("Framework", "CssCache.GetCssNodeAndThrow");
      }
    }

    private TcmCommonStructureNodeInfo GetCssNode(
      TestManagementRequestContext context,
      string workItemPath)
    {
      TcmCommonStructureNodeInfo cssNode = (TcmCommonStructureNodeInfo) null;
      try
      {
        if (this.m_cacheType == CssCacheType.Area)
          cssNode = context.CSSHelper.GetNodeFromAreaPath(workItemPath);
        else if (this.m_cacheType == CssCacheType.Iteration)
          cssNode = context.CSSHelper.GetNodeFromIterationPath(workItemPath);
      }
      catch (ArgumentException ex)
      {
        context.TraceException("Exceptions", (Exception) ex);
        throw new InvalidStructurePathException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidStructurePath, (object) workItemPath));
      }
      catch (CommonStructureSubsystemServiceException ex)
      {
        context.TraceException("Exceptions", (Exception) ex);
        throw new InvalidStructurePathException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidStructurePath, (object) workItemPath));
      }
      catch (ProjectDoesNotExistWithNameException ex)
      {
        context.TraceException("Exceptions", (Exception) ex);
        throw new InvalidStructurePathException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidStructurePath, (object) workItemPath));
      }
      return cssNode;
    }

    public virtual IdAndString GetIdAndThrow(TestManagementRequestContext context, string path)
    {
      IdAndString value = IdAndString.Empty;
      RetryHelper.RetryOnExceptions((Action) (() => value = this.GetIdAndThrowWithRetry(context, path)), 3, typeof (InvalidStructurePathException));
      return value;
    }

    public virtual IdAndString GetIdAndThrowWithRetry(
      TestManagementRequestContext context,
      string path)
    {
      IdAndString idAndString = IdAndString.Empty;
      TcmCommonStructureNodeInfo cssNode = this.GetCssNode(context, path);
      if (cssNode != null)
      {
        context.TraceWarning("Framework", "CssCache - NodeInfo returned from CSS Service has uri= {0}", (object) cssNode.Uri);
        IdAndString idAndPath = this.GetIdAndPath(context, cssNode.Uri);
        if (!idAndPath.Equals(IdAndString.Empty))
        {
          idAndString = new IdAndString(cssNode.Uri, idAndPath.Id);
          context.TraceWarning("Framework", "CssCache - IdAndString created with uri = {0} and id = {1}", (object) cssNode.Uri, (object) idAndPath.Id);
        }
      }
      return !idAndString.Equals(IdAndString.Empty) ? idAndString : throw new InvalidStructurePathException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidStructurePath, (object) path));
    }

    public IdAndString GetIdAndPath(TestManagementRequestContext context, string uri)
    {
      try
      {
        context.TraceEnter("Framework", "CssCache.GetIdAndPath");
        IdAndString idAndPath;
        if (this.GetIdAndPathFromCache(uri, out idAndPath))
        {
          context.TraceWarning("Cache", "CssCache - Fetching value from cache for {0} with values {1}", (object) uri, (object) idAndPath);
          return idAndPath;
        }
        context.TraceVerbose("Cache", "CssCache - Updating for {0}", (object) uri);
        context.TestManagementHost.Replicator.ForceUpdateCss(context, string.Empty, new int?());
        if (this.GetIdAndPathFromCache(uri, out idAndPath))
        {
          context.TraceWarning("Cache", "CssCache - Updated cache for {0} with values {1}", (object) uri, (object) idAndPath);
          return idAndPath;
        }
        context.TraceWarning("Cache", "CssCache - Uri not found for {0}", (object) uri);
        return IdAndString.Empty;
      }
      finally
      {
        context.TraceLeave("Framework", "CssCache.GetIdAndPath");
      }
    }

    public virtual void Update(string path, IdAndString idAndUri)
    {
      lock (this.m_lock)
      {
        IdAndString idAndString;
        if (this.m_uriToPath.TryGetValue(idAndUri.String, out idAndString))
          this.m_pathToUri.Remove(idAndString.String);
        this.m_pathToUri[path] = idAndUri;
        this.m_uriToPath[idAndUri.String] = new IdAndString(path, idAndUri.Id);
      }
    }

    public virtual void RemoveByUri(string uri)
    {
      lock (this.m_lock)
      {
        IdAndString idAndString;
        if (!this.m_uriToPath.TryGetValue(uri, out idAndString))
          return;
        this.m_pathToUri.Remove(idAndString.String);
        this.m_uriToPath.Remove(uri);
      }
    }

    private bool GetIdAndUriFromCache(string path, out IdAndString value)
    {
      lock (this.m_lock)
        return this.m_pathToUri.TryGetValue(path, out value);
    }

    private bool GetIdAndPathFromCache(string uri, out IdAndString value)
    {
      lock (this.m_lock)
        return this.m_uriToPath.TryGetValue(uri, out value);
    }
  }
}
