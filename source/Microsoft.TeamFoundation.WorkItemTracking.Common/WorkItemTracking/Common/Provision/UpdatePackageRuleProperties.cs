// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Provision.UpdatePackageRuleProperties
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System;
using System.Collections;
using System.ComponentModel;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Provision
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  public class UpdatePackageRuleProperties : Hashtable
  {
    public UpdatePackageRuleProperties()
      : base(20)
    {
    }

    public void SetProperties(XmlElement elt)
    {
      foreach (string key in (IEnumerable) this.Keys)
      {
        object obj;
        if ((obj = this[(object) key]) is bool)
          obj = (object) ((bool) this[(object) key] ? 1 : 0);
        elt.SetAttribute(key, obj.ToString());
      }
    }
  }
}
