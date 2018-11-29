using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Mapping;
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

namespace WpfApp1.View
{
    /// <summary>
    /// properityForm.xaml 的交互逻辑
    /// </summary>
    public partial class properityForm : Window
    {
        private Layer layer;

        public Layer Layer
        {
            get { return layer; }
            set { layer = value; }
        }

        public properityForm(Layer layer)
        {
            InitializeComponent();
            this.layer = layer;
            initAsync();
        }

        private  void initAsync()
        {
            FeatureLayer shapeFile = this.layer as FeatureLayer;
           
            if (shapeFile != null)
            {
                ShapefileFeatureTable myShapefile = shapeFile.FeatureTable as ShapefileFeatureTable;
                if (myShapefile != null)
                {
                    
                    ShapefileInfo fileInfo = myShapefile.Info;
                    string x="";
                    
                    foreach(Field field in myShapefile.Fields)
                    {
                        x += field.Name;
                        
                        x += "\n";
                    } 
                    info.Text = x;
                    InfoPanel.DataContext = fileInfo;
                    //ShapefileThumbnailImage.Source = 
                    //    await Esri.ArcGISRuntime.UI.RuntimeImageExtensions.ToImageSourceAsync(fileInfo.Thumbnail);
                }
            }
            
        }

    }
}
