using HarmonyLib;
using Stride.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
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
        public static MethodInfo GetMethodInfo(Action method)
        {
            return method.Method;
        }
        public static MethodInfo GetMethodInfo(Type type, string name)
        {
            return type.GetMethod(name);
        }
        public static MethodInfo GetMethodInfo(Type type, string name, Type[] methodTypes)
        {
            return type.GetMethod(name, methodTypes);
        }
        public static MethodInfo GetMethodInfo<T>(Type type, string name, Type[] methodTypes)
        {
            return type.GetMethod(name, methodTypes).MakeGenericMethod(new[] { typeof(T) });
        }
        public static void ReplaceLabelForPreviousJump(List<CodeInstruction> codes, int startIndex, Label lbl)
        {
            int i = startIndex;
            while (i > -1)
            {
                if (codes[i].Branches(out Label? tmp))
                {
                    codes[i].operand = lbl;
                    return;
                }
                i--;
            }
            throw new ApplicationException("Can't find previous jump code to replace label");
        }
    }
}
