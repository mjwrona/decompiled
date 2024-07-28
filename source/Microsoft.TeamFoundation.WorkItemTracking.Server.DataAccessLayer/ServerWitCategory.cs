// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ServerWitCategory
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Provision;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class ServerWitCategory
  {
    private Dictionary<int, LegacyWorkItemType> m_typesMap;

    public ServerWitCategory(
      IVssRequestContext requestContext,
      int projectId,
      XmlElement category,
      bool skipWorkItemTypeValidation = false)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (category == null)
        throw new ArgumentNullException(nameof (category));
      this.m_typesMap = new Dictionary<int, LegacyWorkItemType>(category.ChildNodes.Count);
      this.ReferenceName = category.GetAttribute(ProvisionAttributes.ReferenceName);
      this.Name = category.GetAttribute(ProvisionAttributes.FriendlyName);
      this.ProjectId = projectId;
      Dictionary<string, LegacyWorkItemType> dictionary1 = new Dictionary<string, LegacyWorkItemType>((IEqualityComparer<string>) requestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(requestContext).ServerStringComparer);
      LegacyWorkItemTypeDictionary service = requestContext.GetService<LegacyWorkItemTypeDictionary>();
      Dictionary<string, LegacyWorkItemType> dictionary2 = (Dictionary<string, LegacyWorkItemType>) null;
      if (!skipWorkItemTypeValidation)
        dictionary2 = service.GetWorkItemTypesForProject(requestContext, projectId).ToDictionary<LegacyWorkItemType, string>((Func<LegacyWorkItemType, string>) (type => type.Name), (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
      for (int i = 0; i < category.ChildNodes.Count; ++i)
      {
        XmlElement childNode = (XmlElement) category.ChildNodes[i];
        string key = childNode.GetAttribute(ProvisionAttributes.WorkItemTypeName);
        LegacyWorkItemType legacyWorkItemType = new LegacyWorkItemType();
        legacyWorkItemType.Name = key;
        if (skipWorkItemTypeValidation)
          legacyWorkItemType.Id = i;
        else
          key = dictionary2.TryGetValue(key, out legacyWorkItemType) ? legacyWorkItemType.Name : throw new LegacyValidationException(InternalsResourceStrings.Format("ErrorUnknownTypeInCategory", (object) this.Name, (object) key));
        if (dictionary1.ContainsKey(key))
          throw new LegacyValidationException(InternalsResourceStrings.Format("ErrorDuplicateTypeInCategory", (object) this.Name, (object) key));
        this.m_typesMap.Add(legacyWorkItemType.Id, legacyWorkItemType);
        dictionary1.Add(key, legacyWorkItemType);
        if (childNode.LocalName == ProvisionTags.DefaultWorkItemTypeReference)
          this.DefaultWorkItemType = legacyWorkItemType;
      }
    }

    public string ReferenceName { get; private set; }

    public string Name { get; private set; }

    public LegacyWorkItemType DefaultWorkItemType { get; private set; }

    public int ProjectId { get; private set; }

    public IEnumerable<LegacyWorkItemType> WorkItemTypes => (IEnumerable<LegacyWorkItemType>) this.m_typesMap.Values;

    public bool Contains(LegacyWorkItemType wType)
    {
      if (wType == null)
        throw new ArgumentNullException(nameof (wType));
      return this.m_typesMap.ContainsKey(wType.Id);
    }
  }
}
