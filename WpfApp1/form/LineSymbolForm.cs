using Esri.ArcGISRuntime.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Esri.ArcGISRuntime.Symbology;
using System.Diagnostics;
using System.IO;

namespace WpfApp1.form
{
    public partial class LineSymbolForm: Form
    {

        #region 变量
        Layer layer;
        SimpleLineSymbol simpleLineSymbol = new SimpleLineSymbol();
        List<SimpleLineSymbolStyle> styleList = new List<SimpleLineSymbolStyle>();
        #endregion

        #region 属性
        public SimpleLineSymbol SimpleLineSymbol
        {
            get
            {
                return simpleLineSymbol;
            }
        }
        #endregion

        #region 初始化
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="layer"></param>
        public LineSymbolForm(Layer layer)
        {
            InitializeComponent();
            this.layer = layer;
            initData();
            initLegendAsync();
            init();
        }


        public LineSymbolForm()
        {
            InitializeComponent();
            initData();
            simpleLineSymbol = new SimpleLineSymbol();
            init();
            initControls();
            //SimpleLineSymbol.MarkerStyle = SimpleLineSymbolMarkerStyle.Arrow;
            
        }

        /// <summary>
        /// 无Layer初始化
        /// </summary>
        private void initControls()
        {
            sizeUpDown.Value = 5;
            simpleLineSymbol.Color = Color.Red;
            colorBtn.BackColor = simpleLineSymbol.Color;
        }


        /// <summary>
        /// 初始化数据源
        /// </summary>
        private void initData()
        {
            styleList.Add(SimpleLineSymbolStyle.Solid);
            styleList.Add(SimpleLineSymbolStyle.Dot);
            styleList.Add(SimpleLineSymbolStyle.Dash);
            styleList.Add(SimpleLineSymbolStyle.DashDot);
            styleList.Add(SimpleLineSymbolStyle.DashDotDot);
            StyleCombox.DataSource = styleList;
        }


        /// <summary>
        /// 由传入的图层初始化lineSymbol
        /// </summary>
        private async void initLegendAsync()
        {
            
            FeatureLayer feature = layer as FeatureLayer;
            SimpleRenderer simpleRenderer = feature.Renderer as SimpleRenderer;
            if(simpleRenderer.Symbol is SimpleLineSymbol)
            {
                simpleLineSymbol = simpleRenderer.Symbol.Clone() as SimpleLineSymbol;
                //初始化控件
                int index = findStyle(x => simpleLineSymbol.Style.ToString().Equals(x.ToString()));
                if (index == -1)
                {
                    index = 0;
                    Debug.Print("未查到到style的索引");
                }
                StyleCombox.SelectedIndex = index;
                sizeUpDown.Value = (decimal)simpleLineSymbol.Width;
                colorBtn.BackColor = simpleLineSymbol.Color;
                transparencyControl.Value = simpleLineSymbol.Color.A;
                await refreshPreview();

            }
            //线样式为非简单样式的情况
            else
            {
                //暂未实现
            }
        }

        private void init()
        {
            //监听style_combobox改变的事件
            StyleCombox.SelectedIndexChanged += (s, e) =>
            {
                simpleLineSymbol.Style = styleList[this.StyleCombox.SelectedIndex];
            };
            

            //handle 透明度改变
            transparencyControl.ValueChanged += (s, e) =>
            {
                Color c = simpleLineSymbol.Color;
                simpleLineSymbol.Color = Color.FromArgb(transparencyControl.Value, c.R, c.G, c.B);
            };

            //handle 宽度改变
            sizeUpDown.ValueChanged += (s, e) =>
            {
                simpleLineSymbol.Width = (double)sizeUpDown.Value;
            };


            //handle 颜色选择
            colorBtn.Click += (s, e) =>
            {
                DialogResult result = colorDialog.ShowDialog();
                if (result == DialogResult.OK && simpleLineSymbol != null)
                {
                    //需要保持A通道
                    transparencyControl.Value = colorDialog.Color.A;
                    colorBtn.BackColor = colorDialog.Color;
                    simpleLineSymbol.Color = colorDialog.Color;
                }
            };

            //handle 确认点击
            okBtn.Click += (s, e) =>
            {
                SimpleRenderer simpleRenderer = new SimpleRenderer(simpleLineSymbol);
                FeatureLayer feature = layer as FeatureLayer;
                if (feature != null)
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



            //监听图例改变属性事件
            simpleLineSymbol.PropertyChanged += async (s, e) =>
            {
                //重绘预览图
                await refreshPreview();
            };

        }

        #endregion

        #region 私有方法
        /// <summary>
        /// 寻找满足某条件的style
        /// 返回其在styleList中的索引
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns>索引,未找到则返回-1</returns>
        private int findStyle(Predicate<SimpleLineSymbolStyle> predicate)
        {
            for(int i = 0; i < styleList.Count; i++)
            {
                if (predicate(styleList[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// 异步刷新预览图
        /// </summary>
        private async Task refreshPreview()
        {
            if (simpleLineSymbol == null)
                return;
            Esri.ArcGISRuntime.UI.RuntimeImage image = await simpleLineSymbol.CreateSwatchAsync(Color.WhiteSmoke);
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
