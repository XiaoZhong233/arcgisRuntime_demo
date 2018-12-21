using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Rasters;
using Esri.ArcGISRuntime.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Raster;

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


        public static void blendRender(RasterLayer rasterLayer)
        {
            BlendRenderingForm brf = new BlendRenderingForm();
            if (brf.ShowDialog() == true)
            {
                ColorRamp colorRamp;
                if (brf.PredefineColorRampType == PresetColorRampType.None)
                    colorRamp = null;
                else
                {
                    colorRamp = ColorRamp.Create(brf.PredefineColorRampType, 256);//创建颜色条带
                    IEnumerable<double> myOutputMinValues = new List<double>();//定义输出最小值参数列表变量
                    IEnumerable<double> myOutputMaxValues = new List<double>();//定义输出最大值参数列表变量
                    IEnumerable<double> mySourceMinValues = new List<double>();//定义输入最小值参数列表变量
                    IEnumerable<double> mySourceMaxValues = new List<double>();//定义输入最大值参数列表变量
                    IEnumerable<double> myNoDataValues = new List<double>();//定义Nodata参数列表变量
                    IEnumerable<double> myGammas = new List<double>();//定义伽马值列表变量
                    BlendRenderer myBlendRenderer = new BlendRenderer(
                        rasterLayer.Raster, // 高程数据源
                        myOutputMinValues, // 每个波段的输出最小值
                        myOutputMaxValues, // 每个波段的输出最大值
                        mySourceMinValues, // 每个波段输入的最小值.
                        mySourceMaxValues, // 每个波段输入的最大值
                        myNoDataValues, // 每个波段的Nodata值
                        myGammas, // 每个波段的伽马值
                        colorRamp, // 
                        brf.CurAltitude, // 高度角
                        brf.CurAzimuth,//方位角（从北起，顺时针）
                        1, // Z值缩放
                        brf.SelSlopeType, // 坡度类型
                        1, // 像素大小缩放因子
                        1, // 像素大小的幂
                        8); // 位的深度
                    rasterLayer.Renderer = myBlendRenderer;//改变图层的渲染器

                }
            }
        }

    }
}
