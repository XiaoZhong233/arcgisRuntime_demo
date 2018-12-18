using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.LocalServices;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Rasters;
using Esri.ArcGISRuntime.Tasks.Geoprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1.form.GP
{
    /// <summary>
    /// 将河网栅格转换成河网要素
    /// </summary>
    class StramToFeature
    {
        private string gpkFile;
        public LocalGeoprocessingService gpService;
        public GeoprocessingTask gpTask;
        public GeoprocessingJob gpJob;


        public StramToFeature(string gpkFile)
        {
            this.gpkFile = gpkFile;
        }


        private void init()
        {
            if (LocalServerManager.localServer != null)
            {
                LocalServerManager.localServer.StatusChanged += async (o, e) =>
                {
                    gpService = new LocalGeoprocessingService(gpkFile);
                    gpService.StatusChanged += async (svc, args) =>
                    {
                        if (args.Status == LocalServerStatus.Started)
                        {
                            var gpSvcUrl = (svc as LocalGeoprocessingService).Url.AbsoluteUri + "\\StramToFeature.gpk";
                            gpTask = new GeoprocessingTask(new Uri(gpSvcUrl));
                            GeoprocessingParameters para = new GeoprocessingParameters(GeoprocessingExecutionType.SynchronousExecute);
                            //输入流向数据
                            string pathToRaster = @"c:\users\administrator\documents\arcgis\localServer\flowdir.tif";
                            para.Inputs.Add("flowdir", new GeoprocessingRaster(new Uri(pathToRaster), ""));
                            //输入河流栅格
                            string pathToRaster2 = @"c:\users\administrator\documents\arcgis\localServer\heliu.tif";
                            para.Inputs.Add("heliu", new GeoprocessingRaster(new Uri(pathToRaster2), ""));

                            para.ReturnZ = true;
                            para.OutputSpatialReference = MainWindow.mainwindow.MyMapView.SpatialReference;
                            gpJob = gpTask.CreateJob(para);
                        }

                        try
                        {
                            //获取输出的河网矢量
                            GeoprocessingResult geoprocessingResult = await gpJob.GetResultAsync();
                            GeoprocessingFeatures resultFeatures = geoprocessingResult.Outputs["outputFeature"] as GeoprocessingFeatures;
                            IFeatureSet interpolateShapeResult = resultFeatures.Features;
                            Esri.ArcGISRuntime.Geometry.Polyline elevationLine =
                            interpolateShapeResult.First().Geometry as Esri.ArcGISRuntime.Geometry.Polyline;

                            
                            //MapPoint startPoint = elevationLine.Parts[0].Points[0];
                            //int count = elevationLine.Parts[0].PointCount;
                            //MapPoint stopPoint = elevationLine.Parts[0].Points[count - 1];
                            //double chazhi = stopPoint.Z - startPoint.Z;
                            //MessageBox.Show("终点的Z值为: " + stopPoint.Z.ToString() + "，起点的Z值为: " + startPoint.Z.ToString());

                        }
                        catch (Exception ex)
                        {

                        }

                    };
                    await gpService.StartAsync();
                };
            }
        }
    }
}
