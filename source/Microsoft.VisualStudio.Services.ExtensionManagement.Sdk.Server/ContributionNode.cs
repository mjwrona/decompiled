// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.ContributionNode
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  public class ContributionNode
  {
    private const string c_contributedProperties = "contributedProperties";
    private List<ContributionNode> m_propertyProviders;
    private List<ContributionNode> m_parents;
    private List<ContributionNode> m_children;
    private Contribution m_contribution;

    public ContributionNode(Contribution contribution) => this.m_contribution = contribution;

    public Contribution Contribution => this.m_contribution;

    public IEnumerable<ContributionNode> Children => (IEnumerable<ContributionNode>) this.m_children;

    public string Id => this.m_contribution.Id;

    public IEnumerable<ContributionNode> Parents => (IEnumerable<ContributionNode>) this.m_parents;

    public override string ToString() => this.m_contribution.ToString();

    public T GetProperty<T>(IVssRequestContext requestContext, string propertyName, T defaultValue = null)
    {
      T propertyValue;
      if (!this.TryGetProperty<T>(requestContext, propertyName, out propertyValue))
        propertyValue = defaultValue;
      return propertyValue;
    }

    public bool IsRequestSensitive(IVssRequestContext requestContext) => this.m_propertyProviders != null;

    public bool TryGetProperty<T>(
      IVssRequestContext requestContext,
      string propertyName,
      out T propertyValue)
    {
      bool property1 = false;
      propertyValue = default (T);
      if (this.m_propertyProviders != null)
      {
        Dictionary<string, Dictionary<string, object>> dictionary1;
        if (!requestContext.TryGetItem<Dictionary<string, Dictionary<string, object>>>("contributedProperties", out dictionary1))
        {
          dictionary1 = new Dictionary<string, Dictionary<string, object>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          requestContext.Items["contributedProperties"] = (object) dictionary1;
        }
        Dictionary<string, object> dictionary2;
        if (!dictionary1.TryGetValue(this.Id, out dictionary2))
        {
          dictionary2 = new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          dictionary1[this.Id] = dictionary2;
          foreach (ContributionNode propertyProvider in this.m_propertyProviders)
          {
            IDictionary<string, object> dictionary3 = (IDictionary<string, object>) null;
            string property2 = propertyProvider.Contribution.GetProperty<string>("type");
            if (string.Equals(property2, "static", StringComparison.OrdinalIgnoreCase))
              dictionary3 = propertyProvider.Contribution.GetProperty<IDictionary<string, object>>("newProperties");
            else if (string.Equals(property2, "dynamic", StringComparison.OrdinalIgnoreCase))
              dictionary3 = requestContext.GetService<IContributionPropertyProviderService>().GetProperties(requestContext, propertyProvider, this).Properties;
            if (dictionary3 != null)
            {
              foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>) dictionary3)
              {
                if (!dictionary2.ContainsKey(keyValuePair.Key))
                  dictionary2[keyValuePair.Key] = keyValuePair.Value;
              }
            }
          }
        }
        object obj;
        if (dictionary2.TryGetValue(propertyName, out obj))
        {
          try
          {
            propertyValue = obj is JToken ? ((JToken) obj).ToObject<T>() : (T) Convert.ChangeType(obj, typeof (T));
            property1 = true;
          }
          catch (Exception ex)
          {
          }
        }
      }
      if (!property1 && this.Contribution.Properties != null)
      {
        JToken jtoken = this.Contribution.Properties.SelectToken(propertyName);
        if (jtoken != null)
        {
          try
          {
            propertyValue = jtoken.ToObject<T>();
            property1 = true;
          }
          catch (Exception ex)
          {
          }
        }
      }
      return property1;
    }

    internal void AddParent(ContributionNode parentNode)
    {
      if (this.m_parents == null)
        this.m_parents = new List<ContributionNode>();
      if (this.m_parents.Contains(parentNode))
        return;
      this.m_parents.Add(parentNode);
    }

    internal void AddChild(ContributionNode childNode)
    {
      if (this.m_children == null)
        this.m_children = new List<ContributionNode>();
      this.m_children.Add(childNode);
      if (!"ms.vss-web.property-provider".Equals(childNode.Contribution.Type, StringComparison.OrdinalIgnoreCase) || !string.Equals(new ContributionIdentifier(this.Contribution.Id).PublisherName, new ContributionIdentifier(childNode.Contribution.Id).PublisherName, StringComparison.OrdinalIgnoreCase))
        return;
      if (this.m_propertyProviders == null)
        this.m_propertyProviders = new List<ContributionNode>();
      this.m_propertyProviders.Add(childNode);
    }
  }
}
