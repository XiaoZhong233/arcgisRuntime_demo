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
    /// SelectGP.xaml 的交互逻辑
    /// </summary>
    public partial class SelectGP : Window
    {

        private GeoprocessingTask _hotspotTask;
        private GeoprocessingJob _hotspotJob;
        private const string _hotspotUrl =
            "https://www.arcgis.com/home/item.html?id=7c02e1c0427346c58e0544fabf816082";

        public SelectGP()
        {
            InitializeComponent();
            InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            MyMapView.Map = new Map(Basemap.CreateTopographic());


            try
            {
                // Create a new geoprocessing task
                _hotspotTask = await GeoprocessingTask.CreateAsync(new Uri(_hotspotUrl));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString()+"错误", "Error");
            }


            executeAsync();
        }

        private async Task executeAsync()
        {
            MyMapView.Map.OperationalLayers.Clear();
            GeoprocessingParameters myHotspotParameters = new GeoprocessingParameters(GeoprocessingExecutionType.AsynchronousSubmit);
            _hotspotJob = _hotspotTask.CreateJob(myHotspotParameters);
            try
            {
                // Execute the geoprocessing analysis and wait for the results
                GeoprocessingResult myAnalysisResult = await _hotspotJob.GetResultAsync();

                // Add results to a map using map server from a geoprocessing task
                // Load to get access to full extent
                await myAnalysisResult.MapImageLayer.LoadAsync();

                // Add the analysis layer to the map view
                MyMapView.Map.OperationalLayers.Add(myAnalysisResult.MapImageLayer);

                // Zoom to the results
                await MyMapView.SetViewpointAsync(new Viewpoint(myAnalysisResult.MapImageLayer.FullExtent));
            }
            catch (TaskCanceledException)
            {
                // This is thrown if the task is canceled. Ignore.
            }
            catch (Exception ex)
            {
                // Display error messages if the geoprocessing task fails
                if (_hotspotJob.Status == JobStatus.Failed && _hotspotJob.Error != null)
                    MessageBox.Show("Executing geoprocessing failed. " + _hotspotJob.Error.Message, "Geoprocessing error");
                else
                    MessageBox.Show("An error occurred. " + ex, "Sample error");
            }

        }
    }
}
