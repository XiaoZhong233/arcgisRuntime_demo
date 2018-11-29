using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WpfApp1.form
{
    public partial class PolygonSymbolForm: Form
    {
        #region 变量
        Layer layer;
        //边框样式
        SimpleLineSymbol simpleLineSymbol = new SimpleLineSymbol();
        //填充样式
        SimpleFillSymbol simpleFillSymbol = new SimpleFillSymbol();
        //顶点样式
        SimpleMarkerSymbol SimpleMarkerSymbol = new SimpleMarkerSymbol();
        //填充样式数据源
        List<SimpleFillSymbolStyle> fillSymbolStyles = new List<SimpleFillSymbolStyle>();
        //边框样式数据源
        List<SimpleLineSymbolStyle> lineSymbolStyles = new List<SimpleLineSymbolStyle>();
        //顶点样式数据源
        List<SimpleMarkerSymbolStyle> markerSymbolStyles = new List<SimpleMarkerSymbolStyle>();
        #endregion

        #region 属性
        public SimpleLineSymbol SimpleLineSymbol
        {
            get
            {
                return simpleLineSymbol;
            }
        }

        public SimpleFillSymbol SimpleFillSymbol
        {
            get
            {
                return simpleFillSymbol;
            }
        }
        #endregion

        #region 初始化
        public PolygonSymbolForm(Layer layer)
        {
            InitializeComponent();
            this.layer = layer;
            initData();
            initLegend();
            init();
        }

        public PolygonSymbolForm()
        {
            InitializeComponent();
            initData();
            init();
            initControls();
        }


        /// <summary>
        /// 无Layer初始化
        /// </summary>
        private void initControls()
        {
            fill_colorBtn.BackColor = simpleFillSymbol.Color;
            widthUpDown.Value = 1;
            simpleLineSymbol.Color = Color.Red;
            lineColor_btn.BackColor = simpleLineSymbol.Color;
        }

        /// <summary>
        /// 初始化combobox数据源
        /// </summary>
        private void initData()
        {
            fillSymbolStyles.Add(SimpleFillSymbolStyle.Solid);
            fillSymbolStyles.Add(SimpleFillSymbolStyle.Vertical);
            fillSymbolStyles.Add(SimpleFillSymbolStyle.Horizontal);
            fillSymbolStyles.Add(SimpleFillSymbolStyle.ForwardDiagonal);
            fillSymbolStyles.Add(SimpleFillSymbolStyle.DiagonalCross);
            fillSymbolStyles.Add(SimpleFillSymbolStyle.Cross);
            fillSymbolStyles.Add(SimpleFillSymbolStyle.BackwardDiagonal);
            fillSymbolStyles.Add(SimpleFillSymbolStyle.Null);

            lineSymbolStyles.Add(SimpleLineSymbolStyle.Solid);
            lineSymbolStyles.Add(SimpleLineSymbolStyle.Dot);
            lineSymbolStyles.Add(SimpleLineSymbolStyle.Dash);
            lineSymbolStyles.Add(SimpleLineSymbolStyle.DashDot);
            lineSymbolStyles.Add(SimpleLineSymbolStyle.DashDotDot);
            lineSymbolStyles.Add(SimpleLineSymbolStyle.Null);

            markerSymbolStyles.Add(SimpleMarkerSymbolStyle.Circle);
            markerSymbolStyles.Add(SimpleMarkerSymbolStyle.Cross);
            markerSymbolStyles.Add(SimpleMarkerSymbolStyle.Diamond);
            markerSymbolStyles.Add(SimpleMarkerSymbolStyle.Square);
            markerSymbolStyles.Add(SimpleMarkerSymbolStyle.Triangle);
            markerSymbolStyles.Add(SimpleMarkerSymbolStyle.X);

            fillType_cbo.DataSource = fillSymbolStyles;
            lineStyle_Combox.DataSource = lineSymbolStyles;
            markerStyle_Combox.DataSource = markerSymbolStyles;
        }

        /// <summary>
        /// 初始化symbol
        /// </summary>
        private async void initLegend()
        {
            FeatureLayer feature = layer as FeatureLayer;
            SimpleRenderer simpleRenderer = feature.Renderer as SimpleRenderer;
            

            if (simpleRenderer.Symbol is SimpleFillSymbol)
            {
                //填充控件初始化
                simpleFillSymbol = simpleRenderer.Symbol.Clone() as SimpleFillSymbol;
                int index1 = findStyleIndex(fillSymbolStyles,x=>simpleFillSymbol.Style.ToString().Equals(x.ToString()));
                if (index1 == -1)
                    return;
                fillType_cbo.SelectedIndex = index1;
                fill_colorBtn.BackColor = simpleFillSymbol.Color;
                fill_transparencyControl.Value = simpleFillSymbol.Color.A;
                
                //边框控件初始化
                if(simpleFillSymbol.Outline is SimpleLineSymbol)
                {
                    simpleLineSymbol = simpleFillSymbol.Outline.Clone() as SimpleLineSymbol;
                    int index2 = findStyleIndex(lineSymbolStyles, x => simpleLineSymbol.Style.ToString().Equals(x.ToString()));
                    if (index2 == -1)
                        return;
                    lineStyle_Combox.SelectedIndex = index2;
                    widthUpDown.Value = (decimal)simpleLineSymbol.Width;
                    lineColor_btn.BackColor = simpleLineSymbol.Color;
                    line_transparencyControl.Value = simpleLineSymbol.Color.A;
                }
                await refreshPreviewAsync();

            }
            else
            {
                //非简单填充样式
                //未实现
            }
        }

        /// <summary>
        /// 初始化控件，注册监听
        /// </summary>
        private void init()
        {

            //-----------------------------------------------------------------------------fillTab初始化
            //属性监听
            simpleFillSymbol.PropertyChanged += async (s, e) =>
            {
                //刷新预览图
                await refreshPreviewAsync();
            };

            //填充样式监听
            fillType_cbo.SelectedValueChanged += (s, e) =>
            {
                simpleFillSymbol.Style = fillSymbolStyles[fillType_cbo.SelectedIndex];
            };

            //颜色点击监听
            fill_colorBtn.Click += (s, e) =>
            {
                DialogResult result = colorDialog.ShowDialog();
                if (result == DialogResult.OK && simpleFillSymbol != null)
                {
                    //需要保持A通道
                    fill_transparencyControl.Value = colorDialog.Color.A;
                    fill_colorBtn.BackColor = colorDialog.Color;
                    simpleFillSymbol.Color = colorDialog.Color;
                }
            };

            //透明度监听
            fill_transparencyControl.ValueChanged += (s, e) =>
            {
                Color c = simpleFillSymbol.Color;
                simpleFillSymbol.Color = Color.FromArgb(fill_transparencyControl.Value, c.R, c.G, c.B);
            };


            //-----------------------------------------------------------------------lineTab初始化

            simpleLineSymbol.PropertyChanged += async (s, e) =>
            {
                //此处触发属性即可刷新，如果在refresh中设置outLine会陷入死循环
                simpleFillSymbol.Outline = simpleLineSymbol;
                //刷新预览图
                //await refreshPreviewAsync();
            };

            //边框样式监听
            lineStyle_Combox.SelectedValueChanged += (s, e) =>
            {
                simpleLineSymbol.Style = lineSymbolStyles[lineStyle_Combox.SelectedIndex];
            };

            //边框宽度监听
            widthUpDown.ValueChanged += (s, e) =>
            {
                simpleLineSymbol.Width = (double)widthUpDown.Value;
            };

            //颜色按钮点击监听
            lineColor_btn.Click += (s, e) =>
            {
                DialogResult result = colorDialog.ShowDialog();
                if (result == DialogResult.OK && simpleLineSymbol != null)
                {
                    //需要保持A通道
                    line_transparencyControl.Value = colorDialog.Color.A;
                    lineColor_btn.BackColor = colorDialog.Color;
                    simpleLineSymbol.Color = colorDialog.Color;
                }
            };

            //透明度监听
            line_transparencyControl.ValueChanged += (s, e) =>
            {
                Color c = simpleLineSymbol.Color;
                simpleLineSymbol.Color = Color.FromArgb(line_transparencyControl.Value, c.R, c.G, c.B);
            };



            ok_btn.Click += (s, e) =>
            {
                simpleFillSymbol.Outline = simpleLineSymbol;
                SimpleRenderer simpleRenderer = new SimpleRenderer(simpleFillSymbol);
                FeatureLayer feature = layer as FeatureLayer;
                if (feature!=null)
                {
                    feature.Renderer = simpleRenderer;
                    Close();
                }
                else
                {
                    //其他要素图层处理
                    Close();
                }
            };
        }
        #endregion

        #region 私有方法
        /// <summary>
        /// 寻找特定的样式索引
        /// </summary>
        /// <param name="list"></param>
        /// <param name="predicate"></param>
        /// <returns>-1代表未找到</returns>
        private int findStyleIndex(List<SimpleFillSymbolStyle> list,Predicate<SimpleFillSymbolStyle> predicate)
        {
            for(int i = 0; i < list.Count; i++)
            {
                if (predicate(list[i]))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// 寻找特定样式的索引
        /// </summary>
        /// <param name="list"></param>
        /// <param name="predicate"></param>
        /// <returns>-1代表未找到</returns>
        private int findStyleIndex(List<SimpleLineSymbolStyle> list,Predicate<SimpleLineSymbolStyle> predicate)
        {
            for(int i = 0; i < list.Count; i++)
            {
                if (predicate(list[i]))
                {
                    return i;
                }
            }
            return -1;
        }


        private async Task refreshPreviewAsync()
        {
            if (simpleFillSymbol == null || simpleLineSymbol==null)
                return;
            Esri.ArcGISRuntime.UI.RuntimeImage image = await simpleFillSymbol.CreateSwatchAsync(Color.WhiteSmoke);
            System.Windows.Media.ImageSource imageSource = await Esri.ArcGISRuntime.UI.RuntimeImageExtensions.ToImageSourceAsync(image);
            Image result = imageWpfToGDI(imageSource);
            previewBox.Image = result;
            previewBox.SizeMode = PictureBoxSizeMode.Zoom;
        }

        /// <summary>
        /// 将madia.imageSource转换成Bitmap
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private System.Drawing.Image imageWpfToGDI(System.Windows.Media.ImageSource image)
        {
            MemoryStream ms = new MemoryStream();
            var encoder = new System.Windows.Media.Imaging.BmpBitmapEncoder();
            encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(image as System.Windows.Media.Imaging.BitmapSource));
            encoder.Save(ms);
            ms.Flush();
            return System.Drawing.Image.FromStream(ms);
        }

        #endregion
    }
}
