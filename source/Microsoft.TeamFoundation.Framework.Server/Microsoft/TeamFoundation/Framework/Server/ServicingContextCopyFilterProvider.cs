// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingContextCopyFilterProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ServicingContextCopyFilterProvider : ICopyFilterProvider
  {
    public const char Delimitator = ',';
    public const string ServicingTokenPrefix = "ServicingContextCopyFilters.";
    private IServicingContext m_servicingContext;

    public ServicingContextCopyFilterProvider(IServicingContext servicingContext)
    {
      ArgumentUtility.CheckForNull<IServicingContext>(servicingContext, nameof (servicingContext));
      this.m_servicingContext = servicingContext;
    }

    public string this[string token]
    {
      get
      {
        ArgumentUtility.CheckStringForNullOrEmpty(token, nameof (token));
        string str = this.m_servicingContext.GetTokenOrDefault<string>("ServicingContextCopyFilters." + token) ?? string.Empty;
        this.m_servicingContext.LogInfo("Getting filters for " + token + ": " + str);
        return str;
      }
      set
      {
        ArgumentUtility.CheckStringForNullOrEmpty(token, nameof (token));
        this.m_servicingContext.LogInfo("Setting filters for " + token + " to: " + value);
        this.m_servicingContext.Tokens["ServicingContextCopyFilters." + token] = value;
      }
    }

    public HashSet<T> GetFilter<T>(string token)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(token, nameof (token));
      HashSet<T> filter = new HashSet<T>();
      string str1 = this[token];
      if (!string.IsNullOrEmpty(str1))
      {
        string str2 = str1;
        char[] chArray = new char[1]{ ',' };
        foreach (string source in str2.Split(chArray))
        {
          if (typeof (T) == typeof (string) && !source.All<char>((Func<char, bool>) (c => ServicingContextCopyFilterProvider.IsAllowedCharacter(c))))
            throw new ArgumentException(token + " must only contain Letters and/or Numbers, found: " + source).AsFatalServicingOrchestrationException<ArgumentException>();
          filter.Add(RegistryUtility.FromString<T>(source));
        }
      }
      return filter;
    }

    public bool HasFilter(string token) => !string.IsNullOrEmpty(this[token]);

    public void AppendFilter<T>(string token, IEnumerable<T> additionalItems)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(token, nameof (token));
      IEnumerable<T> source = this.GetFilter<T>(token).Union<T>(additionalItems.AsEmptyIfNull<T>());
      string str = string.Join(','.ToString(), source.Select<T, string>((Func<T, string>) (x => RegistryUtility.ToString<T>(x))));
      this[token] = str;
    }

    public IDictionary<string, string> GetAllFilterData()
    {
      Dictionary<string, string> allFilterData = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (KeyValuePair<string, string> token in (IEnumerable<KeyValuePair<string, string>>) this.m_servicingContext.Tokens)
      {
        if (token.Key.StartsWith("ServicingContextCopyFilters.", StringComparison.OrdinalIgnoreCase))
          allFilterData.Add(token.Key.Substring("ServicingContextCopyFilters.".Length), token.Value);
      }
      this.m_servicingContext.LogInfo(string.Format("Found {0} tokens under {1}", (object) allFilterData.Count, (object) "ServicingContextCopyFilters."));
      return (IDictionary<string, string>) allFilterData;
    }

    public void FinalizeFilters(string rootRegistryKey)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(rootRegistryKey, nameof (rootRegistryKey));
      string str = rootRegistryKey;
      if (!str.EndsWith("/"))
        str += "/";
      IVssRequestContext deploymentRequestContext = this.m_servicingContext.DeploymentRequestContext;
      IVssRegistryService service = deploymentRequestContext.GetService<IVssRegistryService>();
      string registryPathPattern = str + "**";
      RegistryEntryCollection registryEntryCollection = service.ReadEntries(deploymentRequestContext, new RegistryQuery(registryPathPattern));
      this.m_servicingContext.LogInfo(string.Format("Found {0} existing registryKeys", (object) registryEntryCollection.Count));
      string path1 = str + "AllCopyFiltersLocked";
      if (registryEntryCollection.ContainsPath(path1))
      {
        this.m_servicingContext.LogInfo("Copy Parameters have already been locked, will reuse previous values");
        this.RemoveAllFiltersFromServicingTokens();
        foreach (RegistryEntry registryEntry in registryEntryCollection)
        {
          this.m_servicingContext.LogInfo("Adding servicing token " + registryEntry.Name + "=" + registryEntry.Value);
          this.m_servicingContext.Tokens[registryEntry.Name] = registryEntry.Value;
        }
      }
      else
      {
        this.m_servicingContext.LogInfo("Copy Parameters have not been locked yet, locking now");
        List<RegistryItem> items = new List<RegistryItem>();
        IDictionary<string, string> allFilterData = this.GetAllFilterData();
        foreach (string key in (IEnumerable<string>) allFilterData.Keys)
        {
          string path2 = str + "ServicingContextCopyFilters." + key;
          this.m_servicingContext.LogInfo("Recording " + path2 + " as " + allFilterData[key]);
          items.Add(new RegistryItem(path2, allFilterData[key]));
        }
        items.Add(new RegistryItem(path1, bool.TrueString));
        service.Write(deploymentRequestContext, (IEnumerable<RegistryItem>) items);
      }
    }

    public static bool IsAllowedCharacter(char c) => c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z' || c >= '0' && c <= '9' || c == '.' || c == '_' || c == '/' || c == ':';

    private void RemoveAllFiltersFromServicingTokens() => this.m_servicingContext.Tokens.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (kvp => kvp.Key.StartsWith("ServicingContextCopyFilters.", StringComparison.OrdinalIgnoreCase))).ToList<KeyValuePair<string, string>>().ForEach((Action<KeyValuePair<string, string>>) (token =>
    {
      this.m_servicingContext.LogInfo("Clearing the value for servicing token " + token.Key + "=" + token.Value);
      this.m_servicingContext.Tokens[token.Key] = string.Empty;
    }));
  }
}
