using Esri.ArcGISRuntime.LocalServices;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Rasters;
using Esri.ArcGISRuntime.Tasks.Geoprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.form.GP
{
    /// <summary>
    /// 河网链接与河网分级
    /// </summary>
    class GenerateHeWangFenji
    {
        private string gpkFile;
        public LocalGeoprocessingService gpService;
        public GeoprocessingTask gpTask;
        public GeoprocessingJob gpJob;


        public GenerateHeWangFenji(string gpkFile)
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
                            var gpSvcUrl = (svc as LocalGeoprocessingService).Url.AbsoluteUri + "\\model.gpk";
                            gpTask = new GeoprocessingTask(new Uri(gpSvcUrl));
                            GeoprocessingParameters para = new GeoprocessingParameters(GeoprocessingExecutionType.SynchronousExecute);
                            string pathToRaster = @"c:\users\administrator\documents\arcgis\localServer\flowdir.tif";
                            para.Inputs.Add("inputRaster1", new GeoprocessingRaster(new Uri(pathToRaster), ""));
                            string pathToRaster2 = @"c:\users\administrator\documents\arcgis\localServer\streamLink.tif";
                            para.Inputs.Add("inputRaster2", new GeoprocessingRaster(new Uri(pathToRaster2), ""));
                            para.ReturnZ = true;
                            para.OutputSpatialReference = MainWindow.mainwindow.MyMapView.SpatialReference;
                            gpJob = gpTask.CreateJob(para);
                        }

                        try
                        {
                            GeoprocessingResult geoprocessingResult = await gpJob.GetResultAsync();
                            GeoprocessingRaster resultRaster = geoprocessingResult.Outputs["flowArea"] as GeoprocessingRaster;
                            string pathToRaster = resultRaster.Source.AbsolutePath;
                            var myRaster = new Raster(pathToRaster);
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
