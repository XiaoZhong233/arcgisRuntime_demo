using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.LocalServices;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Rasters;
using Esri.ArcGISRuntime.Tasks;
using Esri.ArcGISRuntime.Tasks.Geoprocessing;
using Esri.ArcGISRuntime.UI;
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

namespace WpfApp1.form.GP
{
    /// <summary>
    /// LocalServerTest.xaml 的交互逻辑
    /// </summary>
    public partial class LocalServerTest : Window
    {

        private Esri.ArcGISRuntime.LocalServices.LocalServer _localServer;
        private object myMapView;

        public LocalServerTest()
        {
            InitializeComponent();
            initMapAsync();
            StartLocalServer();
        }

        private async Task initMapAsync()
        {
            Map map = new Map(Basemap.CreateTopographic());
            string pathToRaster = @"E:\Desktop\ExecuteGPK\LasVegasNED13_geoid1.tif";
            var myRaster = new Esri.ArcGISRuntime.Rasters.Raster(pathToRaster);
            // create a RasterLayer using the Raster
            var newRasterLayer = new RasterLayer(myRaster);
            map.OperationalLayers.Add(newRasterLayer);
            Viewpoint viewPoint = new Viewpoint(36.131, -115.144, 800000);
            MyMapView.Map = map;
            //await MyMapView.SetViewpointGeometryAsync(newRasterLayer.FullExtent);
            await MyMapView.SetViewpointAsync(viewPoint, TimeSpan.FromSeconds(2));
        }


        private async void StartLocalServer()
        {
            // Get the singleton LocalServer object using the static "Instance" property
            _localServer = Esri.ArcGISRuntime.LocalServices.LocalServer.Instance;

            // Handle the StatusChanged event to react when the server is started
            _localServer.StatusChanged += ServerStatusChanged;

            // Start the local server instance
            await _localServer.StartAsync();
        }

        private void ServerStatusChanged(object sender, Esri.ArcGISRuntime.LocalServices.StatusChangedEventArgs e)
        {
            // Check if the server started successfully
            if (e.Status == Esri.ArcGISRuntime.LocalServices.LocalServerStatus.Started)
            {
                btn.Click += async (s, ex) =>
                {
                    await executeGeoprocessingAsync();
                };
            }

        }




        private async Task executeGeoprocessingAsync()
        {
            LocalGeoprocessingService localServiceGP = new LocalGeoprocessingService(@"E:\Desktop\ExecuteGPK\InterpolateShape.gpk");
            localServiceGP.ServiceType = GeoprocessingServiceType.SynchronousExecute;
            localServiceGP.StatusChanged += async (svc, args) =>
            {
                if (args.Status != LocalServerStatus.Started)
                    return;
                    // Get the URL for the specific geoprocessing tool
                var gpSvcUrl = (svc as LocalGeoprocessingService).Url.AbsoluteUri + "/InterpolateShape";
                GeoprocessingTask gpRouteTask = new GeoprocessingTask(new Uri(gpSvcUrl));
                GeoprocessingParameters para = new GeoprocessingParameters(GeoprocessingExecutionType.SynchronousExecute);
                var inputFeatures = new FeatureCollectionTable(new List<Field>(), GeometryType.Polyline, MyMapView.SpatialReference);
                Feature inputFeature = inputFeatures.CreateFeature();
                var geometry = await MyMapView.SketchEditor.StartAsync(SketchCreationMode.Polyline, false);
                inputFeature.Geometry = geometry;
                await inputFeatures.AddFeatureAsync(inputFeature);
                para.Inputs.Add("inputLine", new GeoprocessingFeatures(inputFeatures));
                para.ReturnZ = true;
                para.OutputSpatialReference = MyMapView.SpatialReference;
                GeoprocessingJob routeJob = gpRouteTask.CreateJob(para);

                try
                {
                    GeoprocessingResult geoprocessingResult = await routeJob.GetResultAsync();
                    GeoprocessingFeatures resultFeatures = geoprocessingResult.Outputs["outputLine"] as GeoprocessingFeatures;
                    IFeatureSet interpolateShapeResult = resultFeatures.Features;
                    Esri.ArcGISRuntime.Geometry.Polyline elevationLine =
                    interpolateShapeResult.First().Geometry as Esri.ArcGISRuntime.Geometry.Polyline;
                    MapPoint startPoint = elevationLine.Parts[0].Points[0];
                    int count = elevationLine.Parts[0].PointCount;
                    MapPoint stopPoint = elevationLine.Parts[0].Points[count - 1];
                    double chazhi = stopPoint.Z - startPoint.Z;
                    MessageBox.Show("终点的Z值为: " + stopPoint.Z.ToString() + "，起点的Z值为: " + startPoint.Z.ToString());


                }
                catch (Exception ex)
                {
                    if (routeJob.Status == JobStatus.Failed && routeJob.Error != null)
                        MessageBox.Show("Executing geoprocessing failed. " + routeJob.Error.Message, "Geoprocessing error");
                    else
                        MessageBox.Show("An error occurred. " + ex.ToString(), "Sample error");
                }


            };
            await localServiceGP.StartAsync();
        }

    }
}
