// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Settings.Legacy.BugsBehaviorTranslator
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Settings.Legacy
{
  internal class BugsBehaviorTranslator
  {
    public static Property[] TranslateProperties(Property[] properties)
    {
      Property bugsBehaviorProperty = BugsBehaviorTranslator.ChooseBugsBehaviorProperty(properties);
      return BugsBehaviorTranslator.RewriteProperties(properties, bugsBehaviorProperty);
    }

    private static Property ChooseBugsBehaviorProperty(Property[] existingProperties)
    {
      Property property1 = new Property()
      {
        Name = "BugsBehavior",
        Value = "Off"
      };
      if (existingProperties != null)
      {
        Property property2 = Array.Find<Property>(existingProperties, (Predicate<Property>) (p => VssStringComparer.PropertyName.Equals(p.Name, "BugsBehavior")));
        if (property2 != null)
        {
          property1.Value = property2.Value;
        }
        else
        {
          Property property3 = Array.Find<Property>(existingProperties, (Predicate<Property>) (p => VssStringComparer.PropertyName.Equals(p.Name, "ShowBugsOnBacklog")));
          bool result;
          if (property3 != null && bool.TryParse(property3.Value, out result) & result)
            property1.Value = "AsRequirements";
        }
      }
      return property1;
    }

    private static Property[] RewriteProperties(
      Property[] existingProperties,
      Property bugsBehaviorProperty)
    {
      List<Property> propertyList = new List<Property>((IEnumerable<Property>) (existingProperties ?? new Property[0]));
      string[] propertyNameBlacklist = new string[4]
      {
        "BugsBehavior",
        "ShowBugsOnBacklog",
        "ShowBugsInIterations",
        "BugsInIterationsBehavior"
      };
      propertyList.RemoveAll((Predicate<Property>) (property => ((IEnumerable<string>) propertyNameBlacklist).Contains<string>(property.Name, (IEqualityComparer<string>) VssStringComparer.PropertyName)));
      propertyList.Add(bugsBehaviorProperty);
      return propertyList.ToArray();
    }
  }
}
