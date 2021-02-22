using System;
namespace AstroHelper
{
    public interface IOrbitable
    {
        bool ContainsX(double x);
        double this[double x] { get; }
    }
}
