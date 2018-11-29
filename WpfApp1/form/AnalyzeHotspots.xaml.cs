using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Tasks;
using Esri.ArcGISRuntime.Tasks.Geoprocessing;


namespace WpfApp1.form
{
    /// <summary>
    /// AnalyzeHotspots.xaml 的交互逻辑
    /// </summary>
    public partial class AnalyzeHotspots : Window
    {
        public AnalyzeHotspots()
        {
            InitializeComponent();
            Initialize();
        }

        // URL for the geoprocessing service
        private const string _hotspotUrl =
            "https://sampleserver6.arcgisonline.com/arcgis/rest/services/911CallsHotspot/GPServer/911%20Calls%20Hotspot";

        // The geoprocessing task for hot spot analysis 
        private GeoprocessingTask _hotspotTask;

        // The job that handles the communication between the application and the geoprocessing task
        private GeoprocessingJob _hotspotJob;


        private async void Initialize()
        {
            // Create a map with a topographic basemap
            MyMapView.Map = new Map(Basemap.CreateTopographic());

            try
            {
                // Create a new geoprocessing task
                _hotspotTask = await GeoprocessingTask.CreateAsync(new Uri(_hotspotUrl));
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error");
            }

        }


        private async void OnAnalyzeHotspotsClicked(object sender, RoutedEventArgs e)
        {
            // Clear any existing results
            MyMapView.Map.OperationalLayers.Clear();

            // Show the waiting indication
            ShowBusyOverlay();

            // Get the 'from' and 'to' dates from the date pickers for the geoprocessing analysis
            DateTime myFromDate = FromDate.SelectedDate.Value;
            DateTime myToDate = ToDate.SelectedDate.Value;

            // The end date must be at least one day after the start date
            if (myToDate <= myFromDate.AddDays(1))
            {
                // Show error message
                MessageBox.Show(
                    "Please select valid time range. There has to be at least one day in between To and From dates.",
                    "Invalid date range");

                // Remove the waiting
                ShowBusyOverlay(false);
                return;
            }

            // Create the parameters that are passed to the used geoprocessing task
            GeoprocessingParameters myHotspotParameters = new GeoprocessingParameters(GeoprocessingExecutionType.AsynchronousSubmit);

            // Construct the date query
            string myQueryString = string.Format("(\"DATE\" > date '{0:yyyy-MM-dd} 00:00:00' AND \"DATE\" < date '{1:yyyy-MM-dd} 00:00:00')", myFromDate, myToDate);
            
            // Add the query that contains the date range used in the analysis
            myHotspotParameters.Inputs.Add("Query", new GeoprocessingString(myQueryString));

            // Create job that handles the communication between the application and the geoprocessing task
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
            finally
            {
                // Remove the waiting
                ShowBusyOverlay(false);
            }

        }


        private void OnCancelTaskClicked(object sender, RoutedEventArgs e)
        {
            // Cancel current geoprocessing job
            if (_hotspotJob.Status == JobStatus.Started)
                _hotspotJob.Cancel();

            // Hide the waiting indication
            ShowBusyOverlay(false);
        }

        private void ShowBusyOverlay(bool visibility = true)
        {
            // Function to toggle the visibility of interaction with the GUI for the user to 
            // specify dates for the hot spot analysis. When the analysis is running, the GUI
            // for changing the dates is 'grayed-out' and the progress bar with a cancel 
            // button (aka. waiting object) becomes active.

            if (visibility)
            {
                // The geoprocessing task is processing. The busyOverly is present.
                BusyOverlay.Visibility = Visibility.Visible;
                Progress.IsIndeterminate = true;
            }
            else
            {
                // The user can interact with the date pickers. The waiting is invisible.
                BusyOverlay.Visibility = Visibility.Collapsed;
                Progress.IsIndeterminate = false;
            }
        }
    }
}
