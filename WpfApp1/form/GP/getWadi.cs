using Esri.ArcGISRuntime.LocalServices;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Rasters;
using Esri.ArcGISRuntime.Tasks;
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
    /// 根据gpk文件，由DEM数据获取到洼地
    /// </summary>
    public class getWadi
    {
        private string gpkFile;
        public LocalGeoprocessingService gpService;
        public GeoprocessingTask gpTask;
        public GeoprocessingJob gpJob;

        public getWadi(string gpkFile)
        {
            this.gpkFile = gpkFile;
        }

        private void init()
        {
            
            //localserver为单例模式
            if (LocalServerManager.localServer != null)
            {
                LocalServerManager.localServer.StatusChanged += async (o, e) =>
                {
                    //如果本地服务器初始化成功
                    gpService = new LocalGeoprocessingService(gpkFile);
                    gpService.StatusChanged += async (svc, args) =>
                    {
                        // 如果服务启动了，就获取一个Task
                        if (args.Status == LocalServerStatus.Started)
                        {
                            // 在本地服务器中获取该服务的URL
                            var gpSvcUrl = (svc as LocalGeoprocessingService).Url.AbsoluteUri + "\\获取洼地.gpk";

                            // 创建一个Task
                            gpTask = new GeoprocessingTask(new Uri(gpSvcUrl));

                            //创建参数，异步执行
                            //输入一个raster
                            GeoprocessingParameters para = new GeoprocessingParameters(GeoprocessingExecutionType.SynchronousExecute);
                            string pathToRaster = @"c:\users\administrator\documents\arcgis\localServer\createtin_ti.tif";
                            //var myRaster = new Raster(pathToRaster);
                            para.Inputs.Add("inputRaster", new GeoprocessingRaster(new Uri(pathToRaster),""));
                            //因为是栅格所以得把z值设为true
                            para.ReturnZ = true;
                            para.OutputSpatialReference = MainWindow.mainwindow.MyMapView.SpatialReference;
                            gpJob = gpTask.CreateJob(para);
                        }

                        //获取结果
                        try
                        {
                            GeoprocessingResult geoprocessingResult = await gpJob.GetResultAsync();
                            GeoprocessingRaster resultRaster = geoprocessingResult.Outputs["outputRaster"] as GeoprocessingRaster;
                            string pathToRaster = resultRaster.Source.AbsolutePath;
                            var myRaster = new Raster(pathToRaster);
                            var newRasterLayer = new RasterLayer(myRaster);

                            //把栅格加入到底图（操作图层）中
                            MainWindow.mainwindow.MyMapView.Map.OperationalLayers.Add(newRasterLayer);
                            //缩放至该图层
                            await MainWindow.mainwindow.MyMapView.SetViewpointGeometryAsync(newRasterLayer.FullExtent);

                        }
                        catch (Exception ex)
                        {
                            if (gpJob.Status == JobStatus.Failed && gpJob.Error != null)
                                MessageBox.Show("Executing geoprocessing failed. " + gpJob.Error.Message, "Geoprocessing error");
                            else
                                MessageBox.Show("An error occurred. " + ex.ToString(), "error");
                        }

                    };
                    //开始执行服务
                    await gpService.StartAsync();
                };
            }


        
        }
    }
}
