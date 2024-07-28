// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ComponentFactory
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public sealed class ComponentFactory
  {
    private readonly IComponentCreator[] m_componentCreators;
    private readonly string m_serviceName;
    private readonly string m_dataspaceCategory;
    private readonly int m_minDataspaceCategoryVersion;

    public ComponentFactory(IComponentCreator[] componentCreators, string serviceName)
      : this(componentCreators, serviceName, (string) null)
    {
    }

    public ComponentFactory(
      IComponentCreator[] componentCreators,
      string serviceName,
      string dataspaceCategory,
      int minDataspaceCategoryVersion = 0)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) componentCreators, nameof (componentCreators));
      ArgumentUtility.CheckStringForNullOrEmpty(serviceName, nameof (serviceName));
      switch (dataspaceCategory)
      {
        case "Default":
          throw new ArgumentException("You may not call this overload with DataspaceCategories.Default.");
        case "":
          throw new ArgumentException(dataspaceCategory + " cannot be an empty string.");
        default:
          if (minDataspaceCategoryVersion > 0 && dataspaceCategory == null)
            throw new ArgumentException(string.Format("{0} can be greater than zero only when dataspaceCategory is specified.", (object) minDataspaceCategoryVersion));
          this.m_serviceName = serviceName;
          this.m_dataspaceCategory = dataspaceCategory;
          this.m_minDataspaceCategoryVersion = minDataspaceCategoryVersion;
          this.m_componentCreators = componentCreators;
          if (this.m_componentCreators.Length <= 1)
            break;
          Array.Sort<IComponentCreator>(this.m_componentCreators, (Comparison<IComponentCreator>) ((f1, f2) => f1.ServiceVersion.CompareTo(f2.ServiceVersion)));
          break;
      }
    }

    public string ServiceName => this.m_serviceName;

    public string DataspaceCategory => this.m_dataspaceCategory;

    public int MinDataspaceCategoryVersion => this.m_minDataspaceCategoryVersion;

    public IComponentCreator TransitionCreator => ((IEnumerable<IComponentCreator>) this.m_componentCreators).FirstOrDefault<IComponentCreator>((Func<IComponentCreator, bool>) (creator => creator.IsTransitionCreator));

    public IComponentCreator GetLastComponentCreator() => ((IEnumerable<IComponentCreator>) this.m_componentCreators).LastOrDefault<IComponentCreator>();

    public IComponentCreator GetComponentCreator(int version, int minVersion)
    {
      for (int index = this.m_componentCreators.Length - 1; index >= 0; --index)
      {
        IComponentCreator componentCreator = this.m_componentCreators[index];
        int serviceVersion = componentCreator.ServiceVersion;
        if (serviceVersion <= version)
        {
          if (serviceVersion >= minVersion)
            return componentCreator;
          break;
        }
      }
      return (IComponentCreator) null;
    }
  }
}
