// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.RURequestProperty_Enum`1
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public abstract class RURequestProperty_Enum<T> : RURequestProperty where T : struct, Enum
  {
    public override object ConvertType(string stringValue)
    {
      T result;
      if (!Enum.TryParse<T>(stringValue, out result))
        throw new VssServiceException(string.Format("Could not parse '{0}' as {1} for {2}", (object) stringValue, (object) typeof (T), (object) this.GetName()));
      return (object) Convert.ToByte((object) result);
    }
  }
}
