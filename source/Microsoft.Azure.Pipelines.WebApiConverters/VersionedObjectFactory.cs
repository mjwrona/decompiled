// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebApiConverters.VersionedObjectFactory
// Assembly: Microsoft.Azure.Pipelines.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 42DDCCD8-4E0C-44F8-A5D2-AEF894388AED
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebApiConverters.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Pipelines.WebApiConverters
{
  public class VersionedObjectFactory
  {
    private readonly IReadOnlyDictionary<int, IVersionedObjectCreator> m_creatorDictionary;
    private readonly int m_maxVersion;

    public VersionedObjectFactory(IVersionedObjectCreator[] creators)
    {
      ArgumentUtility.CheckEnumerableForEmpty((IEnumerable) creators, nameof (creators));
      IOrderedEnumerable<IVersionedObjectCreator> source = ((IEnumerable<IVersionedObjectCreator>) creators).OrderBy<IVersionedObjectCreator, int>((Func<IVersionedObjectCreator, int>) (c => c.Version));
      this.m_maxVersion = source.Last<IVersionedObjectCreator>().Version;
      Dictionary<int, IVersionedObjectCreator> dictionary = new Dictionary<int, IVersionedObjectCreator>();
      int num = 0;
      IVersionedObjectCreator versionedObjectCreator1 = source.First<IVersionedObjectCreator>();
      foreach (IVersionedObjectCreator versionedObjectCreator2 in (IEnumerable<IVersionedObjectCreator>) source)
      {
        if (versionedObjectCreator2.Version > num + 1)
        {
          for (int key = num + 1; key < versionedObjectCreator2.Version; ++key)
            dictionary[key] = versionedObjectCreator1;
        }
        dictionary[versionedObjectCreator2.Version] = versionedObjectCreator2;
        num = versionedObjectCreator2.Version;
        versionedObjectCreator1 = versionedObjectCreator2;
      }
      this.m_creatorDictionary = (IReadOnlyDictionary<int, IVersionedObjectCreator>) dictionary;
    }

    public IVersionedObjectCreator GetCreator(int version)
    {
      ArgumentUtility.CheckForNonnegativeInt(version, nameof (version));
      if (version > this.m_maxVersion || version < 1)
        version = this.m_maxVersion;
      return this.m_creatorDictionary[version];
    }
  }
}
