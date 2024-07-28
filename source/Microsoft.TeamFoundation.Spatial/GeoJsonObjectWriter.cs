// Decompiled with JetBrains decompiler
// Type: Microsoft.Spatial.GeoJsonObjectWriter
// Assembly: Microsoft.TeamFoundation.Spatial, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0A67B35E-CAC5-4EE7-B20E-595AE5324896
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Spatial.dll

using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.Spatial
{
  internal sealed class GeoJsonObjectWriter : GeoJsonWriterBase
  {
    private readonly Stack<object> containers = new Stack<object>();
    private string currentPropertyName;
    private object lastCompletedObject;

    internal IDictionary<string, object> JsonObject => this.lastCompletedObject as IDictionary<string, object>;

    private bool IsArray => this.containers.Peek() is IList;

    protected override void StartObjectScope()
    {
      object jsonObject = (object) new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.Ordinal);
      if (this.containers.Count > 0)
        this.AddToScope(jsonObject);
      this.containers.Push(jsonObject);
    }

    protected override void StartArrayScope()
    {
      object jsonObject = (object) new List<object>();
      this.AddToScope(jsonObject);
      this.containers.Push(jsonObject);
    }

    protected override void AddPropertyName(string name) => this.currentPropertyName = name;

    protected override void AddValue(string value) => this.AddToScope((object) value);

    protected override void AddValue(double value) => this.AddToScope((object) value);

    protected override void EndArrayScope() => this.containers.Pop();

    protected override void EndObjectScope()
    {
      object obj = this.containers.Pop();
      if (this.containers.Count != 0)
        return;
      this.lastCompletedObject = obj;
    }

    private void AddToScope(object jsonObject)
    {
      if (this.IsArray)
      {
        this.AsList().Add(jsonObject);
      }
      else
      {
        string currentPropertyName = this.GetAndClearCurrentPropertyName();
        this.AsDictionary().Add(currentPropertyName, jsonObject);
      }
    }

    private string GetAndClearCurrentPropertyName()
    {
      string currentPropertyName = this.currentPropertyName;
      this.currentPropertyName = (string) null;
      return currentPropertyName;
    }

    private IList AsList() => this.containers.Peek() as IList;

    private IDictionary<string, object> AsDictionary() => this.containers.Peek() as IDictionary<string, object>;
  }
}
