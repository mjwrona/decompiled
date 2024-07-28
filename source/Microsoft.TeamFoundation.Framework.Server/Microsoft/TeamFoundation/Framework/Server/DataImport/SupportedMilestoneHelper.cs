// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DataImport.SupportedMilestoneHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server.DataImport
{
  public class SupportedMilestoneHelper : SupportedValueHelper
  {
    private readonly string[] m_restrictedMilestones;
    private static readonly IReadOnlyDictionary<string, string> s_friendlyMilestoneNames = (IReadOnlyDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "Dev14.M102",
        Milestone.Dev14M102Desc()
      },
      {
        "Dev15.M105.5",
        Milestone.Dev15M105_5Desc()
      },
      {
        "Dev15.M112.4",
        Milestone.Dev15M112_4Desc()
      },
      {
        "Dev15.M112.5",
        Milestone.Dev15M112_4Desc()
      },
      {
        "Dev15.M117.5",
        Milestone.Dev15M117_5Desc()
      },
      {
        "Dev15.M125.1",
        Milestone.Dev15M125Desc()
      },
      {
        "Dev16.M122.5",
        Milestone.Dev16M122_5Desc()
      },
      {
        "Dev16.M122.7",
        Milestone.Dev16M122_7Desc()
      },
      {
        "Dev16.M131.6",
        Milestone.Dev16Update2()
      },
      {
        "Dev16.M131.9",
        Milestone.Dev16Update3()
      },
      {
        "Dev16.M131.10",
        "TFS 2018 Update 3.1"
      },
      {
        "Dev16.M131.11",
        "TFS 2018 Update 3.2"
      },
      {
        "Dev17.M143.4",
        Milestone.DevOpsServer2019()
      },
      {
        "Dev17.M143.5",
        Milestone.DevOpsServer2019_0_1RC()
      },
      {
        "Dev17.M143.6",
        Milestone.DevOpsServer2019_0_1()
      },
      {
        "Dev17.M153.1",
        Milestone.DevOpsServer2019Update1RC2()
      },
      {
        "Dev17.M153.3",
        Milestone.DevOpsServer2019Update1()
      },
      {
        "Dev17.M153.5",
        Milestone.DevOpsServer2019Update1_1()
      },
      {
        "Dev17.M153.6",
        Milestone.DevOpsServer2019Update1_2()
      },
      {
        "Dev18.M170.6",
        Milestone.Dev18M170_6Desc()
      },
      {
        "Dev18.M170.8",
        Milestone.Dev18M170_8Desc()
      },
      {
        "Dev18.M170.9",
        Milestone.DevOpsServer2020RTM_2()
      },
      {
        "Dev18.M181.6",
        Milestone.Dev18M181_6Desc()
      },
      {
        "Dev18.M181.8",
        Milestone.DevOpsServer2020Update1_1()
      },
      {
        "Dev18.M181.9",
        Milestone.DevOpsServer2020Update1_2()
      },
      {
        "Dev19.M205.3",
        Milestone.DevOpsServer2022RTW()
      },
      {
        "Dev19.M205.5",
        Milestone.DevOpsServer2022_0_1RTW()
      },
      {
        "Dev19.M225.3",
        Milestone.DevOpsServer2022_1RTW()
      }
    };

    public override string DefaultValues => (string) null;

    public override RegistryQuery RegistryQuery => new RegistryQuery("/Configuration/DataImport/SupportedMilestones");

    public override RegistryQuery? AdditionalRegistryQuery => new RegistryQuery?(new RegistryQuery("/Configuration/DataImport/StretchMilestones"));

    protected override IEnumerable<string> RestrictedValues => (IEnumerable<string>) this.m_restrictedMilestones;

    public SupportedMilestoneHelper(IVssRequestContext requestContext, string restrictedMilestones)
      : base(requestContext)
    {
      if (string.IsNullOrEmpty(restrictedMilestones))
        this.m_restrictedMilestones = Array.Empty<string>();
      else
        this.m_restrictedMilestones = restrictedMilestones.Split(new string[1]
        {
          this.Separator
        }, StringSplitOptions.RemoveEmptyEntries);
    }

    public override void Throw(string value) => throw this.CreateException(value);

    public ServiceLevel EffectiveMilestone(string milestone)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(milestone, nameof (milestone));
      ServiceLevel serviceLevel = new ServiceLevel(milestone);
      this.CheckIsSupported(milestone);
      if (!this.IsAdditionalValue(milestone))
        return serviceLevel;
      ServiceLevel serviceLevel1 = this.SupportedValues.Select<string, ServiceLevel>((Func<string, ServiceLevel>) (x => new ServiceLevel(x))).OrderByDescending<ServiceLevel, ServiceLevel>((Func<ServiceLevel, ServiceLevel>) (x => x)).FirstOrDefault<ServiceLevel>((Func<ServiceLevel, bool>) (x => serviceLevel >= x));
      if (serviceLevel1 == (ServiceLevel) null)
      {
        Log(string.Format("No candidate miles stone was found for {0} given {1}", (object) serviceLevel, (object) string.Join(this.Separator, this.SupportedValues)));
        throw this.CreateException(milestone);
      }
      Log(string.Format("Request Milestone {0} is greater than candidate milestone {1}, so the candidate will be used", (object) serviceLevel, (object) serviceLevel1));
      return serviceLevel1;

      void Log(string message) => TeamFoundationTracingService.TraceRawAlwaysOn(15080312, TraceLevel.Info, "DataImport", this.GetType().Name, message);
    }

    internal Exception CreateException(string milestone)
    {
      Guid serviceType = this.m_requestContext.ServiceInstanceType();
      string str = ServiceTypeMapper.GetServiceTypeMetadataByServiceType(serviceType)?.Name ?? serviceType.ToString();
      ServiceLevel serviceLevel = new ServiceLevel(milestone);
      bool flag = this.RestrictedValues.Any<string>((Func<string, bool>) (x => serviceLevel == new ServiceLevel(x)));
      return (Exception) new MilestoneNotSupportedException(DataImportResources.UnsupportedCollectionMilestoneForService((object) str, (object) SupportedMilestoneHelper.GetFriendlyMilestoneName(milestone), (object) flag.ToString()));
    }

    public static string GetFriendlyMilestoneName(string milestone)
    {
      string str;
      return !SupportedMilestoneHelper.s_friendlyMilestoneNames.TryGetValue(milestone, out str) ? milestone : str + " (" + milestone + ")";
    }
  }
}
