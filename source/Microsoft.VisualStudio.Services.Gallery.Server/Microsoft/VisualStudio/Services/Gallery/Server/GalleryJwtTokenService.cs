// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.GalleryJwtTokenService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class GalleryJwtTokenService : IGalleryJwtTokenService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public string GenerateJwtToken(
      IVssRequestContext requestContext,
      string keyType,
      JwtClaims jwtClaims)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      string s = vssRequestContext.GetService<IGalleryKeyService>().ReadKey(vssRequestContext, keyType);
      string jwtToken = (string) null;
      if (!string.IsNullOrEmpty(s))
        jwtToken = TokenManagement.GenerateJwtToken(jwtClaims, Convert.FromBase64String(s));
      return jwtToken;
    }

    public JwtClaims ParseJwtToken(
      IVssRequestContext requestContext,
      string keyType,
      string signedToken)
    {
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      JwtClaims jwtToken = (JwtClaims) null;
      bool allowCachedRead = true;
      IGalleryKeyService service = vssRequestContext.GetService<IGalleryKeyService>();
      foreach (string readKey in service.ReadKeys(vssRequestContext, keyType, allowCachedRead))
      {
        try
        {
          jwtToken = TokenManagement.ParseJwtToken(signedToken, Convert.FromBase64String(readKey));
          break;
        }
        catch (InvalidSignatureException ex)
        {
          allowCachedRead = false;
        }
        catch (InvalidTokenException ex)
        {
        }
      }
      if (jwtToken == null && !allowCachedRead)
      {
        foreach (string readKey in service.ReadKeys(vssRequestContext, keyType, allowCachedRead))
        {
          try
          {
            jwtToken = TokenManagement.ParseJwtToken(signedToken, Convert.FromBase64String(readKey));
            break;
          }
          catch (InvalidSignatureException ex)
          {
          }
          catch (InvalidTokenException ex)
          {
          }
        }
      }
      return jwtToken;
    }
  }
}
