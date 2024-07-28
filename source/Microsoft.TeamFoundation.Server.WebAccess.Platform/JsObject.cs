// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.JsObject
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [Serializable]
  public class JsObject : Dictionary<string, object>
  {
    public JsObject()
    {
    }

    protected JsObject(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public void AddObject(JsObject data)
    {
      ArgumentUtility.CheckForNull<JsObject>(data, nameof (data));
      foreach (KeyValuePair<string, object> keyValuePair in (Dictionary<string, object>) data)
        this[keyValuePair.Key] = !this.ContainsKey(keyValuePair.Key) ? keyValuePair.Value : throw new InvalidOperationException("Duplicate entry found.  Key = " + keyValuePair.Key);
    }
  }
}
