// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.RegionProximityUtil
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.Azure.Documents
{
  internal static class RegionProximityUtil
  {
    internal static Dictionary<string, Dictionary<string, long>> SourceRegionToTargetRegionsRTTInMs = new Dictionary<string, Dictionary<string, long>>()
    {
      {
        "Australia Central",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            0L
          },
          {
            "Australia Central 2",
            (long) byte.MaxValue
          },
          {
            "Australia East",
            (long) byte.MaxValue
          },
          {
            "Australia Southeast",
            (long) byte.MaxValue
          },
          {
            "Brazil South",
            (long) byte.MaxValue
          },
          {
            "Canada Central",
            (long) byte.MaxValue
          },
          {
            "Canada East",
            (long) byte.MaxValue
          },
          {
            "Central India",
            (long) byte.MaxValue
          },
          {
            "Central US",
            (long) byte.MaxValue
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            (long) byte.MaxValue
          },
          {
            "East US",
            (long) byte.MaxValue
          },
          {
            "East US 2",
            (long) byte.MaxValue
          },
          {
            "France Central",
            (long) byte.MaxValue
          },
          {
            "France South",
            (long) byte.MaxValue
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            (long) byte.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            (long) byte.MaxValue
          },
          {
            "Japan East",
            (long) byte.MaxValue
          },
          {
            "Japan West",
            (long) byte.MaxValue
          },
          {
            "Korea Central",
            (long) byte.MaxValue
          },
          {
            "Korea South",
            (long) byte.MaxValue
          },
          {
            "North Central US",
            (long) byte.MaxValue
          },
          {
            "North Europe",
            (long) byte.MaxValue
          },
          {
            "Norway East",
            (long) byte.MaxValue
          },
          {
            "Norway West",
            (long) byte.MaxValue
          },
          {
            "South Africa North",
            (long) byte.MaxValue
          },
          {
            "South Africa West",
            (long) byte.MaxValue
          },
          {
            "South Central US",
            (long) byte.MaxValue
          },
          {
            "Southeast Asia",
            (long) byte.MaxValue
          },
          {
            "South India",
            (long) byte.MaxValue
          },
          {
            "Switzerland North",
            (long) byte.MaxValue
          },
          {
            "Switzerland West",
            (long) byte.MaxValue
          },
          {
            "UAE Central",
            (long) byte.MaxValue
          },
          {
            "UAE North",
            (long) byte.MaxValue
          },
          {
            "UK South",
            (long) byte.MaxValue
          },
          {
            "UK West",
            (long) byte.MaxValue
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            (long) byte.MaxValue
          },
          {
            "West Europe",
            (long) byte.MaxValue
          },
          {
            "West India",
            (long) byte.MaxValue
          },
          {
            "West US",
            (long) byte.MaxValue
          },
          {
            "West US 2",
            (long) byte.MaxValue
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "Australia Central 2",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            (long) byte.MaxValue
          },
          {
            "Australia Central 2",
            0L
          },
          {
            "Australia East",
            (long) byte.MaxValue
          },
          {
            "Australia Southeast",
            (long) byte.MaxValue
          },
          {
            "Brazil South",
            (long) byte.MaxValue
          },
          {
            "Canada Central",
            (long) byte.MaxValue
          },
          {
            "Canada East",
            (long) byte.MaxValue
          },
          {
            "Central India",
            (long) byte.MaxValue
          },
          {
            "Central US",
            (long) byte.MaxValue
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            (long) byte.MaxValue
          },
          {
            "East US",
            (long) byte.MaxValue
          },
          {
            "East US 2",
            (long) byte.MaxValue
          },
          {
            "France Central",
            (long) byte.MaxValue
          },
          {
            "France South",
            (long) byte.MaxValue
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            (long) byte.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            (long) byte.MaxValue
          },
          {
            "Japan East",
            (long) byte.MaxValue
          },
          {
            "Japan West",
            (long) byte.MaxValue
          },
          {
            "Korea Central",
            (long) byte.MaxValue
          },
          {
            "Korea South",
            (long) byte.MaxValue
          },
          {
            "North Central US",
            (long) byte.MaxValue
          },
          {
            "North Europe",
            (long) byte.MaxValue
          },
          {
            "Norway East",
            (long) byte.MaxValue
          },
          {
            "Norway West",
            (long) byte.MaxValue
          },
          {
            "South Africa North",
            (long) byte.MaxValue
          },
          {
            "South Africa West",
            (long) byte.MaxValue
          },
          {
            "South Central US",
            (long) byte.MaxValue
          },
          {
            "Southeast Asia",
            (long) byte.MaxValue
          },
          {
            "South India",
            (long) byte.MaxValue
          },
          {
            "Switzerland North",
            (long) byte.MaxValue
          },
          {
            "Switzerland West",
            (long) byte.MaxValue
          },
          {
            "UAE Central",
            (long) byte.MaxValue
          },
          {
            "UAE North",
            (long) byte.MaxValue
          },
          {
            "UK South",
            (long) byte.MaxValue
          },
          {
            "UK West",
            (long) byte.MaxValue
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            (long) byte.MaxValue
          },
          {
            "West Europe",
            (long) byte.MaxValue
          },
          {
            "West India",
            (long) byte.MaxValue
          },
          {
            "West US",
            (long) byte.MaxValue
          },
          {
            "West US 2",
            (long) byte.MaxValue
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "Australia East",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            (long) byte.MaxValue
          },
          {
            "Australia Central 2",
            (long) byte.MaxValue
          },
          {
            "Australia East",
            0L
          },
          {
            "Australia Southeast",
            10L
          },
          {
            "Brazil South",
            305L
          },
          {
            "Canada Central",
            215L
          },
          {
            "Canada East",
            225L
          },
          {
            "Central India",
            225L
          },
          {
            "Central US",
            185L
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            155L
          },
          {
            "East US",
            190L
          },
          {
            "East US 2",
            190L
          },
          {
            "France Central",
            280L
          },
          {
            "France South",
            270L
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            (long) byte.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            (long) byte.MaxValue
          },
          {
            "Japan East",
            105L
          },
          {
            "Japan West",
            110L
          },
          {
            "Korea Central",
            140L
          },
          {
            "Korea South",
            145L
          },
          {
            "North Central US",
            200L
          },
          {
            "North Europe",
            285L
          },
          {
            "Norway East",
            (long) byte.MaxValue
          },
          {
            "Norway West",
            (long) byte.MaxValue
          },
          {
            "South Africa North",
            (long) byte.MaxValue
          },
          {
            "South Africa West",
            (long) byte.MaxValue
          },
          {
            "South Central US",
            165L
          },
          {
            "Southeast Asia",
            170L
          },
          {
            "South India",
            205L
          },
          {
            "Switzerland North",
            (long) byte.MaxValue
          },
          {
            "Switzerland West",
            (long) byte.MaxValue
          },
          {
            "UAE Central",
            (long) byte.MaxValue
          },
          {
            "UAE North",
            (long) byte.MaxValue
          },
          {
            "UK South",
            265L
          },
          {
            "UK West",
            270L
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            175L
          },
          {
            "West Europe",
            275L
          },
          {
            "West India",
            230L
          },
          {
            "West US",
            150L
          },
          {
            "West US 2",
            170L
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "Australia Southeast",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            (long) byte.MaxValue
          },
          {
            "Australia Central 2",
            (long) byte.MaxValue
          },
          {
            "Australia East",
            10L
          },
          {
            "Australia Southeast",
            0L
          },
          {
            "Brazil South",
            310L
          },
          {
            "Canada Central",
            225L
          },
          {
            "Canada East",
            235L
          },
          {
            "Central India",
            235L
          },
          {
            "Central US",
            200L
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            165L
          },
          {
            "East US",
            205L
          },
          {
            "East US 2",
            200L
          },
          {
            "France Central",
            290L
          },
          {
            "France South",
            280L
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            (long) byte.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            (long) byte.MaxValue
          },
          {
            "Japan East",
            115L
          },
          {
            "Japan West",
            125L
          },
          {
            "Korea Central",
            150L
          },
          {
            "Korea South",
            155L
          },
          {
            "North Central US",
            210L
          },
          {
            "North Europe",
            305L
          },
          {
            "Norway East",
            (long) byte.MaxValue
          },
          {
            "Norway West",
            (long) byte.MaxValue
          },
          {
            "South Africa North",
            (long) byte.MaxValue
          },
          {
            "South Africa West",
            (long) byte.MaxValue
          },
          {
            "South Central US",
            175L
          },
          {
            "Southeast Asia",
            185L
          },
          {
            "South India",
            215L
          },
          {
            "Switzerland North",
            (long) byte.MaxValue
          },
          {
            "Switzerland West",
            (long) byte.MaxValue
          },
          {
            "UAE Central",
            (long) byte.MaxValue
          },
          {
            "UAE North",
            (long) byte.MaxValue
          },
          {
            "UK South",
            275L
          },
          {
            "UK West",
            280L
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            185L
          },
          {
            "West Europe",
            285L
          },
          {
            "West India",
            235L
          },
          {
            "West US",
            160L
          },
          {
            "West US 2",
            180L
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "Brazil South",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            (long) byte.MaxValue
          },
          {
            "Australia Central 2",
            (long) byte.MaxValue
          },
          {
            "Australia East",
            300L
          },
          {
            "Australia Southeast",
            310L
          },
          {
            "Brazil South",
            0L
          },
          {
            "Canada Central",
            130L
          },
          {
            "Canada East",
            140L
          },
          {
            "Central India",
            295L
          },
          {
            "Central US",
            150L
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            310L
          },
          {
            "East US",
            120L
          },
          {
            "East US 2",
            130L
          },
          {
            "France Central",
            200L
          },
          {
            "France South",
            185L
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            (long) byte.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            (long) byte.MaxValue
          },
          {
            "Japan East",
            260L
          },
          {
            "Japan West",
            265L
          },
          {
            "Korea Central",
            295L
          },
          {
            "Korea South",
            300L
          },
          {
            "North Central US",
            135L
          },
          {
            "North Europe",
            190L
          },
          {
            "Norway East",
            (long) byte.MaxValue
          },
          {
            "Norway West",
            (long) byte.MaxValue
          },
          {
            "South Africa North",
            (long) byte.MaxValue
          },
          {
            "South Africa West",
            (long) byte.MaxValue
          },
          {
            "South Central US",
            140L
          },
          {
            "Southeast Asia",
            325L
          },
          {
            "South India",
            320L
          },
          {
            "Switzerland North",
            (long) byte.MaxValue
          },
          {
            "Switzerland West",
            (long) byte.MaxValue
          },
          {
            "UAE Central",
            (long) byte.MaxValue
          },
          {
            "UAE North",
            (long) byte.MaxValue
          },
          {
            "UK South",
            180L
          },
          {
            "UK West",
            185L
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            165L
          },
          {
            "West Europe",
            190L
          },
          {
            "West India",
            295L
          },
          {
            "West US",
            170L
          },
          {
            "West US 2",
            185L
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "Canada Central",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            (long) byte.MaxValue
          },
          {
            "Australia Central 2",
            (long) byte.MaxValue
          },
          {
            "Australia East",
            215L
          },
          {
            "Australia Southeast",
            225L
          },
          {
            "Brazil South",
            130L
          },
          {
            "Canada Central",
            0L
          },
          {
            "Canada East",
            10L
          },
          {
            "Central India",
            215L
          },
          {
            "Central US",
            25L
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            210L
          },
          {
            "East US",
            25L
          },
          {
            "East US 2",
            30L
          },
          {
            "France Central",
            100L
          },
          {
            "France South",
            110L
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            (long) byte.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            (long) byte.MaxValue
          },
          {
            "Japan East",
            170L
          },
          {
            "Japan West",
            180L
          },
          {
            "Korea Central",
            200L
          },
          {
            "Korea South",
            210L
          },
          {
            "North Central US",
            15L
          },
          {
            "North Europe",
            75L
          },
          {
            "Norway East",
            (long) byte.MaxValue
          },
          {
            "Norway West",
            (long) byte.MaxValue
          },
          {
            "South Africa North",
            (long) byte.MaxValue
          },
          {
            "South Africa West",
            (long) byte.MaxValue
          },
          {
            "South Central US",
            50L
          },
          {
            "Southeast Asia",
            235L
          },
          {
            "South India",
            230L
          },
          {
            "Switzerland North",
            (long) byte.MaxValue
          },
          {
            "Switzerland West",
            (long) byte.MaxValue
          },
          {
            "UAE Central",
            (long) byte.MaxValue
          },
          {
            "UAE North",
            (long) byte.MaxValue
          },
          {
            "UK South",
            85L
          },
          {
            "UK West",
            90L
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            40L
          },
          {
            "West Europe",
            95L
          },
          {
            "West India",
            200L
          },
          {
            "West US",
            65L
          },
          {
            "West US 2",
            60L
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "Canada East",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            (long) byte.MaxValue
          },
          {
            "Australia Central 2",
            (long) byte.MaxValue
          },
          {
            "Australia East",
            225L
          },
          {
            "Australia Southeast",
            235L
          },
          {
            "Brazil South",
            140L
          },
          {
            "Canada Central",
            10L
          },
          {
            "Canada East",
            0L
          },
          {
            "Central India",
            225L
          },
          {
            "Central US",
            40L
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            225L
          },
          {
            "East US",
            35L
          },
          {
            "East US 2",
            40L
          },
          {
            "France Central",
            110L
          },
          {
            "France South",
            120L
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            (long) byte.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            (long) byte.MaxValue
          },
          {
            "Japan East",
            180L
          },
          {
            "Japan West",
            190L
          },
          {
            "Korea Central",
            215L
          },
          {
            "Korea South",
            220L
          },
          {
            "North Central US",
            25L
          },
          {
            "North Europe",
            85L
          },
          {
            "Norway East",
            (long) byte.MaxValue
          },
          {
            "Norway West",
            (long) byte.MaxValue
          },
          {
            "South Africa North",
            (long) byte.MaxValue
          },
          {
            "South Africa West",
            (long) byte.MaxValue
          },
          {
            "South Central US",
            60L
          },
          {
            "Southeast Asia",
            250L
          },
          {
            "South India",
            240L
          },
          {
            "Switzerland North",
            (long) byte.MaxValue
          },
          {
            "Switzerland West",
            (long) byte.MaxValue
          },
          {
            "UAE Central",
            (long) byte.MaxValue
          },
          {
            "UAE North",
            (long) byte.MaxValue
          },
          {
            "UK South",
            95L
          },
          {
            "UK West",
            100L
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            55L
          },
          {
            "West Europe",
            110L
          },
          {
            "West India",
            220L
          },
          {
            "West US",
            75L
          },
          {
            "West US 2",
            75L
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "Central India",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            (long) byte.MaxValue
          },
          {
            "Australia Central 2",
            (long) byte.MaxValue
          },
          {
            "Australia East",
            225L
          },
          {
            "Australia Southeast",
            235L
          },
          {
            "Brazil South",
            295L
          },
          {
            "Canada Central",
            215L
          },
          {
            "Canada East",
            225L
          },
          {
            "Central India",
            0L
          },
          {
            "Central US",
            240L
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            90L
          },
          {
            "East US",
            190L
          },
          {
            "East US 2",
            195L
          },
          {
            "France Central",
            120L
          },
          {
            "France South",
            110L
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            (long) byte.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            (long) byte.MaxValue
          },
          {
            "Japan East",
            120L
          },
          {
            "Japan West",
            125L
          },
          {
            "Korea Central",
            115L
          },
          {
            "Korea South",
            125L
          },
          {
            "North Central US",
            210L
          },
          {
            "North Europe",
            130L
          },
          {
            "Norway East",
            (long) byte.MaxValue
          },
          {
            "Norway West",
            (long) byte.MaxValue
          },
          {
            "South Africa North",
            (long) byte.MaxValue
          },
          {
            "South Africa West",
            (long) byte.MaxValue
          },
          {
            "South Central US",
            245L
          },
          {
            "Southeast Asia",
            60L
          },
          {
            "South India",
            40L
          },
          {
            "Switzerland North",
            (long) byte.MaxValue
          },
          {
            "Switzerland West",
            (long) byte.MaxValue
          },
          {
            "UAE Central",
            (long) byte.MaxValue
          },
          {
            "UAE North",
            (long) byte.MaxValue
          },
          {
            "UK South",
            125L
          },
          {
            "UK West",
            130L
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            (long) byte.MaxValue
          },
          {
            "West Europe",
            130L
          },
          {
            "West India",
            5L
          },
          {
            "West US",
            230L
          },
          {
            "West US 2",
            235L
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "Central US",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            (long) byte.MaxValue
          },
          {
            "Australia Central 2",
            (long) byte.MaxValue
          },
          {
            "Australia East",
            185L
          },
          {
            "Australia Southeast",
            195L
          },
          {
            "Brazil South",
            150L
          },
          {
            "Canada Central",
            25L
          },
          {
            "Canada East",
            40L
          },
          {
            "Central India",
            240L
          },
          {
            "Central US",
            0L
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            190L
          },
          {
            "East US",
            40L
          },
          {
            "East US 2",
            45L
          },
          {
            "France Central",
            110L
          },
          {
            "France South",
            125L
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            (long) byte.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            (long) byte.MaxValue
          },
          {
            "Japan East",
            145L
          },
          {
            "Japan West",
            150L
          },
          {
            "Korea Central",
            175L
          },
          {
            "Korea South",
            185L
          },
          {
            "North Central US",
            15L
          },
          {
            "North Europe",
            105L
          },
          {
            "Norway East",
            (long) byte.MaxValue
          },
          {
            "Norway West",
            (long) byte.MaxValue
          },
          {
            "South Africa North",
            (long) byte.MaxValue
          },
          {
            "South Africa West",
            (long) byte.MaxValue
          },
          {
            "South Central US",
            25L
          },
          {
            "Southeast Asia",
            210L
          },
          {
            "South India",
            240L
          },
          {
            "Switzerland North",
            (long) byte.MaxValue
          },
          {
            "Switzerland West",
            (long) byte.MaxValue
          },
          {
            "UAE Central",
            (long) byte.MaxValue
          },
          {
            "UAE North",
            (long) byte.MaxValue
          },
          {
            "UK South",
            105L
          },
          {
            "UK West",
            110L
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            15L
          },
          {
            "West Europe",
            125L
          },
          {
            "West India",
            230L
          },
          {
            "West US",
            45L
          },
          {
            "West US 2",
            40L
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "China East",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            long.MaxValue
          },
          {
            "Australia Central 2",
            long.MaxValue
          },
          {
            "Australia East",
            long.MaxValue
          },
          {
            "Australia Southeast",
            long.MaxValue
          },
          {
            "Brazil South",
            long.MaxValue
          },
          {
            "Canada Central",
            long.MaxValue
          },
          {
            "Canada East",
            long.MaxValue
          },
          {
            "Central India",
            long.MaxValue
          },
          {
            "Central US",
            long.MaxValue
          },
          {
            "China East",
            0L
          },
          {
            "China East 2",
            (long) byte.MaxValue
          },
          {
            "China North",
            35L
          },
          {
            "China North 2",
            (long) byte.MaxValue
          },
          {
            "East Asia",
            long.MaxValue
          },
          {
            "East US",
            long.MaxValue
          },
          {
            "East US 2",
            long.MaxValue
          },
          {
            "France Central",
            long.MaxValue
          },
          {
            "France South",
            long.MaxValue
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            long.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            long.MaxValue
          },
          {
            "Japan East",
            long.MaxValue
          },
          {
            "Japan West",
            long.MaxValue
          },
          {
            "Korea Central",
            long.MaxValue
          },
          {
            "Korea South",
            long.MaxValue
          },
          {
            "North Central US",
            long.MaxValue
          },
          {
            "North Europe",
            long.MaxValue
          },
          {
            "Norway East",
            long.MaxValue
          },
          {
            "Norway West",
            long.MaxValue
          },
          {
            "South Africa North",
            long.MaxValue
          },
          {
            "South Africa West",
            long.MaxValue
          },
          {
            "South Central US",
            long.MaxValue
          },
          {
            "Southeast Asia",
            long.MaxValue
          },
          {
            "South India",
            long.MaxValue
          },
          {
            "Switzerland North",
            long.MaxValue
          },
          {
            "Switzerland West",
            long.MaxValue
          },
          {
            "UAE Central",
            long.MaxValue
          },
          {
            "UAE North",
            long.MaxValue
          },
          {
            "UK South",
            long.MaxValue
          },
          {
            "UK West",
            long.MaxValue
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            long.MaxValue
          },
          {
            "West Europe",
            long.MaxValue
          },
          {
            "West India",
            long.MaxValue
          },
          {
            "West US",
            long.MaxValue
          },
          {
            "West US 2",
            long.MaxValue
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "China East 2",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            long.MaxValue
          },
          {
            "Australia Central 2",
            long.MaxValue
          },
          {
            "Australia East",
            long.MaxValue
          },
          {
            "Australia Southeast",
            long.MaxValue
          },
          {
            "Brazil South",
            long.MaxValue
          },
          {
            "Canada Central",
            long.MaxValue
          },
          {
            "Canada East",
            long.MaxValue
          },
          {
            "Central India",
            long.MaxValue
          },
          {
            "Central US",
            long.MaxValue
          },
          {
            "China East",
            (long) byte.MaxValue
          },
          {
            "China East 2",
            0L
          },
          {
            "China North",
            2555L
          },
          {
            "China North 2",
            (long) byte.MaxValue
          },
          {
            "East Asia",
            long.MaxValue
          },
          {
            "East US",
            long.MaxValue
          },
          {
            "East US 2",
            long.MaxValue
          },
          {
            "France Central",
            long.MaxValue
          },
          {
            "France South",
            long.MaxValue
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            long.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            long.MaxValue
          },
          {
            "Japan East",
            long.MaxValue
          },
          {
            "Japan West",
            long.MaxValue
          },
          {
            "Korea Central",
            long.MaxValue
          },
          {
            "Korea South",
            long.MaxValue
          },
          {
            "North Central US",
            long.MaxValue
          },
          {
            "North Europe",
            long.MaxValue
          },
          {
            "Norway East",
            long.MaxValue
          },
          {
            "Norway West",
            long.MaxValue
          },
          {
            "South Africa North",
            long.MaxValue
          },
          {
            "South Africa West",
            long.MaxValue
          },
          {
            "South Central US",
            long.MaxValue
          },
          {
            "Southeast Asia",
            long.MaxValue
          },
          {
            "South India",
            long.MaxValue
          },
          {
            "Switzerland North",
            long.MaxValue
          },
          {
            "Switzerland West",
            long.MaxValue
          },
          {
            "UAE Central",
            long.MaxValue
          },
          {
            "UAE North",
            long.MaxValue
          },
          {
            "UK South",
            long.MaxValue
          },
          {
            "UK West",
            long.MaxValue
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            long.MaxValue
          },
          {
            "West Europe",
            long.MaxValue
          },
          {
            "West India",
            long.MaxValue
          },
          {
            "West US",
            long.MaxValue
          },
          {
            "West US 2",
            long.MaxValue
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "China North",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            long.MaxValue
          },
          {
            "Australia Central 2",
            long.MaxValue
          },
          {
            "Australia East",
            long.MaxValue
          },
          {
            "Australia Southeast",
            long.MaxValue
          },
          {
            "Brazil South",
            long.MaxValue
          },
          {
            "Canada Central",
            long.MaxValue
          },
          {
            "Canada East",
            long.MaxValue
          },
          {
            "Central India",
            long.MaxValue
          },
          {
            "Central US",
            long.MaxValue
          },
          {
            "China East",
            35L
          },
          {
            "China East 2",
            (long) byte.MaxValue
          },
          {
            "China North",
            0L
          },
          {
            "China North 2",
            (long) byte.MaxValue
          },
          {
            "East Asia",
            long.MaxValue
          },
          {
            "East US",
            long.MaxValue
          },
          {
            "East US 2",
            long.MaxValue
          },
          {
            "France Central",
            long.MaxValue
          },
          {
            "France South",
            long.MaxValue
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            long.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            long.MaxValue
          },
          {
            "Japan East",
            long.MaxValue
          },
          {
            "Japan West",
            long.MaxValue
          },
          {
            "Korea Central",
            long.MaxValue
          },
          {
            "Korea South",
            long.MaxValue
          },
          {
            "North Central US",
            long.MaxValue
          },
          {
            "North Europe",
            long.MaxValue
          },
          {
            "Norway East",
            long.MaxValue
          },
          {
            "Norway West",
            long.MaxValue
          },
          {
            "South Africa North",
            long.MaxValue
          },
          {
            "South Africa West",
            long.MaxValue
          },
          {
            "South Central US",
            long.MaxValue
          },
          {
            "Southeast Asia",
            long.MaxValue
          },
          {
            "South India",
            long.MaxValue
          },
          {
            "Switzerland North",
            long.MaxValue
          },
          {
            "Switzerland West",
            long.MaxValue
          },
          {
            "UAE Central",
            long.MaxValue
          },
          {
            "UAE North",
            long.MaxValue
          },
          {
            "UK South",
            long.MaxValue
          },
          {
            "UK West",
            long.MaxValue
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            long.MaxValue
          },
          {
            "West Europe",
            long.MaxValue
          },
          {
            "West India",
            long.MaxValue
          },
          {
            "West US",
            long.MaxValue
          },
          {
            "West US 2",
            long.MaxValue
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "China North 2",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            long.MaxValue
          },
          {
            "Australia Central 2",
            long.MaxValue
          },
          {
            "Australia East",
            long.MaxValue
          },
          {
            "Australia Southeast",
            long.MaxValue
          },
          {
            "Brazil South",
            long.MaxValue
          },
          {
            "Canada Central",
            long.MaxValue
          },
          {
            "Canada East",
            long.MaxValue
          },
          {
            "Central India",
            long.MaxValue
          },
          {
            "Central US",
            long.MaxValue
          },
          {
            "China East",
            (long) byte.MaxValue
          },
          {
            "China East 2",
            (long) byte.MaxValue
          },
          {
            "China North",
            (long) byte.MaxValue
          },
          {
            "China North 2",
            0L
          },
          {
            "East Asia",
            long.MaxValue
          },
          {
            "East US",
            long.MaxValue
          },
          {
            "East US 2",
            long.MaxValue
          },
          {
            "France Central",
            long.MaxValue
          },
          {
            "France South",
            long.MaxValue
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            long.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            long.MaxValue
          },
          {
            "Japan East",
            long.MaxValue
          },
          {
            "Japan West",
            long.MaxValue
          },
          {
            "Korea Central",
            long.MaxValue
          },
          {
            "Korea South",
            long.MaxValue
          },
          {
            "North Central US",
            long.MaxValue
          },
          {
            "North Europe",
            long.MaxValue
          },
          {
            "Norway East",
            long.MaxValue
          },
          {
            "Norway West",
            long.MaxValue
          },
          {
            "South Africa North",
            long.MaxValue
          },
          {
            "South Africa West",
            long.MaxValue
          },
          {
            "South Central US",
            long.MaxValue
          },
          {
            "Southeast Asia",
            long.MaxValue
          },
          {
            "South India",
            long.MaxValue
          },
          {
            "Switzerland North",
            long.MaxValue
          },
          {
            "Switzerland West",
            long.MaxValue
          },
          {
            "UAE Central",
            long.MaxValue
          },
          {
            "UAE North",
            long.MaxValue
          },
          {
            "UK South",
            long.MaxValue
          },
          {
            "UK West",
            long.MaxValue
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            long.MaxValue
          },
          {
            "West Europe",
            long.MaxValue
          },
          {
            "West India",
            long.MaxValue
          },
          {
            "West US",
            long.MaxValue
          },
          {
            "West US 2",
            long.MaxValue
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "East Asia",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            (long) byte.MaxValue
          },
          {
            "Australia Central 2",
            (long) byte.MaxValue
          },
          {
            "Australia East",
            155L
          },
          {
            "Australia Southeast",
            165L
          },
          {
            "Brazil South",
            310L
          },
          {
            "Canada Central",
            210L
          },
          {
            "Canada East",
            225L
          },
          {
            "Central India",
            90L
          },
          {
            "Central US",
            190L
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            0L
          },
          {
            "East US",
            200L
          },
          {
            "East US 2",
            195L
          },
          {
            "France Central",
            190L
          },
          {
            "France South",
            185L
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            (long) byte.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            (long) byte.MaxValue
          },
          {
            "Japan East",
            50L
          },
          {
            "Japan West",
            55L
          },
          {
            "Korea Central",
            35L
          },
          {
            "Korea South",
            40L
          },
          {
            "North Central US",
            205L
          },
          {
            "North Europe",
            220L
          },
          {
            "Norway East",
            (long) byte.MaxValue
          },
          {
            "Norway West",
            (long) byte.MaxValue
          },
          {
            "South Africa North",
            (long) byte.MaxValue
          },
          {
            "South Africa West",
            (long) byte.MaxValue
          },
          {
            "South Central US",
            170L
          },
          {
            "Southeast Asia",
            40L
          },
          {
            "South India",
            65L
          },
          {
            "Switzerland North",
            (long) byte.MaxValue
          },
          {
            "Switzerland West",
            (long) byte.MaxValue
          },
          {
            "UAE Central",
            (long) byte.MaxValue
          },
          {
            "UAE North",
            (long) byte.MaxValue
          },
          {
            "UK South",
            200L
          },
          {
            "UK West",
            205L
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            180L
          },
          {
            "West Europe",
            220L
          },
          {
            "West India",
            85L
          },
          {
            "West US",
            155L
          },
          {
            "West US 2",
            170L
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "East US",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            (long) byte.MaxValue
          },
          {
            "Australia Central 2",
            (long) byte.MaxValue
          },
          {
            "Australia East",
            190L
          },
          {
            "Australia Southeast",
            205L
          },
          {
            "Brazil South",
            120L
          },
          {
            "Canada Central",
            25L
          },
          {
            "Canada East",
            35L
          },
          {
            "Central India",
            190L
          },
          {
            "Central US",
            40L
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            200L
          },
          {
            "East US",
            0L
          },
          {
            "East US 2",
            5L
          },
          {
            "France Central",
            90L
          },
          {
            "France South",
            100L
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            (long) byte.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            (long) byte.MaxValue
          },
          {
            "Japan East",
            150L
          },
          {
            "Japan West",
            160L
          },
          {
            "Korea Central",
            185L
          },
          {
            "Korea South",
            190L
          },
          {
            "North Central US",
            20L
          },
          {
            "North Europe",
            95L
          },
          {
            "Norway East",
            (long) byte.MaxValue
          },
          {
            "Norway West",
            (long) byte.MaxValue
          },
          {
            "South Africa North",
            (long) byte.MaxValue
          },
          {
            "South Africa West",
            (long) byte.MaxValue
          },
          {
            "South Central US",
            40L
          },
          {
            "Southeast Asia",
            220L
          },
          {
            "South India",
            215L
          },
          {
            "Switzerland North",
            (long) byte.MaxValue
          },
          {
            "Switzerland West",
            (long) byte.MaxValue
          },
          {
            "UAE Central",
            (long) byte.MaxValue
          },
          {
            "UAE North",
            (long) byte.MaxValue
          },
          {
            "UK South",
            75L
          },
          {
            "UK West",
            75L
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            60L
          },
          {
            "West Europe",
            140L
          },
          {
            "West India",
            190L
          },
          {
            "West US",
            70L
          },
          {
            "West US 2",
            80L
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "East US 2",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            long.MaxValue
          },
          {
            "Australia Central 2",
            long.MaxValue
          },
          {
            "Australia East",
            190L
          },
          {
            "Australia Southeast",
            200L
          },
          {
            "Brazil South",
            130L
          },
          {
            "Canada Central",
            30L
          },
          {
            "Canada East",
            40L
          },
          {
            "Central India",
            195L
          },
          {
            "Central US",
            45L
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            195L
          },
          {
            "East US",
            5L
          },
          {
            "East US 2",
            0L
          },
          {
            "France Central",
            90L
          },
          {
            "France South",
            100L
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            (long) byte.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            (long) byte.MaxValue
          },
          {
            "Japan East",
            150L
          },
          {
            "Japan West",
            155L
          },
          {
            "Korea Central",
            180L
          },
          {
            "Korea South",
            185L
          },
          {
            "North Central US",
            25L
          },
          {
            "North Europe",
            100L
          },
          {
            "Norway East",
            (long) byte.MaxValue
          },
          {
            "Norway West",
            (long) byte.MaxValue
          },
          {
            "South Africa North",
            (long) byte.MaxValue
          },
          {
            "South Africa West",
            (long) byte.MaxValue
          },
          {
            "South Central US",
            35L
          },
          {
            "Southeast Asia",
            220L
          },
          {
            "South India",
            215L
          },
          {
            "Switzerland North",
            (long) byte.MaxValue
          },
          {
            "Switzerland West",
            (long) byte.MaxValue
          },
          {
            "UAE Central",
            (long) byte.MaxValue
          },
          {
            "UAE North",
            (long) byte.MaxValue
          },
          {
            "UK South",
            80L
          },
          {
            "UK West",
            80L
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            55L
          },
          {
            "West Europe",
            135L
          },
          {
            "West India",
            190L
          },
          {
            "West US",
            65L
          },
          {
            "West US 2",
            75L
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "France Central",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            (long) byte.MaxValue
          },
          {
            "Australia Central 2",
            (long) byte.MaxValue
          },
          {
            "Australia East",
            280L
          },
          {
            "Australia Southeast",
            290L
          },
          {
            "Brazil South",
            200L
          },
          {
            "Canada Central",
            100L
          },
          {
            "Canada East",
            110L
          },
          {
            "Central India",
            120L
          },
          {
            "Central US",
            110L
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            190L
          },
          {
            "East US",
            90L
          },
          {
            "East US 2",
            90L
          },
          {
            "France Central",
            0L
          },
          {
            "France South",
            20L
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            (long) byte.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            (long) byte.MaxValue
          },
          {
            "Japan East",
            250L
          },
          {
            "Japan West",
            245L
          },
          {
            "Korea Central",
            270L
          },
          {
            "Korea South",
            280L
          },
          {
            "North Central US",
            100L
          },
          {
            "North Europe",
            20L
          },
          {
            "Norway East",
            (long) byte.MaxValue
          },
          {
            "Norway West",
            (long) byte.MaxValue
          },
          {
            "South Africa North",
            (long) byte.MaxValue
          },
          {
            "South Africa West",
            (long) byte.MaxValue
          },
          {
            "South Central US",
            120L
          },
          {
            "Southeast Asia",
            160L
          },
          {
            "South India",
            130L
          },
          {
            "Switzerland North",
            (long) byte.MaxValue
          },
          {
            "Switzerland West",
            (long) byte.MaxValue
          },
          {
            "UAE Central",
            (long) byte.MaxValue
          },
          {
            "UAE North",
            (long) byte.MaxValue
          },
          {
            "UK South",
            20L
          },
          {
            "UK West",
            20L
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            130L
          },
          {
            "West Europe",
            20L
          },
          {
            "West India",
            110L
          },
          {
            "West US",
            150L
          },
          {
            "West US 2",
            150L
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "France South",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            (long) byte.MaxValue
          },
          {
            "Australia Central 2",
            (long) byte.MaxValue
          },
          {
            "Australia East",
            270L
          },
          {
            "Australia Southeast",
            280L
          },
          {
            "Brazil South",
            185L
          },
          {
            "Canada Central",
            110L
          },
          {
            "Canada East",
            120L
          },
          {
            "Central India",
            110L
          },
          {
            "Central US",
            125L
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            185L
          },
          {
            "East US",
            100L
          },
          {
            "East US 2",
            100L
          },
          {
            "France Central",
            20L
          },
          {
            "France South",
            0L
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            (long) byte.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            (long) byte.MaxValue
          },
          {
            "Japan East",
            245L
          },
          {
            "Japan West",
            240L
          },
          {
            "Korea Central",
            265L
          },
          {
            "Korea South",
            275L
          },
          {
            "North Central US",
            110L
          },
          {
            "North Europe",
            35L
          },
          {
            "Norway East",
            (long) byte.MaxValue
          },
          {
            "Norway West",
            (long) byte.MaxValue
          },
          {
            "South Africa North",
            (long) byte.MaxValue
          },
          {
            "South Africa West",
            (long) byte.MaxValue
          },
          {
            "South Central US",
            130L
          },
          {
            "Southeast Asia",
            150L
          },
          {
            "South India",
            120L
          },
          {
            "Switzerland North",
            (long) byte.MaxValue
          },
          {
            "Switzerland West",
            (long) byte.MaxValue
          },
          {
            "UAE Central",
            (long) byte.MaxValue
          },
          {
            "UAE North",
            (long) byte.MaxValue
          },
          {
            "UK South",
            35L
          },
          {
            "UK West",
            40L
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            140L
          },
          {
            "West Europe",
            30L
          },
          {
            "West India",
            105L
          },
          {
            "West US",
            160L
          },
          {
            "West US 2",
            160L
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "Germany Central",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            long.MaxValue
          },
          {
            "Australia Central 2",
            long.MaxValue
          },
          {
            "Australia East",
            long.MaxValue
          },
          {
            "Australia Southeast",
            long.MaxValue
          },
          {
            "Brazil South",
            long.MaxValue
          },
          {
            "Canada Central",
            long.MaxValue
          },
          {
            "Canada East",
            long.MaxValue
          },
          {
            "Central India",
            long.MaxValue
          },
          {
            "Central US",
            long.MaxValue
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            long.MaxValue
          },
          {
            "East US",
            long.MaxValue
          },
          {
            "East US 2",
            long.MaxValue
          },
          {
            "France Central",
            long.MaxValue
          },
          {
            "France South",
            long.MaxValue
          },
          {
            "Germany Central",
            0L
          },
          {
            "Germany North",
            long.MaxValue
          },
          {
            "Germany Northeast",
            10L
          },
          {
            "Germany West Central",
            long.MaxValue
          },
          {
            "Japan East",
            long.MaxValue
          },
          {
            "Japan West",
            long.MaxValue
          },
          {
            "Korea Central",
            long.MaxValue
          },
          {
            "Korea South",
            long.MaxValue
          },
          {
            "North Central US",
            long.MaxValue
          },
          {
            "North Europe",
            long.MaxValue
          },
          {
            "Norway East",
            long.MaxValue
          },
          {
            "Norway West",
            long.MaxValue
          },
          {
            "South Africa North",
            long.MaxValue
          },
          {
            "South Africa West",
            long.MaxValue
          },
          {
            "South Central US",
            long.MaxValue
          },
          {
            "Southeast Asia",
            long.MaxValue
          },
          {
            "South India",
            long.MaxValue
          },
          {
            "Switzerland North",
            long.MaxValue
          },
          {
            "Switzerland West",
            long.MaxValue
          },
          {
            "UAE Central",
            long.MaxValue
          },
          {
            "UAE North",
            long.MaxValue
          },
          {
            "UK South",
            long.MaxValue
          },
          {
            "UK West",
            long.MaxValue
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            long.MaxValue
          },
          {
            "West Europe",
            long.MaxValue
          },
          {
            "West India",
            long.MaxValue
          },
          {
            "West US",
            long.MaxValue
          },
          {
            "West US 2",
            long.MaxValue
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "Germany North",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            (long) byte.MaxValue
          },
          {
            "Australia Central 2",
            (long) byte.MaxValue
          },
          {
            "Australia East",
            (long) byte.MaxValue
          },
          {
            "Australia Southeast",
            (long) byte.MaxValue
          },
          {
            "Brazil South",
            (long) byte.MaxValue
          },
          {
            "Canada Central",
            (long) byte.MaxValue
          },
          {
            "Canada East",
            (long) byte.MaxValue
          },
          {
            "Central India",
            (long) byte.MaxValue
          },
          {
            "Central US",
            (long) byte.MaxValue
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            (long) byte.MaxValue
          },
          {
            "East US",
            (long) byte.MaxValue
          },
          {
            "East US 2",
            (long) byte.MaxValue
          },
          {
            "France Central",
            (long) byte.MaxValue
          },
          {
            "France South",
            (long) byte.MaxValue
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            0L
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            (long) byte.MaxValue
          },
          {
            "Japan East",
            (long) byte.MaxValue
          },
          {
            "Japan West",
            (long) byte.MaxValue
          },
          {
            "Korea Central",
            (long) byte.MaxValue
          },
          {
            "Korea South",
            (long) byte.MaxValue
          },
          {
            "North Central US",
            (long) byte.MaxValue
          },
          {
            "North Europe",
            (long) byte.MaxValue
          },
          {
            "Norway East",
            (long) byte.MaxValue
          },
          {
            "Norway West",
            (long) byte.MaxValue
          },
          {
            "South Africa North",
            (long) byte.MaxValue
          },
          {
            "South Africa West",
            (long) byte.MaxValue
          },
          {
            "South Central US",
            (long) byte.MaxValue
          },
          {
            "Southeast Asia",
            (long) byte.MaxValue
          },
          {
            "South India",
            (long) byte.MaxValue
          },
          {
            "Switzerland North",
            (long) byte.MaxValue
          },
          {
            "Switzerland West",
            (long) byte.MaxValue
          },
          {
            "UAE Central",
            (long) byte.MaxValue
          },
          {
            "UAE North",
            (long) byte.MaxValue
          },
          {
            "UK South",
            (long) byte.MaxValue
          },
          {
            "UK West",
            (long) byte.MaxValue
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            (long) byte.MaxValue
          },
          {
            "West Europe",
            (long) byte.MaxValue
          },
          {
            "West India",
            (long) byte.MaxValue
          },
          {
            "West US",
            (long) byte.MaxValue
          },
          {
            "West US 2",
            (long) byte.MaxValue
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "Germany Northeast",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            long.MaxValue
          },
          {
            "Australia Central 2",
            long.MaxValue
          },
          {
            "Australia East",
            long.MaxValue
          },
          {
            "Australia Southeast",
            long.MaxValue
          },
          {
            "Brazil South",
            long.MaxValue
          },
          {
            "Canada Central",
            long.MaxValue
          },
          {
            "Canada East",
            long.MaxValue
          },
          {
            "Central India",
            long.MaxValue
          },
          {
            "Central US",
            long.MaxValue
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            long.MaxValue
          },
          {
            "East US",
            long.MaxValue
          },
          {
            "East US 2",
            long.MaxValue
          },
          {
            "France Central",
            long.MaxValue
          },
          {
            "France South",
            long.MaxValue
          },
          {
            "Germany Central",
            10L
          },
          {
            "Germany North",
            long.MaxValue
          },
          {
            "Germany Northeast",
            0L
          },
          {
            "Germany West Central",
            long.MaxValue
          },
          {
            "Japan East",
            long.MaxValue
          },
          {
            "Japan West",
            long.MaxValue
          },
          {
            "Korea Central",
            long.MaxValue
          },
          {
            "Korea South",
            long.MaxValue
          },
          {
            "North Central US",
            long.MaxValue
          },
          {
            "North Europe",
            long.MaxValue
          },
          {
            "Norway East",
            long.MaxValue
          },
          {
            "Norway West",
            long.MaxValue
          },
          {
            "South Africa North",
            long.MaxValue
          },
          {
            "South Africa West",
            long.MaxValue
          },
          {
            "South Central US",
            long.MaxValue
          },
          {
            "Southeast Asia",
            long.MaxValue
          },
          {
            "South India",
            long.MaxValue
          },
          {
            "Switzerland North",
            long.MaxValue
          },
          {
            "Switzerland West",
            long.MaxValue
          },
          {
            "UAE Central",
            long.MaxValue
          },
          {
            "UAE North",
            long.MaxValue
          },
          {
            "UK South",
            long.MaxValue
          },
          {
            "UK West",
            long.MaxValue
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            long.MaxValue
          },
          {
            "West Europe",
            long.MaxValue
          },
          {
            "West India",
            long.MaxValue
          },
          {
            "West US",
            long.MaxValue
          },
          {
            "West US 2",
            long.MaxValue
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "Germany West Central",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            (long) byte.MaxValue
          },
          {
            "Australia Central 2",
            (long) byte.MaxValue
          },
          {
            "Australia East",
            (long) byte.MaxValue
          },
          {
            "Australia Southeast",
            (long) byte.MaxValue
          },
          {
            "Brazil South",
            (long) byte.MaxValue
          },
          {
            "Canada Central",
            (long) byte.MaxValue
          },
          {
            "Canada East",
            (long) byte.MaxValue
          },
          {
            "Central India",
            (long) byte.MaxValue
          },
          {
            "Central US",
            (long) byte.MaxValue
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            (long) byte.MaxValue
          },
          {
            "East US",
            (long) byte.MaxValue
          },
          {
            "East US 2",
            (long) byte.MaxValue
          },
          {
            "France Central",
            (long) byte.MaxValue
          },
          {
            "France South",
            (long) byte.MaxValue
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            (long) byte.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            0L
          },
          {
            "Japan East",
            (long) byte.MaxValue
          },
          {
            "Japan West",
            (long) byte.MaxValue
          },
          {
            "Korea Central",
            (long) byte.MaxValue
          },
          {
            "Korea South",
            (long) byte.MaxValue
          },
          {
            "North Central US",
            (long) byte.MaxValue
          },
          {
            "North Europe",
            (long) byte.MaxValue
          },
          {
            "Norway East",
            (long) byte.MaxValue
          },
          {
            "Norway West",
            (long) byte.MaxValue
          },
          {
            "South Africa North",
            (long) byte.MaxValue
          },
          {
            "South Africa West",
            (long) byte.MaxValue
          },
          {
            "South Central US",
            (long) byte.MaxValue
          },
          {
            "Southeast Asia",
            (long) byte.MaxValue
          },
          {
            "South India",
            (long) byte.MaxValue
          },
          {
            "Switzerland North",
            (long) byte.MaxValue
          },
          {
            "Switzerland West",
            (long) byte.MaxValue
          },
          {
            "UAE Central",
            (long) byte.MaxValue
          },
          {
            "UAE North",
            (long) byte.MaxValue
          },
          {
            "UK South",
            (long) byte.MaxValue
          },
          {
            "UK West",
            (long) byte.MaxValue
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            (long) byte.MaxValue
          },
          {
            "West Europe",
            (long) byte.MaxValue
          },
          {
            "West India",
            (long) byte.MaxValue
          },
          {
            "West US",
            (long) byte.MaxValue
          },
          {
            "West US 2",
            (long) byte.MaxValue
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "Japan East",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            (long) byte.MaxValue
          },
          {
            "Australia Central 2",
            (long) byte.MaxValue
          },
          {
            "Australia East",
            105L
          },
          {
            "Australia Southeast",
            115L
          },
          {
            "Brazil South",
            260L
          },
          {
            "Canada Central",
            170L
          },
          {
            "Canada East",
            180L
          },
          {
            "Central India",
            120L
          },
          {
            "Central US",
            145L
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            50L
          },
          {
            "East US",
            150L
          },
          {
            "East US 2",
            150L
          },
          {
            "France Central",
            250L
          },
          {
            "France South",
            245L
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            (long) byte.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            (long) byte.MaxValue
          },
          {
            "Japan East",
            0L
          },
          {
            "Japan West",
            10L
          },
          {
            "Korea Central",
            35L
          },
          {
            "Korea South",
            40L
          },
          {
            "North Central US",
            160L
          },
          {
            "North Europe",
            235L
          },
          {
            "Norway East",
            (long) byte.MaxValue
          },
          {
            "Norway West",
            (long) byte.MaxValue
          },
          {
            "South Africa North",
            (long) byte.MaxValue
          },
          {
            "South Africa West",
            (long) byte.MaxValue
          },
          {
            "South Central US",
            120L
          },
          {
            "Southeast Asia",
            65L
          },
          {
            "South India",
            100L
          },
          {
            "Switzerland North",
            (long) byte.MaxValue
          },
          {
            "Switzerland West",
            (long) byte.MaxValue
          },
          {
            "UAE Central",
            (long) byte.MaxValue
          },
          {
            "UAE North",
            (long) byte.MaxValue
          },
          {
            "UK South",
            225L
          },
          {
            "UK West",
            225L
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            130L
          },
          {
            "West Europe",
            230L
          },
          {
            "West India",
            120L
          },
          {
            "West US",
            105L
          },
          {
            "West US 2",
            110L
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "Japan West",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            (long) byte.MaxValue
          },
          {
            "Australia Central 2",
            (long) byte.MaxValue
          },
          {
            "Australia East",
            110L
          },
          {
            "Australia Southeast",
            125L
          },
          {
            "Brazil South",
            265L
          },
          {
            "Canada Central",
            180L
          },
          {
            "Canada East",
            190L
          },
          {
            "Central India",
            120L
          },
          {
            "Central US",
            150L
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            55L
          },
          {
            "East US",
            160L
          },
          {
            "East US 2",
            155L
          },
          {
            "France Central",
            245L
          },
          {
            "France South",
            240L
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            (long) byte.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            (long) byte.MaxValue
          },
          {
            "Japan East",
            10L
          },
          {
            "Japan West",
            0L
          },
          {
            "Korea Central",
            40L
          },
          {
            "Korea South",
            50L
          },
          {
            "North Central US",
            165L
          },
          {
            "North Europe",
            250L
          },
          {
            "Norway East",
            (long) byte.MaxValue
          },
          {
            "Norway West",
            (long) byte.MaxValue
          },
          {
            "South Africa North",
            (long) byte.MaxValue
          },
          {
            "South Africa West",
            (long) byte.MaxValue
          },
          {
            "South Central US",
            130L
          },
          {
            "Southeast Asia",
            75L
          },
          {
            "South India",
            105L
          },
          {
            "Switzerland North",
            (long) byte.MaxValue
          },
          {
            "Switzerland West",
            (long) byte.MaxValue
          },
          {
            "UAE Central",
            (long) byte.MaxValue
          },
          {
            "UAE North",
            (long) byte.MaxValue
          },
          {
            "UK South",
            230L
          },
          {
            "UK West",
            235L
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            140L
          },
          {
            "West Europe",
            240L
          },
          {
            "West India",
            130L
          },
          {
            "West US",
            115L
          },
          {
            "West US 2",
            120L
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "Korea Central",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            (long) byte.MaxValue
          },
          {
            "Australia Central 2",
            (long) byte.MaxValue
          },
          {
            "Australia East",
            140L
          },
          {
            "Australia Southeast",
            150L
          },
          {
            "Brazil South",
            295L
          },
          {
            "Canada Central",
            200L
          },
          {
            "Canada East",
            215L
          },
          {
            "Central India",
            115L
          },
          {
            "Central US",
            175L
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            35L
          },
          {
            "East US",
            185L
          },
          {
            "East US 2",
            180L
          },
          {
            "France Central",
            270L
          },
          {
            "France South",
            265L
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            (long) byte.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            (long) byte.MaxValue
          },
          {
            "Japan East",
            35L
          },
          {
            "Japan West",
            40L
          },
          {
            "Korea Central",
            0L
          },
          {
            "Korea South",
            5L
          },
          {
            "North Central US",
            190L
          },
          {
            "North Europe",
            270L
          },
          {
            "Norway East",
            (long) byte.MaxValue
          },
          {
            "Norway West",
            (long) byte.MaxValue
          },
          {
            "South Africa North",
            (long) byte.MaxValue
          },
          {
            "South Africa West",
            (long) byte.MaxValue
          },
          {
            "South Central US",
            155L
          },
          {
            "Southeast Asia",
            65L
          },
          {
            "South India",
            95L
          },
          {
            "Switzerland North",
            (long) byte.MaxValue
          },
          {
            "Switzerland West",
            (long) byte.MaxValue
          },
          {
            "UAE Central",
            (long) byte.MaxValue
          },
          {
            "UAE North",
            (long) byte.MaxValue
          },
          {
            "UK South",
            260L
          },
          {
            "UK West",
            260L
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            165L
          },
          {
            "West Europe",
            265L
          },
          {
            "West India",
            125L
          },
          {
            "West US",
            140L
          },
          {
            "West US 2",
            145L
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "Korea South",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            (long) byte.MaxValue
          },
          {
            "Australia Central 2",
            (long) byte.MaxValue
          },
          {
            "Australia East",
            145L
          },
          {
            "Australia Southeast",
            150L
          },
          {
            "Brazil South",
            300L
          },
          {
            "Canada Central",
            210L
          },
          {
            "Canada East",
            220L
          },
          {
            "Central India",
            125L
          },
          {
            "Central US",
            185L
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            40L
          },
          {
            "East US",
            190L
          },
          {
            "East US 2",
            185L
          },
          {
            "France Central",
            280L
          },
          {
            "France South",
            275L
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            (long) byte.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            (long) byte.MaxValue
          },
          {
            "Japan East",
            40L
          },
          {
            "Japan West",
            50L
          },
          {
            "Korea Central",
            5L
          },
          {
            "Korea South",
            0L
          },
          {
            "North Central US",
            200L
          },
          {
            "North Europe",
            275L
          },
          {
            "Norway East",
            (long) byte.MaxValue
          },
          {
            "Norway West",
            (long) byte.MaxValue
          },
          {
            "South Africa North",
            (long) byte.MaxValue
          },
          {
            "South Africa West",
            (long) byte.MaxValue
          },
          {
            "South Central US",
            160L
          },
          {
            "Southeast Asia",
            75L
          },
          {
            "South India",
            100L
          },
          {
            "Switzerland North",
            (long) byte.MaxValue
          },
          {
            "Switzerland West",
            (long) byte.MaxValue
          },
          {
            "UAE Central",
            (long) byte.MaxValue
          },
          {
            "UAE North",
            (long) byte.MaxValue
          },
          {
            "UK South",
            265L
          },
          {
            "UK West",
            265L
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            170L
          },
          {
            "West Europe",
            270L
          },
          {
            "West India",
            130L
          },
          {
            "West US",
            145L
          },
          {
            "West US 2",
            150L
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "North Central US",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            (long) byte.MaxValue
          },
          {
            "Australia Central 2",
            (long) byte.MaxValue
          },
          {
            "Australia East",
            200L
          },
          {
            "Australia Southeast",
            210L
          },
          {
            "Brazil South",
            135L
          },
          {
            "Canada Central",
            15L
          },
          {
            "Canada East",
            25L
          },
          {
            "Central India",
            210L
          },
          {
            "Central US",
            15L
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            205L
          },
          {
            "East US",
            20L
          },
          {
            "East US 2",
            25L
          },
          {
            "France Central",
            100L
          },
          {
            "France South",
            110L
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            (long) byte.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            (long) byte.MaxValue
          },
          {
            "Japan East",
            160L
          },
          {
            "Japan West",
            165L
          },
          {
            "Korea Central",
            190L
          },
          {
            "Korea South",
            200L
          },
          {
            "North Central US",
            0L
          },
          {
            "North Europe",
            90L
          },
          {
            "Norway East",
            (long) byte.MaxValue
          },
          {
            "Norway West",
            (long) byte.MaxValue
          },
          {
            "South Africa North",
            (long) byte.MaxValue
          },
          {
            "South Africa West",
            (long) byte.MaxValue
          },
          {
            "South Central US",
            35L
          },
          {
            "Southeast Asia",
            225L
          },
          {
            "South India",
            (long) byte.MaxValue
          },
          {
            "Switzerland North",
            (long) byte.MaxValue
          },
          {
            "Switzerland West",
            (long) byte.MaxValue
          },
          {
            "UAE Central",
            (long) byte.MaxValue
          },
          {
            "UAE North",
            (long) byte.MaxValue
          },
          {
            "UK South",
            95L
          },
          {
            "UK West",
            95L
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            30L
          },
          {
            "West Europe",
            100L
          },
          {
            "West India",
            200L
          },
          {
            "West US",
            55L
          },
          {
            "West US 2",
            50L
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "North Europe",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            (long) byte.MaxValue
          },
          {
            "Australia Central 2",
            (long) byte.MaxValue
          },
          {
            "Australia East",
            285L
          },
          {
            "Australia Southeast",
            305L
          },
          {
            "Brazil South",
            190L
          },
          {
            "Canada Central",
            75L
          },
          {
            "Canada East",
            85L
          },
          {
            "Central India",
            130L
          },
          {
            "Central US",
            105L
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            220L
          },
          {
            "East US",
            95L
          },
          {
            "East US 2",
            100L
          },
          {
            "France Central",
            20L
          },
          {
            "France South",
            35L
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            (long) byte.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            (long) byte.MaxValue
          },
          {
            "Japan East",
            235L
          },
          {
            "Japan West",
            250L
          },
          {
            "Korea Central",
            270L
          },
          {
            "Korea South",
            275L
          },
          {
            "North Central US",
            90L
          },
          {
            "North Europe",
            0L
          },
          {
            "Norway East",
            (long) byte.MaxValue
          },
          {
            "Norway West",
            (long) byte.MaxValue
          },
          {
            "South Africa North",
            (long) byte.MaxValue
          },
          {
            "South Africa West",
            (long) byte.MaxValue
          },
          {
            "South Central US",
            120L
          },
          {
            "Southeast Asia",
            180L
          },
          {
            "South India",
            150L
          },
          {
            "Switzerland North",
            (long) byte.MaxValue
          },
          {
            "Switzerland West",
            (long) byte.MaxValue
          },
          {
            "UAE Central",
            (long) byte.MaxValue
          },
          {
            "UAE North",
            (long) byte.MaxValue
          },
          {
            "UK South",
            10L
          },
          {
            "UK West",
            15L
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            115L
          },
          {
            "West Europe",
            20L
          },
          {
            "West India",
            130L
          },
          {
            "West US",
            140L
          },
          {
            "West US 2",
            135L
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "Norway East",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            (long) byte.MaxValue
          },
          {
            "Australia Central 2",
            (long) byte.MaxValue
          },
          {
            "Australia East",
            (long) byte.MaxValue
          },
          {
            "Australia Southeast",
            (long) byte.MaxValue
          },
          {
            "Brazil South",
            (long) byte.MaxValue
          },
          {
            "Canada Central",
            (long) byte.MaxValue
          },
          {
            "Canada East",
            (long) byte.MaxValue
          },
          {
            "Central India",
            (long) byte.MaxValue
          },
          {
            "Central US",
            (long) byte.MaxValue
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            (long) byte.MaxValue
          },
          {
            "East US",
            (long) byte.MaxValue
          },
          {
            "East US 2",
            (long) byte.MaxValue
          },
          {
            "France Central",
            (long) byte.MaxValue
          },
          {
            "France South",
            (long) byte.MaxValue
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            (long) byte.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            (long) byte.MaxValue
          },
          {
            "Japan East",
            (long) byte.MaxValue
          },
          {
            "Japan West",
            (long) byte.MaxValue
          },
          {
            "Korea Central",
            (long) byte.MaxValue
          },
          {
            "Korea South",
            (long) byte.MaxValue
          },
          {
            "North Central US",
            (long) byte.MaxValue
          },
          {
            "North Europe",
            (long) byte.MaxValue
          },
          {
            "Norway East",
            0L
          },
          {
            "Norway West",
            (long) byte.MaxValue
          },
          {
            "South Africa North",
            (long) byte.MaxValue
          },
          {
            "South Africa West",
            (long) byte.MaxValue
          },
          {
            "South Central US",
            (long) byte.MaxValue
          },
          {
            "Southeast Asia",
            (long) byte.MaxValue
          },
          {
            "South India",
            (long) byte.MaxValue
          },
          {
            "Switzerland North",
            (long) byte.MaxValue
          },
          {
            "Switzerland West",
            (long) byte.MaxValue
          },
          {
            "UAE Central",
            (long) byte.MaxValue
          },
          {
            "UAE North",
            (long) byte.MaxValue
          },
          {
            "UK South",
            (long) byte.MaxValue
          },
          {
            "UK West",
            (long) byte.MaxValue
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            (long) byte.MaxValue
          },
          {
            "West Europe",
            (long) byte.MaxValue
          },
          {
            "West India",
            (long) byte.MaxValue
          },
          {
            "West US",
            (long) byte.MaxValue
          },
          {
            "West US 2",
            (long) byte.MaxValue
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "Norway West",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            (long) byte.MaxValue
          },
          {
            "Australia Central 2",
            (long) byte.MaxValue
          },
          {
            "Australia East",
            (long) byte.MaxValue
          },
          {
            "Australia Southeast",
            (long) byte.MaxValue
          },
          {
            "Brazil South",
            (long) byte.MaxValue
          },
          {
            "Canada Central",
            (long) byte.MaxValue
          },
          {
            "Canada East",
            (long) byte.MaxValue
          },
          {
            "Central India",
            (long) byte.MaxValue
          },
          {
            "Central US",
            (long) byte.MaxValue
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            (long) byte.MaxValue
          },
          {
            "East US",
            (long) byte.MaxValue
          },
          {
            "East US 2",
            (long) byte.MaxValue
          },
          {
            "France Central",
            (long) byte.MaxValue
          },
          {
            "France South",
            (long) byte.MaxValue
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            (long) byte.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            (long) byte.MaxValue
          },
          {
            "Japan East",
            (long) byte.MaxValue
          },
          {
            "Japan West",
            (long) byte.MaxValue
          },
          {
            "Korea Central",
            (long) byte.MaxValue
          },
          {
            "Korea South",
            (long) byte.MaxValue
          },
          {
            "North Central US",
            (long) byte.MaxValue
          },
          {
            "North Europe",
            (long) byte.MaxValue
          },
          {
            "Norway East",
            (long) byte.MaxValue
          },
          {
            "Norway West",
            0L
          },
          {
            "South Africa North",
            (long) byte.MaxValue
          },
          {
            "South Africa West",
            (long) byte.MaxValue
          },
          {
            "South Central US",
            (long) byte.MaxValue
          },
          {
            "Southeast Asia",
            (long) byte.MaxValue
          },
          {
            "South India",
            (long) byte.MaxValue
          },
          {
            "Switzerland North",
            (long) byte.MaxValue
          },
          {
            "Switzerland West",
            (long) byte.MaxValue
          },
          {
            "UAE Central",
            (long) byte.MaxValue
          },
          {
            "UAE North",
            (long) byte.MaxValue
          },
          {
            "UK South",
            (long) byte.MaxValue
          },
          {
            "UK West",
            (long) byte.MaxValue
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            (long) byte.MaxValue
          },
          {
            "West Europe",
            (long) byte.MaxValue
          },
          {
            "West India",
            (long) byte.MaxValue
          },
          {
            "West US",
            (long) byte.MaxValue
          },
          {
            "West US 2",
            (long) byte.MaxValue
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "South Africa North",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            (long) byte.MaxValue
          },
          {
            "Australia Central 2",
            (long) byte.MaxValue
          },
          {
            "Australia East",
            (long) byte.MaxValue
          },
          {
            "Australia Southeast",
            (long) byte.MaxValue
          },
          {
            "Brazil South",
            (long) byte.MaxValue
          },
          {
            "Canada Central",
            (long) byte.MaxValue
          },
          {
            "Canada East",
            (long) byte.MaxValue
          },
          {
            "Central India",
            (long) byte.MaxValue
          },
          {
            "Central US",
            (long) byte.MaxValue
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            (long) byte.MaxValue
          },
          {
            "East US",
            (long) byte.MaxValue
          },
          {
            "East US 2",
            (long) byte.MaxValue
          },
          {
            "France Central",
            (long) byte.MaxValue
          },
          {
            "France South",
            (long) byte.MaxValue
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            (long) byte.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            (long) byte.MaxValue
          },
          {
            "Japan East",
            (long) byte.MaxValue
          },
          {
            "Japan West",
            (long) byte.MaxValue
          },
          {
            "Korea Central",
            (long) byte.MaxValue
          },
          {
            "Korea South",
            (long) byte.MaxValue
          },
          {
            "North Central US",
            (long) byte.MaxValue
          },
          {
            "North Europe",
            (long) byte.MaxValue
          },
          {
            "Norway East",
            (long) byte.MaxValue
          },
          {
            "Norway West",
            (long) byte.MaxValue
          },
          {
            "South Africa North",
            0L
          },
          {
            "South Africa West",
            (long) byte.MaxValue
          },
          {
            "South Central US",
            (long) byte.MaxValue
          },
          {
            "Southeast Asia",
            (long) byte.MaxValue
          },
          {
            "South India",
            (long) byte.MaxValue
          },
          {
            "Switzerland North",
            (long) byte.MaxValue
          },
          {
            "Switzerland West",
            (long) byte.MaxValue
          },
          {
            "UAE Central",
            (long) byte.MaxValue
          },
          {
            "UAE North",
            (long) byte.MaxValue
          },
          {
            "UK South",
            (long) byte.MaxValue
          },
          {
            "UK West",
            (long) byte.MaxValue
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            (long) byte.MaxValue
          },
          {
            "West Europe",
            (long) byte.MaxValue
          },
          {
            "West India",
            (long) byte.MaxValue
          },
          {
            "West US",
            (long) byte.MaxValue
          },
          {
            "West US 2",
            (long) byte.MaxValue
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "South Africa West",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            (long) byte.MaxValue
          },
          {
            "Australia Central 2",
            (long) byte.MaxValue
          },
          {
            "Australia East",
            (long) byte.MaxValue
          },
          {
            "Australia Southeast",
            (long) byte.MaxValue
          },
          {
            "Brazil South",
            (long) byte.MaxValue
          },
          {
            "Canada Central",
            (long) byte.MaxValue
          },
          {
            "Canada East",
            (long) byte.MaxValue
          },
          {
            "Central India",
            (long) byte.MaxValue
          },
          {
            "Central US",
            (long) byte.MaxValue
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            (long) byte.MaxValue
          },
          {
            "East US",
            (long) byte.MaxValue
          },
          {
            "East US 2",
            (long) byte.MaxValue
          },
          {
            "France Central",
            (long) byte.MaxValue
          },
          {
            "France South",
            (long) byte.MaxValue
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            (long) byte.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            (long) byte.MaxValue
          },
          {
            "Japan East",
            (long) byte.MaxValue
          },
          {
            "Japan West",
            (long) byte.MaxValue
          },
          {
            "Korea Central",
            (long) byte.MaxValue
          },
          {
            "Korea South",
            (long) byte.MaxValue
          },
          {
            "North Central US",
            (long) byte.MaxValue
          },
          {
            "North Europe",
            (long) byte.MaxValue
          },
          {
            "Norway East",
            (long) byte.MaxValue
          },
          {
            "Norway West",
            (long) byte.MaxValue
          },
          {
            "South Africa North",
            (long) byte.MaxValue
          },
          {
            "South Africa West",
            0L
          },
          {
            "South Central US",
            (long) byte.MaxValue
          },
          {
            "Southeast Asia",
            (long) byte.MaxValue
          },
          {
            "South India",
            (long) byte.MaxValue
          },
          {
            "Switzerland North",
            (long) byte.MaxValue
          },
          {
            "Switzerland West",
            (long) byte.MaxValue
          },
          {
            "UAE Central",
            (long) byte.MaxValue
          },
          {
            "UAE North",
            (long) byte.MaxValue
          },
          {
            "UK South",
            (long) byte.MaxValue
          },
          {
            "UK West",
            (long) byte.MaxValue
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            (long) byte.MaxValue
          },
          {
            "West Europe",
            (long) byte.MaxValue
          },
          {
            "West India",
            (long) byte.MaxValue
          },
          {
            "West US",
            (long) byte.MaxValue
          },
          {
            "West US 2",
            (long) byte.MaxValue
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "South Central US",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            (long) byte.MaxValue
          },
          {
            "Australia Central 2",
            (long) byte.MaxValue
          },
          {
            "Australia East",
            165L
          },
          {
            "Australia Southeast",
            175L
          },
          {
            "Brazil South",
            140L
          },
          {
            "Canada Central",
            50L
          },
          {
            "Canada East",
            60L
          },
          {
            "Central India",
            245L
          },
          {
            "Central US",
            25L
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            170L
          },
          {
            "East US",
            40L
          },
          {
            "East US 2",
            35L
          },
          {
            "France Central",
            120L
          },
          {
            "France South",
            130L
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            (long) byte.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            (long) byte.MaxValue
          },
          {
            "Japan East",
            120L
          },
          {
            "Japan West",
            130L
          },
          {
            "Korea Central",
            155L
          },
          {
            "Korea South",
            160L
          },
          {
            "North Central US",
            35L
          },
          {
            "North Europe",
            120L
          },
          {
            "Norway East",
            (long) byte.MaxValue
          },
          {
            "Norway West",
            (long) byte.MaxValue
          },
          {
            "South Africa North",
            (long) byte.MaxValue
          },
          {
            "South Africa West",
            (long) byte.MaxValue
          },
          {
            "South Central US",
            0L
          },
          {
            "Southeast Asia",
            190L
          },
          {
            "South India",
            220L
          },
          {
            "Switzerland North",
            (long) byte.MaxValue
          },
          {
            "Switzerland West",
            (long) byte.MaxValue
          },
          {
            "UAE Central",
            (long) byte.MaxValue
          },
          {
            "UAE North",
            (long) byte.MaxValue
          },
          {
            "UK South",
            100L
          },
          {
            "UK West",
            105L
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            25L
          },
          {
            "West Europe",
            145L
          },
          {
            "West India",
            245L
          },
          {
            "West US",
            35L
          },
          {
            "West US 2",
            45L
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "Southeast Asia",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            (long) byte.MaxValue
          },
          {
            "Australia Central 2",
            (long) byte.MaxValue
          },
          {
            "Australia East",
            170L
          },
          {
            "Australia Southeast",
            185L
          },
          {
            "Brazil South",
            325L
          },
          {
            "Canada Central",
            235L
          },
          {
            "Canada East",
            250L
          },
          {
            "Central India",
            60L
          },
          {
            "Central US",
            210L
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            35L
          },
          {
            "East US",
            215L
          },
          {
            "East US 2",
            220L
          },
          {
            "France Central",
            160L
          },
          {
            "France South",
            150L
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            (long) byte.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            (long) byte.MaxValue
          },
          {
            "Japan East",
            65L
          },
          {
            "Japan West",
            75L
          },
          {
            "Korea Central",
            65L
          },
          {
            "Korea South",
            75L
          },
          {
            "North Central US",
            225L
          },
          {
            "North Europe",
            180L
          },
          {
            "Norway East",
            (long) byte.MaxValue
          },
          {
            "Norway West",
            (long) byte.MaxValue
          },
          {
            "South Africa North",
            (long) byte.MaxValue
          },
          {
            "South Africa West",
            (long) byte.MaxValue
          },
          {
            "South Central US",
            190L
          },
          {
            "Southeast Asia",
            0L
          },
          {
            "South India",
            35L
          },
          {
            "Switzerland North",
            (long) byte.MaxValue
          },
          {
            "Switzerland West",
            (long) byte.MaxValue
          },
          {
            "UAE Central",
            (long) byte.MaxValue
          },
          {
            "UAE North",
            (long) byte.MaxValue
          },
          {
            "UK South",
            180L
          },
          {
            "UK West",
            175L
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            195L
          },
          {
            "West Europe",
            180L
          },
          {
            "West India",
            55L
          },
          {
            "West US",
            170L
          },
          {
            "West US 2",
            175L
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "South India",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            (long) byte.MaxValue
          },
          {
            "Australia Central 2",
            (long) byte.MaxValue
          },
          {
            "Australia East",
            205L
          },
          {
            "Australia Southeast",
            215L
          },
          {
            "Brazil South",
            320L
          },
          {
            "Canada Central",
            230L
          },
          {
            "Canada East",
            240L
          },
          {
            "Central India",
            40L
          },
          {
            "Central US",
            240L
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            54L
          },
          {
            "East US",
            215L
          },
          {
            "East US 2",
            215L
          },
          {
            "France Central",
            130L
          },
          {
            "France South",
            120L
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            (long) byte.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            (long) byte.MaxValue
          },
          {
            "Japan East",
            100L
          },
          {
            "Japan West",
            105L
          },
          {
            "Korea Central",
            95L
          },
          {
            "Korea South",
            100L
          },
          {
            "North Central US",
            (long) byte.MaxValue
          },
          {
            "North Europe",
            150L
          },
          {
            "Norway East",
            (long) byte.MaxValue
          },
          {
            "Norway West",
            (long) byte.MaxValue
          },
          {
            "South Africa North",
            (long) byte.MaxValue
          },
          {
            "South Africa West",
            (long) byte.MaxValue
          },
          {
            "South Central US",
            220L
          },
          {
            "Southeast Asia",
            35L
          },
          {
            "South India",
            0L
          },
          {
            "Switzerland North",
            (long) byte.MaxValue
          },
          {
            "Switzerland West",
            (long) byte.MaxValue
          },
          {
            "UAE Central",
            (long) byte.MaxValue
          },
          {
            "UAE North",
            (long) byte.MaxValue
          },
          {
            "UK South",
            145L
          },
          {
            "UK West",
            155L
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            230L
          },
          {
            "West Europe",
            150L
          },
          {
            "West India",
            25L
          },
          {
            "West US",
            205L
          },
          {
            "West US 2",
            200L
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "Switzerland North",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            (long) byte.MaxValue
          },
          {
            "Australia Central 2",
            (long) byte.MaxValue
          },
          {
            "Australia East",
            (long) byte.MaxValue
          },
          {
            "Australia Southeast",
            (long) byte.MaxValue
          },
          {
            "Brazil South",
            (long) byte.MaxValue
          },
          {
            "Canada Central",
            (long) byte.MaxValue
          },
          {
            "Canada East",
            (long) byte.MaxValue
          },
          {
            "Central India",
            (long) byte.MaxValue
          },
          {
            "Central US",
            (long) byte.MaxValue
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            (long) byte.MaxValue
          },
          {
            "East US",
            (long) byte.MaxValue
          },
          {
            "East US 2",
            (long) byte.MaxValue
          },
          {
            "France Central",
            (long) byte.MaxValue
          },
          {
            "France South",
            (long) byte.MaxValue
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            (long) byte.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            (long) byte.MaxValue
          },
          {
            "Japan East",
            (long) byte.MaxValue
          },
          {
            "Japan West",
            (long) byte.MaxValue
          },
          {
            "Korea Central",
            (long) byte.MaxValue
          },
          {
            "Korea South",
            (long) byte.MaxValue
          },
          {
            "North Central US",
            (long) byte.MaxValue
          },
          {
            "North Europe",
            (long) byte.MaxValue
          },
          {
            "Norway East",
            (long) byte.MaxValue
          },
          {
            "Norway West",
            (long) byte.MaxValue
          },
          {
            "South Africa North",
            (long) byte.MaxValue
          },
          {
            "South Africa West",
            (long) byte.MaxValue
          },
          {
            "South Central US",
            (long) byte.MaxValue
          },
          {
            "Southeast Asia",
            (long) byte.MaxValue
          },
          {
            "South India",
            (long) byte.MaxValue
          },
          {
            "Switzerland North",
            0L
          },
          {
            "Switzerland West",
            (long) byte.MaxValue
          },
          {
            "UAE Central",
            (long) byte.MaxValue
          },
          {
            "UAE North",
            (long) byte.MaxValue
          },
          {
            "UK South",
            (long) byte.MaxValue
          },
          {
            "UK West",
            (long) byte.MaxValue
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            (long) byte.MaxValue
          },
          {
            "West Europe",
            (long) byte.MaxValue
          },
          {
            "West India",
            (long) byte.MaxValue
          },
          {
            "West US",
            (long) byte.MaxValue
          },
          {
            "West US 2",
            (long) byte.MaxValue
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "Switzerland West",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            (long) byte.MaxValue
          },
          {
            "Australia Central 2",
            (long) byte.MaxValue
          },
          {
            "Australia East",
            (long) byte.MaxValue
          },
          {
            "Australia Southeast",
            (long) byte.MaxValue
          },
          {
            "Brazil South",
            (long) byte.MaxValue
          },
          {
            "Canada Central",
            (long) byte.MaxValue
          },
          {
            "Canada East",
            (long) byte.MaxValue
          },
          {
            "Central India",
            (long) byte.MaxValue
          },
          {
            "Central US",
            (long) byte.MaxValue
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            (long) byte.MaxValue
          },
          {
            "East US",
            (long) byte.MaxValue
          },
          {
            "East US 2",
            (long) byte.MaxValue
          },
          {
            "France Central",
            (long) byte.MaxValue
          },
          {
            "France South",
            (long) byte.MaxValue
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            (long) byte.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            (long) byte.MaxValue
          },
          {
            "Japan East",
            (long) byte.MaxValue
          },
          {
            "Japan West",
            (long) byte.MaxValue
          },
          {
            "Korea Central",
            (long) byte.MaxValue
          },
          {
            "Korea South",
            (long) byte.MaxValue
          },
          {
            "North Central US",
            (long) byte.MaxValue
          },
          {
            "North Europe",
            (long) byte.MaxValue
          },
          {
            "Norway East",
            (long) byte.MaxValue
          },
          {
            "Norway West",
            (long) byte.MaxValue
          },
          {
            "South Africa North",
            (long) byte.MaxValue
          },
          {
            "South Africa West",
            (long) byte.MaxValue
          },
          {
            "South Central US",
            (long) byte.MaxValue
          },
          {
            "Southeast Asia",
            (long) byte.MaxValue
          },
          {
            "South India",
            (long) byte.MaxValue
          },
          {
            "Switzerland North",
            (long) byte.MaxValue
          },
          {
            "Switzerland West",
            0L
          },
          {
            "UAE Central",
            (long) byte.MaxValue
          },
          {
            "UAE North",
            (long) byte.MaxValue
          },
          {
            "UK South",
            (long) byte.MaxValue
          },
          {
            "UK West",
            (long) byte.MaxValue
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            (long) byte.MaxValue
          },
          {
            "West Europe",
            (long) byte.MaxValue
          },
          {
            "West India",
            (long) byte.MaxValue
          },
          {
            "West US",
            (long) byte.MaxValue
          },
          {
            "West US 2",
            (long) byte.MaxValue
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "UAE Central",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            (long) byte.MaxValue
          },
          {
            "Australia Central 2",
            (long) byte.MaxValue
          },
          {
            "Australia East",
            (long) byte.MaxValue
          },
          {
            "Australia Southeast",
            (long) byte.MaxValue
          },
          {
            "Brazil South",
            (long) byte.MaxValue
          },
          {
            "Canada Central",
            (long) byte.MaxValue
          },
          {
            "Canada East",
            (long) byte.MaxValue
          },
          {
            "Central India",
            (long) byte.MaxValue
          },
          {
            "Central US",
            (long) byte.MaxValue
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            (long) byte.MaxValue
          },
          {
            "East US",
            (long) byte.MaxValue
          },
          {
            "East US 2",
            (long) byte.MaxValue
          },
          {
            "France Central",
            (long) byte.MaxValue
          },
          {
            "France South",
            (long) byte.MaxValue
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            (long) byte.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            (long) byte.MaxValue
          },
          {
            "Japan East",
            (long) byte.MaxValue
          },
          {
            "Japan West",
            (long) byte.MaxValue
          },
          {
            "Korea Central",
            (long) byte.MaxValue
          },
          {
            "Korea South",
            (long) byte.MaxValue
          },
          {
            "North Central US",
            (long) byte.MaxValue
          },
          {
            "North Europe",
            (long) byte.MaxValue
          },
          {
            "Norway East",
            (long) byte.MaxValue
          },
          {
            "Norway West",
            (long) byte.MaxValue
          },
          {
            "South Africa North",
            (long) byte.MaxValue
          },
          {
            "South Africa West",
            (long) byte.MaxValue
          },
          {
            "South Central US",
            (long) byte.MaxValue
          },
          {
            "Southeast Asia",
            (long) byte.MaxValue
          },
          {
            "South India",
            (long) byte.MaxValue
          },
          {
            "Switzerland North",
            (long) byte.MaxValue
          },
          {
            "Switzerland West",
            (long) byte.MaxValue
          },
          {
            "UAE Central",
            0L
          },
          {
            "UAE North",
            (long) byte.MaxValue
          },
          {
            "UK South",
            (long) byte.MaxValue
          },
          {
            "UK West",
            (long) byte.MaxValue
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            (long) byte.MaxValue
          },
          {
            "West Europe",
            (long) byte.MaxValue
          },
          {
            "West India",
            (long) byte.MaxValue
          },
          {
            "West US",
            (long) byte.MaxValue
          },
          {
            "West US 2",
            (long) byte.MaxValue
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "UAE North",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            (long) byte.MaxValue
          },
          {
            "Australia Central 2",
            (long) byte.MaxValue
          },
          {
            "Australia East",
            (long) byte.MaxValue
          },
          {
            "Australia Southeast",
            (long) byte.MaxValue
          },
          {
            "Brazil South",
            (long) byte.MaxValue
          },
          {
            "Canada Central",
            (long) byte.MaxValue
          },
          {
            "Canada East",
            (long) byte.MaxValue
          },
          {
            "Central India",
            (long) byte.MaxValue
          },
          {
            "Central US",
            (long) byte.MaxValue
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            (long) byte.MaxValue
          },
          {
            "East US",
            (long) byte.MaxValue
          },
          {
            "East US 2",
            (long) byte.MaxValue
          },
          {
            "France Central",
            (long) byte.MaxValue
          },
          {
            "France South",
            (long) byte.MaxValue
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            (long) byte.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            (long) byte.MaxValue
          },
          {
            "Japan East",
            (long) byte.MaxValue
          },
          {
            "Japan West",
            (long) byte.MaxValue
          },
          {
            "Korea Central",
            (long) byte.MaxValue
          },
          {
            "Korea South",
            (long) byte.MaxValue
          },
          {
            "North Central US",
            (long) byte.MaxValue
          },
          {
            "North Europe",
            (long) byte.MaxValue
          },
          {
            "Norway East",
            (long) byte.MaxValue
          },
          {
            "Norway West",
            (long) byte.MaxValue
          },
          {
            "South Africa North",
            (long) byte.MaxValue
          },
          {
            "South Africa West",
            (long) byte.MaxValue
          },
          {
            "South Central US",
            (long) byte.MaxValue
          },
          {
            "Southeast Asia",
            (long) byte.MaxValue
          },
          {
            "South India",
            (long) byte.MaxValue
          },
          {
            "Switzerland North",
            (long) byte.MaxValue
          },
          {
            "Switzerland West",
            (long) byte.MaxValue
          },
          {
            "UAE Central",
            (long) byte.MaxValue
          },
          {
            "UAE North",
            0L
          },
          {
            "UK South",
            (long) byte.MaxValue
          },
          {
            "UK West",
            (long) byte.MaxValue
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            (long) byte.MaxValue
          },
          {
            "West Europe",
            (long) byte.MaxValue
          },
          {
            "West India",
            (long) byte.MaxValue
          },
          {
            "West US",
            (long) byte.MaxValue
          },
          {
            "West US 2",
            (long) byte.MaxValue
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "UK South",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            (long) byte.MaxValue
          },
          {
            "Australia Central 2",
            (long) byte.MaxValue
          },
          {
            "Australia East",
            265L
          },
          {
            "Australia Southeast",
            275L
          },
          {
            "Brazil South",
            180L
          },
          {
            "Canada Central",
            85L
          },
          {
            "Canada East",
            95L
          },
          {
            "Central India",
            125L
          },
          {
            "Central US",
            105L
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            200L
          },
          {
            "East US",
            75L
          },
          {
            "East US 2",
            80L
          },
          {
            "France Central",
            20L
          },
          {
            "France South",
            35L
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            (long) byte.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            (long) byte.MaxValue
          },
          {
            "Japan East",
            225L
          },
          {
            "Japan West",
            230L
          },
          {
            "Korea Central",
            260L
          },
          {
            "Korea South",
            265L
          },
          {
            "North Central US",
            95L
          },
          {
            "North Europe",
            10L
          },
          {
            "Norway East",
            (long) byte.MaxValue
          },
          {
            "Norway West",
            (long) byte.MaxValue
          },
          {
            "South Africa North",
            (long) byte.MaxValue
          },
          {
            "South Africa West",
            (long) byte.MaxValue
          },
          {
            "South Central US",
            100L
          },
          {
            "Southeast Asia",
            180L
          },
          {
            "South India",
            145L
          },
          {
            "Switzerland North",
            (long) byte.MaxValue
          },
          {
            "Switzerland West",
            (long) byte.MaxValue
          },
          {
            "UAE Central",
            (long) byte.MaxValue
          },
          {
            "UAE North",
            (long) byte.MaxValue
          },
          {
            "UK South",
            0L
          },
          {
            "UK West",
            5L
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            120L
          },
          {
            "West Europe",
            10L
          },
          {
            "West India",
            120L
          },
          {
            "West US",
            135L
          },
          {
            "West US 2",
            140L
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "UK West",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            (long) byte.MaxValue
          },
          {
            "Australia Central 2",
            (long) byte.MaxValue
          },
          {
            "Australia East",
            270L
          },
          {
            "Australia Southeast",
            280L
          },
          {
            "Brazil South",
            180L
          },
          {
            "Canada Central",
            90L
          },
          {
            "Canada East",
            100L
          },
          {
            "Central India",
            130L
          },
          {
            "Central US",
            105L
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            205L
          },
          {
            "East US",
            75L
          },
          {
            "East US 2",
            80L
          },
          {
            "France Central",
            20L
          },
          {
            "France South",
            40L
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            (long) byte.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            (long) byte.MaxValue
          },
          {
            "Japan East",
            225L
          },
          {
            "Japan West",
            235L
          },
          {
            "Korea Central",
            260L
          },
          {
            "Korea South",
            265L
          },
          {
            "North Central US",
            95L
          },
          {
            "North Europe",
            15L
          },
          {
            "Norway East",
            (long) byte.MaxValue
          },
          {
            "Norway West",
            (long) byte.MaxValue
          },
          {
            "South Africa North",
            (long) byte.MaxValue
          },
          {
            "South Africa West",
            (long) byte.MaxValue
          },
          {
            "South Central US",
            105L
          },
          {
            "Southeast Asia",
            155L
          },
          {
            "South India",
            175L
          },
          {
            "Switzerland North",
            (long) byte.MaxValue
          },
          {
            "Switzerland West",
            (long) byte.MaxValue
          },
          {
            "UAE Central",
            (long) byte.MaxValue
          },
          {
            "UAE North",
            (long) byte.MaxValue
          },
          {
            "UK South",
            5L
          },
          {
            "UK West",
            0L
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            120L
          },
          {
            "West Europe",
            10L
          },
          {
            "West India",
            120L
          },
          {
            "West US",
            140L
          },
          {
            "West US 2",
            145L
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "USDoD Central",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            long.MaxValue
          },
          {
            "Australia Central 2",
            long.MaxValue
          },
          {
            "Australia East",
            long.MaxValue
          },
          {
            "Australia Southeast",
            long.MaxValue
          },
          {
            "Brazil South",
            long.MaxValue
          },
          {
            "Canada Central",
            long.MaxValue
          },
          {
            "Canada East",
            long.MaxValue
          },
          {
            "Central India",
            long.MaxValue
          },
          {
            "Central US",
            long.MaxValue
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            long.MaxValue
          },
          {
            "East US",
            long.MaxValue
          },
          {
            "East US 2",
            long.MaxValue
          },
          {
            "France Central",
            long.MaxValue
          },
          {
            "France South",
            long.MaxValue
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            long.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            long.MaxValue
          },
          {
            "Japan East",
            long.MaxValue
          },
          {
            "Japan West",
            long.MaxValue
          },
          {
            "Korea Central",
            long.MaxValue
          },
          {
            "Korea South",
            long.MaxValue
          },
          {
            "North Central US",
            long.MaxValue
          },
          {
            "North Europe",
            long.MaxValue
          },
          {
            "Norway East",
            long.MaxValue
          },
          {
            "Norway West",
            long.MaxValue
          },
          {
            "South Africa North",
            long.MaxValue
          },
          {
            "South Africa West",
            long.MaxValue
          },
          {
            "South Central US",
            long.MaxValue
          },
          {
            "Southeast Asia",
            long.MaxValue
          },
          {
            "South India",
            long.MaxValue
          },
          {
            "Switzerland North",
            long.MaxValue
          },
          {
            "Switzerland West",
            long.MaxValue
          },
          {
            "UAE Central",
            long.MaxValue
          },
          {
            "UAE North",
            long.MaxValue
          },
          {
            "UK South",
            long.MaxValue
          },
          {
            "UK West",
            long.MaxValue
          },
          {
            "USDoD Central",
            0L
          },
          {
            "USDoD East",
            44L
          },
          {
            "USGov Arizona",
            42L
          },
          {
            "USGov Texas",
            24L
          },
          {
            "USGov Virginia",
            44L
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            long.MaxValue
          },
          {
            "West Europe",
            long.MaxValue
          },
          {
            "West India",
            long.MaxValue
          },
          {
            "West US",
            long.MaxValue
          },
          {
            "West US 2",
            long.MaxValue
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "USDoD East",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            long.MaxValue
          },
          {
            "Australia Central 2",
            long.MaxValue
          },
          {
            "Australia East",
            long.MaxValue
          },
          {
            "Australia Southeast",
            long.MaxValue
          },
          {
            "Brazil South",
            long.MaxValue
          },
          {
            "Canada Central",
            long.MaxValue
          },
          {
            "Canada East",
            long.MaxValue
          },
          {
            "Central India",
            long.MaxValue
          },
          {
            "Central US",
            long.MaxValue
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            long.MaxValue
          },
          {
            "East US",
            long.MaxValue
          },
          {
            "East US 2",
            long.MaxValue
          },
          {
            "France Central",
            long.MaxValue
          },
          {
            "France South",
            long.MaxValue
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            long.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            long.MaxValue
          },
          {
            "Japan East",
            long.MaxValue
          },
          {
            "Japan West",
            long.MaxValue
          },
          {
            "Korea Central",
            long.MaxValue
          },
          {
            "Korea South",
            long.MaxValue
          },
          {
            "North Central US",
            long.MaxValue
          },
          {
            "North Europe",
            long.MaxValue
          },
          {
            "Norway East",
            long.MaxValue
          },
          {
            "Norway West",
            long.MaxValue
          },
          {
            "South Africa North",
            long.MaxValue
          },
          {
            "South Africa West",
            long.MaxValue
          },
          {
            "South Central US",
            long.MaxValue
          },
          {
            "Southeast Asia",
            long.MaxValue
          },
          {
            "South India",
            long.MaxValue
          },
          {
            "Switzerland North",
            long.MaxValue
          },
          {
            "Switzerland West",
            long.MaxValue
          },
          {
            "UAE Central",
            long.MaxValue
          },
          {
            "UAE North",
            long.MaxValue
          },
          {
            "UK South",
            long.MaxValue
          },
          {
            "UK West",
            long.MaxValue
          },
          {
            "USDoD Central",
            44L
          },
          {
            "USDoD East",
            0L
          },
          {
            "USGov Arizona",
            45L
          },
          {
            "USGov Texas",
            27L
          },
          {
            "USGov Virginia",
            2L
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            long.MaxValue
          },
          {
            "West Europe",
            long.MaxValue
          },
          {
            "West India",
            long.MaxValue
          },
          {
            "West US",
            long.MaxValue
          },
          {
            "West US 2",
            long.MaxValue
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "USGov Arizona",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            long.MaxValue
          },
          {
            "Australia Central 2",
            long.MaxValue
          },
          {
            "Australia East",
            long.MaxValue
          },
          {
            "Australia Southeast",
            long.MaxValue
          },
          {
            "Brazil South",
            long.MaxValue
          },
          {
            "Canada Central",
            long.MaxValue
          },
          {
            "Canada East",
            long.MaxValue
          },
          {
            "Central India",
            long.MaxValue
          },
          {
            "Central US",
            long.MaxValue
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            long.MaxValue
          },
          {
            "East US",
            long.MaxValue
          },
          {
            "East US 2",
            long.MaxValue
          },
          {
            "France Central",
            long.MaxValue
          },
          {
            "France South",
            long.MaxValue
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            long.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            long.MaxValue
          },
          {
            "Japan East",
            long.MaxValue
          },
          {
            "Japan West",
            long.MaxValue
          },
          {
            "Korea Central",
            long.MaxValue
          },
          {
            "Korea South",
            long.MaxValue
          },
          {
            "North Central US",
            long.MaxValue
          },
          {
            "North Europe",
            long.MaxValue
          },
          {
            "Norway East",
            long.MaxValue
          },
          {
            "Norway West",
            long.MaxValue
          },
          {
            "South Africa North",
            long.MaxValue
          },
          {
            "South Africa West",
            long.MaxValue
          },
          {
            "South Central US",
            long.MaxValue
          },
          {
            "Southeast Asia",
            long.MaxValue
          },
          {
            "South India",
            long.MaxValue
          },
          {
            "Switzerland North",
            long.MaxValue
          },
          {
            "Switzerland West",
            long.MaxValue
          },
          {
            "UAE Central",
            long.MaxValue
          },
          {
            "UAE North",
            long.MaxValue
          },
          {
            "UK South",
            long.MaxValue
          },
          {
            "UK West",
            long.MaxValue
          },
          {
            "USDoD Central",
            42L
          },
          {
            "USDoD East",
            46L
          },
          {
            "USGov Arizona",
            0L
          },
          {
            "USGov Texas",
            20L
          },
          {
            "USGov Virginia",
            45L
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            long.MaxValue
          },
          {
            "West Europe",
            long.MaxValue
          },
          {
            "West India",
            long.MaxValue
          },
          {
            "West US",
            long.MaxValue
          },
          {
            "West US 2",
            long.MaxValue
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "USGov Texas",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            long.MaxValue
          },
          {
            "Australia Central 2",
            long.MaxValue
          },
          {
            "Australia East",
            long.MaxValue
          },
          {
            "Australia Southeast",
            long.MaxValue
          },
          {
            "Brazil South",
            long.MaxValue
          },
          {
            "Canada Central",
            long.MaxValue
          },
          {
            "Canada East",
            long.MaxValue
          },
          {
            "Central India",
            long.MaxValue
          },
          {
            "Central US",
            long.MaxValue
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            long.MaxValue
          },
          {
            "East US",
            long.MaxValue
          },
          {
            "East US 2",
            long.MaxValue
          },
          {
            "France Central",
            long.MaxValue
          },
          {
            "France South",
            long.MaxValue
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            long.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            long.MaxValue
          },
          {
            "Japan East",
            long.MaxValue
          },
          {
            "Japan West",
            long.MaxValue
          },
          {
            "Korea Central",
            long.MaxValue
          },
          {
            "Korea South",
            long.MaxValue
          },
          {
            "North Central US",
            long.MaxValue
          },
          {
            "North Europe",
            long.MaxValue
          },
          {
            "Norway East",
            long.MaxValue
          },
          {
            "Norway West",
            long.MaxValue
          },
          {
            "South Africa North",
            long.MaxValue
          },
          {
            "South Africa West",
            long.MaxValue
          },
          {
            "South Central US",
            long.MaxValue
          },
          {
            "Southeast Asia",
            long.MaxValue
          },
          {
            "South India",
            long.MaxValue
          },
          {
            "Switzerland North",
            long.MaxValue
          },
          {
            "Switzerland West",
            long.MaxValue
          },
          {
            "UAE Central",
            long.MaxValue
          },
          {
            "UAE North",
            long.MaxValue
          },
          {
            "UK South",
            long.MaxValue
          },
          {
            "UK West",
            long.MaxValue
          },
          {
            "USDoD Central",
            24L
          },
          {
            "USDoD East",
            28L
          },
          {
            "USGov Arizona",
            20L
          },
          {
            "USGov Texas",
            0L
          },
          {
            "USGov Virginia",
            35L
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            long.MaxValue
          },
          {
            "West Europe",
            long.MaxValue
          },
          {
            "West India",
            long.MaxValue
          },
          {
            "West US",
            long.MaxValue
          },
          {
            "West US 2",
            long.MaxValue
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "USGov Virginia",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            long.MaxValue
          },
          {
            "Australia Central 2",
            long.MaxValue
          },
          {
            "Australia East",
            long.MaxValue
          },
          {
            "Australia Southeast",
            long.MaxValue
          },
          {
            "Brazil South",
            long.MaxValue
          },
          {
            "Canada Central",
            long.MaxValue
          },
          {
            "Canada East",
            long.MaxValue
          },
          {
            "Central India",
            long.MaxValue
          },
          {
            "Central US",
            long.MaxValue
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            long.MaxValue
          },
          {
            "East US",
            long.MaxValue
          },
          {
            "East US 2",
            long.MaxValue
          },
          {
            "France Central",
            long.MaxValue
          },
          {
            "France South",
            long.MaxValue
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            long.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            long.MaxValue
          },
          {
            "Japan East",
            long.MaxValue
          },
          {
            "Japan West",
            long.MaxValue
          },
          {
            "Korea Central",
            long.MaxValue
          },
          {
            "Korea South",
            long.MaxValue
          },
          {
            "North Central US",
            long.MaxValue
          },
          {
            "North Europe",
            long.MaxValue
          },
          {
            "Norway East",
            long.MaxValue
          },
          {
            "Norway West",
            long.MaxValue
          },
          {
            "South Africa North",
            long.MaxValue
          },
          {
            "South Africa West",
            long.MaxValue
          },
          {
            "South Central US",
            long.MaxValue
          },
          {
            "Southeast Asia",
            long.MaxValue
          },
          {
            "South India",
            long.MaxValue
          },
          {
            "Switzerland North",
            long.MaxValue
          },
          {
            "Switzerland West",
            long.MaxValue
          },
          {
            "UAE Central",
            long.MaxValue
          },
          {
            "UAE North",
            long.MaxValue
          },
          {
            "UK South",
            long.MaxValue
          },
          {
            "UK West",
            long.MaxValue
          },
          {
            "USDoD Central",
            44L
          },
          {
            "USDoD East",
            2L
          },
          {
            "USGov Arizona",
            45L
          },
          {
            "USGov Texas",
            35L
          },
          {
            "USGov Virginia",
            0L
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            long.MaxValue
          },
          {
            "West Europe",
            long.MaxValue
          },
          {
            "West India",
            long.MaxValue
          },
          {
            "West US",
            long.MaxValue
          },
          {
            "West US 2",
            long.MaxValue
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "USNat East",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            long.MaxValue
          },
          {
            "Australia Central 2",
            long.MaxValue
          },
          {
            "Australia East",
            long.MaxValue
          },
          {
            "Australia Southeast",
            long.MaxValue
          },
          {
            "Brazil South",
            long.MaxValue
          },
          {
            "Canada Central",
            long.MaxValue
          },
          {
            "Canada East",
            long.MaxValue
          },
          {
            "Central India",
            long.MaxValue
          },
          {
            "Central US",
            long.MaxValue
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            long.MaxValue
          },
          {
            "East US",
            long.MaxValue
          },
          {
            "East US 2",
            long.MaxValue
          },
          {
            "France Central",
            long.MaxValue
          },
          {
            "France South",
            long.MaxValue
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            long.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            long.MaxValue
          },
          {
            "Japan East",
            long.MaxValue
          },
          {
            "Japan West",
            long.MaxValue
          },
          {
            "Korea Central",
            long.MaxValue
          },
          {
            "Korea South",
            long.MaxValue
          },
          {
            "North Central US",
            long.MaxValue
          },
          {
            "North Europe",
            long.MaxValue
          },
          {
            "Norway East",
            long.MaxValue
          },
          {
            "Norway West",
            long.MaxValue
          },
          {
            "South Africa North",
            long.MaxValue
          },
          {
            "South Africa West",
            long.MaxValue
          },
          {
            "South Central US",
            long.MaxValue
          },
          {
            "Southeast Asia",
            long.MaxValue
          },
          {
            "South India",
            long.MaxValue
          },
          {
            "Switzerland North",
            long.MaxValue
          },
          {
            "Switzerland West",
            long.MaxValue
          },
          {
            "UAE Central",
            long.MaxValue
          },
          {
            "UAE North",
            long.MaxValue
          },
          {
            "UK South",
            long.MaxValue
          },
          {
            "UK West",
            long.MaxValue
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            0L
          },
          {
            "USNat West",
            (long) byte.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            long.MaxValue
          },
          {
            "West Europe",
            long.MaxValue
          },
          {
            "West India",
            long.MaxValue
          },
          {
            "West US",
            long.MaxValue
          },
          {
            "West US 2",
            long.MaxValue
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "USNat West",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            long.MaxValue
          },
          {
            "Australia Central 2",
            long.MaxValue
          },
          {
            "Australia East",
            long.MaxValue
          },
          {
            "Australia Southeast",
            long.MaxValue
          },
          {
            "Brazil South",
            long.MaxValue
          },
          {
            "Canada Central",
            long.MaxValue
          },
          {
            "Canada East",
            long.MaxValue
          },
          {
            "Central India",
            long.MaxValue
          },
          {
            "Central US",
            long.MaxValue
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            long.MaxValue
          },
          {
            "East US",
            long.MaxValue
          },
          {
            "East US 2",
            long.MaxValue
          },
          {
            "France Central",
            long.MaxValue
          },
          {
            "France South",
            long.MaxValue
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            long.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            long.MaxValue
          },
          {
            "Japan East",
            long.MaxValue
          },
          {
            "Japan West",
            long.MaxValue
          },
          {
            "Korea Central",
            long.MaxValue
          },
          {
            "Korea South",
            long.MaxValue
          },
          {
            "North Central US",
            long.MaxValue
          },
          {
            "North Europe",
            long.MaxValue
          },
          {
            "Norway East",
            long.MaxValue
          },
          {
            "Norway West",
            long.MaxValue
          },
          {
            "South Africa North",
            long.MaxValue
          },
          {
            "South Africa West",
            long.MaxValue
          },
          {
            "South Central US",
            long.MaxValue
          },
          {
            "Southeast Asia",
            long.MaxValue
          },
          {
            "South India",
            long.MaxValue
          },
          {
            "Switzerland North",
            long.MaxValue
          },
          {
            "Switzerland West",
            long.MaxValue
          },
          {
            "UAE Central",
            long.MaxValue
          },
          {
            "UAE North",
            long.MaxValue
          },
          {
            "UK South",
            long.MaxValue
          },
          {
            "UK West",
            long.MaxValue
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            (long) byte.MaxValue
          },
          {
            "USNat West",
            0L
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            long.MaxValue
          },
          {
            "West Europe",
            long.MaxValue
          },
          {
            "West India",
            long.MaxValue
          },
          {
            "West US",
            long.MaxValue
          },
          {
            "West US 2",
            long.MaxValue
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "USSec East",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            long.MaxValue
          },
          {
            "Australia Central 2",
            long.MaxValue
          },
          {
            "Australia East",
            long.MaxValue
          },
          {
            "Australia Southeast",
            long.MaxValue
          },
          {
            "Brazil South",
            long.MaxValue
          },
          {
            "Canada Central",
            long.MaxValue
          },
          {
            "Canada East",
            long.MaxValue
          },
          {
            "Central India",
            long.MaxValue
          },
          {
            "Central US",
            long.MaxValue
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            long.MaxValue
          },
          {
            "East US",
            long.MaxValue
          },
          {
            "East US 2",
            long.MaxValue
          },
          {
            "France Central",
            long.MaxValue
          },
          {
            "France South",
            long.MaxValue
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            long.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            long.MaxValue
          },
          {
            "Japan East",
            long.MaxValue
          },
          {
            "Japan West",
            long.MaxValue
          },
          {
            "Korea Central",
            long.MaxValue
          },
          {
            "Korea South",
            long.MaxValue
          },
          {
            "North Central US",
            long.MaxValue
          },
          {
            "North Europe",
            long.MaxValue
          },
          {
            "Norway East",
            long.MaxValue
          },
          {
            "Norway West",
            long.MaxValue
          },
          {
            "South Africa North",
            long.MaxValue
          },
          {
            "South Africa West",
            long.MaxValue
          },
          {
            "South Central US",
            long.MaxValue
          },
          {
            "Southeast Asia",
            long.MaxValue
          },
          {
            "South India",
            long.MaxValue
          },
          {
            "Switzerland North",
            long.MaxValue
          },
          {
            "Switzerland West",
            long.MaxValue
          },
          {
            "UAE Central",
            long.MaxValue
          },
          {
            "UAE North",
            long.MaxValue
          },
          {
            "UK South",
            long.MaxValue
          },
          {
            "UK West",
            long.MaxValue
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            0L
          },
          {
            "USSec West",
            (long) byte.MaxValue
          },
          {
            "West Central US",
            long.MaxValue
          },
          {
            "West Europe",
            long.MaxValue
          },
          {
            "West India",
            long.MaxValue
          },
          {
            "West US",
            long.MaxValue
          },
          {
            "West US 2",
            long.MaxValue
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "USSec West",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            long.MaxValue
          },
          {
            "Australia Central 2",
            long.MaxValue
          },
          {
            "Australia East",
            long.MaxValue
          },
          {
            "Australia Southeast",
            long.MaxValue
          },
          {
            "Brazil South",
            long.MaxValue
          },
          {
            "Canada Central",
            long.MaxValue
          },
          {
            "Canada East",
            long.MaxValue
          },
          {
            "Central India",
            long.MaxValue
          },
          {
            "Central US",
            long.MaxValue
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            long.MaxValue
          },
          {
            "East US",
            long.MaxValue
          },
          {
            "East US 2",
            long.MaxValue
          },
          {
            "France Central",
            long.MaxValue
          },
          {
            "France South",
            long.MaxValue
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            long.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            long.MaxValue
          },
          {
            "Japan East",
            long.MaxValue
          },
          {
            "Japan West",
            long.MaxValue
          },
          {
            "Korea Central",
            long.MaxValue
          },
          {
            "Korea South",
            long.MaxValue
          },
          {
            "North Central US",
            long.MaxValue
          },
          {
            "North Europe",
            long.MaxValue
          },
          {
            "Norway East",
            long.MaxValue
          },
          {
            "Norway West",
            long.MaxValue
          },
          {
            "South Africa North",
            long.MaxValue
          },
          {
            "South Africa West",
            long.MaxValue
          },
          {
            "South Central US",
            long.MaxValue
          },
          {
            "Southeast Asia",
            long.MaxValue
          },
          {
            "South India",
            long.MaxValue
          },
          {
            "Switzerland North",
            long.MaxValue
          },
          {
            "Switzerland West",
            long.MaxValue
          },
          {
            "UAE Central",
            long.MaxValue
          },
          {
            "UAE North",
            long.MaxValue
          },
          {
            "UK South",
            long.MaxValue
          },
          {
            "UK West",
            long.MaxValue
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            (long) byte.MaxValue
          },
          {
            "USSec West",
            0L
          },
          {
            "West Central US",
            long.MaxValue
          },
          {
            "West Europe",
            long.MaxValue
          },
          {
            "West India",
            long.MaxValue
          },
          {
            "West US",
            long.MaxValue
          },
          {
            "West US 2",
            long.MaxValue
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "West Central US",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            (long) byte.MaxValue
          },
          {
            "Australia Central 2",
            (long) byte.MaxValue
          },
          {
            "Australia East",
            175L
          },
          {
            "Australia Southeast",
            185L
          },
          {
            "Brazil South",
            165L
          },
          {
            "Canada Central",
            45L
          },
          {
            "Canada East",
            55L
          },
          {
            "Central India",
            (long) byte.MaxValue
          },
          {
            "Central US",
            15L
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            180L
          },
          {
            "East US",
            60L
          },
          {
            "East US 2",
            55L
          },
          {
            "France Central",
            130L
          },
          {
            "France South",
            140L
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            (long) byte.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            (long) byte.MaxValue
          },
          {
            "Japan East",
            130L
          },
          {
            "Japan West",
            140L
          },
          {
            "Korea Central",
            165L
          },
          {
            "Korea South",
            170L
          },
          {
            "North Central US",
            30L
          },
          {
            "North Europe",
            115L
          },
          {
            "Norway East",
            (long) byte.MaxValue
          },
          {
            "Norway West",
            (long) byte.MaxValue
          },
          {
            "South Africa North",
            (long) byte.MaxValue
          },
          {
            "South Africa West",
            (long) byte.MaxValue
          },
          {
            "South Central US",
            25L
          },
          {
            "Southeast Asia",
            200L
          },
          {
            "South India",
            230L
          },
          {
            "Switzerland North",
            (long) byte.MaxValue
          },
          {
            "Switzerland West",
            (long) byte.MaxValue
          },
          {
            "UAE Central",
            (long) byte.MaxValue
          },
          {
            "UAE North",
            (long) byte.MaxValue
          },
          {
            "UK South",
            120L
          },
          {
            "UK West",
            120L
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            0L
          },
          {
            "West Europe",
            135L
          },
          {
            "West India",
            (long) byte.MaxValue
          },
          {
            "West US",
            30L
          },
          {
            "West US 2",
            25L
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "West Europe",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            (long) byte.MaxValue
          },
          {
            "Australia Central 2",
            (long) byte.MaxValue
          },
          {
            "Australia East",
            275L
          },
          {
            "Australia Southeast",
            285L
          },
          {
            "Brazil South",
            190L
          },
          {
            "Canada Central",
            95L
          },
          {
            "Canada East",
            110L
          },
          {
            "Central India",
            130L
          },
          {
            "Central US",
            125L
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            215L
          },
          {
            "East US",
            135L
          },
          {
            "East US 2",
            135L
          },
          {
            "France Central",
            20L
          },
          {
            "France South",
            30L
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            (long) byte.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            (long) byte.MaxValue
          },
          {
            "Japan East",
            230L
          },
          {
            "Japan West",
            240L
          },
          {
            "Korea Central",
            265L
          },
          {
            "Korea South",
            270L
          },
          {
            "North Central US",
            100L
          },
          {
            "North Europe",
            20L
          },
          {
            "Norway East",
            (long) byte.MaxValue
          },
          {
            "Norway West",
            (long) byte.MaxValue
          },
          {
            "South Africa North",
            (long) byte.MaxValue
          },
          {
            "South Africa West",
            (long) byte.MaxValue
          },
          {
            "South Central US",
            145L
          },
          {
            "Southeast Asia",
            180L
          },
          {
            "South India",
            150L
          },
          {
            "Switzerland North",
            (long) byte.MaxValue
          },
          {
            "Switzerland West",
            (long) byte.MaxValue
          },
          {
            "UAE Central",
            (long) byte.MaxValue
          },
          {
            "UAE North",
            (long) byte.MaxValue
          },
          {
            "UK South",
            10L
          },
          {
            "UK West",
            10L
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            135L
          },
          {
            "West Europe",
            0L
          },
          {
            "West India",
            120L
          },
          {
            "West US",
            165L
          },
          {
            "West US 2",
            155L
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "West India",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            (long) byte.MaxValue
          },
          {
            "Australia Central 2",
            (long) byte.MaxValue
          },
          {
            "Australia East",
            230L
          },
          {
            "Australia Southeast",
            235L
          },
          {
            "Brazil South",
            295L
          },
          {
            "Canada Central",
            210L
          },
          {
            "Canada East",
            220L
          },
          {
            "Central India",
            5L
          },
          {
            "Central US",
            230L
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            85L
          },
          {
            "East US",
            190L
          },
          {
            "East US 2",
            190L
          },
          {
            "France Central",
            110L
          },
          {
            "France South",
            105L
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            (long) byte.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            (long) byte.MaxValue
          },
          {
            "Japan East",
            120L
          },
          {
            "Japan West",
            130L
          },
          {
            "Korea Central",
            125L
          },
          {
            "Korea South",
            130L
          },
          {
            "North Central US",
            200L
          },
          {
            "North Europe",
            130L
          },
          {
            "Norway East",
            (long) byte.MaxValue
          },
          {
            "Norway West",
            (long) byte.MaxValue
          },
          {
            "South Africa North",
            (long) byte.MaxValue
          },
          {
            "South Africa West",
            (long) byte.MaxValue
          },
          {
            "South Central US",
            245L
          },
          {
            "Southeast Asia",
            55L
          },
          {
            "South India",
            25L
          },
          {
            "Switzerland North",
            (long) byte.MaxValue
          },
          {
            "Switzerland West",
            (long) byte.MaxValue
          },
          {
            "UAE Central",
            (long) byte.MaxValue
          },
          {
            "UAE North",
            (long) byte.MaxValue
          },
          {
            "UK South",
            120L
          },
          {
            "UK West",
            120L
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            (long) byte.MaxValue
          },
          {
            "West Europe",
            120L
          },
          {
            "West India",
            0L
          },
          {
            "West US",
            225L
          },
          {
            "West US 2",
            230L
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "West US",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            (long) byte.MaxValue
          },
          {
            "Australia Central 2",
            (long) byte.MaxValue
          },
          {
            "Australia East",
            145L
          },
          {
            "Australia Southeast",
            155L
          },
          {
            "Brazil South",
            170L
          },
          {
            "Canada Central",
            65L
          },
          {
            "Canada East",
            75L
          },
          {
            "Central India",
            230L
          },
          {
            "Central US",
            45L
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            155L
          },
          {
            "East US",
            70L
          },
          {
            "East US 2",
            65L
          },
          {
            "France Central",
            150L
          },
          {
            "France South",
            160L
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            (long) byte.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            (long) byte.MaxValue
          },
          {
            "Japan East",
            105L
          },
          {
            "Japan West",
            115L
          },
          {
            "Korea Central",
            140L
          },
          {
            "Korea South",
            145L
          },
          {
            "North Central US",
            55L
          },
          {
            "North Europe",
            140L
          },
          {
            "Norway East",
            (long) byte.MaxValue
          },
          {
            "Norway West",
            (long) byte.MaxValue
          },
          {
            "South Africa North",
            (long) byte.MaxValue
          },
          {
            "South Africa West",
            (long) byte.MaxValue
          },
          {
            "South Central US",
            35L
          },
          {
            "Southeast Asia",
            170L
          },
          {
            "South India",
            205L
          },
          {
            "Switzerland North",
            (long) byte.MaxValue
          },
          {
            "Switzerland West",
            (long) byte.MaxValue
          },
          {
            "UAE Central",
            (long) byte.MaxValue
          },
          {
            "UAE North",
            (long) byte.MaxValue
          },
          {
            "UK South",
            135L
          },
          {
            "UK West",
            140L
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            30L
          },
          {
            "West Europe",
            165L
          },
          {
            "West India",
            225L
          },
          {
            "West US",
            0L
          },
          {
            "West US 2",
            25L
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "West US 2",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            (long) byte.MaxValue
          },
          {
            "Australia Central 2",
            (long) byte.MaxValue
          },
          {
            "Australia East",
            170L
          },
          {
            "Australia Southeast",
            180L
          },
          {
            "Brazil South",
            185L
          },
          {
            "Canada Central",
            60L
          },
          {
            "Canada East",
            75L
          },
          {
            "Central India",
            235L
          },
          {
            "Central US",
            35L
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            170L
          },
          {
            "East US",
            80L
          },
          {
            "East US 2",
            75L
          },
          {
            "France Central",
            150L
          },
          {
            "France South",
            160L
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            (long) byte.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            (long) byte.MaxValue
          },
          {
            "Japan East",
            110L
          },
          {
            "Japan West",
            120L
          },
          {
            "Korea Central",
            145L
          },
          {
            "Korea South",
            150L
          },
          {
            "North Central US",
            50L
          },
          {
            "North Europe",
            135L
          },
          {
            "Norway East",
            (long) byte.MaxValue
          },
          {
            "Norway West",
            (long) byte.MaxValue
          },
          {
            "South Africa North",
            (long) byte.MaxValue
          },
          {
            "South Africa West",
            (long) byte.MaxValue
          },
          {
            "South Central US",
            45L
          },
          {
            "Southeast Asia",
            175L
          },
          {
            "South India",
            200L
          },
          {
            "Switzerland North",
            (long) byte.MaxValue
          },
          {
            "Switzerland West",
            (long) byte.MaxValue
          },
          {
            "UAE Central",
            (long) byte.MaxValue
          },
          {
            "UAE North",
            (long) byte.MaxValue
          },
          {
            "UK South",
            140L
          },
          {
            "UK West",
            145L
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            25L
          },
          {
            "West Europe",
            155L
          },
          {
            "West India",
            230L
          },
          {
            "West US",
            25L
          },
          {
            "West US 2",
            0L
          },
          {
            "Central US EUAP",
            long.MaxValue
          },
          {
            "East US 2 EUAP",
            long.MaxValue
          }
        }
      },
      {
        "Central US EUAP",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            long.MaxValue
          },
          {
            "Australia Central 2",
            long.MaxValue
          },
          {
            "Australia East",
            long.MaxValue
          },
          {
            "Australia Southeast",
            long.MaxValue
          },
          {
            "Brazil South",
            long.MaxValue
          },
          {
            "Canada Central",
            long.MaxValue
          },
          {
            "Canada East",
            long.MaxValue
          },
          {
            "Central India",
            long.MaxValue
          },
          {
            "Central US",
            long.MaxValue
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            long.MaxValue
          },
          {
            "East US",
            long.MaxValue
          },
          {
            "East US 2",
            long.MaxValue
          },
          {
            "France Central",
            long.MaxValue
          },
          {
            "France South",
            long.MaxValue
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            long.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            long.MaxValue
          },
          {
            "Japan East",
            long.MaxValue
          },
          {
            "Japan West",
            long.MaxValue
          },
          {
            "Korea Central",
            long.MaxValue
          },
          {
            "Korea South",
            long.MaxValue
          },
          {
            "North Central US",
            long.MaxValue
          },
          {
            "North Europe",
            long.MaxValue
          },
          {
            "Norway East",
            long.MaxValue
          },
          {
            "Norway West",
            long.MaxValue
          },
          {
            "South Africa North",
            long.MaxValue
          },
          {
            "South Africa West",
            long.MaxValue
          },
          {
            "South Central US",
            long.MaxValue
          },
          {
            "Southeast Asia",
            long.MaxValue
          },
          {
            "South India",
            long.MaxValue
          },
          {
            "Switzerland North",
            long.MaxValue
          },
          {
            "Switzerland West",
            long.MaxValue
          },
          {
            "UAE Central",
            long.MaxValue
          },
          {
            "UAE North",
            long.MaxValue
          },
          {
            "UK South",
            long.MaxValue
          },
          {
            "UK West",
            long.MaxValue
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            long.MaxValue
          },
          {
            "West Europe",
            long.MaxValue
          },
          {
            "West India",
            long.MaxValue
          },
          {
            "West US",
            long.MaxValue
          },
          {
            "West US 2",
            long.MaxValue
          },
          {
            "Central US EUAP",
            0L
          },
          {
            "East US 2 EUAP",
            40L
          }
        }
      },
      {
        "East US 2 EUAP",
        new Dictionary<string, long>()
        {
          {
            "Australia Central",
            long.MaxValue
          },
          {
            "Australia Central 2",
            long.MaxValue
          },
          {
            "Australia East",
            long.MaxValue
          },
          {
            "Australia Southeast",
            long.MaxValue
          },
          {
            "Brazil South",
            long.MaxValue
          },
          {
            "Canada Central",
            long.MaxValue
          },
          {
            "Canada East",
            long.MaxValue
          },
          {
            "Central India",
            long.MaxValue
          },
          {
            "Central US",
            long.MaxValue
          },
          {
            "China East",
            long.MaxValue
          },
          {
            "China East 2",
            long.MaxValue
          },
          {
            "China North",
            long.MaxValue
          },
          {
            "China North 2",
            long.MaxValue
          },
          {
            "East Asia",
            long.MaxValue
          },
          {
            "East US",
            long.MaxValue
          },
          {
            "East US 2",
            long.MaxValue
          },
          {
            "France Central",
            long.MaxValue
          },
          {
            "France South",
            long.MaxValue
          },
          {
            "Germany Central",
            long.MaxValue
          },
          {
            "Germany North",
            long.MaxValue
          },
          {
            "Germany Northeast",
            long.MaxValue
          },
          {
            "Germany West Central",
            long.MaxValue
          },
          {
            "Japan East",
            long.MaxValue
          },
          {
            "Japan West",
            long.MaxValue
          },
          {
            "Korea Central",
            long.MaxValue
          },
          {
            "Korea South",
            long.MaxValue
          },
          {
            "North Central US",
            long.MaxValue
          },
          {
            "North Europe",
            long.MaxValue
          },
          {
            "Norway East",
            long.MaxValue
          },
          {
            "Norway West",
            long.MaxValue
          },
          {
            "South Africa North",
            long.MaxValue
          },
          {
            "South Africa West",
            long.MaxValue
          },
          {
            "South Central US",
            long.MaxValue
          },
          {
            "Southeast Asia",
            long.MaxValue
          },
          {
            "South India",
            long.MaxValue
          },
          {
            "Switzerland North",
            long.MaxValue
          },
          {
            "Switzerland West",
            long.MaxValue
          },
          {
            "UAE Central",
            long.MaxValue
          },
          {
            "UAE North",
            long.MaxValue
          },
          {
            "UK South",
            long.MaxValue
          },
          {
            "UK West",
            long.MaxValue
          },
          {
            "USDoD Central",
            long.MaxValue
          },
          {
            "USDoD East",
            long.MaxValue
          },
          {
            "USGov Arizona",
            long.MaxValue
          },
          {
            "USGov Texas",
            long.MaxValue
          },
          {
            "USGov Virginia",
            long.MaxValue
          },
          {
            "USNat East",
            long.MaxValue
          },
          {
            "USNat West",
            long.MaxValue
          },
          {
            "USSec East",
            long.MaxValue
          },
          {
            "USSec West",
            long.MaxValue
          },
          {
            "West Central US",
            long.MaxValue
          },
          {
            "West Europe",
            long.MaxValue
          },
          {
            "West India",
            long.MaxValue
          },
          {
            "West US",
            long.MaxValue
          },
          {
            "West US 2",
            long.MaxValue
          },
          {
            "Central US EUAP",
            40L
          },
          {
            "East US 2 EUAP",
            0L
          }
        }
      }
    };

    public static List<string> GetRegionsForLinkType(
      GeoLinkTypes geoLinkType,
      List<string> existingRegions)
    {
      foreach (string existingRegion in existingRegions)
      {
        if (string.IsNullOrEmpty(existingRegion) || !RegionProximityUtil.SourceRegionToTargetRegionsRTTInMs.ContainsKey(existingRegion))
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "existingRegion {0} is not a valid region.", (object) existingRegion));
      }
      List<List<string>> stringListList = new List<List<string>>();
      foreach (string existingRegion in existingRegions)
        stringListList.Add(RegionProximityUtil.GetRegionsForLinkType(geoLinkType, existingRegion));
      if (stringListList.Count < 1)
        return new List<string>();
      IEnumerable<string> strings = stringListList[0].AsEnumerable<string>();
      for (int index = 1; index < stringListList.Count; ++index)
        strings = strings.Intersect<string>((IEnumerable<string>) stringListList[index]);
      return existingRegions.Except<string>(strings).Any<string>() ? new List<string>() : strings.ToList<string>();
    }

    public static List<string> GetRegionsForLinkType(GeoLinkTypes geoLinkType, string sourceRegion)
    {
      if (string.IsNullOrEmpty(sourceRegion) || !RegionProximityUtil.SourceRegionToTargetRegionsRTTInMs.ContainsKey(sourceRegion))
        throw new ArgumentException("sourceRegion is not a valid region.");
      List<string> stringList = new List<string>();
      long typeThresholdInMs = RegionProximityUtil.GetLinkTypeThresholdInMs(geoLinkType);
      Dictionary<string, long> targetRegionsRttInM = RegionProximityUtil.SourceRegionToTargetRegionsRTTInMs[sourceRegion];
      foreach (string key in targetRegionsRttInM.Keys)
      {
        if (targetRegionsRttInM[key] <= typeThresholdInMs)
          stringList.Add(key);
      }
      return !stringList.Contains(sourceRegion) ? new List<string>() : stringList;
    }

    public static List<string> GeneratePreferredRegionList(string sourceRegion)
    {
      List<KeyValuePair<string, long>> keyValuePairList = !string.IsNullOrEmpty(sourceRegion) && RegionProximityUtil.SourceRegionToTargetRegionsRTTInMs.ContainsKey(sourceRegion) ? RegionProximityUtil.SourceRegionToTargetRegionsRTTInMs[sourceRegion].ToList<KeyValuePair<string, long>>() : throw new ArgumentException("sourceRegion is not a valid region.");
      keyValuePairList.Sort((Comparison<KeyValuePair<string, long>>) ((x, y) => x.Value.CompareTo(y.Value)));
      List<string> preferredRegionList = new List<string>();
      foreach (KeyValuePair<string, long> keyValuePair in keyValuePairList)
        preferredRegionList.Add(keyValuePair.Key);
      return preferredRegionList;
    }

    private static long GetLinkTypeThresholdInMs(GeoLinkTypes geoLinkType)
    {
      switch (geoLinkType)
      {
        case GeoLinkTypes.Strong:
          return 100;
        case GeoLinkTypes.Medium:
          return 200;
        case GeoLinkTypes.Weak:
          return long.MaxValue;
        default:
          throw new ArgumentException("GeoLinkType provided is invalid.");
      }
    }
  }
}
