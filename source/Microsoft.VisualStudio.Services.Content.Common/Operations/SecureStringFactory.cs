// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.Operations.SecureStringFactory
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;

namespace Microsoft.VisualStudio.Services.Content.Common.Operations
{
  public class SecureStringFactory
  {
    public static SecureString Create(string value)
    {
      SecureString retVal = (SecureString) null;
      if (!string.IsNullOrEmpty(value))
      {
        retVal = new SecureString();
        ((IEnumerable<char>) value.ToCharArray()).ToList<char>().ForEach((Action<char>) (c => retVal.AppendChar(c)));
        GC.Collect();
      }
      return retVal;
    }
  }
}
