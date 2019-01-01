using System;

namespace Assets.Scripts.Astronomical
{
    public class ViewerDataProvider
    {
        private static readonly Lazy<ViewerDataProvider> LazyInstance =
            new Lazy<ViewerDataProvider>(() => new ViewerDataProvider());

        public static ViewerDataProvider Instance => LazyInstance.Value;

        public ViewerData Data { get; }

        private ViewerDataProvider()
        {
            this.Data = new ViewerData();
        }
    }
}