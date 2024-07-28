// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.TfsDiagnosticService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace Microsoft.TeamFoundation.Client
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class TfsDiagnosticService
  {
    private static TfsDiagnosticService s_instance = (TfsDiagnosticService) null;
    private SortedDictionary<string, ITfsDiagnosticProvider> m_providers;
    private const char c_areaPathDelimiter = '/';
    private static readonly char[] c_areaPathDelimiterArray = new char[1]
    {
      '/'
    };
    private static readonly string c_rootNodeName = "TfsDiagnostics";

    internal TfsDiagnosticService()
    {
      this.m_providers = new SortedDictionary<string, ITfsDiagnosticProvider>((IComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.Register((ITfsDiagnosticProvider) new CommonDiagnosticProvider());
    }

    public static TfsDiagnosticService Instance
    {
      get
      {
        if (TfsDiagnosticService.s_instance == null)
          TfsDiagnosticService.s_instance = new TfsDiagnosticService();
        return TfsDiagnosticService.s_instance;
      }
    }

    public static string BuildAreaPath(string featureTeam, params string[] featureNames)
    {
      if (string.IsNullOrEmpty(featureTeam))
        throw new ArgumentNullException(nameof (featureTeam));
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append('/');
      stringBuilder.Append("TFS");
      stringBuilder.Append('/');
      stringBuilder.Append(featureTeam);
      foreach (string featureName in featureNames)
      {
        stringBuilder.Append('/');
        stringBuilder.Append(featureName);
      }
      return stringBuilder.ToString();
    }

    public static char[] AreaPathDelimiters => TfsDiagnosticService.c_areaPathDelimiterArray;

    public void Register(ITfsDiagnosticProvider provider)
    {
      if (provider == null)
        throw new ArgumentNullException(nameof (provider));
      lock (this.m_providers)
      {
        foreach (string areaPath in provider.AreaPaths)
        {
          string key = this.NormalizeAreaPath(areaPath);
          if (string.IsNullOrEmpty(key) || this.m_providers.ContainsKey(key))
            throw new InvalidOperationException("provider.AreaPaths");
          this.m_providers[key] = provider;
        }
      }
    }

    public void Unregister(ITfsDiagnosticProvider provider)
    {
      if (provider == null)
        throw new ArgumentNullException(nameof (provider));
      lock (this.m_providers)
      {
        foreach (string areaPath in provider.AreaPaths)
        {
          string key = this.NormalizeAreaPath(areaPath);
          if (string.IsNullOrEmpty(key))
            throw new InvalidOperationException("provider.AreaPaths");
          this.m_providers.Remove(key);
        }
      }
    }

    internal ITfsDiagnosticProvider[] GetProviders()
    {
      List<ITfsDiagnosticProvider> diagnosticProviderList = new List<ITfsDiagnosticProvider>();
      lock (this.m_providers)
      {
        foreach (ITfsDiagnosticProvider diagnosticProvider in this.m_providers.Values)
          diagnosticProviderList.Add(diagnosticProvider);
      }
      return diagnosticProviderList.ToArray();
    }

    public Tree<TfsDiagnosticNodeInfo> GetProvidersAsTree()
    {
      lock (this.m_providers)
        return this.BuildTree(this.m_providers);
    }

    public void SaveState(string areaPath, bool recursive, string outputFilename) => this.SaveState(new string[1]
    {
      areaPath
    }, recursive, outputFilename);

    public void SaveState(string[] areaPaths, bool recursive, string outputFilename)
    {
      using (StreamWriter streamWriter = new StreamWriter(outputFilename))
      {
        this.WriteState(this.GetProvidersForAreaPath(areaPaths, recursive), (TextWriter) streamWriter);
        streamWriter.Close();
      }
    }

    internal StringBuilder GetState(string areaPath, bool recursive) => this.GetState(new string[1]
    {
      areaPath
    }, recursive);

    public StringBuilder GetState(string[] areaPaths, bool recursive)
    {
      StringBuilder state = (StringBuilder) null;
      using (StringWriter stringWriter = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture))
      {
        this.WriteState(this.GetProvidersForAreaPath(areaPaths, recursive), (TextWriter) stringWriter);
        state = stringWriter.GetStringBuilder();
        stringWriter.Close();
      }
      return state;
    }

    internal void TraceState(string areaPath, bool recursive) => this.TraceState(new string[1]
    {
      areaPath
    }, recursive);

    internal void TraceState(string[] areaPaths, bool recursive)
    {
      using (TfsDiagnosticService.TfsDiagnosticTraceWriter diagnosticTraceWriter = new TfsDiagnosticService.TfsDiagnosticTraceWriter())
      {
        this.WriteState(this.GetProvidersForAreaPath(areaPaths, recursive), (TextWriter) diagnosticTraceWriter);
        diagnosticTraceWriter.Close();
      }
    }

    private void WriteState(
      SortedDictionary<string, ITfsDiagnosticProvider> providers,
      TextWriter textWriter)
    {
      using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, new XmlWriterSettings()
      {
        Indent = true,
        Encoding = Encoding.UTF8
      }))
      {
        xmlWriter.WriteStartDocument();
        using (new XmlElementWriterUtility(TfsDiagnosticService.c_rootNodeName, xmlWriter))
        {
          xmlWriter.WriteAttributeString("xmlns", "a", (string) null, "http://schemas.microsoft.com/2003/10/Serialization/Arrays");
          xmlWriter.WriteAttributeString("xmlns", "i", (string) null, "http://www.w3.org/2001/XMLSchema-instance");
          foreach (TreeNode<TfsDiagnosticNodeInfo> rootNode in (Collection<TreeNode<TfsDiagnosticNodeInfo>>) this.BuildTree(providers).RootNodes)
            this.WriteStateForNode(rootNode, xmlWriter);
        }
        xmlWriter.WriteEndDocument();
        xmlWriter.Close();
      }
    }

    private void WriteStateForNode(TreeNode<TfsDiagnosticNodeInfo> node, XmlWriter xmlWriter)
    {
      using (new XmlElementWriterUtility(node.Value.Name, xmlWriter))
      {
        try
        {
          if (node.Value.Provider != null)
          {
            node.Value.Provider.WriteState(node.Value.AreaPath, xmlWriter);
            xmlWriter.Flush();
          }
          if (!node.HasChildren)
            return;
          foreach (TreeNode<TfsDiagnosticNodeInfo> childNode in (Collection<TreeNode<TfsDiagnosticNodeInfo>>) node.ChildNodes)
            this.WriteStateForNode(childNode, xmlWriter);
        }
        catch (Exception ex)
        {
          using (new XmlElementWriterUtility("TfsDiagnosticException", xmlWriter))
          {
            using (new XmlElementWriterUtility("Type", xmlWriter))
              xmlWriter.WriteString(ex.GetType().ToString());
            using (new XmlElementWriterUtility("Message", xmlWriter))
              xmlWriter.WriteString(ex.Message);
            if (ex.InnerException == null)
              return;
            using (new XmlElementWriterUtility("InnerException", xmlWriter))
            {
              using (new XmlElementWriterUtility("Type", xmlWriter))
                xmlWriter.WriteString(ex.InnerException.GetType().ToString());
              using (new XmlElementWriterUtility("Message", xmlWriter))
                xmlWriter.WriteString(ex.InnerException.Message);
            }
          }
        }
      }
    }

    private string NormalizeAreaPath(string areaPath)
    {
      StringBuilder stringBuilder = new StringBuilder(areaPath);
      for (int index = stringBuilder.Length - 1; index > 0 && stringBuilder[index] == '/'; --index)
        stringBuilder.Length = index;
      return stringBuilder.ToString();
    }

    private SortedDictionary<string, ITfsDiagnosticProvider> GetProvidersForAreaPath(
      string[] areaPaths,
      bool recursive)
    {
      SortedDictionary<string, ITfsDiagnosticProvider> providersForAreaPath = new SortedDictionary<string, ITfsDiagnosticProvider>((IComparer<string>) StringComparer.OrdinalIgnoreCase);
      lock (this.m_providers)
      {
        if (areaPaths.Length == 0)
        {
          foreach (string key in this.m_providers.Keys)
            providersForAreaPath.Add(key, this.m_providers[key]);
        }
        else
        {
          foreach (string areaPath in areaPaths)
          {
            string key1 = this.NormalizeAreaPath(areaPath);
            if (recursive)
            {
              string str = key1 + "/";
              foreach (string key2 in this.m_providers.Keys)
              {
                if (key2.Equals(key1, StringComparison.OrdinalIgnoreCase) || key2.StartsWith(str, StringComparison.OrdinalIgnoreCase))
                  providersForAreaPath[key2] = this.m_providers[key2];
              }
            }
            else
            {
              ITfsDiagnosticProvider diagnosticProvider;
              if (this.m_providers.TryGetValue(key1, out diagnosticProvider))
                providersForAreaPath[key1] = diagnosticProvider;
            }
          }
        }
      }
      return providersForAreaPath;
    }

    private Tree<TfsDiagnosticNodeInfo> BuildTree(
      SortedDictionary<string, ITfsDiagnosticProvider> providers)
    {
      Tree<TfsDiagnosticNodeInfo> tree = new Tree<TfsDiagnosticNodeInfo>();
      foreach (string key in providers.Keys)
      {
        string[] strArray = key.Split(TfsDiagnosticService.c_areaPathDelimiterArray, StringSplitOptions.RemoveEmptyEntries);
        TreeNode<TfsDiagnosticNodeInfo> parent = (TreeNode<TfsDiagnosticNodeInfo>) null;
        foreach (string str in strArray)
        {
          TreeNode<TfsDiagnosticNodeInfo> treeNode = parent != null ? parent.Find(str, (string) null, StringComparison.OrdinalIgnoreCase) : tree.Find(str, (string) null, StringComparison.OrdinalIgnoreCase);
          if (treeNode == null)
          {
            treeNode = new TreeNode<TfsDiagnosticNodeInfo>(new TfsDiagnosticNodeInfo(str, this.BuildAreaPathForNode(parent, str), (ITfsDiagnosticProvider) null));
            if (parent != null)
              parent.ChildNodes.Add(treeNode);
            else
              tree.RootNodes.Add(treeNode);
          }
          parent = treeNode;
        }
        parent.Value = new TfsDiagnosticNodeInfo(parent.Value.Name, key, providers[key]);
      }
      return tree;
    }

    private string BuildAreaPathForNode(TreeNode<TfsDiagnosticNodeInfo> parent, string nodeName) => parent != null ? parent.Value.AreaPath + "/" + nodeName : "/" + nodeName;

    private class TfsDiagnosticTraceWriter : TextWriter
    {
      internal TfsDiagnosticTraceWriter()
        : base((IFormatProvider) CultureInfo.InvariantCulture)
      {
      }

      public override void Write(char[] buffer, int index, int count) => TeamFoundationTrace.Info(new string(buffer, index, count));

      public override Encoding Encoding => Encoding.UTF8;
    }
  }
}
