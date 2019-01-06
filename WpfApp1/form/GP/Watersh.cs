using Esri.ArcGISRuntime.LocalServices;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Tasks.Geoprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.form.GP
{
    /// <summary>
    /// 提取子流域
    /// 参数1 河流栅格
    /// 参数2 河流方向
    /// </summary>
    public class Watersh
    {
        private string gpkFile;
        public LocalGeoprocessingService gpService;
        public GeoprocessingTask gpTask;
        public GeoprocessingJob gpJob;

        public Watersh(string gpkFile)
        {
            this.gpkFile = gpkFile;
        }

        private async Task initAsync()
        {
            if (LocalServerManager.localServer != null)
            {
                LocalServerManager.localServer.StatusChanged += async (o, e) =>
                {
                    if (e.Status != LocalServerStatus.Started)
                    {
                        throw new Exception("运行失败");
                    }
                    gpService = new LocalGeoprocessingService(gpkFile);
                    gpService.StatusChanged += async (svc, args) =>
                    {
                        var gpSvcUrl = (svc as LocalGeoprocessingService).Url.AbsoluteUri + "\\模型.gpk";
                        gpTask = new GeoprocessingTask(new Uri(gpSvcUrl));
                        GeoprocessingParameters para = new GeoprocessingParameters(GeoprocessingExecutionType.SynchronousExecute);
                        //输入两个参数
                        string pathToRaster = @"c:\users\administrator\documents\arcgis\localServer\flowdir.tif";
                        para.Inputs.Add("inputRaster1", new GeoprocessingRaster(new Uri(pathToRaster), ""));
                        string pathToRaster2 = @"c:\users\administrator\documents\arcgis\localServer\river.tif";
                        para.Inputs.Add("inputRaster2", new GeoprocessingRaster(new Uri(pathToRaster2), ""));
                        para.ReturnZ = true;
                        para.OutputSpatialReference = MainWindow.mainwindow.MyMapView.SpatialReference;
                        gpJob = gpTask.CreateJob(para);



                        //获取结果
                        try
                        {
                            GeoprocessingResult geoprocessingResult = await gpJob.GetResultAsync();
                            GeoprocessingRaster resultRaster = geoprocessingResult.Outputs["Watersh"] as GeoprocessingRaster;
                            string Result = resultRaster.Source.AbsolutePath;
                            var myRaster = new Esri.ArcGISRuntime.Rasters.Raster(Result);

                            var newRasterLayer = new RasterLayer(myRaster);
                            MainWindow.mainwindow.MyMapView.Map.OperationalLayers.Add(newRasterLayer);
                            await MainWindow.mainwindow.MyMapView.SetViewpointGeometryAsync(newRasterLayer.FullExtent);

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
