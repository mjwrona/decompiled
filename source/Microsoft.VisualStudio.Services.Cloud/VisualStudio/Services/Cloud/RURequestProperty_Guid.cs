// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.RURequestProperty_Guid
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public abstract class RURequestProperty_Guid : RURequestProperty
  {
    public override object ConvertType(string stringValue)
    {
      Guid result;
      if (!Guid.TryParse(stringValue, out result))
        throw new VssServiceException("Could not parse '" + stringValue + "' as Guid for " + this.GetName());
      return (object) result;
    }
  }
}
