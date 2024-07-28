// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.ODataExpandPath
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.OData.UriParser
{
  [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "ODataExpandPathCollection just doesn't sound right")]
  public class ODataExpandPath : ODataPath
  {
    public ODataExpandPath(IEnumerable<ODataPathSegment> segments)
      : base(segments)
    {
      this.ValidatePath();
    }

    public ODataExpandPath(params ODataPathSegment[] segments)
      : base(segments)
    {
      this.ValidatePath();
    }

    internal IEdmNavigationProperty GetNavigationProperty() => ((NavigationPropertySegment) this.LastSegment).NavigationProperty;

    private void ValidatePath()
    {
      int num = 0;
      bool flag = false;
      foreach (ODataPathSegment odataPathSegment in (ODataPath) this)
      {
        switch (odataPathSegment)
        {
          case TypeSegment _:
            if (num == this.Count - 1)
              throw new ODataException(Microsoft.OData.Strings.ODataExpandPath_OnlyLastSegmentMustBeNavigationProperty);
            break;
          case PropertySegment _:
            if (num == this.Count - 1)
              throw new ODataException(Microsoft.OData.Strings.ODataExpandPath_OnlyLastSegmentMustBeNavigationProperty);
            break;
          case NavigationPropertySegment _:
            if (num < this.Count - 1 | flag)
              throw new ODataException(Microsoft.OData.Strings.ODataExpandPath_OnlyLastSegmentMustBeNavigationProperty);
            flag = true;
            break;
          default:
            throw new ODataException(Microsoft.OData.Strings.ODataExpandPath_InvalidExpandPathSegment((object) odataPathSegment.GetType().Name));
        }
        ++num;
      }
    }
  }
}
