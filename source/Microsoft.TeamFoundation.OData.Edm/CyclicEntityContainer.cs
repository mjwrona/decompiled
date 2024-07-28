// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Edm.CyclicEntityContainer
// Assembly: Microsoft.TeamFoundation.OData.Edm, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C95BF5-2544-413A-959F-F9F18C11D35F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Edm.dll

using Microsoft.OData.Edm.Validation;
using System.Collections.Generic;

namespace Microsoft.OData.Edm
{
  internal class CyclicEntityContainer : BadEntityContainer
  {
    public CyclicEntityContainer(string name, EdmLocation location)
      : base(name, (IEnumerable<EdmError>) new EdmError[1]
      {
        new EdmError(location, EdmErrorCode.BadCyclicEntityContainer, Strings.Bad_CyclicEntityContainer((object) name))
      })
    {
    }
  }
}
