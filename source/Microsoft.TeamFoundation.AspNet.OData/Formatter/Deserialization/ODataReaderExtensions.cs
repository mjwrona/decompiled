// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Formatter.Deserialization.ODataReaderExtensions
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData;
using System.Collections.Generic;

namespace Microsoft.AspNet.OData.Formatter.Deserialization
{
  public static class ODataReaderExtensions
  {
    public static ODataItemBase ReadResourceOrResourceSet(this ODataReader reader)
    {
      if (reader == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (reader));
      ODataItemBase odataItemBase1 = (ODataItemBase) null;
      Stack<ODataItemBase> odataItemBaseStack = new Stack<ODataItemBase>();
      while (reader.Read())
      {
        switch (reader.State)
        {
          case ODataReaderState.ResourceSetStart:
            ODataResourceSetWrapper resourceSetWrapper1 = new ODataResourceSetWrapper((ODataResourceSet) reader.Item);
            if (odataItemBaseStack.Count > 0)
              ((ODataNestedResourceInfoWrapper) odataItemBaseStack.Peek()).NestedItems.Add((ODataItemBase) resourceSetWrapper1);
            else
              odataItemBase1 = (ODataItemBase) resourceSetWrapper1;
            odataItemBaseStack.Push((ODataItemBase) resourceSetWrapper1);
            continue;
          case ODataReaderState.ResourceSetEnd:
            odataItemBaseStack.Pop();
            continue;
          case ODataReaderState.ResourceStart:
            ODataResource odataResource = (ODataResource) reader.Item;
            ODataResourceWrapper odataResourceWrapper = (ODataResourceWrapper) null;
            if (odataResource != null)
              odataResourceWrapper = new ODataResourceWrapper(odataResource);
            if (odataItemBaseStack.Count == 0)
            {
              odataItemBase1 = (ODataItemBase) odataResourceWrapper;
            }
            else
            {
              ODataItemBase odataItemBase2 = odataItemBaseStack.Peek();
              if (odataItemBase2 is ODataResourceSetWrapper resourceSetWrapper2)
                resourceSetWrapper2.Resources.Add(odataResourceWrapper);
              else
                ((ODataNestedResourceInfoWrapper) odataItemBase2).NestedItems.Add((ODataItemBase) odataResourceWrapper);
            }
            odataItemBaseStack.Push((ODataItemBase) odataResourceWrapper);
            continue;
          case ODataReaderState.ResourceEnd:
            odataItemBaseStack.Pop();
            continue;
          case ODataReaderState.NestedResourceInfoStart:
            ODataNestedResourceInfoWrapper resourceInfoWrapper = new ODataNestedResourceInfoWrapper((ODataNestedResourceInfo) reader.Item);
            ((ODataResourceWrapper) odataItemBaseStack.Peek()).NestedResourceInfos.Add(resourceInfoWrapper);
            odataItemBaseStack.Push((ODataItemBase) resourceInfoWrapper);
            continue;
          case ODataReaderState.NestedResourceInfoEnd:
            odataItemBaseStack.Pop();
            continue;
          case ODataReaderState.EntityReferenceLink:
            ODataEntityReferenceLinkBase referenceLinkBase = new ODataEntityReferenceLinkBase((ODataEntityReferenceLink) reader.Item);
            ((ODataNestedResourceInfoWrapper) odataItemBaseStack.Peek()).NestedItems.Add((ODataItemBase) referenceLinkBase);
            continue;
          default:
            continue;
        }
      }
      return odataItemBase1;
    }
  }
}
