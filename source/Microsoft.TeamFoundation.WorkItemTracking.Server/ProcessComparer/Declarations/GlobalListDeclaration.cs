// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations.GlobalListDeclaration
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessComparer.Declarations
{
  public class GlobalListDeclaration
  {
    public string Name { get; set; }

    public List<GlobalListItemDeclaration> Items { get; set; } = new List<GlobalListItemDeclaration>();

    public static List<GlobalListDeclaration> ReadGlobalLists(
      XElement globalListsElement,
      Action<string> logError)
    {
      List<GlobalListDeclaration> globalListDeclarationList = new List<GlobalListDeclaration>();
      if (globalListsElement == null)
        return globalListDeclarationList;
      foreach (XElement element1 in globalListsElement.Elements((XName) "GLOBALLIST"))
      {
        GlobalListDeclaration globalListDeclaration = new GlobalListDeclaration()
        {
          Name = Utilities.RequireAttribute(element1, (XName) "name", logError)
        };
        globalListDeclarationList.Add(globalListDeclaration);
        foreach (XElement element2 in element1.Elements((XName) "LISTITEM"))
        {
          GlobalListItemDeclaration listItemDeclaration = new GlobalListItemDeclaration()
          {
            Value = Utilities.RequireAttribute(element2, (XName) "value", logError)
          };
          globalListDeclaration.Items.Add(listItemDeclaration);
        }
      }
      return globalListDeclarationList;
    }
  }
}
