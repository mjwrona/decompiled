// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.PropertiesCollectionPatchHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class PropertiesCollectionPatchHelper
  {
    public static readonly char PathStartChar = '/';

    public static PropertiesCollection ReadPatchDocument(
      IPatchDocument<IDictionary<string, object>> document)
    {
      ArgumentUtility.CheckForNull<IPatchDocument<IDictionary<string, object>>>(document, nameof (document));
      IEnumerable<IPatchOperation<IDictionary<string, object>>> operations1 = document.Operations;
      IEnumerable<IPatchOperation<IDictionary<string, object>>> patchOperations1 = operations1 != null ? operations1.Where<IPatchOperation<IDictionary<string, object>>>((Func<IPatchOperation<IDictionary<string, object>>, bool>) (x => x.Operation == Operation.Add)) : (IEnumerable<IPatchOperation<IDictionary<string, object>>>) null;
      IEnumerable<IPatchOperation<IDictionary<string, object>>> operations2 = document.Operations;
      IEnumerable<IPatchOperation<IDictionary<string, object>>> patchOperations2 = operations2 != null ? operations2.Where<IPatchOperation<IDictionary<string, object>>>((Func<IPatchOperation<IDictionary<string, object>>, bool>) (x => x.Operation == Operation.Replace)) : (IEnumerable<IPatchOperation<IDictionary<string, object>>>) null;
      IEnumerable<IPatchOperation<IDictionary<string, object>>> operations3 = document.Operations;
      IEnumerable<IPatchOperation<IDictionary<string, object>>> patchOperations3 = operations3 != null ? operations3.Where<IPatchOperation<IDictionary<string, object>>>((Func<IPatchOperation<IDictionary<string, object>>, bool>) (x => x.Operation == Operation.Remove)) : (IEnumerable<IPatchOperation<IDictionary<string, object>>>) null;
      IEnumerable<IPatchOperation<IDictionary<string, object>>> operations4 = document.Operations;
      IEnumerable<IPatchOperation<IDictionary<string, object>>> source = operations4 != null ? operations4.Where<IPatchOperation<IDictionary<string, object>>>((Func<IPatchOperation<IDictionary<string, object>>, bool>) (x => x.Operation != Operation.Remove && x.Operation != Operation.Add && x.Operation != Operation.Replace)) : (IEnumerable<IPatchOperation<IDictionary<string, object>>>) null;
      if (document.Operations == null || !patchOperations1.Any<IPatchOperation<IDictionary<string, object>>>() && !patchOperations2.Any<IPatchOperation<IDictionary<string, object>>>() && !patchOperations3.Any<IPatchOperation<IDictionary<string, object>>>() || source != null && source.Any<IPatchOperation<IDictionary<string, object>>>())
        throw new PropertiesCollectionPatchException(FrameworkResources.InvalidPropertiesPatchOperationException());
      return PropertiesCollectionPatchHelper.PreparePropertiesCollection(patchOperations1, patchOperations2, patchOperations3);
    }

    private static PropertiesCollection PreparePropertiesCollection(
      IEnumerable<IPatchOperation<IDictionary<string, object>>> addOperations,
      IEnumerable<IPatchOperation<IDictionary<string, object>>> replaceOperations,
      IEnumerable<IPatchOperation<IDictionary<string, object>>> removeOperations)
    {
      PropertiesCollection propertiesCollection = new PropertiesCollection();
      foreach (IPatchOperation<IDictionary<string, object>> addOperation in addOperations)
      {
        if (string.Equals(addOperation.Path, string.Empty))
        {
          foreach (KeyValuePair<string, object> keyValuePair in (IEnumerable<KeyValuePair<string, object>>) (addOperation.Value as IDictionary<string, object>))
            propertiesCollection.Add(keyValuePair.Key, keyValuePair.Value);
        }
        else
          propertiesCollection.Add(addOperation.Path.TrimStart(PropertiesCollectionPatchHelper.PathStartChar), addOperation.Value);
      }
      foreach (IPatchOperation<IDictionary<string, object>> replaceOperation in replaceOperations)
      {
        if (string.Equals(replaceOperation.Path, string.Empty))
          throw new PropertiesCollectionPatchException(FrameworkResources.PropertiesReplacePatchPathCannotBeEmpty());
        propertiesCollection.Add(replaceOperation.Path.TrimStart(PropertiesCollectionPatchHelper.PathStartChar), replaceOperation.Value);
      }
      foreach (IPatchOperation<IDictionary<string, object>> removeOperation in removeOperations)
      {
        if (string.Equals(removeOperation.Path, string.Empty))
          throw new PropertiesCollectionPatchException(FrameworkResources.PropertiesRemovePatchPathCannotBeEmpty());
        propertiesCollection.Add(removeOperation.Path.TrimStart(PropertiesCollectionPatchHelper.PathStartChar), (object) null);
      }
      return propertiesCollection;
    }
  }
}
