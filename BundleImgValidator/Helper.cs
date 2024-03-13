using Stride.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BundleImgValidator
{
    static class Helper
    {
        public static MethodInfo GetMethodInfo(Func<Texture> method)
        {
            return method.Method;
        }
        public static MethodInfo GetMethodInfo(Type type, string name)
        {
            return type.GetMethod(name);
        }
    }
}
