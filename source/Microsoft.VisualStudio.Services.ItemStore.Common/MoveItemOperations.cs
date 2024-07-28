// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Common.MoveItemOperations
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 44753C0C-D541-4975-AF3F-2B606DE6FF70
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Common.dll

using Microsoft.VisualStudio.Services.Content.Common;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ItemStore.Common
{
  public class MoveItemOperations : IEnumerable<MoveOperation>, IEnumerable
  {
    private readonly Dictionary<Locator, Locator> MovesToPerform = new Dictionary<Locator, Locator>();
    private readonly Dictionary<Locator, Locator> ReverseOfMovesToPerform = new Dictionary<Locator, Locator>();

    public Locator this[Locator key]
    {
      get => this.MovesToPerform[key];
      set => this.Add(key, value);
    }

    public void Add(Locator source, Locator destination)
    {
      if (source == (Locator) null)
        throw new ArgumentNullException("source cannot be null");
      if (destination == (Locator) null)
        throw new ArgumentNullException("destination cannot be null");
      if (source.Equals(destination))
        throw new ArgumentException("source and destination cannot be the same location.");
      if (this.MovesToPerform.ContainsKey(source))
        throw new ArgumentException("Provided source path already exists as a source path.");
      if (this.MovesToPerform.ContainsKey(destination))
        throw new ArgumentException("Provided destination path already exists as a source path.");
      if (this.ReverseOfMovesToPerform.ContainsKey(source))
        throw new ArgumentException("Provided source path already exists as a destination path.");
      this.MovesToPerform[source] = !this.ReverseOfMovesToPerform.ContainsKey(destination) ? destination : throw new ArgumentException("Provided destination path already exists as a destination path.");
      this.ReverseOfMovesToPerform[destination] = source;
    }

    public void Remove(Locator source, Locator destination)
    {
      if (source == (Locator) null)
        throw new ArgumentNullException("source cannot be null");
      if (destination == (Locator) null)
        throw new ArgumentNullException("destination cannot be null");
      if (!this.MovesToPerform.ContainsKey(source))
        throw new ArgumentException("No move operation from the specified source path exists.");
      if (!this.ReverseOfMovesToPerform.ContainsKey(destination))
        throw new ArgumentException("No move operation from the specified destination path exists.");
      if (!(this.MovesToPerform[source] == destination))
        throw new ArgumentException("destination path is not the current destination of the item from source path.");
      this.MovesToPerform.Remove(source);
      this.ReverseOfMovesToPerform.Remove(destination);
    }

    public IEnumerator<MoveOperation> GetEnumerator()
    {
      foreach (KeyValuePair<Locator, Locator> keyValuePair in this.MovesToPerform)
        yield return new MoveOperation(keyValuePair.Key, keyValuePair.Value);
    }

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.GetEnumerator();

    public Locator GetSourcePath(Locator destination) => this.ReverseOfMovesToPerform[destination];
  }
}
