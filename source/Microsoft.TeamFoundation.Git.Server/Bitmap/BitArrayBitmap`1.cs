// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Bitmap.BitArrayBitmap`1
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server.Bitmap
{
  internal class BitArrayBitmap<T> : AbstractBitmap<T>
  {
    private int m_count;
    private readonly int m_numObjects;
    private readonly BitArray m_elements;

    public BitArrayBitmap(ITwoWayReadOnlyList<T> objectList, BitArray elements = null)
      : base(objectList)
    {
      this.m_numObjects = objectList.Count;
      this.m_elements = elements ?? new BitArray(this.m_numObjects);
      if (elements == null)
        return;
      foreach (bool element in elements)
      {
        if (element)
          ++this.m_count;
      }
    }

    public override int Count => this.m_count;

    public override bool ContainsIndex(int index) => this.m_elements[index];

    public override IEnumerable<int> Indices
    {
      get
      {
        for (int i = 0; i < this.m_numObjects; ++i)
        {
          if (this.m_elements[i])
            yield return i;
        }
      }
    }

    public BitArray Elements => this.m_elements;

    public override bool AddIndex(int index)
    {
      this.CheckForReadOnly();
      if (this.m_elements[index])
        return false;
      this.m_elements[index] = true;
      ++this.m_count;
      return true;
    }

    public override int GetSize() => 4 * ((this.m_elements.Count + 32) / 32) + 16;
  }
}
