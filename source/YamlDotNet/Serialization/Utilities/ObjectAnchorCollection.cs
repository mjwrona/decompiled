// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.Utilities.ObjectAnchorCollection
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using YamlDotNet.Core;

namespace YamlDotNet.Serialization.Utilities
{
  internal sealed class ObjectAnchorCollection
  {
    private readonly IDictionary<string, object> objectsByAnchor = (IDictionary<string, object>) new Dictionary<string, object>();
    private readonly IDictionary<object, string> anchorsByObject = (IDictionary<object, string>) new Dictionary<object, string>();

    public void Add(string anchor, object @object)
    {
      this.objectsByAnchor.Add(anchor, @object);
      if (@object == null)
        return;
      this.anchorsByObject.Add(@object, anchor);
    }

    public bool TryGetAnchor(object @object, out string anchor) => this.anchorsByObject.TryGetValue(@object, out anchor);

    public object this[string anchor]
    {
      get
      {
        object obj;
        if (this.objectsByAnchor.TryGetValue(anchor, out obj))
          return obj;
        throw new AnchorNotFoundException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "The anchor '{0}' does not exists", new object[1]
        {
          (object) anchor
        }));
      }
    }
  }
}
