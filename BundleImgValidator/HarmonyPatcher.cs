using BundleImgValidator;
using DistantWorlds.Types;
using HarmonyLib;
using Stride.Core;
using Stride.Core.IO;
using Stride.Core.Serialization.Contents;
using Stride.Core.Storage;
using Stride.Engine;
using Stride.Games;
using Stride.Graphics;
using Stride.Graphics.Data;
using System.Net.Http.Headers;
using System.Reflection;
using System.Reflection.Emit;

public static class Mod
{
    public static void Init()
    {
        BundleImgValidator.Preloader.Init();
    }
}

namespace BundleImgValidator
{
    public class Preloader
    {
        public static void Init()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            HarmonyPatcher.Init();
        }
        private static System.Reflection.Assembly? CurrentDomain_AssemblyResolve(object? sender, ResolveEventArgs args)
        {
            AssemblyName name = new AssemblyName(args.Name);
            if (name.Name == "0Harmony")
            {
                var asmLoc = typeof(HarmonyPatcher).Assembly.Location;
                var asmDir = Path.GetDirectoryName(asmLoc);
                var harmonyDll = Path.Join(asmDir, "0Harmony.dll");
                return Assembly.LoadFrom(harmonyDll);
            }
            return null;
        }
    }
    public class HarmonyPatcher
    {
        public static void Init()
        {
            int i = 0;
            int j = 1;
            if (i < j)
            { i = 3; }
            else
            { i = 4; }
            Core.ModRootLocation = Path.GetDirectoryName(Assembly.GetCallingAssembly().Location);
            //Logger.ChannelFilter = Logger.LogChannel.All;
            //Harmony.DEBUG = true;
            //FileLog.Reset();
            var harmony = new Harmony("DW2.BundleImbValidator");
            harmony.PatchAll();

            //FileLog.Log($"{DateTime.Now} patch done");
            //FileLog.FlushBuffer();

        }
    }
    [HarmonyDebug]
    [HarmonyPatch(typeof(DistantWorlds.Types.Galaxy))]
    [HarmonyPatch(nameof(DistantWorlds.Types.Galaxy.LoadImageForAllItemsStatic))]
    public class GalaxyLogStreamPatcher
    {
        public static void Prefix()
        {
            Core.PrepareLogStream();
        }
        public static void Postfix()
        {
            Core.CloseLogStream();
        }
    }
    [HarmonyDebug]
    [HarmonyPatch(typeof(DistantWorlds.Types.Galaxy))]
    [HarmonyPatch(nameof(DistantWorlds.Types.Galaxy.LoadImagesForItems))]
    public class GalaxyLoadImagesForItemsPatcher
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool imageFileNamePatched = false;
            var codes = new List<CodeInstruction>(instructions);
            for (var i = 0; i < codes.Count; i++)
            {
                if (!imageFileNamePatched && codes[i].Calls(Helper.GetMethodInfo(typeof(ContentManager), nameof(ContentManager.Exists))) && codes[i + 1].opcode == OpCodes.Brtrue)
                {
                    i += 2;
                    //get obj name
                    codes.Insert(i++, new CodeInstruction(OpCodes.Ldloc_1));
                    codes.Insert(i++, new CodeInstruction(OpCodes.Callvirt, typeof(IDrawableSummary).GetProperty(nameof(IDrawableSummary.Name)).GetGetMethod()));
                    // get local text variable containing current ImageFileName
                    codes.Insert(i++, new CodeInstruction(OpCodes.Ldloc_2));
                    //log missing asset
                    codes.Insert(i++, new CodeInstruction(OpCodes.Call, Helper.GetMethodInfo(typeof(Core), nameof(Core.LogMissingFile))));

                    imageFileNamePatched = true;
                    i += 5;
                }

                if (codes[i].Calls(Helper.GetMethodInfo(typeof(ColonyEventDefinition), nameof(ColonyEventDefinition.SetIcon))))
                {
                    i++;
                    //get obj name
                    codes.Insert(i++, new CodeInstruction(OpCodes.Ldloc_1));
                    codes.Insert(i++, new CodeInstruction(OpCodes.Callvirt, typeof(IDrawableSummary).GetProperty(nameof(IDrawableSummary.Name)).GetGetMethod()));
                    //get icon file name
                    codes.Insert(i++, new CodeInstruction(OpCodes.Ldloc_S, 4));
                    codes.Insert(i++, new CodeInstruction(OpCodes.Ldfld, typeof(ColonyEventDefinition).GetField(nameof(ColonyEventDefinition.IconFilename))));
                    //log missing asset
                    codes.Insert(i++, new CodeInstruction(OpCodes.Call, Helper.GetMethodInfo(typeof(Core), nameof(Core.LogMissingFile))));
                    break;
                }

            }
            return codes.AsEnumerable();
        }
    }

    [HarmonyDebug]
    [HarmonyPatch(typeof(DistantWorlds.Types.Galaxy))]
    [HarmonyPatch(nameof(DistantWorlds.Types.Galaxy.LoadRaceFlagImages))]
    public class GalaxyLoadRaceFlagImagesPatcher
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool setTextureFlag = false;
            var codes = new List<CodeInstruction>(instructions);
            for (var i = 0; i < codes.Count; i++)
            {
                if (!setTextureFlag && codes[i].Calls(Helper.GetMethodInfo(typeof(Galaxy), nameof(Galaxy.GetEmpireColorFromFlag))) && codes[i + 1].labels.Count > 0)
                {
                    Label afterElseLabel = codes[i + 1].labels[0];
                    i++;
                    //set after els label to jump to from if
                    codes.Insert(i++, new CodeInstruction(OpCodes.Br, afterElseLabel));
                    //get race name
                    LocalBuilder q = new LocalBuilder();
                    System.Reflection.Emit.ILGenerator t = new System.Reflection.Emit.ILGenerator();
                    //ILGenerator t = ConstructorBuilder.GetILGenerator();
                    codes.Insert(i++, new CodeInstruction(OpCodes.Ldloc_1, 0));
                    codes.Insert(i++, new CodeInstruction(OpCodes.Callvirt, typeof(Race).GetProperty(nameof(Race.Name)).GetGetMethod()));
                    // get local text variable containing current FlagFileName
                    codes.Insert(i++, new CodeInstruction(OpCodes.Ldloc_1));
                    codes.Insert(i++, new CodeInstruction(OpCodes.Ldfld, typeof(Race).GetField(nameof(Race.FlagFilename))));
                    //log missing asset
                    codes.Insert(i++, new CodeInstruction(OpCodes.Call, Helper.GetMethodInfo(typeof(Core), nameof(Core.LogMissingFile))));

                    setTextureFlag = true;
                    i += 5;
                }

                if (codes[i].Calls(Helper.GetMethodInfo(typeof(ColonyEventDefinition), nameof(ColonyEventDefinition.SetIcon))))
                {
                    i++;
                    //get obj name
                    codes.Insert(i++, new CodeInstruction(OpCodes.Ldloc_1));
                    codes.Insert(i++, new CodeInstruction(OpCodes.Callvirt, typeof(IDrawableSummary).GetProperty(nameof(IDrawableSummary.Name)).GetGetMethod()));
                    //get icon file name
                    codes.Insert(i++, new CodeInstruction(OpCodes.Ldloc_S, 4));
                    codes.Insert(i++, new CodeInstruction(OpCodes.Ldfld, typeof(ColonyEventDefinition).GetField(nameof(ColonyEventDefinition.IconFilename))));
                    //log missing asset
                    codes.Insert(i++, new CodeInstruction(OpCodes.Call, Helper.GetMethodInfo(typeof(Core), nameof(Core.LogMissingFile))));
                    break;
                }

            }
            return codes.AsEnumerable();
        }
    }


    public class BundleValidator
    {

        //public static void Main(string[] args)
        //{
        //    StanAloneTest();
        //}
        //public static void StanAloneTest()
        //{
        //    try
        //    {
        //        //
        //        //string text = ((VirtualFileSystem.ResolveProviderUnsafe("/asset", true).Provider == null) ? "/asset" : null);
        //        //string text = ((VirtualFileSystem.ResolveProviderUnsafe("d:\\Games\\Distant Worlds 2\\data\\db\\bundles\\", true).Provider == null) ? "/asset" : null);
        //        ObjectDatabase objectDatabase = ObjectDatabase.CreateDefaultDatabase();
        //        DatabaseFileProvider t = new DatabaseFileProvider(objectDatabase);
        //        ServiceRegistry Services = new ServiceRegistry();
        //        Services.AddService<IDatabaseFileProviderService>(new DatabaseFileProviderService(t));
        //        var Content = new ContentManager(Services);
        //        Services.AddService<IContentManager>(Content);
        //        //var GraphicsDeviceManager = new GraphicsDeviceManager(new Game());
        //        //Services.AddService<IGraphicsDeviceManager>(GraphicsDeviceManager);
        //        //Services.AddService<IGraphicsDeviceService>(GraphicsDeviceManager);

        //        Task.WaitAll(objectDatabase.LoadBundle("CoreContent"),
        //        objectDatabase.LoadBundle("Pirates"),
        //        objectDatabase.LoadBundle("Abandoned"),
        //        objectDatabase.LoadBundle("Creatures"));


        //        //IContentSerializer<Stride.Graphics.Image> serializer = (IContentSerializer<Stride.Graphics.Image>)Activator.CreateInstance(typeof(TextureSerializer).Assembly.GetType("Stride.Graphics.Data.ImageTextureSerializer", true), true);
        //        //Content.Serializer.RegisterSerializer(serializer);
        //        //var texture = Content.Load<Texture>("UserInterface/Placeholder", null);//need graphic service to load texture

        //        bool res = Content.Exists("UserInterface/Placeholder");
        //        Console.WriteLine(res);
        //        Console.ReadLine();
        //        var tt = Content.GetStats();
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        Console.ReadLine();
        //    }
        //}
    }
}
