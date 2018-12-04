﻿using Esri.ArcGISRuntime.LocalServices;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Tasks;
using Esri.ArcGISRuntime.Tasks.Geoprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WpfApp1.form
{
    /// <summary>
    /// LocalServerGeoprocessing.xaml 的交互逻辑
    /// </summary>
    public partial class LocalServerGeoprocessing : Window
    {
        // Hold a reference to the local geoprocessing service
        private LocalGeoprocessingService _gpService;

        // Hold a reference to the task
        private GeoprocessingTask _gpTask;

        // Hold a reference to the job
        private GeoprocessingJob _gpJob;


        public LocalServerGeoprocessing()
        {
            InitializeComponent();
            // set up the sample
            Initialize();
        }

        private async void Initialize()
        {
            // Create a map and add it to the view
            MyMapView.Map = new Map(Basemap.CreateLightGrayCanvasVector());

            //// Load the tiled layer and get the path
            //string rasterPath = GetRasterPath();

            //// Create a tile cache using the path to the raster
            //TileCache myTileCache = new TileCache(rasterPath);

            //// Create the tiled layer from the tile cache
            //ArcGISTiledLayer tiledLayer = new ArcGISTiledLayer(myTileCache);

            //// Try to load the tiled layer
            //try
            //{
            //    // Wait for the layer to load
            //    await tiledLayer.LoadAsync();

            //    // Zoom to extent of the tiled layer
            //    await MyMapView.SetViewpointGeometryAsync(tiledLayer.FullExtent);
            //}
            //catch (Exception)
            //{
            //    MessageBox.Show("Couldn't load the tile package, ending sample load.");
            //    return;
            //}

            //// Add the layer to the map
            //MyMapView.Map.OperationalLayers.Add(tiledLayer);

            // Try to start Local Server
            try
            {
                // Start the local server instance
                await LocalServer.Instance.StartAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Please ensure that local server is installed prior to using the sample. See instructions in readme.md or metadata.json. Message: {0}", ex.Message), "Local Server failed to start");
                return;
            }

            // Get the path to the geoprocessing task
            string gpServiceUrl = GetGpPath();

            // Create the geoprocessing service
            gpServiceUrl = @"E:\Desktop\学习\大三上\GIS课程设计\gpk\ClustersOutliers.gpk";
            _gpService = new LocalGeoprocessingService(gpServiceUrl, GeoprocessingServiceType.AsynchronousSubmitWithMapServiceResult);

            // Take action once the service loads
            _gpService.StatusChanged += GpServiceOnStatusChanged;

            // Try to start the service
            try
            {
                // Start the service
                await _gpService.StartAsync();
            }
            catch (Exception)
            {
                MessageBox.Show("geoprocessing service failed to start.");
            }
        }

        private async void GpServiceOnStatusChanged(object sender, StatusChangedEventArgs statusChangedEventArgs)
        {
            // Return if the server hasn't started
            if (statusChangedEventArgs.Status != LocalServerStatus.Started) return;

            // Create the geoprocessing task from the service
            _gpTask = await GeoprocessingTask.CreateAsync(new Uri(_gpService.Url + "/Contour"));

            // Update UI
            MyUpdateContourButton.IsEnabled = true;
            MyLoadingIndicator.Visibility = Visibility.Collapsed;
        }

        private void GenerateContours()
        {
            // Show the progress bar
            MyLoadingIndicator.Visibility = Visibility.Visible;
            MyLoadingIndicator.IsIndeterminate = false;

            // Create the geoprocessing parameters
            GeoprocessingParameters gpParams = new GeoprocessingParameters(GeoprocessingExecutionType.AsynchronousSubmit);

            // Add the interval parameter to the geoprocessing parameters
            gpParams.Inputs["ContourInterval"] = new GeoprocessingDouble(MyContourSlider.Value);

            // Create the job
            _gpJob = _gpTask.CreateJob(gpParams);

            // Update the UI when job progress changes
            _gpJob.ProgressChanged += (sender, args) =>
            {
                Dispatcher.Invoke(() => { MyLoadingIndicator.Value = _gpJob.Progress; });
            };

            // Be notified when the task completes (or other change happens)
            _gpJob.JobChanged += GpJobOnJobChanged;

            // Start the job
            _gpJob.Start();
        }

        private async void GpJobOnJobChanged(object o, EventArgs eventArgs)
        {
            // Show message if job failed
            if (_gpJob.Status == JobStatus.Failed)
            {
                MessageBox.Show("Job Failed");
                return;
            }

            // Return if not succeeded
            if (_gpJob.Status != JobStatus.Succeeded) { return; }

            // Get the URL to the map service
            string gpServiceResultUrl = _gpService.Url.ToString();

            // Get the URL segment for the specific job results
            string jobSegment = "MapServer/jobs/" + _gpJob.ServerJobId;

            // Update the URL to point to the specific job from the service
            gpServiceResultUrl = gpServiceResultUrl.Replace("GPServer", jobSegment);

            // Create a map image layer to show the results
            ArcGISMapImageLayer myMapImageLayer = new ArcGISMapImageLayer(new Uri(gpServiceResultUrl));

            // Load the layer
            await myMapImageLayer.LoadAsync();

            // This is needed because the event comes from outside of the UI thread
            Dispatcher.Invoke(() =>
            {
                // Add the layer to the map
                MyMapView.Map.OperationalLayers.Add(myMapImageLayer);

                // Hide the progress bar
                MyLoadingIndicator.Visibility = Visibility.Collapsed;

                // Disable the generate button
                MyUpdateContourButton.IsEnabled = false;

                // Enable the reset button
                MyResetButton.IsEnabled = true;
            });
        }

        private static string GetRasterPath()
        {
            //return DataManager.GetDataFolder("f7c7b4a30fb9415896ba0d1921fe014b", "RasterHillshade.tpk");
            return "null";
        }

        private static string GetGpPath()
        {
            //return DataManager.GetDataFolder("da9e565a52ca41c1937cff1a01017068", "Contour.gpk");
            return @"E:\Desktop\学习\大三上\GIS课程设计\gpk\ClustersOutliers.gpk";
        }

        private void MyResetButton_OnClick(object sender, RoutedEventArgs e)
        {
            // Remove the contour
            MyMapView.Map.OperationalLayers.RemoveAt(1);

            // Enable the generate button
            MyUpdateContourButton.IsEnabled = true;

            // Disable the reset button
            MyResetButton.IsEnabled = false;
        }

        private void MyUpdateContourButton_OnClick(object sender, RoutedEventArgs e)
        {
            // Disable the generate button
            ((Button)sender).IsEnabled = false;

            // Generate the contours
            GenerateContours();
        }
    }
}