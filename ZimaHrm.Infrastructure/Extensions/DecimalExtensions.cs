using System;

namespace ZimaHrm.Core.Infrastructure.Extensions
{
  public static class DecimalExtensions
  {
    public static decimal ToRound(this decimal value)
    {
      return Math.Round(value);
    }
    public static decimal? ToDecimal(this string t)
    {
      decimal? d;
      try
      {
        d = Convert.ToDecimal(t);
      }
      catch
      {
        d = null;
      }
      return d;
    }
  }
}
