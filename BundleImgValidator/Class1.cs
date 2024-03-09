using Stride.Core;
using Stride.Core.IO;
using Stride.Core.Serialization.Contents;
using Stride.Core.Storage;
using Stride.Graphics;

namespace BundleImgValidator
{
    public class Class1
    {
        public static void Test()
        {
            string text = ((VirtualFileSystem.ResolveProviderUnsafe("/asset", true).Provider == null) ? "/asset" : null);
            ObjectDatabase objectDatabase = ObjectDatabase.CreateDefaultDatabase();
            DatabaseFileProvider t = new DatabaseFileProvider(objectDatabase);
            ServiceRegistry Services = new ServiceRegistry();
            Services.AddService<IDatabaseFileProviderService>(new DatabaseFileProviderService(t));
            var Content = new ContentManager(Services);
            Services.AddService<IContentManager>(Content);
            Services.AddService(Content);


            var texture = Content.Load<Texture>("UserInterface/Placeholder", null);
            var tt = Content.GetStats();
        }
    }
}
