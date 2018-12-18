using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class RasterManager
    {
        MainWindow mainWindow;
        MapView mapView;
        List<RasterLayer> rasterLayers;

        public RasterManager()
        {
            if (MainWindow.mainwindow == null)
                throw new NullReferenceException("主窗体为空");
            mainWindow = MainWindow.mainwindow;
            mapView = MainWindow.mainwindow.MyMapView;
            rasterLayers = MainWindow.mainwindow.rasterLayers;
        }


        public static void 

    }
}
