using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.LocalServices;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Tasks;
using Esri.ArcGISRuntime.Tasks.Geoprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace WpfApp1
{
    /// <summary>
    /// 本地服务器 使用gpk
    /// </summary>
    public class LocalServerManager
    {
        private MainWindow mainWindow;
        private Esri.ArcGISRuntime.LocalServices.LocalServer localServer;

        private LocalGeoprocessingService gpService;
        private GeoprocessingTask gpTask;
        private GeoprocessingJob gpJob;
        private string path = @"E:\Desktop\学习\大三上\GIS课程设计\gpk\ClustersOutliers.gpk";


        public LocalServerManager(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            //init();
            //initGPService();
        }

        public void init()
        {
            mainWindow.MyMapView.Map.LoadStatusChanged += (o, e) =>
            {
                switch (e.Status.ToString())
                {
                    case "Loading":
                        break;
                    case "Loaded":
                        StartLocalServer();
                        break;
                }
            };

            mainWindow.Closed += (s, e) =>
            {
                localServer.StopAsync();
            };
            
            



        }

        private async void StartLocalServer()
        {
            localServer = Esri.ArcGISRuntime.LocalServices.LocalServer.Instance;
            localServer.StatusChanged += ServerStatusChanged;
            await localServer.StartAsync();
        }

        private void ServerStatusChanged(object sender, Esri.ArcGISRuntime.LocalServices.StatusChangedEventArgs e)
        {
            if (e.Status == Esri.ArcGISRuntime.LocalServices.LocalServerStatus.Started)
            {
                //TODO:本地服务器加载成功时...do something
                gpService = new LocalGeoprocessingService(path);

                // Handle the status changed event to check when it's loaded
                gpService.StatusChanged += (svc, args) =>
                {
                    // If service started successfully, create a gp task
                    if (args.Status == LocalServerStatus.Started)
                    {
                        // Get the URL for the specific geoprocessing tool
                        var gpSvcUrl = (svc as LocalGeoprocessingService).Url.AbsoluteUri + "/MessageInABottle";

                        // Create the geoprocessing task
                        gpTask = new GeoprocessingTask(new Uri(gpSvcUrl));


                        // Create parameters, run the task, process results, etc.
                        // ...
                    }
                };
            }
        }

        private void initGPService()
        {
            //test
            mainWindow.test1.Click += async (s, e) =>
            {
                await gpService.StartAsync();
            };

        }


        private async void GpServiceOnStatusChanged(object sender, StatusChangedEventArgs statusChangedEventArgs)
        {
            // Return if the server hasn't started
            if (statusChangedEventArgs.Status != LocalServerStatus.Started) return;

            // Create the geoprocessing task from the service
            gpTask = await GeoprocessingTask.CreateAsync(new Uri(gpService.Url + "/ClustersOutliers"));
        }

        private void generateResult()
        {
            GeoprocessingParameters gpParams = new GeoprocessingParameters(GeoprocessingExecutionType.AsynchronousSubmit);
            gpJob = gpTask.CreateJob(gpParams);
            gpJob.JobChanged += _gpJob_JobChangedAsync;
        }

        private async void _gpJob_JobChangedAsync(object sender, EventArgs e)
        {
            if (gpJob.Status == JobStatus.Failed)
            {
                MessageBox.Show("Job Failed");
                return;
            }

            // Return if not succeeded
            if (gpJob.Status != JobStatus.Succeeded) { return; }

            // Get the URL to the map service
            string gpServiceResultUrl = gpService.Url.ToString();
            // Get the URL segment for the specific job results
            string jobSegment = "MapServer/jobs/" + gpJob.ServerJobId;
            // Update the URL to point to the specific job from the service
            gpServiceResultUrl = gpServiceResultUrl.Replace("GPServer", jobSegment);
            // Create a map image layer to show the results
            ArcGISMapImageLayer myMapImageLayer = new ArcGISMapImageLayer(new Uri(gpServiceResultUrl));
            // Load the layer
            await myMapImageLayer.LoadAsync();
            mainWindow.MyMapView.Map.OperationalLayers.Add(myMapImageLayer);


        }
    }
}
