// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Provision.CategoryHelper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Provision
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class CategoryHelper
  {
    public static XmlDocument Export<T>(
      IEnumerable<T> categories,
      Func<T, string> getReferenceName,
      Func<T, string> getDisplayName,
      Func<T, string> getDefaultType,
      Func<T, IEnumerable<string>> getTypes)
    {
      XmlDocument xmlDocument = new XmlDocument();
      string qualifiedName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}:{1}", (object) ProvisionTags.CategoriesPrefix, (object) ProvisionTags.Categories);
      XmlElement element1 = xmlDocument.CreateElement(qualifiedName, ProvisionValues.CategoriesNamespace);
      xmlDocument.AppendChild((XmlNode) element1);
      foreach (T category in categories)
      {
        XmlElement element2 = element1.OwnerDocument.CreateElement(ProvisionTags.Category);
        element2.SetAttribute(ProvisionAttributes.ReferenceName, getReferenceName(category));
        element2.SetAttribute(ProvisionAttributes.FriendlyName, getDisplayName(category));
        XmlElement element3 = element1.OwnerDocument.CreateElement(ProvisionTags.DefaultWorkItemTypeReference);
        string y = getDefaultType(category);
        element3.SetAttribute(ProvisionAttributes.WorkItemTypeName, y);
        element2.AppendChild((XmlNode) element3);
        foreach (string x in getTypes(category))
        {
          if (!TFStringComparer.WorkItemTypeName.Equals(x, y))
          {
            XmlElement element4 = element1.OwnerDocument.CreateElement(ProvisionTags.WorkItemTypeReference);
            element4.SetAttribute(ProvisionAttributes.WorkItemTypeName, x);
            element2.AppendChild((XmlNode) element4);
          }
        }
        element1.AppendChild((XmlNode) element2);
      }
      return xmlDocument;
    }
  }
}
